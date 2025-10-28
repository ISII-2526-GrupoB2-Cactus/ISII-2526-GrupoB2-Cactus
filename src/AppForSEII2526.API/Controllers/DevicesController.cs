using AppForSEII2526.API.DTOs.DeviceDTo;
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

        // GET: api/devices/getdevicesforreview?year=2023&brand=Samsung
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
                .Include(d => d.Model) // Incluye la entidad relacionada Model (si existe)
                .Where(d =>
                    (string.IsNullOrEmpty(brand) || d.Brand.Contains(brand)) &&
                    (year == null || d.Year == year)
                )
                .OrderBy(d => d.Year)
                .ThenBy(d => d.Name)
                .Select(d => new DeviceForReviewDTO(
                    d.Id,
                    d.Brand,        // orden correcto según tu constructor
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
    }
}




