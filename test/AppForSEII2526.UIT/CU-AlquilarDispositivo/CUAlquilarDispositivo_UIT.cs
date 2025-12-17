using AppForSEII2526.UT.UIT.Shared;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AppForSEII2526.UIT.CU_AlquilarDispositivo
{
    public class CUAlquilarDispositivo_UIT : UC_UIT
    {
        public CUAlquilarDispositivo_UIT(ITestOutputHelper output) : base(output)
        {
            Initial_step_opening_the_web_page();
            listDevices = new ListDevicesForRental_PO(_driver, _output);
        }

        private const int deviceId1 = 1;
        private const string deviceName1 = "iPhone 15";
        private const string deviceBrand1 = "Apple";
        private const string deviceModel1 = "iPhone 15";
        private const string devicePriceForRenting1 = "50.5";
        private const int deviceYear1 = 2023;
        private const string deviceColor1 = "Black";

        private const string deviceName2 = "PlayStation 5";
        private const string deviceBrand2 = "Sony";
        private const string deviceModel2 = "PlayStation 5";
        private const string devicePriceForRenting2 = "120.75";
        private const int deviceYear2 = 2023;
        private const string deviceColor2 = "White";


        /*
        private const string deviceName3 = "MacBook Air";
        private const string deviceBrand3 = "Apple";
        private const string deviceModel3 = "MacBook Air";
        private const string devicePriceForRenting3 = "75.80000305175781";
        private const int deviceYear3 = 2023;
        private const string deviceColor3 = "White";


        private const string deviceName4 = "Galaxy S23";
        private const string deviceBrand4 = "Samsung";
        private const string deviceModel4 = "Galaxy S23";
        private const string devicePriceForRenting4 = "89.98999786376953";
        private const int deviceYear4 = 2023;
        private const string deviceColor4 = "White";


        private const string deviceName5 = "Surface Pro 9";
        private const string deviceBrand5 = "Microsoft";
        private const string deviceModel5 = "Surface Pro 9";
        private const string devicePriceForRenting5 = "90.8499984741211";
        private const int deviceYear5 = 2023;
        private const string deviceColor5 = "White";

        */


        private ListDevicesForRental_PO listDevices;

        private void Precondition_perform_login()
        {
            // Comentado temporalmente - login desactivado para desarrollo
            // Perform_login("maria@alu.uclm.es", "Password123!");
        }

        private void InitialStepsForRentalDevices_UIT()
        {
            // Precondition_perform_login(); // Login comentado
            _driver.Navigate().GoToUrl(_URI + "rental/SelectDevicesForRental");
            listDevices.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("searchDevices"));
        }

        
        [Theory]
        [InlineData(deviceName1, deviceBrand1, deviceModel1, devicePriceForRenting1, "iPhone", "")]
        [InlineData(deviceName2, deviceBrand2, deviceModel2, devicePriceForRenting2, "", "120,75")] // Filtro por precio
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_4_5_AF1_filteringbyModelandPrice(string name, string brand,
            string model, string price, string filterModel, string filterPrice)
        {
            //Arrange
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);
            var expectedDevices = new List<string[]> { new string[] { name, brand, model, price }, };

            //Act
            InitialStepsForRentalDevices_UIT();
            //ilterModel y filterPrice 
            listDevices.FilterDevices(filterModel, filterPrice, from, to);

            //Assert            
            Assert.True(listDevices.CheckListOfDevices(expectedDevices));
        }

        /*
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_6_AF1_filteringbyDate()
        {
            //Arrange
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);
            var expectedDevices = new List<string[]> {
                new string[] { deviceName1, deviceBrand1, deviceModel1, devicePriceForRenting1 },
                new string[] { deviceName2, deviceBrand2, deviceModel2, devicePriceForRenting2 },

                //new string[] { deviceName3, deviceBrand3, deviceModel3, devicePriceForRenting3 },
                //new string[] { deviceName4, deviceBrand4, deviceModel4, devicePriceForRenting4 },
                //new string[] { deviceName5, deviceBrand5, deviceModel5, devicePriceForRenting5 },

            };

            //Act
            InitialStepsForRentalDevices_UIT();
            listDevices.FilterDevices("", "", from, to);

            //Assert
            Assert.True(listDevices.CheckListOfDevices(expectedDevices));
        }
        */

        public static IEnumerable<object[]> TestCasesFor_UC2_4_5_AF2_errorindates()
        {
            var allTests = new List<object[]> {
                new object[] { DateTime.Today.AddDays(-1), DateTime.Today.AddDays(2), "Your rental period must be later" },
                new object[] { DateTime.Today.AddDays(-2), DateTime.Today.AddDays(-1), "Your rental period must be later" },
                new object[] { DateTime.Today.AddDays(7), DateTime.Today.AddDays(5), "Your rental must end after than its starts" },
            };

            return allTests;
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_UC2_4_5_AF2_errorindates))]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_7_8_9_AF2_errorindates(DateTime from, DateTime to, string error)
        {
            //Arrange

            //Act
            InitialStepsForRentalDevices_UIT();
            listDevices.FilterDevices("", "", from, to);

            //Assert
            Assert.True(listDevices.CheckMessageError(error), $"Error in the message box for test {from} - {to}");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_10_AF3_ModifySelectedDevices()
        {
            //Arrange
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            //Act
            InitialStepsForRentalDevices_UIT();
            listDevices.FilterDevices("", "", from, to);
            listDevices.SelectDevices(new List<string> { deviceName1, deviceName2 });
            listDevices.ModifyRentingCart(deviceName2);

            //Assert - Verificar que el dispositivo 1 está en el carrito y el dispositivo 2 no
            Assert.True(listDevices.CheckShoppingCart(deviceName1));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_11_AF4_RentButtonNotAvailable()
        {
            //Arrange
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            //Act
            InitialStepsForRentalDevices_UIT();
            listDevices.FilterDevices("", "", from, to);
            listDevices.SelectDevices(new List<string> { deviceName1 });
            listDevices.ModifyRentingCart(deviceName1);

            //Assert            
            Assert.True(listDevices.CheckRentDevicesDisabled(), "Rent button should be disabled");
        }

        [Theory]
        [InlineData("", "Calle Libertad 9, Ciudad Real", "CustomerNameSurname")] // UC2_12 - Nombre vacío
        [InlineData("Maria Martinez Gonzalez", "", "DeliveryAddress")] // UC2_15 - Dirección vacía
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_12_15_AF5_testingErrorsMandatorydata(string nameSurname, string deliveryAddress,
            string expectedMessageError)
        {
            //Arrange
            var createRental = new CreateRental_PO(_driver, _output);
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            //Act
            InitialStepsForRentalDevices_UIT();
            listDevices.FilterDevices("", "", from, to);
            listDevices.SelectDevices(new List<string> { deviceName1 });
            listDevices.RentDevices();
            createRental.FillInRentalInfo("maria@alu.uclm.es", nameSurname, deliveryAddress, "CreditCard");
            createRental.PressRentYourDevices();

            //Assert
            Assert.True(createRental.CheckValidationError(expectedMessageError), $"Expected error: {expectedMessageError}");
        }

        [Theory]
        [InlineData("M", "Calle Libertad 9, Ciudad Real", "CustomerNameSurname")] // UC2_13 - Nombre muy corto (<2 chars) - debe dar error
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_13_AF5_NameTooShortValidation(string nameSurname, string deliveryAddress, string expectedMessageError)
        {
            //Arrange - MinimumLength = 2 en DTO
            var createRental = new CreateRental_PO(_driver, _output);
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            //Act
            InitialStepsForRentalDevices_UIT();
            listDevices.FilterDevices("", "", from, to);
            listDevices.SelectDevices(new List<string> { deviceName1 });
            listDevices.RentDevices();
            createRental.FillInRentalInfo("maria@alu.uclm.es", nameSurname, deliveryAddress, "CreditCard");
            createRental.PressRentYourDevices();

            //Assert - Con MinimumLength = 2, nombre de 1 char debe dar error
            Assert.True(createRental.CheckValidationError(expectedMessageError), $"Expected error: {expectedMessageError}");
        }

        [Theory]
        [InlineData("Maria Martinez Gonzalez Diaz Rodriguez Perez Garcia", "Calle Libertad 9, Ciudad Real", "CustomerNameSurname")] // UC2_14 - Nombre muy largo (>50 chars) - debe dar error
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_14_AF5_NameTooLongValidation(string nameSurname, string deliveryAddress, string expectedMessageError)
        {
            //Arrange - MaxLength = 50 en DTO
            var createRental = new CreateRental_PO(_driver, _output);
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            //Act
            InitialStepsForRentalDevices_UIT();
            listDevices.FilterDevices("", "", from, to);
            listDevices.SelectDevices(new List<string> { deviceName1 });
            listDevices.RentDevices();
            createRental.FillInRentalInfo("maria@alu.uclm.es", nameSurname, deliveryAddress, "CreditCard");
            createRental.PressRentYourDevices();

            //Assert - Con MaxLength = 50, nombre > 50 chars debe dar error
            Assert.True(createRental.CheckValidationError(expectedMessageError), $"Expected error: {expectedMessageError}");
        }

        [Theory]
        [InlineData("Maria Martinez Gonzalez", "C", "DeliveryAddress")] // UC2_16 - Dirección muy corta (<2 chars) - debe dar error
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_16_AF5_AddressLengthValidation(string nameSurname, string deliveryAddress, string expectedMessageError)
        {
            //Arrange - MinimumLength = 2 en DTO (si aplica la misma restricción)
            var createRental = new CreateRental_PO(_driver, _output);
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            //Act
            InitialStepsForRentalDevices_UIT();
            listDevices.FilterDevices("", "", from, to);
            listDevices.SelectDevices(new List<string> { deviceName1 });
            listDevices.RentDevices();
            createRental.FillInRentalInfo("maria@alu.uclm.es", nameSurname, deliveryAddress, "CreditCard");
            createRental.PressRentYourDevices();

            //Assert - Con MinimumLength aplicado, dirección corta debe dar error
            Assert.True(createRental.CheckValidationError(expectedMessageError), $"Expected error: {expectedMessageError}");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_17_AF6_ModifyRentalItems()
        {
            //Arrange
            var createRental = new CreateRental_PO(_driver, _output);
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);

            //Act
            InitialStepsForRentalDevices_UIT();
            listDevices.FilterDevices("", "", from, to);
            listDevices.SelectDevices(new List<string> { deviceName1, deviceName2 });
            listDevices.RentDevices();

            createRental.PressModifyDevices();
            listDevices.ModifyRentingCart(deviceName2);
            listDevices.RentDevices();

            //Assert
            var expectedRentalItems = new List<string[]> {
                new string[] { deviceName1, deviceBrand1, deviceModel1, devicePriceForRenting1 },
            };
            Assert.True(createRental.CheckListOfRentalItems(expectedRentalItems));
        }

        [Fact(Skip = "First change the quantity of renting of the devices to 0")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_18_AF0_DevicesNotAvailableForRentalPeriod()
        {
            //Arrange
            var from = DateTime.Today.AddDays(2);
            var to = DateTime.Today.AddDays(3);
            var expectedMessage = "There are no devices available for being rented";

            //Act
            InitialStepsForRentalDevices_UIT();
            listDevices.FilterDevices("", "", from, to);

            //Assert
            Assert.True(listDevices.CheckMessageErrorNotAvailableDevices(expectedMessage));
        }

        [Theory]
        [InlineData("Maria Martinez Gonzalez", "Calle Libertad 9, Ciudad Real", "CreditCard")]
        [InlineData("Maria Martinez Gonzalez", "Calle Libertad 9, Ciudad Real", "PayPal")]
        [InlineData("Maria Martinez Gonzalez", "Calle Libertad 9, Ciudad Real", "Cash")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_1_2_3_BasicFlow(string nameSurname, string deliveryAddress, string paymentMethod)
        {
            //Arrange
            var createRental = new CreateRental_PO(_driver, _output);
            var detailRental = new DetailRental_PO(_driver, _output);
            var from = DateTime.Today.AddDays(1);
            var to = DateTime.Today.AddDays(2);

            //Act
            InitialStepsForRentalDevices_UIT();
            listDevices.FilterDevices("", "", from, to);
            listDevices.SelectDevices(new List<string> { deviceName1 });
            listDevices.RentDevices();

            // Usar un email válido como CustomerUserName
            createRental.FillInRentalInfo("maria@alu.uclm.es", nameSurname, deliveryAddress, paymentMethod);
            createRental.PressRentYourDevices();
            createRental.PressOkModalDialog();

            //Assert
            decimal totalPrice = decimal.Parse(devicePriceForRenting1, System.Globalization.CultureInfo.InvariantCulture) * (to - from).Days;
            Assert.True(detailRental.CheckRentalDetail(nameSurname,
                deliveryAddress, paymentMethod, DateTime.Now, from, to, totalPrice.ToString("0.00") + " €"),
                "Error: detail rental is not as expected");

            var expectedRentalItems = new List<string[]>
            {
                new string[] { deviceName1, deviceBrand1, deviceModel1, devicePriceForRenting1 + " €", "1" },
            };

            Assert.True(detailRental.CheckListOfDevices(expectedRentalItems),
                "Error: rental items are not as expected");
        }
    }
}