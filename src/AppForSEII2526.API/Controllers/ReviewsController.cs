using AppForSEII2526.API.DTOs.ReviewDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(ApplicationDbContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        //que no sea nulo el comentario y que no empiece por reseña para
        [HttpGet]
        [Route("[action]/{id}")]
        [ProducesResponseType(typeof(ReviewDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetReview(int id)
        {
            if (_context.Review == null)
            {
                _logger.LogError("Error: No existen Reseñas en la tabla");
                return NotFound();
            }

            var review = await _context.Review
                .Where(r => r.ReviewId == id)
                .Include(r => r.ReviewItems)
                    .ThenInclude(ri => ri.Device)
                        .ThenInclude(d => d.Model)
                .Include(r => r.ApplicationUser)
                .Select(r => new ReviewDetailDTO(
                    r.ApplicationUser.CustomerUserName,
                    r.ApplicationUser.CustomerCountry,
                    r.ReviewTitle,
                    r.DateOfReview,
                    r.ReviewItems
                        .Select(ri => new ReviewItemDTO(
                            ri.DeviceId,
                            ri.Device.Name,
                            ri.Device.Model.Name,
                            ri.Device.Year,
                            ri.Rating,
                            ri.Comments))
                        .ToList<ReviewItemDTO>())
                {
                    Id = r.ReviewId
                })
                .FirstOrDefaultAsync();

            if (review == null)
            {
                _logger.LogError($"Error: No existe ninguna review con id {id}");
                return NotFound();
            }

            return Ok(review);
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ReviewDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateReview(ReviewForCreateDTO reviewForCreate)
        {
            // Validaciones como en el ejemplo
            if (string.IsNullOrWhiteSpace(reviewForCreate.ReviewTitle))
                ModelState.AddModelError("ReviewTitle", "Error! El titulo no puede estar vacio");

            if (string.IsNullOrWhiteSpace(reviewForCreate.CustomerCountry))
                ModelState.AddModelError("CustomerCountry", "Error! El pais no puede estar vacio");

            if (reviewForCreate.ReviewItems.Count == 0)
                ModelState.AddModelError("ReviewItems", "Error! Debes incluir un dispositivo para reseñar");

            var user = _context.Users.FirstOrDefault(au => au.UserName == reviewForCreate.CustomerUserName);
            if (user == null)
                ModelState.AddModelError("ReviewApplicationUser", "Error! Usuario no registrado");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // Obtener dispositivos 
            var deviceIds = reviewForCreate.ReviewItems.Select(ri => ri.Id).ToList();

            var devices = _context.Device
                .Include(d => d.Model)
                .Where(d => deviceIds.Contains(d.Id))
                
                .Select(d => new {
                    d.Id,
                    d.Name,
                    ModelName = d.Model.Name,
                    d.Year
                })
                .ToList();

            
            Review review = new Review(
                customerId: reviewForCreate.CustomerUserName,
                dateOfReview: DateTime.Now,
                overallRating: reviewForCreate.AverageRating,
                reviewTitle: reviewForCreate.ReviewTitle,
                reviewItems: new List<ReviewItem>(), // Lista vacía inicial
                user: user
            );

            
            foreach (var item in reviewForCreate.ReviewItems)
            {
                var device = devices.FirstOrDefault(d => d.Id == item.Id);

                if (device == null)
                {
                    ModelState.AddModelError("ReviewItems", $"Error! El dispositivo con id {item.Id} no existe");
                }
                else if (item.Rating < 1 || item.Rating > 5)
                {
                    ModelState.AddModelError("ReviewItems", $"Error! La puntuacion del dispositivo '{device.Name}' debe estar entre 1 y 5");
                }
                /*
                 *EJERCICIO EXAMEN (MODIFICACION SPRINT2) 
                 */
                else if (item.Comments ==null  || !item.Comments.StartsWith("Reseña para"))
                {
                    ModelState.AddModelError("ReviewItems", $"Error! El comentario del dispositivo '{device.Name}' debe empezar por 'Reseña para'");
                }
                else
                {
                    
                    var reviewItem = new ReviewItem(
                        deviceId: device.Id,
                        rating: item.Rating,
                        comments: item.Comments
                    )
                    {
                        Review = review 
                    };

                    review.ReviewItems.Add(reviewItem);
                }
            }

            
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            _context.Review.Add(review);

            try
            {
                // Guardar tanto Review como sus ReviewItems 
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la reseña");
                
                var innerException = ex.InnerException != null ? ex.InnerException.Message : "No inner exception";
                _logger.LogError($"Inner exception: {innerException}");

                ModelState.AddModelError("Review", $"Error! Ha ocurrido un error,prueba mas tarde");
                return Conflict($"Error: {ex.Message} - Inner: {innerException}");
            }

            
            var savedReview = await _context.Review
                .Include(r => r.ReviewItems)
                .ThenInclude(ri => ri.Device)
                .ThenInclude(d => d.Model)
                .FirstOrDefaultAsync(r => r.ReviewId == review.ReviewId);

            
            var reviewDetail = new ReviewDetailDTO(
                customerUserName: savedReview.CustomerId,
                customerCountry: reviewForCreate.CustomerCountry,
                reviewTitle: savedReview.ReviewTitle,
                reviewDate: savedReview.DateOfReview,
                reviewItems: savedReview.ReviewItems.Select(ri => new ReviewItemDTO(
                    id: ri.DeviceId,
                    name: ri.Device.Name,
                    model: ri.Device.Model.Name,
                    year: ri.Device.Year,
                    rating: ri.Rating,
                    comments: ri.Comments
                )).ToList()
            )
            {
                Id = savedReview.ReviewId
            };

            return CreatedAtAction("GetReview", new { id = savedReview.ReviewId }, reviewDetail);
        }
    }
}
