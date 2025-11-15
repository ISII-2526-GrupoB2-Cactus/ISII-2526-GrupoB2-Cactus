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
        private const string _userName = "Lucia";  // CustomerUserName, no email
        private const string _customerNameSurname = "Lucia Romero";

        public PostReview_test()
        {
            var user = new ApplicationUser
            {
                Id = "1",
                CustomerUserName = "Lucia",  // Esto es lo que busca el controller
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
                    Brand = "Apple",
                    Color = "Negro",
                    Name = "iPhone 15 Pro",
                    PriceForPurchase = 1200,
                    QuantityForPurchase = 5,
                    Year = 2023,
                    Model = model1
                },
                new Device
                {
                    Id = 2,
                    Brand = "Samsung",
                    Color = "Plateado",
                    Name = "Samsung Galaxy S24",
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
            // CASO 1: Review sin items - DEBE SER EL PRIMERO
            yield return new object[]
            {
                new ReviewForCreateDTO(
                    "Lucia",  // CustomerUserName correcto
                    "Lucia Romero",
                    DateTime.Now.AddDays(-1),
                    "Review sin items de al menos 10 caracteres",  // Mínimo 10 caracteres
                    new List<ReviewItemDTO>()
                ),
                "Error! Debes incluir al menos un dispositivo para reseñar"
            };

            // CASO 2: Fecha futura - SEGUNDO
            yield return new object[]
            {
                new ReviewForCreateDTO(
                    "Lucia",
                    "Lucia Romero",
                    DateTime.Now.AddDays(1),  // Fecha futura
                    "Review futura titulo largo suficiente",
                    new List<ReviewItemDTO>()
                    {
                        new ReviewItemDTO(1, 4, "Buen dispositivo")
                    }
                ),
                "Error! La fecha de reseña no puede ser futura"
            };

            // CASO 3: Usuario no registrado - TERCERO
            yield return new object[]
            {
                new ReviewForCreateDTO(
                    "UsuarioInexistente",  // CustomerUserName que no existe
                    "Usuario Inexistente",
                    DateTime.Now.AddDays(-1),
                    "Review usuario titulo largo suficiente",
                    new List<ReviewItemDTO>()
                    {
                        new ReviewItemDTO(1, 4, "Comentario")
                    }
                ),
                "Error! El nombre de usuario no está registrado"
            };

            // CASO 4: Dispositivo no existe - CUARTO
            yield return new object[]
            {
                new ReviewForCreateDTO(
                    "Lucia",
                    "Lucia Romero",
                    DateTime.Now.AddDays(-1),
                    "Review dispositivo titulo largo suficiente",
                    new List<ReviewItemDTO>()
                    {
                        new ReviewItemDTO(999, 4, "Dispositivo que no existe")
                    }
                ),
                "Error! El dispositivo con ID 999 no existe"
            };

            // CASO 5: Rating menor a 1 - QUINTO
            yield return new object[]
            {
                new ReviewForCreateDTO(
                    "Lucia",
                    "Lucia Romero",
                    DateTime.Now.AddDays(-1),
                    "Review rating bajo titulo largo",
                    new List<ReviewItemDTO>()
                    {
                        new ReviewItemDTO(1, 0, "Rating muy bajo")
                    }
                ),
                "Error! La puntuación para el dispositivo 1 debe estar entre 1 y 5"
            };

            // CASO 6: Rating mayor a 5 - SEXTO
            yield return new object[]
            {
                new ReviewForCreateDTO(
                    "Lucia",
                    "Lucia Romero",
                    DateTime.Now.AddDays(-1),
                    "Review rating alto titulo largo",
                    new List<ReviewItemDTO>()
                    {
                        new ReviewItemDTO(1, 6, "Rating muy alto")
                    }
                ),
                "Error! La puntuación para el dispositivo 1 debe estar entre 1 y 5"
            };
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

            // Buscar el error en todos los errores disponibles
            var allErrors = problemDetails.Errors.Values.SelectMany(errors => errors).ToList();

            // Verificar que existe algún error que contenga el texto esperado
            Assert.Contains(allErrors, error => error.Contains(errorExpected));
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            var reviewDate = DateTime.Now.AddDays(-2);
            var reviewDTO = new ReviewForCreateDTO(
                "Lucia",  // CustomerUserName correcto
                "Lucia Romero",
                reviewDate,
                "Excelente dispositivo Samsung con titulo largo",  // Más de 10 caracteres
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(2, 5, "Samsung espectacular")
                }
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualReviewDetailDTO = Assert.IsType<ReviewDetailDTO>(createdResult.Value);

            Assert.Equal("Lucia", actualReviewDetailDTO.CustomerUserName);
            Assert.Equal("Lucia Romero", actualReviewDetailDTO.CustomerNameSurname);
            Assert.Single(actualReviewDetailDTO.ReviewItems);
            Assert.Equal(2, actualReviewDetailDTO.ReviewItems.First().DeviceId);
            Assert.Equal(5, actualReviewDetailDTO.ReviewItems.First().Rating);
            Assert.Equal(5.0, actualReviewDetailDTO.AverageRating);
        }
    }
}