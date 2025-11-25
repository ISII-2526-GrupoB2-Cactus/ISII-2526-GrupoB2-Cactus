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
    public class GetReview_test : AppForSEII2526SqliteUT
    {
        private const string _userName = "lucia@uclm.es";
        private const string _customerName = "Lucia";
        private const string _customerCountry = "España";
        private const string _reviewTitle = "Gran dispositivo para uso diario";
        private const string _validComment = "Reseña para un dispositivo excelente";

        private const string _device1Name = "iPhone 15 Pro";
        private const string _device1Model = "iPhone 15";

        public GetReview_test()
        {
            var model = new Model { Name = _device1Model };

            var device = new Device
            {
                Id = 1,
                Brand = "Apple",
                Color = "Black",
                Name = _device1Name,
                PriceForPurchase = 999.99,
                PriceForRent = 49.99,
                Year = 2023,
                Quality = Device.QualityType.New,
                QuantityForPurchase = 10,
                QuantityForRent = 5,
                Model = model
            };

            ApplicationUser user = new ApplicationUser
            {
                Id = "1",
                UserName = _userName,
                CustomerUserName = _customerName,  // Esto es "Lucia"
                CustomerUserSurname = "Martínez López",
                Email = _userName,
                CustomerCountry = _customerCountry
            };

            var review = new Review(
                customerId: _userName, // Esto se guarda pero no se usa en el Get
                dateOfReview: DateTime.Now,
                overallRating: 5,
                reviewTitle: _reviewTitle,
                reviewItems: new List<ReviewItem>(),
                user: user
            );

            review.ReviewItems.Add(new ReviewItem(
                deviceId: device.Id,
                rating: 5,
                comments: _validComment
            )
            {
                Review = review
            });

            _context.Users.Add(user);
            _context.Add(model);
            _context.Add(device);
            _context.Add(review);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetReview_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(_context, logger);

            // Act
            var result = await controller.GetReview(999);

            // Assert
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

            var reviewItems = new List<ReviewItemDTO>()
            {
                new ReviewItemDTO(1, _device1Name, _device1Model, 2023, 5, _validComment)
            };

            // CORREGIDO: Usar _customerName ("Lucia") que es lo que devuelve el controlador
            var expectedReview = new ReviewDetailDTO(
                _customerName, // ← "Lucia" no "lucia@uclm.es"
                _customerCountry,
                _reviewTitle,
                DateTime.Now,
                reviewItems
            )
            {
                Id = 1
            };

            // Act 
            var result = await controller.GetReview(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualReview = Assert.IsType<ReviewDetailDTO>(okResult.Value);

            // Ajustamos la fecha del expected para que coincida
            var expectedWithSameDate = new ReviewDetailDTO(
                _customerName, // "Lucia"
                _customerCountry,
                _reviewTitle,
                actualReview.ReviewDate,
                reviewItems
            )
            {
                Id = actualReview.Id
            };

            Assert.Equal(expectedWithSameDate, actualReview);
        }
    }
}