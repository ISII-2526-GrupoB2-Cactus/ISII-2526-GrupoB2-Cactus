using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DeviceDTo;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
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
                    PriceForPurchase = 1200,
                    PriceForRent = 100,
                    Year = 2023,
                    Quality = Device.QualityType.New,
                    QuantityForPurchase = 5,
                    QuantityForRent = 3,
                    Model = models[0]
                },
                new Device
                {
                    Id = 2,
                    Brand = "Apple",
                    Color = "White",
                    Name = "iPhone Pro",
                    PriceForPurchase = 1300,
                    PriceForRent = 120,
                    Year = 2023,
                    Quality = Device.QualityType.New,
                    QuantityForPurchase = 8,
                    QuantityForRent = 2,
                    Model = models[1]
                },
                new Device
                {
                    Id = 3,
                    Brand = "Lenovo",
                    Color = "Gray",
                    Name = "ThinkPad X1",
                    PriceForPurchase = 1500,
                    PriceForRent = 150,
                    Year = 2022,
                    Quality = Device.QualityType.LikeNew,
                    QuantityForPurchase = 6,
                    QuantityForRent = 4,
                    Model = models[2]
                }
            };

            ApplicationUser user = new ApplicationUser
            {
                Id = "1",
                CustomerUserName = "Maria",
                CustomerUserSurname = "Martinez Gonzalez",
                Email = "maria.martinez128@alu.uclm.es",
                UserName = "maria.martinez128@alu.uclm.es"
            };

            _context.Model.AddRange(models);
            _context.Device.AddRange(devices);
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        // Casos de prueba
        public static IEnumerable<object[]> TestCasesFor_GetDevicesForReview_OK()
        {
            var deviceDTOs = new List<DeviceForReviewDTO>()
            {
                new DeviceForReviewDTO(1, "Samsung", "Galaxy Ultra", 2023, "Galaxy S23", "Black"),
                new DeviceForReviewDTO(2, "Apple", "iPhone Pro", 2023, "iPhone 15", "White"),
                new DeviceForReviewDTO(3, "Lenovo", "ThinkPad X1", 2022, "ThinkPad X1", "Gray"),
            };

            // Caso 1: Sin filtros - todos los dispositivos ordenados por año y nombre
            var tc1 = deviceDTOs.OrderBy(d => d.Year).ThenBy(d => d.Name).ToList();

            // Caso 2: Filtrar solo por marca "Apple"
            var tc2 = new List<DeviceForReviewDTO> { deviceDTOs[1] };

            // Caso 3: Filtrar solo por año 2022
            var tc3 = new List<DeviceForReviewDTO> { deviceDTOs[2] };

            // Caso 4: Filtrar por año 2023 y marca "Samsung"
            var tc4 = new List<DeviceForReviewDTO> { deviceDTOs[0] };

            return new List<object[]>
            {
                new object[] { null, null, tc1 },
                new object[] { null, "Apple", tc2 },
                new object[] { 2022, null, tc3 },
                new object[] { 2023, "Samsung", tc4 },
            };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetDevicesForReview_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForReview_OK_test(int? year, string? brand, IList<DeviceForReviewDTO> expectedDevices)
        {
            // Arrange
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;

            var controller = new DevicesController(_context, logger);

            // Act
            var result = await controller.GetDevicesForReview(year, brand);

            // Assert
            // Verificamos que el tipo de respuesta es OK y obtenemos la lista de dispositivos
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDevices = Assert.IsType<List<DeviceForReviewDTO>>(okResult.Value);

            // Verificamos que la cantidad de dispositivos esperados y actuales coinciden
            Assert.Equal(expectedDevices.Count, actualDevices.Count);

            // Verificamos que cada dispositivo coincide en todas sus propiedades
            for (int i = 0; i < expectedDevices.Count; i++)
            {
                Assert.Equal(expectedDevices[i].Id, actualDevices[i].Id);
                Assert.Equal(expectedDevices[i].Brand, actualDevices[i].Brand);
                Assert.Equal(expectedDevices[i].Name, actualDevices[i].Name);
                Assert.Equal(expectedDevices[i].Year, actualDevices[i].Year);
                Assert.Equal(expectedDevices[i].Model, actualDevices[i].Model);
                Assert.Equal(expectedDevices[i].Color, actualDevices[i].Color);
            }
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDevicesForReview_NotFound_WhenNoDevices_test()
        {
            // Arrange
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;

            var controller = new DevicesController(_context, logger);

            // Act
            // Buscamos un año que no existe en la base de datos
            var result = await controller.GetDevicesForReview(1990, "Sony");

            // Assert
            // Verificamos que el tipo de respuesta es NotFound
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFoundResult.Value);
            Assert.Equal("No se encontraron dispositivos que coincidan con los filtros aplicados.", message);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDevicesForReview_FilterByYear_test()
        {
            // Arrange
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;

            var controller = new DevicesController(_context, logger);

            // Act
            // Filtramos solo por año 2023 (sin marca)
            var result = await controller.GetDevicesForReview(2023, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDevices = Assert.IsType<List<DeviceForReviewDTO>>(okResult.Value);

            // Debe retornar 2 dispositivos (Samsung y Apple del 2023)
            Assert.Equal(2, actualDevices.Count);
            Assert.All(actualDevices, d => Assert.Equal(2023, d.Year));
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDevicesForReview_FilterByBrand_test()
        {
            // Arrange
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;

            var controller = new DevicesController(_context, logger);

            // Act
            // Filtramos solo por marca "Lenovo" (sin año)
            var result = await controller.GetDevicesForReview(null, "Lenovo");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDevices = Assert.IsType<List<DeviceForReviewDTO>>(okResult.Value);

            // Debe retornar 1 dispositivo (Lenovo)
            Assert.Single(actualDevices);
            Assert.Equal("Lenovo", actualDevices[0].Brand);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDevicesForReview_AllDevices_OrderedCorrectly_test()
        {
            // Arrange
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;

            var controller = new DevicesController(_context, logger);

            // Act
            // Sin filtros, deben venir todos ordenados por año y luego por nombre
            var result = await controller.GetDevicesForReview(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDevices = Assert.IsType<List<DeviceForReviewDTO>>(okResult.Value);

            // Verificamos que están ordenados correctamente
            Assert.Equal(3, actualDevices.Count);

            // Primero el de 2022
            Assert.Equal(2022, actualDevices[0].Year);
            Assert.Equal("ThinkPad X1", actualDevices[0].Name);

            // Luego los de 2023 ordenados por nombre
            Assert.Equal(2023, actualDevices[1].Year);
            Assert.Equal("Galaxy Ultra", actualDevices[1].Name);

            Assert.Equal(2023, actualDevices[2].Year);
            Assert.Equal("iPhone Pro", actualDevices[2].Name);
        }
    }

    // Clase adicional para probar el caso donde la tabla Device está vacía
    public class GetDevicesForReview_EmptyTable_test : AppForSEII2526SqliteUT
    {
        public GetDevicesForReview_EmptyTable_test()
        {
            // No inicializamos ningún dispositivo para simular tabla vacía
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDevicesForReview_NotFound_WhenTableEmpty_test()
        {
            // Arrange
            var mock = new Mock<ILogger<DevicesController>>();
            ILogger<DevicesController> logger = mock.Object;

            var controller = new DevicesController(_context, logger);

            // Act
            var result = await controller.GetDevicesForReview(null, null);

            // Assert
            // Verificamos que el tipo de respuesta es NotFound cuando la tabla está vacía
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFoundResult.Value);
            Assert.Equal("No existen dispositivos en la base de datos.", message);
        }
    }
}