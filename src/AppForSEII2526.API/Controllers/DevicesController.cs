using AppForSEII2526.API.DTOs.DeviceDTo;
using AppForSEII2526.API.DTOs.ReviewDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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


        //Ejemplo de operacion
        /*
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ComputeDivision(decimal op1, decimal op2)
        {
            if (op2 == 0)
            {
                _logger.LogError($"{DateTime.Now} Exception: op2=0, division by 0");
                return BadRequest("op2 must be different from 0");
            }
            decimal result = decimal.Round(op1 / op2, 2);
            return Ok(result);


        }
        */


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<DeviceForReviewDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDevicesForReview(int? id, int? year, string? model, string? brand)
        {
            if (_context.Device == null)
            {
                _logger.LogError("Error: No existen dispositivos en la tabla");
                return NotFound();
            }

            var devices = await _context.Device
                .Include(d => d.Model)
                .Where(d =>
                    (!id.HasValue || d.Id == id) &&
                    (string.IsNullOrEmpty(brand) || d.Brand.Contains(brand)) &&
                    (!year.HasValue || d.Year == year) &&
                    (string.IsNullOrEmpty(model) || d.Model.Name.Contains(model)) 
                )
                .OrderBy(d => d.Year)
                .ThenBy(d => d.PriceForRent)
                .Select(d => new DeviceForReviewDTO(
                    d.Id,
                    d.Brand,
                    d.Name,
                    d.Year,
                     d.Model.Name,
                    d.Color
                
                ))
                .ToListAsync();

            if (!devices.Any()) 
            {
                _logger.LogWarning($"No se encontraron dispositivos con los filtros aplicados");
                return NotFound("No se encontraron dispositivos con los criterios de búsqueda");
            }

            return Ok(devices);
        }


        
    }


}



