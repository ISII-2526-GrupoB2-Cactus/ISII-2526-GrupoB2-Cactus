using AppForSEII2526.API.DTOs.PurchaseDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using AppForSEII2526.API.DTOs.DeviceDTOs;

namespace AppForSEII2526.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PurchaseController> _logger;

        public PurchaseController(ApplicationDbContext context, ILogger<PurchaseController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(PurchaseDetailDTO), (int)HttpStatusCode.OK)] 
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetPurchase(int id)
        {
            if (_context.Purchase == null)
            {
                _logger.LogError("Error: la tabla Purchase no existe");
                return NotFound();
            }

            var purchasedto = await _context.Purchase
                .Where(p => p.Id == id)
                .Include(p => p.PurchaseItems)               // join PurchaseItems
                    .ThenInclude(pi => pi.Device)            // join Device
                        .ThenInclude(d => d.Model)           // join Model (para d.Model.Name)
                .Select(p => new PurchaseDetailDTO(
                    p.Id,
                    p.DeliveryAddress,
                    p.PurchaseItems.Select(pi => new PurchaseItemDTO(
                        pi.Device.Id,
                        pi.Device.Brand,
                        // Model en tu DTO es string ponemos el nombre del modelo:
                        pi.Device.Model != null ? pi.Device.Model.Name : string.Empty,
                        pi.Device.Color,
                        // Tu DTO usa decimal Device.PriceForPurchase es double
                        (decimal)pi.Device.PriceForPurchase,
                        pi.Quantity,
                        // Si tu entidad PurchaseItem tiene Description, se proyecta; si no, null:
                        pi.Description
                    )).ToList<PurchaseItemDTO>(),
                    p.ApplicationUser.CustomerUserName,
                    p.ApplicationUser.CustomerUserSurname,
                    p.PaymentMethod,
                    p.PurchaseDate
                ))
                .FirstOrDefaultAsync();

            if (purchasedto == null)
            {
                _logger.LogError($"Error: no existe la compra con id {id}");
                return NotFound();
            }

            return Ok(purchasedto);
        }


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<DeviceForPurchaseDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDetailsForPurchase(string? name, string? color)
        {
            var devices = await _context.Device
                .Include(d => d.Model)
                .Where(d =>
                    (string.IsNullOrEmpty(name) || d.Name.Contains(name)) &&
                    (string.IsNullOrEmpty(color) || d.Color.Contains(color)) 
                    //(string.IsNullOrEmpty(model) || (d.Model != null && d.Model.NameModel.Contains(model)))
                )
                .OrderBy(d => d.Name)
                .ThenBy(d => d.Brand)
                .Select(d => new DeviceForPurchaseDTO(
                    d.Id,
                    d.Brand,
                    d.Name,
                    d.Model.Name,
                    d.Color,
                    (decimal)d.PriceForPurchase,
                    d.QuantityForPurchase,
                    d.Year
                ))
                .ToListAsync();

            return Ok(devices);
        }




        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(PurchaseDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreatePurchase([FromBody] PurchaseForCreateDTO purchaseDto)
        {
            if (purchaseDto == null)
            {
                return BadRequest("Objeto de compra incorrecto.");
            }

            // Validación de datos obligatorios
            if (!string.IsNullOrEmpty(purchaseDto.UserName) &&
                !string.IsNullOrEmpty(purchaseDto.NameSurname) &&
                !string.IsNullOrEmpty(purchaseDto.DeliveryAddress) &&
                purchaseDto.PaymentMethod != null &&
                purchaseDto.PurchaseItems != null && purchaseDto.PurchaseItems.Any())
            {
                
                List<PurchaseItem> seleccionados = new List<PurchaseItem>();
                double totalPrice = 0;
                int totalQuantity = 0;

                foreach (var item in purchaseDto.PurchaseItems)
                {
                    if (item.Quantity > 0)
                    {
                        var device = await _context.Device
                            .Include(d => d.Model)
                            .FirstOrDefaultAsync(x => x.Id == item.DeviceID);

                        if (device != null)
                        {
                            double itemPrice = (double)item.PriceForPurchase * item.Quantity;
                            totalPrice += itemPrice;
                            totalQuantity += item.Quantity;

                            // Crear el objeto PurchaseItem
                            seleccionados.Add(new PurchaseItem
                            {
                                DeviceId = device.Id,
                                Quantity = item.Quantity,
                                Description = item.Description,
                                Price = (double)item.PriceForPurchase
                            });
                        }
                        else
                        {
                            return BadRequest($"El dispositivo con ID {item.DeviceID} no está disponible.");
                        }
                    }
                    else
                    {
                        return BadRequest("La cantidad de algún dispositivo no es válida.");
                    }
                }



                // Buscar el usuario asociado
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == purchaseDto.UserName);

                if (user == null)
                {
                    return BadRequest("Usuario no encontrado en el sistema.");
                }

                // Crear la entidad Purchase con los totales calculados
                var purchase = new Purchase
                {
                    ApplicationUser = user,  //asigna el usuario directamente
                    DeliveryAddress = purchaseDto.DeliveryAddress,
                    PaymentMethod = purchaseDto.PaymentMethod,
                    PurchaseDate = DateTime.Now,
                    TotalPrice = totalPrice,
                    TotalQuantity = totalQuantity,
                    PurchaseItems = seleccionados
                };


                _context.Purchase.Add(purchase);
                await _context.SaveChangesAsync();

                // Cargar las relaciones necesarias
                await _context.Entry(purchase)
                    .Collection(p => p.PurchaseItems)
                    .Query()
                    .Include(i => i.Device)
                        .ThenInclude(d => d.Model)
                    .LoadAsync();

                // Crear el DTO de respuesta
                var purchaseDetails = new PurchaseDetailDTO(
                    purchase.Id,
                    purchase.DeliveryAddress,
                    purchase.PurchaseItems.Select(item => new PurchaseItemDTO(
                        item.Device.Id,
                        item.Device.Brand,
                        item.Device.Model?.Name ?? string.Empty,
                        item.Device.Color,
                        (decimal)item.Price,
                        item.Quantity,
                        item.Description
                    )).ToList(),
                    purchase.ApplicationUser.CustomerUserName,
                    purchase.ApplicationUser.CustomerUserSurname,
                    purchase.PaymentMethod,
                    purchase.PurchaseDate
                );

                return CreatedAtAction(nameof(GetPurchase), new { id = purchase.Id }, purchaseDetails);
            }
            else
            {
                return BadRequest("Faltan datos obligatorios o la lista de dispositivos está vacía.");
            }
        }





    }
}
