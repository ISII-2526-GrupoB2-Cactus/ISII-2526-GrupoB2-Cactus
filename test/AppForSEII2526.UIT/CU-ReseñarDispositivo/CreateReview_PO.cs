using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.ReviewDevices
{
    internal class CreateReviewPO : PageObject
    {
        private By _ReviewTitleBy = By.Id("ReviewTitle");
        private IWebElement _reviewTitle() => _driver.FindElement(_ReviewTitleBy);
        private IWebElement _CustomerUserName() => _driver.FindElement(By.Id("CustomerUserName"));
        private IWebElement _Country() => _driver.FindElement(By.Id("Country"));
        private IWebElement _Comentario(int deviceId) => _driver.FindElement(By.Id("comentario_" + deviceId));
        private IWebElement _Rating(int deviceId) => _driver.FindElement(By.Id("rating_" + deviceId));

        public CreateReviewPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void FillInReviewInfo(string reviewTitle, string customerUserName, string country)
        {
            WaitForBeingVisible(_ReviewTitleBy);
            _reviewTitle().Click();
            System.Threading.Thread.Sleep(100);
            _reviewTitle().SendKeys(reviewTitle);
            System.Threading.Thread.Sleep(100);
            _CustomerUserName().Click();
            System.Threading.Thread.Sleep(100);
            _CustomerUserName().SendKeys(customerUserName);
            System.Threading.Thread.Sleep(100);
            _Country().Click();
            System.Threading.Thread.Sleep(100);
            _Country().SendKeys(country);
        }

        public void AddDeviceReviewComent(int deviceId, string comentario)
        {
            try
            {
                WaitForBeingVisible(By.Id($"comentario_{deviceId}"));
                var element = _Comentario(deviceId);
                element.Click();
                System.Threading.Thread.Sleep(100);
                element.SendKeys(comentario);
            }
            catch (NoSuchElementException ex)
            {
                _output.WriteLine($"Error: No se encontraron los campos para el dispositivo {deviceId}");
                _output.WriteLine($"Detalles: {ex.Message}");
                throw;
            }
        }

        public void AddDeviceReviewRating(int deviceId, int rating)
        {
            try
            {
                WaitForBeingVisible(By.Id($"rating_{deviceId}"));
                var selectElement = new SelectElement(_Rating(deviceId));
                selectElement.SelectByValue(rating.ToString());
                System.Threading.Thread.Sleep(200);
            }
            catch (NoSuchElementException ex)
            {
                _output.WriteLine($"Error: No se encontraron los campos para el dispositivo {deviceId}");
                _output.WriteLine($"Detalles: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error al seleccionar rating para dispositivo {deviceId}: {ex.Message}");
                throw;
            }
        }

        public void FillInReviewComent(string reviewComent, int deviceId)
        {
            _driver.FindElement(By.Id("comentario_" + deviceId)).SendKeys(reviewComent);
        }

        public void PressReviewYourDevices()
        {
            System.Threading.Thread.Sleep(1000);
            _driver.FindElement(By.Id("Submit")).Click();
        }

        public void PressOkModalDialog()
        {
            try
            {
                WaitForBeingClickable(By.Id("Button_DialogOK"));
                _driver.FindElement(By.Id("Button_DialogOK")).Click();
            }
            catch (NoSuchElementException ex)
            {
                _output.WriteLine($"Error: No se encontró el botón Button_DialogOK");
                _output.WriteLine($"Detalles: {ex.Message}");
                throw;
            }
        }

        public void PressModifyMovies()
        {
            _driver.FindElement(By.Id("ModifyDevices")).Click();
        }

        public bool CheckListOfReviewItems(List<string[]> expectedReviewItems)
        {
            return CheckBodyTable(expectedReviewItems, By.Id("TableOfReviewItems"));
        }

        public bool CheckValidationError(string expectedError)
        {
            return _driver.PageSource.Contains(expectedError);
        }
    }
}