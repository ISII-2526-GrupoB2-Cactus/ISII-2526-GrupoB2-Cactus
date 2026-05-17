using AppForSEII2526.API.DTOs.DeviceDTo;
using AppForSEII2526.API.DTOs.DeviceDTO;
using AppForSEII2526.API.DTOs.DeviceDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        //used to enable your controller to access to the database
        private readonly ApplicationDbContext _context;
        //used to log any information when your system is running
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(ApplicationDbContext context, ILogger<DevicesController> logger)
        {
            _context = context;
            _logger = logger;
        }

 
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<DeviceForReviewDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetDevicesForReview(int? year, string? brand)
        {
            if (_context.Device == null)
            {
                _logger.LogError("Error: No existen dispositivos en la tabla");
                return NotFound("No existen dispositivos en la base de datos.");
            }

            var devices = await _context.Device
                .Include(d => d.Model) 
                .Where(d =>
                    (string.IsNullOrEmpty(brand) || d.Brand.Contains(brand)) &&
                    (year == null || d.Year == year)
                )
                .OrderBy(d => d.Year)
                .ThenBy(d => d.Name)
                .Select(d => new DeviceForReviewDTO(
                    d.Id,
                    d.Brand,        
                    d.Name,
                    d.Year,
                    d.Model.Name,
                    d.Color
                ))
                .ToListAsync();

            if (devices == null || devices.Count == 0)
            {
                _logger.LogWarning("No se encontraron dispositivos que coincidan con los filtros aplicados");
                return NotFound("No se encontraron dispositivos que coincidan con los filtros aplicados.");
            }

            _logger.LogInformation($"Se encontraron {devices.Count} dispositivos disponibles para reseña.");
            return Ok(devices);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<DeviceParaAlquilarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDevicesForRental(string? model, double? priceForRent)
        {
            var devices = await _context.Device
                .Include(d => d.Model)
                .Where(d =>
                    (string.IsNullOrEmpty(model) || d.Model.Name.Contains(model)) &&
                    (priceForRent == null || d.PriceForRent == priceForRent)
                )
                .OrderBy(d => d.PriceForRent)
                //.ThenBy(d => d.Year)
                .Select(d => new DeviceParaAlquilarDTO(
                    d.Id,
                    d.Name,
                    d.Model.Name,
                    d.Brand,
                    d.Year,
                    d.Color,
                    d.PriceForRent
                ))
                .ToListAsync();

            return Ok(devices);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<DeviceForPurchaseDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<List<DeviceForPurchaseDTO>>> GetDeviceForPurchase(string? name, string? color)
        {
            // Validaciones de filtros

            if (!string.IsNullOrEmpty(color) && !_context.Device.Any(d => d.Color.ToLower() == color.ToLower()))
            {
                return BadRequest("El color indicado no existe en el catálogo de dispositivos.");
            }

            if (!string.IsNullOrEmpty(name) && !_context.Device.Any(d => d.Name.ToLower().Contains(name.ToLower())))
            {
                return BadRequest("No existe ningún dispositivo con ese nombre.");
            }

            // Consulta principal
            var devices = await _context.Device
                .Include(d => d.Model)
                .Where(d =>
                    (string.IsNullOrEmpty(name) || d.Name.Contains(name)) &&
                    (string.IsNullOrEmpty(color) || d.Color.Contains(color))
                )
                .OrderBy(d => d.Name)
                .ThenBy(d => d.Brand)
                .Select(d => new DeviceForPurchaseDTO(
                    d.Id,
                    d.Brand,
                    d.Name,
                    d.Model != null ? d.Model.Name : string.Empty,
                    d.Color,
                    (decimal)d.PriceForPurchase,
                    d.QuantityForPurchase,
                    d.Year
                ))
                .ToListAsync();

            return Ok(devices);
        }
    }
}




