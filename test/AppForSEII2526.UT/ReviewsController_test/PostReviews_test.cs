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
        private const string _userName = "carlos@uclm.es";
        private const string _customerName = "Carlos";
        private const string _customerCountry = "España";
        private const string _reviewTitle = "Excelente dispositivo para trabajo diario";
        private const string _validComment = "Reseña para un dispositivo muy eficiente"; //MODIFICACION EXAMEN 

        private const string _device1Name = "iPhone 15 Pro";
        private const string _device1Brand = "Apple";
        private const string _device1Model = "iPhone 15";
        private const string _device2Name = "Galaxy S24 Ultra";
        private const string _device2Brand = "Samsung";
        private const string _device2Model = "Galaxy S24";

        public PostReviews_test()
        {
            var models = new List<Model>() {
                new Model { Name = _device1Model },
                new Model { Name = _device2Model },
            };

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
                    Model = models[0]
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
                    Model = models[1]
                },
            };

            ApplicationUser user = new ApplicationUser
            {
                Id = "1",
                UserName = _userName,
                CustomerUserName = _customerName,
                CustomerUserSurname = "García López",
                Email = _userName,
                CustomerCountry = _customerCountry
            };

            _context.Users.Add(user);
            _context.AddRange(models);
            _context.AddRange(devices);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateReview()
        {
            var reviewNoItems = new ReviewForCreateDTO(_reviewTitle, _customerCountry, _userName, new List<ReviewItemDTO>());

            var reviewEmptyTitle = new ReviewForCreateDTO("", _customerCountry, _userName,
                new List<ReviewItemDTO>() { new ReviewItemDTO(1, _device1Name, _device1Model, 2023, 5, _validComment) });

            var reviewEmptyCountry = new ReviewForCreateDTO(_reviewTitle, "", _userName,
                new List<ReviewItemDTO>() { new ReviewItemDTO(1, _device1Name, _device1Model, 2023, 5, _validComment) });

            var reviewInvalidUser = new ReviewForCreateDTO(_reviewTitle, _customerCountry, "usuario.inexistente@uclm.es",
                new List<ReviewItemDTO>() { new ReviewItemDTO(1, _device1Name, _device1Model, 2023, 5, _validComment) });

            var reviewInvalidDevice = new ReviewForCreateDTO(_reviewTitle, _customerCountry, _userName,
                new List<ReviewItemDTO>() { new ReviewItemDTO(999, "Dispositivo Inexistente", "Modelo Inexistente", 2023, 5, _validComment) });

            var reviewLowRating = new ReviewForCreateDTO(_reviewTitle, _customerCountry, _userName,
                new List<ReviewItemDTO>() { new ReviewItemDTO(1, _device1Name, _device1Model, 2023, 0, _validComment) });

            var reviewHighRating = new ReviewForCreateDTO(_reviewTitle, _customerCountry, _userName,
                new List<ReviewItemDTO>() { new ReviewItemDTO(1, _device1Name, _device1Model, 2023, 6, _validComment) });

            /*MODIFICACION EXAMEN
             * 
             */
            var reviewInvalidComment = new ReviewForCreateDTO(_reviewTitle, _customerCountry, _userName,
                new List<ReviewItemDTO>() { new ReviewItemDTO(1, _device1Name, _device1Model, 2023, 5, "Comentario inválido") });

            var reviewNullComment = new ReviewForCreateDTO(_reviewTitle, _customerCountry, _userName,
                new List<ReviewItemDTO>() { new ReviewItemDTO(1, _device1Name, _device1Model, 2023, 5, null) });

            var allTests = new List<object[]>
            {
                new object[] { reviewNoItems, "Error! Debes incluir un dispositivo para reseñar" },
                new object[] { reviewEmptyTitle, "Error! El titulo no puede estar vacio" },
                new object[] { reviewEmptyCountry, "Error! El pais no puede estar vacio" },
                new object[] { reviewInvalidUser, "Error! Usuario no registrado" },
                new object[] { reviewInvalidDevice, "Error! El dispositivo con id 999 no existe" },
                new object[] { reviewLowRating, "Error! La puntuacion del dispositivo 'iPhone 15 Pro' debe estar entre 1 y 5" },
                new object[] { reviewHighRating, "Error! La puntuacion del dispositivo 'iPhone 15 Pro' debe estar entre 1 y 5" },
                new object[] { reviewInvalidComment, "Error! El comentario del dispositivo 'iPhone 15 Pro' debe empezar por 'Reseña para'" },
                new object[] { reviewNullComment, "Error! El comentario del dispositivo 'iPhone 15 Pro' debe empezar por 'Reseña para'" },
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
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

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
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

            var reviewItems = new List<ReviewItemDTO>()
            {
                new ReviewItemDTO(1, _device1Name, _device1Model, 2023, 5, _validComment)
            };

            var reviewDTO = new ReviewForCreateDTO(_reviewTitle, _customerCountry, _userName, reviewItems);

            var expectedReviewDetailDTO = new ReviewDetailDTO(
                _userName,
                _customerCountry,
                _reviewTitle,
                DateTime.Now,
                reviewItems
            )
            {
                Id = 1
            };

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualReviewDetailDTO = Assert.IsType<ReviewDetailDTO>(createdResult.Value);

            // Para que funcione el Equal, ajustamos la fecha del expected
            var expectedWithSameDate = new ReviewDetailDTO(
                _userName,
                _customerCountry,
                _reviewTitle,
                actualReviewDetailDTO.ReviewDate,
                reviewItems
            )
            {
                Id = actualReviewDetailDTO.Id
            };

            Assert.Equal(expectedWithSameDate, actualReviewDetailDTO);
        }
    }
}