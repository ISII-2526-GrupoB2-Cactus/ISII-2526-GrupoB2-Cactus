using AppForSEII2526.UIT.Shared;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class UCReviewDevices_UIT : UC_UIT
    {
        public UCReviewDevices_UIT(ITestOutputHelper output) : base(output)
        {
            Initial_step_opening_the_web_page();
            selectDevices = new SelectDevicesForReviewPO(_driver, _output);
        }

        private const int deviceId1 = 1;
        private const string deviceName1 = "iPhone 15";
        private const string deviceColor1 = "Negro";
        private const string deviceBrand1 = "Apple";
        private const int deviceYear1 = 2023;
        private const string deviceModel1 = "iPhone 15";
        private const int devicerating1 = 5;
        private const string devicecomentario1 = "Reseña para";

        private const int deviceId2 = 2;
        private const string deviceName2 = "Galaxy S23";
        private const string deviceColor2 = "Gris";
        private const string deviceBrand2 = "Samsung";
        private const int deviceYear2 = 2023;
        private const string deviceModel2 = "Galaxy S23";


        private const int deviceId3 = 2;
        private const string deviceName3 = "Galaxy S23";
        private const string deviceColor3 = "Gris";
        private const string deviceBrand3 = "Samsung";
        private const int deviceYear3 = 2023;
        private const string deviceModel3 = "Galaxy S23";
        private SelectDevicesForReviewPO selectDevices;

        private void Precondition_perform_login()
        {
            Perform_login("maria@alu.uclm.es", "Password1234%");
        }

        private void InitialStepsForReviewDevice_UIT()
        {
            //Precondition_perform_login();
            Thread.Sleep(1000);

            selectDevices.WaitForBeingVisibleIgnoringExeptionTypes(By.Id("CreateReview"));
            _driver.FindElement(By.Id("CreateReview")).Click();
        }

        [Theory]
        [InlineData(deviceBrand1, deviceColor1, deviceName1, deviceYear1, deviceModel1, "Apple", 2023)]
        [InlineData(deviceBrand2, deviceColor2, deviceName2, deviceYear2, deviceModel2, "Samsung", 2023)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_2_3_filteringbyBrandandYear(string brand, string color,
            string name, int year, string model, string filterBrand, int filterYear)
        {
            var expectedDevices = new List<string[]> { new string[] { name, brand, model, year.ToString(), color } };

            InitialStepsForReviewDevice_UIT();
            selectDevices.SearchDevices(filterBrand, filterYear);

            Assert.True(selectDevices.CheckListOfDevices(expectedDevices));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_4_ModifySelecteDevices()
        {
            InitialStepsForReviewDevice_UIT();
            selectDevices.SelectDevices(new List<string> { deviceId1.ToString(), deviceId2.ToString() });
            Thread.Sleep(500);
            selectDevices.ModifyReviewCart(deviceName2);

            Assert.True(selectDevices.CheckShoppingCart(deviceName1));
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_5_ReviewButtonNotAvailable()
        {
            InitialStepsForReviewDevice_UIT();
            selectDevices.SelectDevices(new List<string> { deviceId1.ToString() });
            Thread.Sleep(500);
            selectDevices.ModifyReviewCart(deviceName1);
            Thread.Sleep(500);

            Assert.True(selectDevices.CheckReviewDeviceDisabled(), "You must select at least one device");
        }

        [Theory]
        [InlineData("maria@alu.uclm.es", "Spain", "", "Good device", 5, "El título debe tener entre 10 y 50 caracteres")]
        [InlineData("maria@alu.uclm.es", "", "Perfecto rendimiento", "Good device", 5, "Por favor, ingrese pais")]
        [InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Good device", null, "")]
        [InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Very good device", 5, "")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_6_7_8_9_testingErrorsMandatorydata(string username, string country, string reviretitle, string comentario, int? rating,
           string expectedMessageError)
        {
            var createreview = new CreateReviewPO(_driver, _output);

            InitialStepsForReviewDevice_UIT();
            selectDevices.SelectDevices(new List<string> { deviceId1.ToString() });
            selectDevices.ReviewDevices();
            createreview.FillInReviewInfo(reviretitle, username, country);
            Thread.Sleep(1000);
            if (!string.IsNullOrEmpty(comentario))
            {
                createreview.AddDeviceReviewComent(deviceId1, comentario);
                Thread.Sleep(500);
            }
            if (rating.HasValue)
            {
                createreview.AddDeviceReviewRating(deviceId1, rating.Value);
                Thread.Sleep(500);
            }
            createreview.PressReviewYourDevices();

            Assert.True(createreview.CheckValidationError(expectedMessageError), $"Expected error: {expectedMessageError}");
        }

        [Fact]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_10_ModifyRentalItems()
        {
            var createreview = new CreateReviewPO(_driver, _output);

            InitialStepsForReviewDevice_UIT();
            selectDevices.SelectDevices(new List<string> { deviceId1.ToString() });
            selectDevices.ReviewDevices();
            Thread.Sleep(1000);
            createreview.PressModifyMovies();

            Assert.True(selectDevices.CheckShoppingCart(deviceName1));
        }

        [Theory]
        [InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Great product", -5, "")]
        [InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Not recommended", 5, "")]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC3_11_12_testingErrorsvalidationdata(string username, string country, string reviretitle, string comentario, int rating,
          string expectedMessageError)
        {
            var createreview = new CreateReviewPO(_driver, _output);

            InitialStepsForReviewDevice_UIT();
            selectDevices.SelectDevices(new List<string> { deviceId1.ToString() });
            selectDevices.ReviewDevices();
            createreview.FillInReviewInfo(reviretitle, username, country);
            createreview.AddDeviceReviewComent(deviceId1, comentario);
            Thread.Sleep(1000);
            createreview.AddDeviceReviewRating(deviceId1, rating);
            Thread.Sleep(1000);
            createreview.PressReviewYourDevices();

            Assert.True(createreview.CheckValidationError(expectedMessageError), $"Expected error: {expectedMessageError}");
        }

        [Theory]
        [InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Reseña para", 5)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_1_BasicFlow(string username, string country, string reviretitle, string comentario, int rating)
        {
            var createreview = new CreateReviewPO(_driver, _output);
            var detailReview = new DetailReviewPO(_driver, _output);

            InitialStepsForReviewDevice_UIT();
            selectDevices.SelectDevices(new List<string> { deviceId1.ToString() });
            selectDevices.ReviewDevices();
            createreview.FillInReviewInfo(reviretitle, username, country);
            Thread.Sleep(500);
            createreview.AddDeviceReviewComent(deviceId1, comentario);
            Thread.Sleep(1000);
            createreview.AddDeviceReviewRating(deviceId1, rating);
            Thread.Sleep(1000);
            createreview.PressReviewYourDevices();
            Thread.Sleep(500);
            createreview.PressOkModalDialog();
            Thread.Sleep(500);

            Assert.True(detailReview.CheckReviewDetail(username, reviretitle, country),
                "Error: detail review is not as expected");

            var expectedReviewItems = new List<string[]>
            {
                new string[] { deviceName1, deviceModel1, deviceYear1.ToString(), devicerating1.ToString(), devicecomentario1 }
            };

            Assert.True(detailReview.CheckListOfDevices(expectedReviewItems),
                "Error: rental items are not as expected");
        }

        /*[Theory]
        [InlineData("maria@uclm.es", "Spain", "Perfecto rendimiento", "Reseña para", 5, "Apple", 2023)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UCExamen(string username, string country, string reviretitle, string comentario, int rating, string brandfiltro, int? yearfiltro)
        {
            var createreview = new CreateReviewPO(_driver, _output);
            var detailReview = new DetailReviewPO(_driver, _output);

            InitialStepsForReviewDevice_UIT();
            selectDevices.SelectDevices(new List<string> { deviceId2.ToString() });
            Thread.Sleep(500);
            selectDevices.SearchDevices(brandfiltro, yearfiltro ?? 0);
            selectDevices.Boraranio();
            Thread.Sleep(1000);
            selectDevices.SelectDevices(new List<string> { deviceId1.ToString() });
            Thread.Sleep(500);
            selectDevices.ModifyReviewCart(deviceName2);
            selectDevices.ReviewDevices();
            createreview.FillInReviewInfo(reviretitle, username, country);
            Thread.Sleep(500);
            createreview.AddDeviceReviewComent(deviceId1, comentario);
            Thread.Sleep(1000);
            createreview.AddDeviceReviewRating(deviceId1, rating);
            Thread.Sleep(1000);
            createreview.PressReviewYourDevices();
            Thread.Sleep(500);
            createreview.PressOkModalDialog();
            Thread.Sleep(500);

            Assert.True(detailReview.CheckReviewDetail(username, reviretitle, country),
                "Error: detail review is not as expected");

            var expectedReviewItems = new List<string[]>
            {
                new string[] { deviceName1, deviceModel1, deviceYear1.ToString(), devicerating1.ToString(), devicecomentario1 }
            };

            Assert.True(detailReview.CheckListOfDevices(expectedReviewItems),
                "Error: rental items are not as expected");
        }*/

        
        [Theory]
        [InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Reseña para", 5)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC2_2_BasicFlowOptizado(string username, string country, string reviretitle, string comentario, int rating)
        {
            var createreview = new CreateReviewPO(_driver, _output); //Crea la reseña
            var detailReview = new DetailReviewPO(_driver, _output); //Mira que se creo bien 

            InitialStepsForReviewDevice_UIT(); //Inicializa la pagina
            selectDevices.SelectDevices(new List<string> { deviceId1.ToString() }); //Selecciona el primer dispositivo
            selectDevices.ReviewDevices(); //Se va a crear la reseña 
            createreview.FillInReviewInfo(reviretitle, username, country);
            createreview.AddDeviceReviewComent(deviceId1, comentario);
            createreview.AddDeviceReviewRating(deviceId1, rating); //Rellena la reseña
            createreview.PressReviewYourDevices(); //La crea 
            createreview.PressOkModalDialog(); //Le da a aceptar 

            var expectedReviewItems = new List<string[]>
            {
                new string[] { deviceName1, deviceModel1, deviceYear1.ToString(), devicerating1.ToString(), devicecomentario1 }
            };//Espera que salga iPhone 15", "iPhone 15", "2023", "5", "Reseña para

            Assert.True(detailReview.CheckCompleteReview(username, reviretitle, country, expectedReviewItems),
                "Error: la reseña no se creó correctamente con todos los datos esperados");
            //El metodo CheckCompleteReview mira que los datos introducidos se hayan guardado bien y sean correctos 
        }

        [Theory]
        [InlineData("maria@alu.uclm.es", "Spain", "Perfecto rendimiento", "Reseña para", 5)]
        [Trait("LevelTesting", "Funcional Testing")]
        public void UC_ExamenNuestro(string username, string country, string reviewTitle, string comentario, int rating)
        {
            var createreview = new CreateReviewPO(_driver, _output);
            var detailReview = new DetailReviewPO(_driver, _output);
            //Filtro por uno, añado, filtro por otro, añado y borro el primero

            InitialStepsForReviewDevice_UIT();

            //Filtro por el primero
            selectDevices.SearchDevices(deviceBrand3, 0);
            selectDevices.SelectDevices(new List<string> { deviceId3.ToString() }); //Añado dispositivo

            
            //Filtro por el segundo
            selectDevices.SearchDevices(null, deviceYear1);
            selectDevices.SelectDevices(new List<string> { deviceId1.ToString() }); //Añado dispositivo

            selectDevices.SearchDevices(null, 0);
            selectDevices.SelectDevices(new List<string> { deviceId2.ToString() }); //Añado dispositivo

            selectDevices.ModifyReviewCart(deviceName3); //Borro el primero
            selectDevices.ModifyReviewCart(deviceName1); //Borro el segundo

            selectDevices.ReviewDevices(); //Y ya termino con el flujo basico 

            createreview.FillInReviewInfo(reviewTitle, username, country);
            createreview.AddDeviceReviewComent(deviceId1, comentario);
            createreview.AddDeviceReviewRating(deviceId1, rating);
            createreview.PressReviewYourDevices();
            createreview.PressOkModalDialog();

            var expectedReviewItems = new List<string[]> { new string[] { deviceName1, deviceModel1, deviceYear1.ToString(), rating.ToString(), comentario } };

            Assert.True(detailReview.CheckCompleteReview(username, reviewTitle, country, expectedReviewItems),
                "Error: la reseña no contiene únicamente el segundo dispositivo tras eliminar el primero.");
        }


    }
}