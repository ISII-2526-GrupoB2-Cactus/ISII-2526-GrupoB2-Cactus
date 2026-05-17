using AppForSEII2526.API.DTOs.PurchaseDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AppForSEII2526.API.Controllers
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
        [Route("[action]/{id}")]
        [ProducesResponseType(typeof(PurchaseDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetPurchase(int id)
        {
            if (_context.Purchase == null)
            {
                _logger.LogError("Error: No existen compras en la tabla Purchase.");
                return NotFound("No existen compras en la base de datos.");
            }

            var purchase = await _context.Purchase
                .Where(p => p.Id == id)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Device)
                        .ThenInclude(d => d.Model)
                .Include(p => p.ApplicationUser)
                .Select(p => new PurchaseDetailDTO(
                    p.Id,
                    p.DeliveryAddress,
                    p.PurchaseItems.Select(pi => new PurchaseItemDTO(
                        pi.Device.Id,
                        pi.Device.Brand,
                        pi.Device.Model != null ? pi.Device.Model.Name : string.Empty,
                        pi.Device.Color,
                        (decimal)pi.Price,
                        pi.Quantity,
                        pi.Description
                    )).ToList<PurchaseItemDTO>(),
                    p.ApplicationUser.CustomerUserName,
                    p.ApplicationUser.CustomerUserSurname,
                    p.PaymentMethod,
                    p.PurchaseDate
                ))
                .FirstOrDefaultAsync();

            if (purchase == null)
            {
                _logger.LogError($"Error: No existe ninguna compra con id {id}.");
                return NotFound();
            }

            _logger.LogInformation($"Compra encontrada con ID {id}.");
            return Ok(purchase);
        }


        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(PurchaseDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreatePurchase(PurchaseForCreateDTO purchaseForCreate)
        {
            if (purchaseForCreate == null)
            {
                return BadRequest("Objeto de compra incorrecto.");
            }

            // Validación de datos obligatorios
            if (!string.IsNullOrEmpty(purchaseForCreate.CustomerName) &&
                !string.IsNullOrEmpty(purchaseForCreate.CustomerSurname) &&
                !string.IsNullOrEmpty(purchaseForCreate.DeliveryAddress) &&
                purchaseForCreate.PurchaseItems != null && purchaseForCreate.PurchaseItems.Any())
            {
                List<PurchaseItem> seleccionados = new List<PurchaseItem>();
                double totalPrice = 0;
                int totalQuantity = 0;

                foreach (var item in purchaseForCreate.PurchaseItems)
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

                 

                        if (device.Brand.StartsWith("Motorola") || device.Brand.StartsWith("Nokia") || device.Model.Name.StartsWith("Motorola") || device.Model.Name.StartsWith("Nokia"))
                        {
                            return BadRequest($"Error: las tecnologias de estas marcas estan temporalmente no disponibles");
                        }

                    }
                    else
                    {
                        return BadRequest("La cantidad de algún dispositivo no es válida.");
                    }
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.CustomerUserName == purchaseForCreate.CustomerName &&
                              u.CustomerUserSurname == purchaseForCreate.CustomerSurname);

                if (user == null)
                {
                    return BadRequest("Usuario no encontrado en el sistema.");
                }

                var purchase = new Purchase
                {
                    DeliveryAddress = purchaseForCreate.DeliveryAddress,
                    PaymentMethod = purchaseForCreate.PaymentMethod,
                    PurchaseDate = DateTime.Now,
                    TotalPrice = totalPrice,
                    TotalQuantity = totalQuantity,
                    PurchaseItems = seleccionados,
                    ApplicationUser = user
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
