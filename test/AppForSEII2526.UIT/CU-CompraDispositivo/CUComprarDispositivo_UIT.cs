using AppForSEII2526.UT.UIT.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.CU_CompraDispositivo
{

    public class CUComprarDispositivo_UIT : UC_UIT
    {
        private SelectDevicesForPurchase_PO selectDevicesForPurchase_PO;
        private const int deviceId1 = 1;
        private const string deviceName1 = "iPhone 15";
        private const string deviceBrand1 = "Apple";
        private const string deviceModel1 = "iPhone 15";
        private const string deviceColor1 = "Negro";
        private const string devicePrice1 = "1200";

        private const int deviceId2 = 2;
        private const string deviceName2 = "Galaxy S23";
        private const string deviceBrand2 = "Samsung";
        private const string deviceModel2 = "Galaxy S23";
        private const string deviceColor2 = "Gris";
        private const string devicePrice2 = "999";



        public CUComprarDispositivo_UIT(ITestOutputHelper output) : base(output)
        {
            selectDevicesForPurchase_PO = new SelectDevicesForPurchase_PO(_driver, _output);
        }

        /*
        private void Precondition_perform_login()
        {
            Perform_login("laura@alu.uclm.es", "Password123!");
        }
        */

        private void InitialStepsForPurchase()
        {
            Initial_step_opening_the_web_page();
            //Precondition_perform_login();

            selectDevicesForPurchase_PO.WaitForBeingVisible(By.Id("CreatePurchase"));
            _driver.FindElement(By.Id("CreatePurchase")).Click();
        }



        // UC – Filtrar dispositivos
        [Theory]
        [InlineData(deviceName1, deviceColor1)]
        [InlineData(deviceName2, deviceColor2)]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC_Compra_AF1_FiltrarDispositivos(string name, string color)
        {
            // Arrange
            InitialStepsForPurchase();

            var expectedDevices = new List<string[]>
            {
                new string[] { name }
            };

            // Act
            selectDevicesForPurchase_PO.SearchDevice(name, color);

            // Assert
            Assert.True(
                selectDevicesForPurchase_PO.CheckListOfDevices(expectedDevices),
                "La lista de dispositivos no coincide con el filtro"
            );
        }

        // UC – Añadir y quitar del carrito
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC_Compra_AF2_AddAndRemoveDevice()
        {
            // Arrange
            InitialStepsForPurchase();

            // Act
            selectDevicesForPurchase_PO.AddDeviceToPurchaseCart(deviceId1);
            selectDevicesForPurchase_PO.RemoveDeviceFromPurchaseCart(deviceId1);

            // Assert
            Assert.True(
                selectDevicesForPurchase_PO.PurchaseNotAvailable(),
                "El botón de compra debería no estar disponible tras vaciar el carrito"
            );
        }


        // UC – Añadir varias unidades
        [Fact]
        [Trait("LevelTesting", "Functional Testing")]
        public void UC_Compra_AF3_AddMultipleUnitsOfSameDevice()
        {
            // Arrange
            InitialStepsForPurchase();

            // Act
            selectDevicesForPurchase_PO.AddDeviceToPurchaseCart(deviceId1);
            selectDevicesForPurchase_PO.AddDeviceToPurchaseCart(deviceId1);
            selectDevicesForPurchase_PO.AddDeviceToPurchaseCart(deviceId1);

            // Assert
            Assert.False(
                selectDevicesForPurchase_PO.PurchaseNotAvailable(),
                "El botón de compra debería estar disponible con productos en el carrito"
            );
        }

    }
}
