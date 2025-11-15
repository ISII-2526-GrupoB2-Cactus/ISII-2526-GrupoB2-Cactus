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
            var reviewId = _context.Review.First().ReviewId;

            // Act
            var result = await controller.GetReview(reviewId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<ReviewDetailDTO>(okResult.Value);

            Assert.Equal(reviewId, dto.Id);
            Assert.Equal("Lucia", dto.CustomerUserName);
            Assert.Equal("Lucia Romero", dto.CustomerNameSurname);
            Assert.Single(dto.ReviewItems);
            Assert.Equal("iPhone espectacular, la cámara es increíble", dto.ReviewItems.First().Comments);
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