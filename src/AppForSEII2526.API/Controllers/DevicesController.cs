using AppForSEII2526.API.DTOs.DeviceDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Drawing2D;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DevicesController> _logger;

        public DevicesController(ApplicationDbContext context, ILogger<DevicesController> logger)
        {
            _context = context;
            _logger = logger;
        }


        
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<DeviceParaAlquilarDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetDevicesForReview(string? model, double? priceForRent)
        {
            var devices = await _context.Device
                .Include(d => d.Model)
                .Where(d =>
                    ((string.IsNullOrEmpty(model)) || d.Model.Name.Contains(model)) &&
                    ((priceForRent == null) || d.PriceForRent == priceForRent)
                )
                .OrderBy(d => d.PriceForRent)
                //.ThenBy(d => d.Year)
                .Select(d => new DeviceParaAlquilarDTO(
                    d.Id,
                    d.Name,
                    d.Model,
                    d.Brand,
                    d.Year,
                    d.Color,
                    d.PriceForRent
                ))
                .ToListAsync();

            return Ok(devices);
        }
        


      




    }
}

