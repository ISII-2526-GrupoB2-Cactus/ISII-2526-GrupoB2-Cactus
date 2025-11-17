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
            var user = new ApplicationUser
            {
                Id = "1",
                CustomerUserName = "Lucia",
                CustomerUserSurname = "Romero",
                Email = "lucia.romero@alu.uclm.es",
                UserName = "lucia.romero@alu.uclm.es"
            };

            var model = new Model { Id = 1, Name = "iPhone 15" };

            var device = new Device
            {
                Id = 1,
                Brand = "Apple",
                Color = "Negro",
                Name = "iPhone 15 Pro",
                PriceForPurchase = 1200,
                QuantityForPurchase = 5,
                Year = 2023,
                Model = model
            };

            var review = new Review
            {
                ReviewId = 1,
                DateOfReview = DateTime.Now.AddDays(-5),
                OverallRating = 5,
                ReviewTitle = "Excelente smartphone",
                ApplicationUser = user,
                ReviewItems = new List<ReviewItem>
                {
                    new ReviewItem
                    {
                        DeviceId = device.Id,
                        Rating = 5,
                        Comments = "iPhone espectacular, la cámara es increíble"
                    }
                }
            };

            _context.Add(user);
            _context.Add(model);
            _context.Add(device);
            _context.Review.Add(review);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReview_OK_test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            // Obtener la review de la base de datos para tener los datos reales
            var reviewFromDb = _context.Review.First();

            // Crear el DTO esperado completo con las propiedades CORRECTAS
            var expectedReview = new ReviewDetailDTO
            {
                Id = 1,
                CustomerUserName = "Lucia",
                CustomerNameSurname = "Lucia Romero", //Me aparece un error aqui en la development 
                ReviewDate = reviewFromDb.DateOfReview,
                ReviewItems = new List<ReviewItemDTO>
                {
                    new ReviewItemDTO
                    {
                        DeviceId = 1,
                        Rating = 5,
                        Comments = "iPhone espectacular, la cámara es increíble"
                    }
                }
            };

            // Act
            var result = await controller.GetReview(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualReview = Assert.IsType<ReviewDetailDTO>(okResult.Value);

            //Cambio en el equals para comparar los DTOs
            Assert.Equal(expectedReview, actualReview);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReview_NotFound_test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mockLogger.Object);

            // Act
            var result = await controller.GetReview(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}