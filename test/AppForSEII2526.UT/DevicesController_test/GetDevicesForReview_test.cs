using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DeviceDTo;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.DevicesController_test
{
    public class GetDevicesForReview_test : AppForSEII2526SqliteUT
    {
        public GetDevicesForReview_test()
        {
            var models = new List<Model>()
            {
                new Model { Id = 1, Name = "Galaxy S23" },
                new Model { Id = 2, Name = "iPhone 15" },
                new Model { Id = 3, Name = "ThinkPad X1" },
            };

            var devices = new List<Device>()
            {
                new Device
                {
                    Id = 1,
                    Brand = "Samsung",
                    Color = "Black",
                    Name = "Galaxy Ultra",
                    Year = 2023,
                    Model = models[0]
                },
                new Device
                {
                    Id = 2,
                    Brand = "Apple",
                    Color = "White",
                    Name = "iPhone Pro",
                    Year = 2023,
                    Model = models[1]
                },
                new Device
                {
                    Id = 3,
                    Brand = "Lenovo",
                    Color = "Gray",
                    Name = "ThinkPad X1",
                    Year = 2022,
                    Model = models[2]
                }
            }; 
            /

            _context.Model.AddRange(models);
            _context.Device.AddRange(devices);
            _context.SaveChanges();
        }

        // Casos correctos con distintos filtros
        public static IEnumerable<object[]> TestCasesFor_GetDevicesForReview_OK()
        {
            var dtoList = new List<DeviceForReviewDTO>()
            {
                new DeviceForReviewDTO(1, "Samsung", "Galaxy Ultra", 2023, "Galaxy S23", "Black"),
                new DeviceForReviewDTO(2, "Apple", "iPhone Pro", 2023, "iPhone 15", "White"),
                new DeviceForReviewDTO(3, "Lenovo", "ThinkPad X1", 2022, "ThinkPad X1", "Gray")
            };

            var allDevices = dtoList.OrderBy(d => d.Year).ThenBy(d => d.Name).ToList();
            var onlyApple = new List<DeviceForReviewDTO> { dtoList[1] };
            var only2022 = new List<DeviceForReviewDTO> { dtoList[2] };
            var samsung2023 = new List<DeviceForReviewDTO> { dtoList[0] };

            return new List<object[]>
            {
                new object[] { null, null, allDevices },
                new object[] { null, "Apple", onlyApple },
                new object[] { 2022, null, only2022 },
                new object[] { 2023, "Samsung", samsung2023 }
            };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetDevicesForReview_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForReview_OK_test(int? year, string? brand, IList<DeviceForReviewDTO> expectedDevices)
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DevicesController>>();
            var controller = new DevicesController(_context, mockLogger.Object);

            // Act
            var result = await controller.GetDevicesForReview(year, brand);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDevices = Assert.IsType<List<DeviceForReviewDTO>>(okResult.Value);

            //Comparación directa de DTOs usando el método Equals 
            Assert.Equal(expectedDevices, actualDevices);
        }


        // Caso sin dispositivos que coincidan
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForReview_NotFound_WhenNoDevices_test()
        {
            var mockLogger = new Mock<ILogger<DevicesController>>();
            var controller = new DevicesController(_context, mockLogger.Object);

            // Act
            var result = await controller.GetDevicesForReview(1990, "Sony");

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFoundResult.Value);
            Assert.Equal("No se encontraron dispositivos que coincidan con los filtros aplicados.", message);
        }

        // Caso tabla vacía 
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForReview_NotFound_WhenTableEmpty_test()
        {
            var emptyContext = CreateEmptyContext();
            var mockLogger = new Mock<ILogger<DevicesController>>();
            var controller = new DevicesController(emptyContext, mockLogger.Object);

            // Act
            var result = await controller.GetDevicesForReview(null, null);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFoundResult.Value);

            
            Assert.Equal("No se encontraron dispositivos que coincidan con los filtros aplicados.", message);
        }

        private ApplicationDbContext CreateEmptyContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            return context;
        }
    }
}