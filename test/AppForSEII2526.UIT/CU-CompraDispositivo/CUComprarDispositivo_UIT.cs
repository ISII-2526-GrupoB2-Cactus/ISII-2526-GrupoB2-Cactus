using AppForSEII2526.UT.UIT.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppForSEII2526.UIT.CU_CompraDispositivo
{

    public class CUComprarDispositivo_UIT : UC_UIT
    {
        private SelectDevicesForPurchase_PO selectDevicesForPurchase_PO;
        private const int deviceId1 = 1;
        private const string deviceName1 = "iPhone 15";
        private const string deviceBrand1 = "Apple";
        private const string deviceModel1 = "Galaxy S23";
        private const string deviceColor1 = "Negro";
        private const string devicePrice1 = "1200";

        private const int deviceId2 = 2;
        private const string deviceName2 = "Galaxy S23";
        private const string deviceBrand2 = "Samsung";
        private const string deviceModel2 = "iPhone 15";
        private const string deviceColor2 = "Gris";
        private const string devicePrice2 = "999";



        public CUComprarDispositivo_UIT(ITestOutputHelper output) : base(output)
        {
            selectDevicesForPurchase_PO = new SelectDevicesForPurchase_PO(_driver, _output);
        }


        private void Precondition_perform_login()
        {
            Perform_login("laura@alu.uclm.es", "Password123!");
        }



        private void InitialStepsForPurchase()
        {
            Initial_step_opening_the_web_page();
            Precondition_perform_login();

            selectDevicesForPurchase_PO.WaitForBeingVisible(By.Id("CreatePurchase"));
            _driver.FindElement(By.Id("CreatePurchase")).Click();
        }


        
        [Theory]
        [InlineData("Laura", "Gonzalez Rico", "Calle Angel 1", "CreditCard")]
        [InlineData("Laura", "Gonzalez Rico", "Calle Angel 1", "PayPal")]
        [Trait("LevelTesting", "Functional Testing")]
        public void CP_01_02_FlujoBasico(string name, string surname, string deliveryAddress, string paymentMethod)
        {
            
            var createPurchase = new CreatePurchase_PO(_driver, _output);
            var detailPurchase = new DetailPurchase_PO(_driver, _output);

            InitialStepsForPurchase();

           
            selectDevicesForPurchase_PO.AddDeviceToPurchaseCart(deviceId1);

            
            _driver.FindElement(By.Id("purchaseDeviceButton")).Click();

            
            createPurchase.FillInPurchaseInfo(
                name,
                surname,
                deliveryAddress,
                paymentMethod
            );

            
            createPurchase.PressPurchaseDevices();
            createPurchase.PressOkModalDialog();

            
            Assert.True(
                detailPurchase.CheckPurchaseDetail(name, surname, deliveryAddress, paymentMethod, DateTime.Now, devicePrice1 + " €" ),
                "El detalle de la compra no es correcto"
            );

            
            var expectedDevices = new List<string[]>
            {
                new string[]
                {
                    deviceBrand1,
                    deviceModel1,
                    deviceColor1,
                    devicePrice1 + " €",
                    "1"
                }
            };

            Assert.True(
                detailPurchase.CheckListOfPurchasedDevices(expectedDevices),
                "Los dispositivos comprados no son correctos"
            );
        }

        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void CP_03_NoDispositivoDisponible()
        {
            
            InitialStepsForPurchase();

            selectDevicesForPurchase_PO.SearchDevice("NoExiste", "Rosa");

            
            Assert.True(
                selectDevicesForPurchase_PO.NoDevicesAvailable(),
                "No debería haber dispositivos disponibles para vender"
            );
        }



        [Theory]
        [InlineData(deviceName1, deviceBrand1, deviceModel1, deviceColor1, devicePrice1, "iPhone 15", "")]
        [InlineData(deviceName1, deviceBrand1, deviceModel1, deviceColor1, devicePrice1, "", "Negro")]
        [InlineData(deviceName1, deviceBrand1, deviceModel1, deviceColor1, devicePrice1, "iPhone 15", "Negro")]
        [Trait("LevelTesting", "Functional Testing")]
        public void CP_04_05_06_Filtros(string name, string brand, string model, string color, string price, string filterName, string filterColor)
        {
                InitialStepsForPurchase();
                var expectedDevices = new List<string[]> { new string[] { name, brand, model, color, price + " €" }, };

                selectDevicesForPurchase_PO.SearchDevice(filterName, filterColor);


                Assert.True(
                    selectDevicesForPurchase_PO.CheckListOfDevices(expectedDevices),
                    "La lista de dispositivos no coincide con el filtro aplicado"
                );
        }


            


         
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void CP_07_AñadirYQuitarDispositivo()
        {
            
            InitialStepsForPurchase();

            
            selectDevicesForPurchase_PO.AddDeviceToPurchaseCart(deviceId1);
            selectDevicesForPurchase_PO.RemoveDeviceFromPurchaseCart(deviceId1);

            
            Assert.True(
                selectDevicesForPurchase_PO.PurchaseNotAvailable(),
                "El botón de compra debe estar oculto si el carrito está vacío"
            );
        }



        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void CP_08_ComprasinDispositivos()
        {

            InitialStepsForPurchase();


            Assert.True(
                selectDevicesForPurchase_PO.PurchaseNotAvailable(),
                "No se debe poder comprar sin dispositivos en el carrito"
            );
        }

        
        [Theory]
        [InlineData("", "Gonzalez Rico", "Calle Angel 1", "CreditCard", "The CustomerName field is required.")]
        [InlineData("Laura", "", "Calle Angel 1", "CreditCard", "The CustomerSurname field is required.")]
        [InlineData("Laura", "Gonzalez Rico", "", "CreditCard", "The DeliveryAddress field is required.")]
        [Trait("LevelTesting", "Functional Testing")]
        public void CP_9_10_11_12_DatosObligatorios(string name, string surname, string deliveryAddress, string paymentMethod, string expectedError)
        {
            
            InitialStepsForPurchase();
            var createPurchase = new CreatePurchase_PO(_driver, _output);

            selectDevicesForPurchase_PO.AddDeviceToPurchaseCart(deviceId1);
            _driver.FindElement(By.Id("purchaseDeviceButton")).Click();

            
            createPurchase.FillInPurchaseInfo(name, surname, deliveryAddress, paymentMethod);
            createPurchase.PressPurchaseDevices();

            
            Assert.True(
                createPurchase.CheckValidationError(expectedError),
                $"Debería aparecer el error: {expectedError}"
            );
        }



        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void CP_13_VolverAtrasTrasRellenarDatosUsuario()
        {
            
            InitialStepsForPurchase();
            var createPurchase = new CreatePurchase_PO(_driver, _output);

            
            selectDevicesForPurchase_PO.AddDeviceToPurchaseCart(deviceId1);
            _driver.FindElement(By.Id("purchaseDeviceButton")).Click();

            
            createPurchase.FillInPurchaseInfo(
                "Laura",
                "Gonzalez Rico",
                "C/ Pez 1",
                "CreditCard"
            );

            
            createPurchase.PressModifyDevices();

            
            _driver.FindElement(By.Id("purchaseDeviceButton")).Click();

            
            Assert.True(
                createPurchase.CheckPurchaseFormData(
                    "Laura",
                    "Gonzalez Rico",
                    "C/ Pez 1",
                    "CreditCard"
                ),
                "Los datos del usuario deberían mantenerse al volver al formulario"
            );
        }




    }
}
