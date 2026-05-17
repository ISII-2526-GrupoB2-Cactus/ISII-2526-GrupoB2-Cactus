using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private ILogger _logger;

        public ModelsController(ApplicationDbContext context, ILogger<ModelsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET:
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetModels(string? modelName)
        {

            IList<string> models = await _context.Model
                .Where(model => (modelName == null || model.Name.Contains(modelName))) // where clause             
                .OrderBy(model => model.Name)
                .Select(model => model.Name)
                .ToListAsync();

            return Ok(models);
        }

    }
}
