using AppForSEII2526.API.DTOs.ReviewDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        [Route("[action]/{id}")]
        [ProducesResponseType(typeof(ReviewDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetReview(int id)
        {
            if (_context.Review == null)
            {
                _logger.LogError("Error: No existen reviews en la tabla");
                return NotFound();
            }

            var review = await _context.Review
                .Where(r => r.ReviewId == id)
                .Include(r => r.ReviewItems)
                    .ThenInclude(ri => ri.Device)
                        .ThenInclude(d => d.Model)
                .Include(r => r.ApplicationUser)
                .Select(r => new ReviewDetailDTO(
                    r.ReviewId,
                    r.DateOfReview,
                    r.ApplicationUser.CustomerUserName,  
                    $"{r.ApplicationUser.CustomerUserName} {r.ApplicationUser.CustomerUserSurname}", 
                    r.ReviewItems.Select(ri => new ReviewItemDTO(
                        ri.DeviceId,
                        ri.Rating,
                        ri.Comments ?? ""
                    )).ToList<ReviewItemDTO>()
                ))
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
            if (reviewForCreate.ReviewDate > DateTime.Now)
                ModelState.AddModelError("ReviewDate", "Error! La fecha de reseña no puede ser futura");

            if (reviewForCreate.ReviewItems.Count == 0)
                ModelState.AddModelError("ReviewItems", "Error! Debes incluir al menos un dispositivo para reseñar");

          
            var user = await _context.Users
                .FirstOrDefaultAsync(au => au.CustomerUserName == reviewForCreate.CustomerUserName);
            if (user == null)
            {
                ModelState.AddModelError("CustomerUserName", "Error! El nombre de usuario no está registrado");
            }

           
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

           
            var deviceIds = reviewForCreate.ReviewItems.Select(ri => ri.DeviceId).ToList();
            var devices = await _context.Device
                .Where(d => deviceIds.Contains(d.Id))
                .Select(d => d.Id)
                .ToListAsync();

            
            foreach (var deviceId in deviceIds)
            {
                if (!devices.Contains(deviceId))
                {
                    ModelState.AddModelError("ReviewItems", $"Error! El dispositivo con ID {deviceId} no existe");
                }
            }

            
            foreach (var item in reviewForCreate.ReviewItems)
            {
                if (item.Rating < 1 || item.Rating > 5)
                {
                    ModelState.AddModelError("ReviewItems", $"Error! La puntuación para el dispositivo {item.DeviceId} debe estar entre 1 y 5");
                }
            }

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));

           
            var overallRating = reviewForCreate.ReviewItems.Any() ?
                (int)Math.Round(reviewForCreate.ReviewItems.Average(ri => ri.Rating)) : 0;

          
            var review = new Review
            {
                DateOfReview = reviewForCreate.ReviewDate,
                OverallRating = overallRating,
                ReviewTitle = reviewForCreate.ReviewTitle,
                ReviewItems = new List<ReviewItem>(),
                ApplicationUser = user  
            };

           
            foreach (var item in reviewForCreate.ReviewItems)
            {
                var reviewItem = new ReviewItem
                {
                    DeviceId = item.DeviceId,
                    Rating = item.Rating,
                    Comments = item.Comments ?? string.Empty,
                    Review = review
                };
                review.ReviewItems.Add(reviewItem);
            }

            _context.Review.Add(review);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Reseña creada exitosamente con ID: {review.ReviewId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la reseña en la base de datos");
                ModelState.AddModelError("Review", "Error! Hubo un problema al guardar la reseña, por favor intente más tarde");
                return Conflict($"Error: {ex.Message}");
            }

            var reviewDetail = new ReviewDetailDTO(
                review.ReviewId,
                review.DateOfReview,
                reviewForCreate.CustomerUserName,
                $"{user.CustomerUserName} {user.CustomerUserSurname}",
                reviewForCreate.ReviewItems
            );

            return CreatedAtAction("GetReview", new { id = review.ReviewId }, reviewDetail);
        }

    }
}