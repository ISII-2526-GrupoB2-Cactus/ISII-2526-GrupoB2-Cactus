using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReviewDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.ReviewsController_test
{
    public class PostReviews_test : AppForSEII2526SqliteUT
    {
        private const string _userName = "lucia.garcia@uclm.es";
        private const string _customerUserName = "Lucía"; // CORREGIDO: Esto es lo que busca el controlador
        private const string _customerNameSurname = "Lucía García";
        private const string _device1Name = "Galaxy S22";
        private const string _device1Brand = "Samsung";
        private const string _device2Name = "iPhone 14";
        private const string _device2Brand = "Apple";

        public PostReviews_test()
        {
            // Crear modelos
            var models = new List<Model>()
            {
                new Model { Id = 1, Name = "Galaxy Series" },
                new Model { Id = 2, Name = "iPhone Series" }
            };

            // Crear dispositivos
            var devices = new List<Device>()
            {
                new Device(1, _device1Brand, "Negro", _device1Name, 850.0, 50.0, 2022, Device.QualityType.New, 5, 2),
                new Device(2, _device2Brand, "Blanco", _device2Name, 950.0, 70.0, 2023, Device.QualityType.New, 0, 2)
            };

            devices[0].Model = models[0];
            devices[1].Model = models[1];

            // Crear usuario - CORREGIDO: CustomerUserName debe coincidir con lo que busca el controlador
            ApplicationUser user = new ApplicationUser
            {
                Id = "1",
                UserName = _userName,
                Email = _userName,
                CustomerUserName = _customerUserName, // Esto es lo que busca el controlador
                CustomerUserSurname = "García"
            };

            // Crear review inicial (ya existente en el sistema)
            var review = new Review
            {
                ReviewId = 1,
                CustomerId = 1,
                DateOfReview = DateTime.Today.AddDays(-5),
                OverallRating = 4,
                ReviewTitle = "Muy buen dispositivo en general",
                ApplicationUser = user,
                ReviewItems = new List<ReviewItem>()
            };
            review.ReviewItems.Add(new ReviewItem("Excelente batería y rendimiento", 5, review.ReviewId, devices[0].Id)
            {
                Device = devices[0],
                Review = review
            });

            // Guardar en contexto
            _context.Users.Add(user);
            _context.Model.AddRange(models);
            _context.Device.AddRange(devices);
            _context.Review.Add(review);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateReview()
        {
            // CORREGIDO: Usar CustomerUserName en lugar de UserName
            var reviewNoItem = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today,
                "Reseña sin dispositivos",
                new List<ReviewItemDTO>()
            );

            var reviewFutureDate = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today.AddDays(5),
                "Reseña con fecha futura",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 5, "Buen dispositivo")
                }
            );

            var reviewUserNotRegistered = new ReviewForCreateDTO(
                "UsuarioInexistente", // CORREGIDO: CustomerUserName que no existe
                _customerNameSurname,
                DateTime.Today,
                "Usuario no registrado en el sistema",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 5, "Comentario válido")
                }
            );

            var allTests = new List<object[]>
            {
                new object[] { reviewNoItem, "ReviewItems" },
                new object[] { reviewFutureDate, "ReviewDate" },
                new object[] { reviewUserNotRegistered, "CustomerUserName" }
            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateReview))]
        public async Task CreateReview_InitialValidationErrors_test(ReviewForCreateDTO reviewDTO, string expectedErrorKey)
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
            Assert.Contains(expectedErrorKey, problemDetails.Errors.Keys);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_DeviceNotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            var reviewDTO = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today.AddDays(-1),
                "Review con dispositivo inexistente",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(999, 5, "Dispositivo que no existe")
                }
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            Assert.Contains("ReviewItems", problemDetails.Errors.Keys);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_RatingTooLow_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            var reviewDTO = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today.AddDays(-2),
                "Review con rating inválido",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 0, "Rating demasiado bajo")
                }
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            Assert.Contains("ReviewItems", problemDetails.Errors.Keys);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_RatingTooHigh_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            var reviewDTO = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today.AddDays(-2),
                "Review con rating inválido",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 6, "Rating demasiado alto")
                }
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            Assert.Contains("ReviewItems", problemDetails.Errors.Keys);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            var reviewDTO = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today.AddDays(-3),
                "Review completamente válida",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 5, "Excelente dispositivo"),
                    new ReviewItemDTO(2, 4, "Muy bueno también")
                }
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var reviewDetail = Assert.IsType<ReviewDetailDTO>(createdResult.Value);
            Assert.Equal(2, reviewDetail.Id);
            Assert.Equal(2, reviewDetail.ReviewItems.Count);
            Assert.Equal(4.5, reviewDetail.AverageRating);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_MultipleValidationErrors_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            // Review con múltiples errores
            var reviewDTO = new ReviewForCreateDTO(
                "UsuarioInexistente", // usuario no existe
                _customerNameSurname,
                DateTime.Today.AddDays(10), // fecha futura
                "Múltiples errores",
                new List<ReviewItemDTO>() // sin items
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            Assert.True(problemDetails.Errors.Count >= 2);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_WithNullComments_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            var reviewDTO = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today.AddDays(-1),
                "Review con comentarios null",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 5, null)
                }
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var reviewDetail = Assert.IsType<ReviewDetailDTO>(createdResult.Value);
            Assert.Equal(string.Empty, reviewDetail.ReviewItems[0].Comments);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_DatabaseException_test()
        {
            // Arrange - Mock para simular excepción en SaveChangesAsync
            var mockContext = new Mock<ApplicationDbContext>();

            // Configurar los DbSet necesarios
            var mockUsers = new Mock<DbSet<ApplicationUser>>();
            var mockDevices = new Mock<DbSet<Device>>();
            var mockReviews = new Mock<DbSet<Review>>();

            var user = _context.Users.First();
            var device = _context.Device.First();

            var usersList = new List<ApplicationUser> { user }.AsQueryable();
            var devicesList = new List<Device> { device }.AsQueryable();

            mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(usersList.Provider);
            mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(usersList.Expression);
            mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(usersList.ElementType);
            mockUsers.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(usersList.GetEnumerator());

            mockDevices.As<IQueryable<Device>>().Setup(m => m.Provider).Returns(devicesList.Provider);
            mockDevices.As<IQueryable<Device>>().Setup(m => m.Expression).Returns(devicesList.Expression);
            mockDevices.As<IQueryable<Device>>().Setup(m => m.ElementType).Returns(devicesList.ElementType);
            mockDevices.As<IQueryable<Device>>().Setup(m => m.GetEnumerator()).Returns(devicesList.GetEnumerator());

            mockContext.Setup(m => m.Users).Returns(mockUsers.Object);
            mockContext.Setup(m => m.Device).Returns(mockDevices.Object);
            mockContext.Setup(m => m.Review).Returns(mockReviews.Object);

            // Simular excepción en SaveChangesAsync
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new Exception("Database error"));

            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(mockContext.Object, mockLogger.Object);

            var reviewDTO = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today,
                "Test database exception",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 5, "Test")
                }
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.IsType<string>(conflictResult.Value);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_SingleItem_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            var reviewDTO = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today.AddDays(-1),
                "Review con un solo item",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 3, "Buen dispositivo")
                }
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var reviewDetail = Assert.IsType<ReviewDetailDTO>(createdResult.Value);
            Assert.Single(reviewDetail.ReviewItems);
            Assert.Equal(3.0, reviewDetail.AverageRating);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_OverallRatingCalculation_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            var reviewDTO = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today.AddDays(-1),
                "Prueba de cálculo de rating",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 1, "Muy malo"),
                    new ReviewItemDTO(2, 5, "Excelente")
                }
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var reviewDetail = Assert.IsType<ReviewDetailDTO>(createdResult.Value);
            Assert.Equal(3.0, reviewDetail.AverageRating);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_AllValidationsPass_CreatedAtAction_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            var reviewDTO = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today.AddDays(-5),
                "Review exitosa",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 4, "Muy bueno")
                }
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetReview", createdResult.ActionName);
            Assert.Equal(2, createdResult.RouteValues["id"]);
        }

        // TEST NUEVO: Para cubrir el caso donde no hay dispositivos en la validación
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateReview_MultipleDevicesValidation_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(_context, mock.Object);

            var reviewDTO = new ReviewForCreateDTO(
                _customerUserName, // CORREGIDO
                _customerNameSurname,
                DateTime.Today.AddDays(-1),
                "Review con múltiples validaciones",
                new List<ReviewItemDTO>()
                {
                    new ReviewItemDTO(1, 5, "Válido"),
                    new ReviewItemDTO(999, 6, "Inválido - dispositivo no existe y rating alto")
                }
            );

            // Act
            var result = await controller.CreateReview(reviewDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            Assert.Contains("ReviewItems", problemDetails.Errors.Keys);
        }
    }
}