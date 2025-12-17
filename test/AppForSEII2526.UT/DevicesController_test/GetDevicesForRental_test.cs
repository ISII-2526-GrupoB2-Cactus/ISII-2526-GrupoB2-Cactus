using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DeviceDTO;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.DevicesController_test
{
    public class GetDevicesForRental_test : AppForSEII2526SqliteUT
    {
        public GetDevicesForRental_test()
        {
            var models = new List<Model>() {
                new Model { Name = "iPhone 15" },
                new Model { Name = "Galaxy S24" },
                new Model { Name = "iPad Pro" },
            };

            var devices = new List<Device>(){
                new Device { Id = 1, Brand = "Apple", Color = "Black", Name = "iPhone 15 Pro", PriceForPurchase = 999.99, PriceForRent = 49.99, Year = 2023, Quality = Device.QualityType.New, QuantityForPurchase = 10, QuantityForRent = 5, Model = models[0] },
                new Device { Id = 2, Brand = "Samsung", Color = "White", Name = "Galaxy S24 Ultra", PriceForPurchase = 899.99, PriceForRent = 39.99, Year = 2024, Quality = Device.QualityType.New, QuantityForPurchase = 8, QuantityForRent = 4, Model = models[1] },
                new Device { Id = 3, Brand = "Apple", Color = "Silver", Name = "iPad Pro 12.9", PriceForPurchase = 1299.99, PriceForRent = 69.99, Year = 2023, Quality = Device.QualityType.LikeNew, QuantityForPurchase = 6, QuantityForRent = 3, Model = models[2] },
            };

            _context.AddRange(models);
            _context.AddRange(devices);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetDevicesForRental_OK()
        {
            var deviceDTOs = new List<DeviceParaAlquilarDTO>() {
                new DeviceParaAlquilarDTO(1, "iPhone 15 Pro", "iPhone 15", "Apple", 2023, "Black", 49.99),
                new DeviceParaAlquilarDTO(2, "Galaxy S24 Ultra", "Galaxy S24", "Samsung", 2024, "White", 39.99),
                new DeviceParaAlquilarDTO(3, "iPad Pro 12.9", "iPad Pro", "Apple", 2023, "Silver", 69.99)
            };

            var devicesTC1 = new List<DeviceParaAlquilarDTO>() { deviceDTOs[1], deviceDTOs[0], deviceDTOs[2] }
                    //the GetDevicesForRental method returns the devices ordered by price
                    .OrderBy(d => d.PriceForRent).ToList();

            var devicesTC2 = new List<DeviceParaAlquilarDTO>() { deviceDTOs[0] };
            var devicesTC3 = new List<DeviceParaAlquilarDTO>() { deviceDTOs[1] };
            var devicesTC4 = new List<DeviceParaAlquilarDTO>() { };

            var allTests = new List<object[]>
            {             //filters to apply - expected devices
                new object[] { null, null, devicesTC1 },
                new object[] { "iPhone", null, devicesTC2 },
                new object[] { null, 39.99, devicesTC3 },
                new object[] { "NonExisting", 999.99, devicesTC4 },
            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetDevicesForRental_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForRental_OK_test(string? model, double? priceForRent, IList<DeviceParaAlquilarDTO> expectedDevices)
        {
            // Arrange
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            var controller = new DevicesController(_context, logger);

            // Act
            var result = await controller.GetDevicesForRental(model, priceForRent);

            //Assert
            //we check that the response type is OK 
            var okResult = Assert.IsType<OkObjectResult>(result);
            //and obtain the list of devices
            var devicesDTOActual = Assert.IsType<List<DeviceParaAlquilarDTO>>(okResult.Value);
            Assert.Equal(expectedDevices, devicesDTOActual);
        }


        /*
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDevicesForRental_BadRequest_test()
        {
            // Arrange
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            var controller = new DevicesController(_context, logger);

            // Act
            // Pasamos parámetros que deberían causar error
            var result = await controller.GetDevicesForRental(null, -10.0); // Precio negativo

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
            var problem = problemDetails.Errors.First().Value[0];

            Assert.Equal("Price for rent cannot be negative", problem);
        }
        */

    }
}




