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
    public class GetRentals_test : AppForSEII2526SqliteUT
    {
        public GetRentals_test()
        {
            var models = new List<Model>() {
                new Model { Name = "iPhone 15" },
                new Model { Name = "Galaxy S24" },
            };

            var devices = new List<Device>(){
                new Device { Id = 1, Brand = "Apple", Color = "Black", Name = "iPhone 15 Pro", PriceForPurchase = 999.99, PriceForRent = 49.99, Year = 2023, Quality = Device.QualityType.New, QuantityForPurchase = 10, QuantityForRent = 5, Model = models[0] },
                new Device { Id = 2, Brand = "Samsung", Color = "White", Name = "Galaxy S24 Ultra", PriceForPurchase = 899.99, PriceForRent = 39.99, Year = 2024, Quality = Device.QualityType.New, QuantityForPurchase = 8, QuantityForRent = 4, Model = models[1] },
            };

            ApplicationUser user = new ApplicationUser
            {
                Id = "1",
                UserName = "carlos@uclm.es",
                CustomerUserName = "Carlos",
                CustomerUserSurname = "García López",
                Email = "carlos@uclm.es"
            };

            var rental = new Rental("carlos@uclm.es", "Carlos García López",
                   user, "Calle Gran Vía 123, Madrid 28013",
                    DateTime.Now, PaymentMethodType.CreditCard,
                    DateTime.Today.AddDays(2), DateTime.Today.AddDays(5),
                    new List<RentDevice>());

            rental.RentDevices.Add(new RentDevice(rental.Id, devices[0].Id, devices[0].PriceForRent, 1));

            _context.Users.Add(user);
            _context.AddRange(models);
            _context.AddRange(devices);
            _context.Add(rental);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRental_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<RentalsController>>();
            ILogger<RentalsController> logger = mock.Object;

            var controller = new RentalsController(_context, logger);

            // Act
            var result = await controller.GetRental(0);

            //Assert
            //we check that the response type is OK and obtain the list of movies
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetRental_Found_test()
        {
            // Arrange
            var mock = new Mock<ILogger<RentalsController>>();
            ILogger<RentalsController> logger = mock.Object;
            var controller = new RentalsController(_context, logger);

            var expectedRental = new RentalDetailDTO(1, DateTime.Now, "carlos@uclm.es", "Carlos García López",
                        "Calle Gran Vía 123, Madrid 28013", PaymentMethodType.CreditCard,
                        DateTime.Today.AddDays(2), DateTime.Today.AddDays(5),
                        new List<RentalItemDTO>());
            expectedRental.RentalItems.Add(new RentalItemDTO(1, "iPhone 15 Pro", "Apple", "iPhone 15", 49.99, 1));

            // Act 
            var result = await controller.GetRental(1);

            //Assert
            //we check that the response type is OK and obtain the rental
            var okResult = Assert.IsType<OkObjectResult>(result);
            var rentalDTOActual = Assert.IsType<RentalDetailDTO>(okResult.Value);
            var eq = expectedRental.Equals(rentalDTOActual);
            //we check that the expected and actual are the same
            Assert.Equal(expectedRental, rentalDTOActual);
        }
    }
}