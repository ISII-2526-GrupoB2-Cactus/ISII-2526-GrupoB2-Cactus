using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DeviceDTo;
using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.Models;


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
            }; //Hasta aqui el contenido de la bbdd en memoria 

            ApplicationUser user = new ApplicationUser
            {
                Id = "1",
                CustomerUserName = "Maria",
                CustomerUserSurname = "Martinez Gonzalez",
                Email = "maria.martinez128@alu.uclm.es",
                UserName = "maria.martinez128@alu.uclm.es"
            };

            
            _context.AddRange(models);
            _context.AddRange(devices);
            _context.Add(user);
            _context.SaveChanges();
        }

        //Casos de prueba
        public static IEnumerable<object[]> TestCasesFor_GetDevicesForReview_OK() //Se configuran los casos de prueba 
        {
            var deviceDTOs = new List<DeviceForReviewDTO>()
            {
                new DeviceForReviewDTO(1, "Samsung", "Galaxy Ultra", 2023, "Galaxy S23", "Black"),
                new DeviceForReviewDTO(2, "Apple", "iPhone Pro", 2023, "iPhone 15", "White"),
                new DeviceForReviewDTO(3, "Lenovo", "ThinkPad X1", 2022, "ThinkPad X1", "Gray"),
            };

            var tc1 = deviceDTOs.OrderBy(d => d.Year).ThenBy(d => d.Name).ToList();

            var tc2 = new List<DeviceForReviewDTO> { deviceDTOs[1] };

            var tc3 = new List<DeviceForReviewDTO> { deviceDTOs[2] };

            var tc4 = new List<DeviceForReviewDTO> { deviceDTOs[0] };

            return new List<object[]> //Exactamente la configuracion de DTOs y parametros de entrada que le vamos a pasar al metodo
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
            var mockLogger = new Mock<ILogger<DevicesController>>();
            var controller = new DevicesController(_context, mockLogger.Object);

            // Act
            var result = await controller.GetDevicesForReview(year, brand);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDevices = Assert.IsType<List<DeviceForReviewDTO>>(okResult.Value);

            Assert.Equal(expectedDevices.Count, actualDevices.Count);

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

        [Fact] //Es fact porque se le pasan directamente los datos 
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDevicesForReview_NotFound_WhenNoDevices_test()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DevicesController>>();
            var controller = new DevicesController(_context, mockLogger.Object);

            // Act (buscamos un año que no existe)
            var result = await controller.GetDevicesForReview(1990, "Sony");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFoundResult.Value);
            Assert.Equal("No se encontraron dispositivos que coincidan con los filtros aplicados.", message);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDevicesForReview_NotFound_WhenTableEmpty_test()
        {
            var emptyContext = CreateEmptyContext();
            var mockLogger = new Mock<ILogger<DevicesController>>();
            var controller = new DevicesController(emptyContext, mockLogger.Object);

            // Act
            var result = await controller.GetDevicesForReview(null, null);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var message = Assert.IsType<string>(notFoundResult.Value);
            Assert.Equal("No existen dispositivos en la base de datos.", message);
        }

        private ApplicationDbContext CreateEmptyContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("Data Source=:memory:") // Usar SQLite en memoria
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            return context;
        }
    }
}