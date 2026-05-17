using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.PurchaseDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.PurchaseController_test
{
    public class PostPurchase_test : AppForSEII2526SqliteUT
    {
        public PostPurchase_test()
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
                CustomerUserName = "Laura",
                CustomerUserSurname = "Gonzalez",
                Email = "laura.gonzalez@alu.uclm.es",
                UserName = "laura.gonzalez@alu.uclm.es"
            };

            _context.Add(model);
            _context.Add(device);
            _context.Add(user);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_OK_test()
        {
            // Arrange
            var mock = new Mock<ILogger<PurchaseController>>();
            ILogger<PurchaseController> logger = mock.Object;
            var controller = new PurchaseController(_context, logger);

            var dto = new PurchaseForCreateDTO
            {
                DeliveryAddress = "Calle Mayor 22, Madrid",
                CustomerName = "Laura",
                CustomerSurname = "Gonzalez",
                PaymentMethod = PaymentMethod.CreditCard,
                PurchaseItems = new List<PurchaseItemDTO>
                {
                    new PurchaseItemDTO(10, "Apple", "iPhone 15", "Negro", 1200, 1, "Compra correcta")
                }
            };

            // Act
            var result = await controller.CreatePurchase(dto);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            var purchaseDetail = Assert.IsType<PurchaseDetailDTO>(created.Value);

            Assert.Equal("Laura", purchaseDetail.CustomerName);
            Assert.Single(purchaseDetail.PurchaseItems);
            Assert.Equal("iPhone 15", purchaseDetail.PurchaseItems.First().Model);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_BadRequest_UserNotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<PurchaseController>>();
            ILogger<PurchaseController> logger = mock.Object;
            var controller = new PurchaseController(_context, logger);

            var dto = new PurchaseForCreateDTO
            {
                DeliveryAddress = "Calle Sol 10, Toledo",
                CustomerName = "NoExiste",
                CustomerSurname = "Nadie",
                PaymentMethod = PaymentMethod.PayPal,
                PurchaseItems = new List<PurchaseItemDTO>
                {
                    new PurchaseItemDTO(10, "Apple", "iPhone 15", "Negro", 1200, 1, "Usuario inexistente")
                }
            };

            // Act
            var result = await controller.CreatePurchase(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Usuario no encontrado", badRequest.Value.ToString());
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_BadRequest_DeviceNotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<PurchaseController>>();
            ILogger<PurchaseController> logger = mock.Object;
            var controller = new PurchaseController(_context, logger);

            var dto = new PurchaseForCreateDTO
            {
                DeliveryAddress = "Calle Falsa 123",
                CustomerName = "Laura",
                CustomerSurname = "Gonzalez",
                PaymentMethod = PaymentMethod.CreditCard,
                PurchaseItems = new List<PurchaseItemDTO>
                {
                    new PurchaseItemDTO(999, "Apple", "Inexistente", "Gris", 999, 1, "Dispositivo no válido")
                }
            };

            // Act
            var result = await controller.CreatePurchase(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("no está disponible", badRequest.Value.ToString());
        }




        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_BadRequest_StartWith()
        {
            // Arrange
            var mock = new Mock<ILogger<PurchaseController>>();
            ILogger<PurchaseController> logger = mock.Object;
            var controller = new PurchaseController(_context, logger);

            var dto = new PurchaseForCreateDTO
            {
                DeliveryAddress = "Calle aceña",
                CustomerName = "Laura",
                CustomerSurname = "Gonzalez",
                PaymentMethod = PaymentMethod.CreditCard,
                PurchaseItems = new List<PurchaseItemDTO>
                {
                    new PurchaseItemDTO(3, "Nokia", "N1", "Blue", 300, 1, "Error: las tecnologías de estas marcas ya no están disponibles.")
                }
            };

            // Act
            var result = await controller.CreatePurchase(dto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("no está disponible", badRequest.Value.ToString());
        }



    }
}