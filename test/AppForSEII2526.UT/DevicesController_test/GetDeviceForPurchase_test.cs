using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DeviceDTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UT.DevicesController_test
{
    public class GetDeviceForPurchase_test : AppForSEII2526SqliteUT
    {

        public GetDeviceForPurchase_test()
        {
            var models = new List<Model>()
            {
                new Model { Id = 1, Name = "Galaxy S23" },
                new Model { Id = 2, Name = "iPhone 15" },
                new Model { Id = 3, Name = "PlayStation 5" }
            };

            var devices = new List<Device>()
            {
                new Device
                {
                    Id = 5,
                    Brand = "Samsung",
                    Color = "Negro",
                    Name = "Galaxy S23",
                    PriceForPurchase = 999.99,
                    QuantityForPurchase = 10,
                    Year = 2024,
                    Model = models[0]
                },
                new Device
                {
                    Id = 6,
                    Brand = "Apple",
                    Color = "Gris",
                    Name = "MacBook Pro 14",
                    PriceForPurchase = 1899.99,
                    QuantityForPurchase = 9,
                    Year = 2023,
                    Model = models[1]
                },
                new Device
                {
                    Id = 9,
                    Brand = "Apple",
                    Color = "Azul",
                    Name = "iPad Air 5",
                    PriceForPurchase = 699.99,
                    QuantityForPurchase = 8,
                    Year = 2022,
                    Model = models[1]
                },
                new Device
                {
                    Id = 11,
                    Brand = "Sony",
                    Color = "Gris",
                    Name = "PlayStation 5",
                    PriceForPurchase = 549.99,
                    QuantityForPurchase = 10,
                    Year = 2023,
                    Model = models[2]
                }
            };

            _context.AddRange(models);
            _context.AddRange(devices);
            _context.SaveChanges();
        }

        // Caso correcto con distintos filtros
        public static IEnumerable<object[]> TestCasesFor_GetDeviceForPurchase_OK()
        {
            var dtoList = new List<DeviceForPurchaseDTO>()
            {
                new DeviceForPurchaseDTO(5, "Samsung", "Galaxy S23", "Galaxy S23", "Negro", 999.99m, 10, 2024),
                new DeviceForPurchaseDTO(6, "Apple", "MacBook Pro 14", "iPhone 15", "Gris", 1899.99m, 9, 2023),
                new DeviceForPurchaseDTO(9, "Apple", "iPad Air 5", "iPhone 15", "Azul", 699.99m, 8, 2022),
                new DeviceForPurchaseDTO(11, "Sony", "PlayStation 5", "PlayStation 5", "Gris", 549.99m, 10, 2023)
            };

            var allDevices = dtoList.OrderBy(d => d.Name).ThenBy(d => d.Brand).ToList();
            var onlyApple = dtoList.Where(d => d.Brand == "Apple").ToList();
            var onlyBlue = dtoList.Where(d => d.Color == "Azul").ToList();
            var onlySamsung = dtoList.Where(d => d.Brand == "Samsung").ToList();

            return new List<object[]>
            {
                new object[] { null, null, allDevices },
                new object[] { null, "Azul", onlyBlue },
                new object[] { "MacBook", null, new List<DeviceForPurchaseDTO>{ dtoList[1] } },
                new object[] { null, "Negro", onlySamsung }
            };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetDeviceForPurchase_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDeviceForPurchase_OK_test(string? name, string? color, IList<DeviceForPurchaseDTO> expectedDevices)
        {
            // Arrange
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            var controller = new DevicesController(_context, logger);

            // Act
            var result = await controller.GetDeviceForPurchase(name, color);

            // Extraemos la lista desde el ActionResult
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDevices = Assert.IsType<List<DeviceForPurchaseDTO>>(okResult.Value);

            // Ordenamos ambas listas antes de comparar (por Name y Brand)
            var expectedOrdered = expectedDevices.OrderBy(d => d.Name).ThenBy(d => d.Brand).ToList();
            var actualOrdered = actualDevices.OrderBy(d => d.Name).ThenBy(d => d.Brand).ToList();

            // Assert
            Assert.Equal(expectedOrdered.Count, actualOrdered.Count);

            for (int i = 0; i < expectedOrdered.Count; i++)
            {
                Assert.Equal(expectedOrdered[i].Id, actualOrdered[i].Id);
                Assert.Equal(expectedOrdered[i].Brand, actualOrdered[i].Brand);
                Assert.Equal(expectedOrdered[i].Name, actualOrdered[i].Name);
                Assert.Equal(expectedOrdered[i].Model, actualOrdered[i].Model);
                Assert.Equal(expectedOrdered[i].Color, actualOrdered[i].Color);
                Assert.Equal(expectedOrdered[i].PriceForPurchase, actualOrdered[i].PriceForPurchase);
            }
        }




        // Caso de color inexistente
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDeviceForPurchase_BadRequest_WhenColorNotFound_test()
        {
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            var controller = new DevicesController(_context, logger);


            // Act
            var result = await controller.GetDeviceForPurchase(null, "Verde");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

            var message = Assert.IsType<string>(badRequest.Value);
            Assert.Equal("El color indicado no existe en el catálogo de dispositivos.", message);
        }


        // Caso de nombre inexistente
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDeviceForPurchase_BadRequest_WhenNameNotFound_test()
        {
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            var controller = new DevicesController(_context, logger);

            // Act
            var result = await controller.GetDeviceForPurchase("Motorola", null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

            var message = Assert.IsType<string>(badRequest.Value);
            Assert.Equal("No existe ningún dispositivo con ese nombre.", message);
        }


        // Caso tabla vacía
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDeviceForPurchase_NotFound_WhenTableEmpty_test()
        {
            var emptyContext = CreateEmptyContext();
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;
            var controller = new DevicesController(_context, logger);

            // Act
            var result = await controller.GetDeviceForPurchase(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDevices = Assert.IsType<List<DeviceForPurchaseDTO>>(okResult.Value);
            Assert.Empty(actualDevices);
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