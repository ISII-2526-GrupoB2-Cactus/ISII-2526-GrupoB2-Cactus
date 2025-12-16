using AppForSEII2526.UT.UIT.Shared;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class UCReviewDevices_UIT : UC_UIT
    {

        public UCReviewDevices_UIT(ITestOutputHelper output) : base(output)
        {
            // Initial_step_opening_the_web_page();
            _driver.Navigate().GoToUrl(_URI + "review/selectDevicesForReview");
            selectDevices = new SelectDevicesForReview_PO(_driver, _output);
        }

        private const int deviceId1 = 1;
        private const string deviceName1 = "iPhone 15";
        private const string deviceBrand1 = "Apple";
        private const string deviceModel1 = "iPhone 15";
        private const string deviceColor1 = "Negro";
        private const string deviceYear1 = "2023";
        private const string deviceReviewComment1 = "iPhone 15: cámara increíble y batería duradera";

        private const string deviceName2 = "Galaxy S23";
        private const string deviceBrand2 = "Samsung";
        private const string deviceModel2 = "Galaxy S23";
        private const string deviceColor2 = "Gris";
        private const string deviceYear2 = "2023";

        private const string deviceName3 = "PlayStation 5";
        private const string deviceBrand3 = "Sony";
        private const string deviceModel3 = "PlayStation 5";
        private const string deviceColor3 = "Blanco";
        private const string deviceYear3 = "2023";

        private SelectDevicesForReview_PO selectDevices;

        /*private void Precondition_perform_login()
        {
            // CAMBIA ESTAS CREDENCIALES POR LAS DE TU APLICACIÓN
            // Perform_login("Maria@ejemplo.com", "Password1234!");
        }*/

        /*private void InitialStepsForReviewDevices_UIT()
        {
            Precondition_perform_login();
            // Navegar a la página EXACTA de tu aplicación
            _driver.Navigate().GoToUrl(_URI + "review/selectDevicesForReview");
            // Esperar carga inicial (igual que tus profesoras)
            System.Threading.Thread.Sleep(3000);
        }*/



        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_2_AF1_filteringbyYear()
        {
            //Arrange
            // Filtrar por año 2023 debería mostrar 3 dispositivos ordenados alfabéticamente por nombre:
            // Galaxy S23, iPhone 15, PlayStation 5

            var expectedDevices = new List<string[]> {
                new string[] { deviceName2, deviceBrand2, deviceModel2, deviceYear2, deviceColor2, "Añadir" },
                new string[] { deviceName1, deviceBrand1, deviceModel1, deviceYear1, deviceColor1, "Añadir" },
                new string[] { deviceName3, deviceBrand3, deviceModel3, deviceYear3, deviceColor3, "Añadir" },
            };

            //Act
            //InitialStepsForReviewDevices_UIT();

            // Filtrar: Marca = "Todas", Año = "2023"
            selectDevices.FilterDevices("Todas", "2023");

            //Assert            
            Assert.True(selectDevices.CheckListOfDevices(expectedDevices),
                "Error: Los dispositivos filtrados por año 2023 no coinciden con los esperados");

        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_2_AF2_filteringbyBrand_Samsung()
        {
            //Arrange
            // Según tu documento: UC2_2/filtrar por marca -> "Samsung Galaxy S23"
            var expectedDevices = new List<string[]> {
                new string[] { deviceName2, deviceBrand2, deviceModel2, deviceYear2, deviceColor2, "Añadir" },
                // Si en tu base de datos hay más Samsungs (como el A53), añádelos aquí
                // new string[] { deviceName3, deviceBrand3, deviceModel3, deviceYear3, deviceColor3, "Añadir" },
            };

            //Act
            //InitialStepsForReviewDevices_UIT();

            // Filtrar: Marca = "Samsung", Año = "Todos"
            selectDevices.FilterDevices("Samsung", "Todos");

            //Assert            
            Assert.True(selectDevices.CheckListOfDevices(expectedDevices),
                "Error: Los dispositivos filtrados por marca Samsung no coinciden con los esperados");
        }

        // TERCERA PRUEBA - Filtrar por marca Samsung Y año 2023
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_3_AF1_filteringbyBrandAndYear_Samsung2023()
        {
            //Arrange
            // Según tu documento UC2_3: filtrar por Samsung y 2023 -> "Samsung Galaxy S23"
            var expectedDevices = new List<string[]> {
                new string[] { deviceName2, deviceBrand2, deviceModel2, deviceYear2, deviceColor2, "Añadir" },
            };

            //Act
            //InitialStepsForReviewDevices_UIT();

            // Filtrar: Marca = "Samsung", Año = "2023"
            selectDevices.FilterDevices("Samsung", "2023");

            //Assert            
            Assert.True(selectDevices.CheckListOfDevices(expectedDevices),
                "Error: Al filtrar por Samsung y año 2023, debería mostrar solo Samsung Galaxy S23");
        }


        // QUINTA PRUEBA - No hay dispositivos para reseñar (UC2_5)
        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_5_AF1_NoDevicesForReview()
        {
            //Arrange
            // Según tu documento UC2_5: "No hay dispositivos para reseñar"
            // Esto significa que el botón "Crear reseña" debe estar deshabilitado/oculto
            // cuando no se ha seleccionado ningún dispositivo

            //Act
            //InitialStepsForReviewDevices_UIT();

            // NO seleccionamos ningún dispositivo (carrito vacío)

            //Assert
            // El botón "Crear reseña" debe estar deshabilitado/oculto
            Assert.True(selectDevices.CheckCreateReviewButtonDisabled(),
                "El botón 'Crear reseña' debería estar deshabilitado cuando no hay dispositivos seleccionados");
        }

        


    }
}