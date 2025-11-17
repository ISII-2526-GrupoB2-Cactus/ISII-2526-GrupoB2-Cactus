using AppForSEII2526.API.DTOs.RentalDTOs;
using AppForSEII2526.API.DTOs.ReviewDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        //Get: Con el vamos a hacer las  consultas
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetReview(int id)
        {
            if (_context.Review == null)
            {
                _logger.LogError("Error: No existen reseñas en la tabla");
                return NotFound();
            }
            var review = await _context.Review
                .Where(r => r.ReviewId == id) //Filtro por id
                    .Include(r => r.ReviewItems) //Acceso a ReviewItems
                        .ThenInclude(ri => ri.Device) //Acceso a Device
                            .ThenInclude(d => d.Model) //Acceso a Model
                .Include(r => r.ApplicationUser)
                .Select(r => new ReviewDetailDTO(
                    //Nombre,pais,titulo y fecha de la reseña
                    r.ReviewId,
                    r.ReviewTitle, //Titulo de la reseña en Review 
                    r.ApplicationUser.CustomerCountry ?? "Desconocido", //Pais del cliente en ApplicationUser
                    r.ApplicationUser.UserName ?? "", //Nombre del cliente en ApplicationUser
                    r.DateOfReview, //Fecha de la reseña en Review 
                    r.ReviewItems //De cada dispositivo su nombre,modelo,año,puntuacion y comentario
                    .Select(ri => new ReviewItemDTO(
                        ri.Device.Id,
                        ri.Device.Name,
                        ri.Device.Model.Name,
                        ri.Device.Year,
                        ri.Rating,
                        ri.Comments)).ToList<ReviewItemDTO>()))
                .FirstOrDefaultAsync();

            if (review == null)
            {
                _logger.LogError($"Error: No se ha encontrado la reseña con id {id}");
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
            // 1️⃣ Validar que el DTO no sea nulo
            if (reviewForCreate == null)
            {
                ModelState.AddModelError("reviewForCreate", "El objeto reviewForCreate es requerido.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // 2️⃣ Validar usuario
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == reviewForCreate.CustomerUserName);

            if (user == null)
                ModelState.AddModelError("CustomerUserName", "Usuario no encontrado o no conectado.");

            // 3️⃣ Validaciones básicas
            if (string.IsNullOrWhiteSpace(reviewForCreate.Title))
                ModelState.AddModelError("Title", "El título de la reseña es obligatorio.");

            if (reviewForCreate.ReviewItems == null || reviewForCreate.ReviewItems.Count == 0)
                ModelState.AddModelError("ReviewItems", "Debes reseñar al menos un dispositivo.");

            // 4️⃣ Validar cada ReviewItem
            if (reviewForCreate.ReviewItems != null)
            {
                for (int i = 0; i < reviewForCreate.ReviewItems.Count; i++)
                {
                    var item = reviewForCreate.ReviewItems[i];

                    if (item.Rating < 1 || item.Rating > 5)
                        ModelState.AddModelError($"ReviewItems[{i}].Rating",
                            $"La puntuación debe estar entre 1 y 5.");

                    if (item.DeviceId <= 0)
                        ModelState.AddModelError($"ReviewItems[{i}].DeviceId",
                            $"El ID del dispositivo es requerido.");
                }
            }

            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // 5️⃣ Cargar y validar dispositivos
            var deviceIds = reviewForCreate.ReviewItems.Select(ri => ri.DeviceId).ToList();
            var devices = await _context.Device
                .Include(d => d.Model)
                .Where(d => deviceIds.Contains(d.Id))
                .ToListAsync();

            foreach (var item in reviewForCreate.ReviewItems)
            {
                if (!devices.Any(d => d.Id == item.DeviceId))
                    ModelState.AddModelError("ReviewItems",
                        $"El dispositivo con ID {item.DeviceId} no existe.");
            }

            if (!ModelState.IsValid)
                return BadRequest(new ValidationProblemDetails(ModelState));

            // 6️⃣ Crear Review
            var review = new Review
            {
                ReviewTitle = reviewForCreate.Title,
                CustomerId = user.Id,
                DateOfReview = DateTime.Now,
                ReviewItems = reviewForCreate.ReviewItems.Select(ri => new ReviewItem
                {
                    DeviceId = ri.DeviceId,
                    Rating = ri.Rating,
                    Comments = ri.Comments != null && ri.Comments.Length > 50
                             ? ri.Comments.Substring(0, 50)
                             : ri.Comments // Trunca a 50 caracteres
                }).ToList()
            };

            // 7️⃣ Guardar
            _context.Review.Add(review);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al guardar la reseña en la base de datos");
                return Conflict("Error al guardar la reseña: " + ex.InnerException?.Message ?? ex.Message);
            }

            // 8️⃣ Cargar la reseña guardada con sus relaciones
            var savedReview = await _context.Review
                .Include(r => r.ReviewItems)
                    .ThenInclude(ri => ri.Device)
                        .ThenInclude(d => d.Model)
                .Include(r => r.ApplicationUser)
                .FirstOrDefaultAsync(r => r.ReviewId == review.ReviewId);

            if (savedReview == null)
            {
                return Conflict("Error: No se pudo recuperar la reseña guardada.");
            }

            // 9️⃣ Crear DTO de respuesta
            var reviewItemDTOs = savedReview.ReviewItems.Select(ri =>
                new ReviewItemDTO(
                    ri.DeviceId,
                    ri.Device.Name,
                    ri.Device.Model.Name,
                    ri.Device.Year,
                    ri.Rating,
                    ri.Comments
                )).ToList();

            var result = new ReviewDetailDTO(
                savedReview.ReviewId,
                savedReview.ReviewTitle,
                reviewForCreate.CustomerCountry,
                user.UserName,
                savedReview.DateOfReview,
                reviewItemDTOs
            );

            return CreatedAtAction("GetReview", new { id = savedReview.ReviewId }, result);
        }


    }
}