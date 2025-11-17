using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReviewDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.ReviewsController_test
{
    public class PostReview_test : AppForSEII2526SqliteUT
    {

        private const string _userName = "Lucia";
        private const string _customerNameSurname = "Lucia Romero";
        private const string _device1Name = "iPhone 15 Pro";
        private const string _device1Brand = "Apple";
        private const string _device2Name = "Samsung Galaxy S24";
        private const string _device2Brand = "Samsung";

        public PostReview_test()
        {
            var user = new ApplicationUser
            {
                Id = "1",
                CustomerUserName = _userName,
                CustomerUserSurname = "Romero",
                Email = "lucia.romero@alu.uclm.es",
                UserName = "lucia.romero@alu.uclm.es"
            };

            var model1 = new Model { Id = 1, Name = "iPhone 15" };
            var model2 = new Model { Id = 2, Name = "Galaxy S" };

            var devices = new List<Device>()
            {
                new Device
                {
                    Id = 1,
                    Brand = _device1Brand,
                    Color = "Negro",
                    Name = _device1Name,
                    PriceForPurchase = 1200,
                    QuantityForPurchase = 5,
                    Year = 2023,
                    Model = model1
                },
                new Device
                {
                    Id = 2,
                    Brand = _device2Brand,
                    Color = "Plateado",
                    Name = _device2Name,
                    PriceForPurchase = 1000,
                    QuantityForPurchase = 3,
                    Year = 2024,
                    Model = model2
                }
            };

            _context.Users.Add(user);
            _context.AddRange(model1, model2);
            _context.Device.AddRange(devices);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateReview()
        {
            // Review sin items
            ReviewForCreateDTO reviewNoItems = new ReviewForCreateDTO(
                _userName,
                _customerNameSurname,
                DateTime.Now.AddDays(-1),
                "Review sin items de al menos 10 caracteres",
                new List<ReviewItemDTO>()
            );

            // Fecha futura
            IList<ReviewItemDTO> reviewItems = new List<ReviewItemDTO>() {
                new ReviewItemDTO(1, 4, "Buen dispositivo")
            };

            ReviewForCreateDTO reviewFutureDate = new ReviewForCreateDTO(
                _userName,
                _customerNameSurname,
                DateTime.Now.AddDays(1),
                "Review futura titulo largo suficiente",
                reviewItems
            );

            // Usuario no registrado
            ReviewForCreateDTO applicationUserNotRegistered = new ReviewForCreateDTO(
                "UsuarioInexistente",
                "Usuario Inexistente",
                DateTime.Now.AddDays(-1),
                "Review usuario titulo largo suficiente",
                reviewItems
            );

            // Dispositivo no existe
            ReviewForCreateDTO reviewDeviceNotExists = new ReviewForCreateDTO(
                _userName,
                _customerNameSurname,
                DateTime.Now.AddDays(-1),
                "Review dispositivo titulo largo suficiente",
                new List<ReviewItemDTO>() {
                    new ReviewItemDTO(999, 4, "Dispositivo que no existe")
                }
            );

            // Rating menor a 1
            ReviewForCreateDTO reviewRatingTooLow = new ReviewForCreateDTO(
                _userName,
                _customerNameSurname,
                DateTime.Now.AddDays(-1),
                "Review rating bajo titulo largo",
                new List<ReviewItemDTO>() {
                    new ReviewItemDTO(1, 0, "Rating muy bajo")
                }
            );

            // Rating mayor a 5
            ReviewForCreateDTO reviewRatingTooHigh = new ReviewForCreateDTO(
                _userName,
                _customerNameSurname,
                DateTime.Now.AddDays(-1),
                "Review rating alto titulo largo",
                new List<ReviewItemDTO>() {
                    new ReviewItemDTO(1, 6, "Rating muy alto")
                }
            );

            var allTests = new List<object[]>
            {
                new object[] { reviewNoItems, "Error! Debes incluir al menos un dispositivo para reseñar" },
                new object[] { reviewFutureDate, "Error! La fecha de reseña no puede ser futura" },
                new object[] { applicationUserNotRegistered, "Error! El nombre de usuario no está registrado" },
                new object[] { reviewDeviceNotExists, "Error! El dispositivo con ID 999 no existe" },
                new object[] { reviewRatingTooLow, "Error! La puntuación para el dispositivo 1 debe estar entre 1 y 5" },
                new object[] { reviewRatingTooHigh, "Error! La puntuación para el dispositivo 1 debe estar entre 1 y 5" }
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateReview))]
        public async Task CreateReview_Error_test(ReviewForCreateDTO reviewDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];


            Assert.StartsWith(errorExpected, errorActual);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);


            DateTime reviewDate = DateTime.Now.AddDays(-2);

            ReviewForCreateDTO reviewDTO = new ReviewForCreateDTO(
                _userName,
                _customerNameSurname,
                reviewDate,
                "Excelente dispositivo Samsung con titulo largo",
                new List<ReviewItemDTO>());
            reviewDTO.ReviewItems.Add(new ReviewItemDTO(2, 5, "Samsung espectacular"));


            ReviewDetailDTO expectedReviewDetailDTO = new ReviewDetailDTO(
                1,
                reviewDate,
                _userName,
                _customerNameSurname,
                new List<ReviewItemDTO>());
            expectedReviewDetailDTO.ReviewItems.Add(new ReviewItemDTO(2, 5, "Samsung espectacular"));

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualReviewDetailDTO = Assert.IsType<ReviewDetailDTO>(createdResult.Value);

            Assert.Equal(expectedReviewDetailDTO, actualReviewDetailDTO);
        }
    }
}