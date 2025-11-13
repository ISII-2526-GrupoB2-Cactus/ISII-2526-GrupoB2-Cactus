using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.PurchaseDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.PurchaseController_test
{
    public class GetPurchase_test : AppForSEII2526SqliteUT
    {
        public GetPurchase_test()
        {
            var model = new Model { Id = 1, Name = "iPhone 15" };
            var device = new Device
            {
                Id = 10,
                Brand = "Apple",
                Color = "Negro",
                Name = "iPhone 15",
                PriceForPurchase = 1200,
                QuantityForPurchase = 5,
                Year = 2023,
                Model = model
            };

            var user = new ApplicationUser
            {
                Id = "1",
                CustomerUserName = "Lucia",
                CustomerUserSurname = "Romero",
                Email = "lucia.romero@alu.uclm.es",
                UserName = "lucia.romero@alu.uclm.es"
            };

            var purchase = new Purchase
            {
                ApplicationUser = user,
                DeliveryAddress = "Calle Mayor 22",
                PaymentMethod = PaymentMethod.CreditCard,
                PurchaseDate = DateTime.Now,
                TotalPrice = 1200,
                TotalQuantity = 1,
                PurchaseItems = new List<PurchaseItem>
                {
                    new PurchaseItem { DeviceId = device.Id, Quantity = 1, Price = 1200 }
                }
            };

            _context.Add(model);
            _context.Add(device);
            _context.Add(user);
            _context.Purchase.Add(purchase);
            _context.SaveChanges();

        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetPurchase_OK_test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PurchaseController>>();
            var controller = new PurchaseController(_context, mockLogger.Object);
            var purchaseId = _context.Purchase.First().Id;

            // Act
            var result = await controller.GetPurchase(purchaseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<PurchaseDetailDTO>(okResult.Value);

            Assert.Equal(purchaseId, dto.Id);
            Assert.Equal("Lucia", dto.CustomerName);
            Assert.Single(dto.PurchaseItems);
            Assert.Equal("iPhone 15", dto.PurchaseItems.First().Model);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetPurchase_NotFound_test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<PurchaseController>>();
            var controller = new PurchaseController(_context, mockLogger.Object);

            // Act
            var result = await controller.GetPurchase(999);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("no existe ninguna compra", notFound.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}