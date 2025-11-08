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
    public class GetReviews_test : AppForSEII2526SqliteUT
    {
        private const string _userName = "lucia.garcia@uclm.es";
        private const string _customerUserName = "Lucía"; // CORREGIDO: Esto es lo que busca el controlador
        private const string _customerNameSurname = "Lucía García";
        private const string _device1Name = "Galaxy S22";
        private const string _device1Brand = "Samsung";
        private const string _device2Name = "iPhone 14";
        private const string _device2Brand = "Apple";

        public GetReviews_test()
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
                new Device(1, _device1Brand, "Negro", _device1Name, 850.0, 50.0, 2022, Device.QualityType.New, 5, 2) { Model = models[0] },
                new Device(2, _device2Brand, "Blanco", _device2Name, 950.0, 70.0, 2023, Device.QualityType.New, 3, 2) { Model = models[1] }
            };

            // Crear usuario - CORREGIDO: CustomerUserName debe coincidir con lo que busca el controlador
            ApplicationUser user = new ApplicationUser
            {
                Id = "1",
                UserName = _userName,
                Email = _userName,
                CustomerUserName = _customerUserName, // Esto es lo que busca el controlador
                CustomerUserSurname = "García"
            };

            // Crear review con múltiples items
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

            review.ReviewItems.Add(new ReviewItem("Excelente batería y rendimiento", 5, review.ReviewId, devices[0].Id) { Device = devices[0], Review = review });
            review.ReviewItems.Add(new ReviewItem("Muy buena cámara también", 4, review.ReviewId, devices[1].Id) { Device = devices[1], Review = review });

            // Guardar en contexto
            _context.Users.Add(user);
            _context.Model.AddRange(models);
            _context.Device.AddRange(devices);
            _context.Review.Add(review);
            _context.SaveChanges();
        }

        // ==========================================
        // TEST: Review no encontrada
        // ==========================================
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReview_NotFound_test()
        {
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

            var result = await controller.GetReview(0);

            Assert.IsType<NotFoundResult>(result);
        }

        // ==========================================
        // TEST: Review encontrada
        // ==========================================
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetReview_Found_test()
        {
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

            var result = await controller.GetReview(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualReview = Assert.IsType<ReviewDetailDTO>(okResult.Value);

            // Verificar propiedades principales
            Assert.Equal(1, actualReview.Id);
            Assert.Equal("Lucía", actualReview.CustomerUserName);
            Assert.Equal("Lucía García", actualReview.CustomerNameSurname);
            Assert.Equal(2, actualReview.ReviewItems.Count);
            Assert.Equal(4.5, actualReview.AverageRating);
        }

        // ==========================================
        // TEST: Review no encontrada con ID inválido
        // ==========================================
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetReview_NotFound_InvalidId_test()
        {
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

            var result = await controller.GetReview(999);

            Assert.IsType<NotFoundResult>(result);
        }

        // ==========================================
        // TEST: Verificar todas las propiedades del ReviewDetailDTO
        // ==========================================
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetReview_CheckAllProperties_test()
        {
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

            var result = await controller.GetReview(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var reviewDTOActual = Assert.IsType<ReviewDetailDTO>(okResult.Value);

            // Verificar propiedades principales
            Assert.Equal(1, reviewDTOActual.Id);
            Assert.Equal("Lucía", reviewDTOActual.CustomerUserName);
            Assert.Equal("Lucía García", reviewDTOActual.CustomerNameSurname);
            Assert.Equal(2, reviewDTOActual.ReviewItems.Count);
            Assert.Equal(4.5, reviewDTOActual.AverageRating);

            // Verificar primer item
            Assert.Equal(1, reviewDTOActual.ReviewItems[0].DeviceId);
            Assert.Equal(5, reviewDTOActual.ReviewItems[0].Rating);
            Assert.Equal("Excelente batería y rendimiento", reviewDTOActual.ReviewItems[0].Comments);

            // Verificar segundo item
            Assert.Equal(2, reviewDTOActual.ReviewItems[1].DeviceId);
            Assert.Equal(4, reviewDTOActual.ReviewItems[1].Rating);
            Assert.Equal("Muy buena cámara también", reviewDTOActual.ReviewItems[1].Comments);
        }

        // ==========================================
        // TEST: Review con un solo item
        // ==========================================
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetReview_SingleItem_test()
        {
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

            // Crear review nuevo con un solo item
            var newReview = new Review
            {
                ReviewId = 2,
                CustomerId = 1,
                DateOfReview = DateTime.Today,
                OverallRating = 3,
                ReviewTitle = "Review con un solo dispositivo",
                ApplicationUser = _context.Users.First(),
                ReviewItems = new List<ReviewItem>()
            };
            newReview.ReviewItems.Add(new ReviewItem("Buen dispositivo", 3, 2, 1)
            {
                Device = _context.Device.First(),
                Review = newReview
            });

            _context.Review.Add(newReview);
            _context.SaveChanges();

            var result = await controller.GetReview(2);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var reviewDTO = Assert.IsType<ReviewDetailDTO>(okResult.Value);

            Assert.Single(reviewDTO.ReviewItems);
            Assert.Equal(3, reviewDTO.ReviewItems[0].Rating);
            Assert.Equal("Buen dispositivo", reviewDTO.ReviewItems[0].Comments);
            Assert.Equal(3.0, reviewDTO.AverageRating);
        }

        // ==========================================
        // TEST: Review con comentarios null
        // ==========================================
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetReview_WithNullComments_test()
        {
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

            // Crear review con comentarios null
            var user = _context.Users.First();
            var device = _context.Device.First();

            var reviewWithNullComments = new Review
            {
                ReviewId = 3,
                CustomerId = 1,
                DateOfReview = DateTime.Today.AddDays(-1),
                OverallRating = 4,
                ReviewTitle = "Review con comentarios null",
                ApplicationUser = user,
                ReviewItems = new List<ReviewItem>()
            };
            reviewWithNullComments.ReviewItems.Add(new ReviewItem(null, 4, 3, device.Id)
            {
                Device = device,
                Review = reviewWithNullComments
            });

            _context.Review.Add(reviewWithNullComments);
            _context.SaveChanges();

            var result = await controller.GetReview(3);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var reviewDTO = Assert.IsType<ReviewDetailDTO>(okResult.Value);

            Assert.Equal("", reviewDTO.ReviewItems[0].Comments); // Debería convertir null a string.Empty
        }

        // ==========================================
        // TEST: Tabla Review vacía - usando Mock
        // ==========================================
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetReview_TableEmpty_test()
        {
            // Arrange - Mock para simular tabla Review vacía
            var mockContext = new Mock<ApplicationDbContext>();
            var mockReviewSet = new Mock<DbSet<Review>>();

            // Configurar el DbSet de Review para que sea null (simulando tabla vacía)
            mockContext.Setup(m => m.Review).Returns((DbSet<Review>)null);

            var mockLogger = new Mock<ILogger<ReviewsController>>();
            var controller = new ReviewsController(mockContext.Object, mockLogger.Object);

            // Act
            var result = await controller.GetReview(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}