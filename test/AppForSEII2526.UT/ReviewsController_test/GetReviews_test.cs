using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReviewDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AppForSEII2526.UT.ReviewsController_test
{
    public class GetReview_test : AppForSEII2526SqliteUT
    {
        public GetReview_test()
        {
            //////////////////////////////////////////////////////
            //PRUEBAS PARA COMPROBAR EL GET DEL CONTROLLER REVIEWS
            //////////////////////////////////////////////////////
            var user = new ApplicationUser //Creamos los objetos para guardar en la bd
            {
                Id = "1",
                CustomerUserName = "Lucia",
                CustomerUserSurname = "Romero",
                Email = "lucia.romero@alu.uclm.es",
                UserName = "lucia.romero@alu.uclm.es"
            };

            var model = new Model
            {
                Id = 1,
                Name = "iPhone 15"
            };

            var device = new Device
            {
                Id = 1,
                Brand = "Apple",
                Color = "Negro",
                Name = "iPhone 15 Pro",
                PriceForPurchase = 1200,
                PriceForRent = 50,
                Year = 2023,
                Quality = Device.QualityType.New,
                QuantityForPurchase = 5,
                QuantityForRent = 3,
                Model = model //Creamos el dispositivo asociado al model de antes
            };

            var review = new Review
            {
                ReviewId = 1,
                CustomerId = "1",
                DateOfReview = DateTime.Now.AddDays(-5),
                OverallRating = 5,
                ReviewTitle = "Excelente smartphone",
                ApplicationUser = user
            };

            var reviewItem = new ReviewItem
            {
                DeviceId = device.Id,
                ReviewId = review.ReviewId,
                Rating = 5,
                Comments = "iPhone espectacular, la cámara es increíble",
                Device = device,
                Review = review
            };

            review.ReviewItems = new List<ReviewItem> { reviewItem }; //Junta el reviewItem con la review

            _context.Users.Add(user); //Añadimos los objetos a la bd
            _context.Model.Add(model);
            _context.Device.Add(device);
            _context.Review.Add(review);
            _context.ReviewItem.Add(reviewItem);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReview_NotFound_test() //No devuelve nada porque es un test
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>(); //Crea un objeto falso
            ILogger<ReviewsController> logger = mock.Object;

            var controller = new ReviewsController(_context, logger); //Creamos el controlador real y le pasamos la bd y el logger falso

            // Act: Le vamos a pasar un id 0 que no existe y tiene que devolver (No encontrado)
            var result = await controller.GetReview(0);

            //Assert: Mira si el resultado es no econtrado
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetReview_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

            var expectedReview = new ReviewDetailDTO(1, DateTime.Now.AddDays(-5), "Lucia", "Lucia Romero", 
                        new List<ReviewItemDTO>());
            expectedReview.ReviewItems.Add(new ReviewItemDTO(1, 5, "iPhone espectacular, la cámara es increíble"));//Creamos un objeto reviewDetailDTO esperado

            // Act 
            var result = await controller.GetReview(1); //Buscamos la review 1 y como existe tiene que devolver lo esperado

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result); //Mira si el resultado es OkObjectResult
            var reviewDTOActual = Assert.IsType<ReviewDetailDTO>(okResult.Value); //Mira que sea tipo DTO
            var eq = expectedReview.Equals(reviewDTOActual);
            Assert.Equal(expectedReview, reviewDTOActual); //Mira si el DTO coincide con expectedReview
        }
    }
}