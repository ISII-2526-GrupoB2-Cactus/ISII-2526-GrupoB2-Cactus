using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.RentalDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.RentalsController_test
{
    public class PostRentals_test : AppForSEII2526SqliteUT
    {
        private const string _userName = "carlos@uclm.es";
        private const string _customerNameSurname = "Carlos Garc�a L�pez";
        private const string _deliveryAddress = "Calle Gran V�a 123, Madrid 28013";
        //MODIFICACION
        private const string _invalidAddress = "Gran V�a 123, Madrid 28013";



        private const string _device1Name = "iPhone 15 Pro";
        private const string _device1Brand = "Apple";
        private const string _device1Model = "iPhone 15";
        private const string _device2Name = "Galaxy S24 Ultra";
        private const string _device2Brand = "Samsung";
        private const string _device2Model = "Galaxy S24";

        public PostRentals_test()
        {
            var models = new List<Model>() {
                new Model { Name = _device1Model },
                new Model { Name = _device2Model },
            };

            var devices = new List<Device>(){
                new Device { Id = 1, Brand = _device1Brand, Color = "Black", Name = _device1Name, PriceForPurchase = 999.99, PriceForRent = 49.99, Year = 2023, Quality = Device.QualityType.New, QuantityForPurchase = 10, QuantityForRent = 0, Model = models[0] },
                new Device { Id = 2, Brand = _device2Brand, Color = "White", Name = _device2Name, PriceForPurchase = 899.99, PriceForRent = 39.99, Year = 2024, Quality = Device.QualityType.New, QuantityForPurchase = 8, QuantityForRent = 4, Model = models[1] },
            };

            ApplicationUser user = new ApplicationUser
            {
                Id = "1",
                UserName = _userName,
                CustomerUserName = "Carlos",
                CustomerUserSurname = "Garc�a L�pez",
                Email = _userName
            };

            _context.Users.Add(user);
            _context.AddRange(models);
            _context.AddRange(devices);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_CreateRental()
        {
            var rentalNoItems = new RentalForCreateDTO(_userName, _customerNameSurname,
                _deliveryAddress, PaymentMethodType.CreditCard,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), new List<RentalItemDTO>());

            var rentalItems = new List<RentalItemDTO>() {
                new RentalItemDTO(2, _device2Name, _device2Brand, _device2Model, 39.99, 1)
            };

            var rentalFromBeforeToday = new RentalForCreateDTO(_userName, _customerNameSurname,
                _deliveryAddress, PaymentMethodType.CreditCard,
                DateTime.Today, DateTime.Today.AddDays(5), rentalItems);

            var rentalToBeforeFrom = new RentalForCreateDTO(_userName, _customerNameSurname,
                _deliveryAddress, PaymentMethodType.CreditCard,
                DateTime.Today.AddDays(5), DateTime.Today.AddDays(2), rentalItems);

            var rentalApplicationUser = new RentalForCreateDTO("victor.lopez@uclm.es", _customerNameSurname,
                _deliveryAddress, PaymentMethodType.CreditCard,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(4), rentalItems);

            var rentalDeviceNotAvailable = new RentalForCreateDTO(_userName, _customerNameSurname,
                _deliveryAddress, PaymentMethodType.CreditCard,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(5),
                new List<RentalItemDTO>() {
                    new RentalItemDTO(1, _device1Name, _device1Brand, _device1Model, 49.99, 6)
                });

            //MODIFICACION
            var rentalInvalidAddress = new RentalForCreateDTO(_userName, _customerNameSurname,
                _invalidAddress, PaymentMethodType.CreditCard,
                DateTime.Today.AddDays(2), DateTime.Today.AddDays(5), rentalItems);


            var allTests = new List<object[]>
            {
                new object[] { rentalNoItems, "Error! You must include at least one device to be rented" },
                new object[] { rentalFromBeforeToday, "Error! Your rental date must start later than today" },
                new object[] { rentalToBeforeFrom, "Error! Your rental must end later than it starts" },
                new object[] { rentalApplicationUser, "Error! UserName is not registered" },
                new object[] { rentalDeviceNotAvailable, "Error! Device Name 'iPhone 15 Pro' is not available for being rented" },
                //MODIFICACION
                new object[] { rentalInvalidAddress, "Error! Invalid delivery address. Please include 'Calle' or 'Carretera' in the address"},

            };

            return allTests;
        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateRental))]
        public async Task CreateRental_Error_test(RentalForCreateDTO rentalDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<RentalsController>>();
            ILogger<RentalsController> logger = mock.Object;
            var controller = new RentalsController(_context, logger);

            // Act
            var result = await controller.CreateRental(rentalDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(errorExpected, errorActual);
        }


        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateRental_Success_test()
        {
            // Arrange
            var mock = new Mock<ILogger<RentalsController>>();
            ILogger<RentalsController> logger = mock.Object;
            var controller = new RentalsController(_context, logger);

            DateTime to = DateTime.Today.AddDays(2);
            DateTime from = DateTime.Today.AddDays(5);

            var rentalDTO = new RentalForCreateDTO(_userName, _customerNameSurname,
                _deliveryAddress, PaymentMethodType.CreditCard,
                to, from, new List<RentalItemDTO>()
                {
                    new RentalItemDTO(2, _device2Name, _device2Brand, _device2Model, 39.99, 1)
                });

            var expectedRentalDetailDTO = new RentalDetailDTO(1, DateTime.Now,
                _userName, _customerNameSurname,
                _deliveryAddress, PaymentMethodType.CreditCard,
                to, from, new List<RentalItemDTO>()
                {
                    new RentalItemDTO(2, _device2Name, _device2Brand, _device2Model, 39.99, 1)
                });

            // Act
            var result = await controller.CreateRental(rentalDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualRentalDetailDTO = Assert.IsType<RentalDetailDTO>(createdResult.Value);

            Assert.Equal(expectedRentalDetailDTO, actualRentalDetailDTO);
        }
    }
}
