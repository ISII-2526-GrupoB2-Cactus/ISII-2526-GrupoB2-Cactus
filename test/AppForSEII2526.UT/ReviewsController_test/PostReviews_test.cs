using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReviewDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.ReviewsController_test
{
    public class PostReviews_test : AppForSEII2526SqliteUT
    {
        private const string _userName = "lucia.romero@alu.uclm.es";
        private const string _customerNameSurname = "Lucia Romero";
        private const string _reviewTitle = "Excelente experiencia con los dispositivos";

        private const string _device1Name = "iPhone 15 Pro";
        private const string _device1Brand = "Apple";
        private const string _device1Model = "iPhone 15";
        private const string _device2Name = "Galaxy S24 Ultra";
        private const string _device2Brand = "Samsung";
        private const string _device2Model = "Galaxy S24";

        public PostReviews_test()
        {
            //////////////////////////////////////////////////////
            //PRUEBAS PARA COMPROBAR EL POST DEL CONTROLLER REVIEWS
            //////////////////////////////////////////////////////
            var user = new ApplicationUser
            {
                Id = "1",
                UserName = _userName,
                CustomerUserName = "Lucia",
                CustomerUserSurname = "Romero",
                Email = _userName
            };

            var model1 = new Model { Id = 1, Name = _device1Model };
            var model2 = new Model { Id = 2, Name = _device2Model };

            var devices = new List<Device>(){
                new Device {
                    Id = 1,
                    Brand = _device1Brand,
                    Color = "Black",
                    Name = _device1Name,
                    PriceForPurchase = 999.99,
                    PriceForRent = 49.99,
                    Year = 2023,
                    Quality = Device.QualityType.New,
                    QuantityForPurchase = 10,
                    QuantityForRent = 5,
                    Model = model1
                },
                new Device {
                    Id = 2,
                    Brand = _device2Brand,
                    Color = "White",
                    Name = _device2Name,
                    PriceForPurchase = 899.99,
                    PriceForRent = 39.99,
                    Year = 2024,
                    Quality = Device.QualityType.New,
                    QuantityForPurchase = 8,
                    QuantityForRent = 4,
                    Model = model2
                },
            };

            _context.Users.Add(user);
            _context.Model.AddRange(new[] { model1, model2 });
            _context.Device.AddRange(devices);
            _context.SaveChanges();
            //Creamos los objetos para guardar en la bd
        }

        public static IEnumerable<object[]> TestCasesFor_CreateReview()//Usamos el mismo test con distintas pruebas
        {
            // CASO 1: Review sin items
            var reviewNoItems = new ReviewForCreateDTO(_userName, _customerNameSurname,
                DateTime.Now.AddDays(-1), _reviewTitle, new List<ReviewItemDTO>());

            // CASO 2: Review con fecha futura  
            var reviewFutureDate = new ReviewForCreateDTO(_userName, _customerNameSurname,
                DateTime.Now.AddDays(1), _reviewTitle, new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 5, "Excelente dispositivo") // Usamos dispositivo que SÍ existe
                });

            // CASO 3: Usuario no registrado
            var reviewUserNotRegistered = new ReviewForCreateDTO("usuario.inexistente@uclm.es", _customerNameSurname,
                DateTime.Now.AddDays(-1), _reviewTitle, new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 5, "Excelente dispositivo") // Usamos dispositivo que SÍ existe
                });

            // CASO 4: Rating inválido (bajo) 
            var reviewInvalidRatingLow = new ReviewForCreateDTO(_userName, _customerNameSurname,
                DateTime.Now.AddDays(-1), _reviewTitle,
                new List<ReviewItemDTO>() {
                    new ReviewItemDTO(1, 0, "Muy mal dispositivo") // Dispositivo 1 SÍ existe
                });

            // CASO 5: Rating inválido (alto)   
            var reviewInvalidRatingHigh = new ReviewForCreateDTO(_userName, _customerNameSurname,
                DateTime.Now.AddDays(-1), _reviewTitle,
                new List<ReviewItemDTO>() {
                    new ReviewItemDTO(1, 6, "Dispositivo perfecto") // Dispositivo 1 SÍ existe
                });

            

            var allTests = new List<object[]>
            {
                new object[] { reviewNoItems, "Error! Debes incluir al menos un dispositivo para reseñar" },
                new object[] { reviewFutureDate, "Error! La fecha de reseña no puede ser futura" },
                new object[] { reviewUserNotRegistered, "Error! El nombre de usuario no está registrado" },
                new object[] { reviewInvalidRatingLow, "Error! La puntuación para el dispositivo 1 debe estar entre 1 y 5" },
                new object[] { reviewInvalidRatingHigh, "Error! La puntuación para el dispositivo 1 debe estar entre 1 y 5" },
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateReview))] //Va a ejecutar el metodo tantas veces como TestCases..haya
        public async Task CreateReview_Error_test(ReviewForCreateDTO reviewDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value); //Comprueba que el error que nos ha dado empieza por error

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
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

            var reviewDate = DateTime.Now.AddDays(-2);

            var reviewDTO = new ReviewForCreateDTO(_userName, _customerNameSurname,
                reviewDate, _reviewTitle, new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 5, "iPhone espectacular, la cámara es increíble")
                });

            // El ID esperado será 1 ya que es la primera review que se crea
            var expectedReviewDetailDTO = new ReviewDetailDTO(1, DateTime.Now,
                _userName, _customerNameSurname,
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 5, "iPhone espectacular, la cámara es increíble")
                });

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualReviewDetailDTO = Assert.IsType<ReviewDetailDTO>(createdResult.Value);

            // Usamos el Equals de los DTOs EXACTAMENTE como hace tu profesora
            Assert.Equal(expectedReviewDetailDTO, actualReviewDetailDTO);
        }
    }
}