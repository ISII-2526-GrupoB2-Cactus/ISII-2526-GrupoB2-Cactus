using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class CreateReview_PO : PageObject
    {
        private By _reviewTitleBy = By.Id("ReviewTitle");
        private IWebElement _reviewTitle() => _driver.FindElement(_reviewTitleBy);
        private IWebElement _customerCountry() => _driver.FindElement(By.Id("CustomerCountry"));
        private IWebElement _customerUserName() => _driver.FindElement(By.Id("CustomerUserName"));
        private IWebElement _submitButton() => _driver.FindElement(By.Id("Submit"));
        private IWebElement _modifyDevicesButton() => _driver.FindElement(By.Id("ModifyDevices"));

        public CreateReview_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public void FillInReviewInfo(string reviewTitle, string customerCountry, string customerUserName = "")
        {
            WaitForBeingVisible(_reviewTitleBy);
            _reviewTitle().SendKeys(reviewTitle);
            _customerCountry().SendKeys(customerCountry);

            if (!string.IsNullOrEmpty(customerUserName))
            {
                _customerUserName().SendKeys(customerUserName);
            }
        }

        public void FillInDeviceComment(string comment, int deviceId)
        {
            _driver.FindElement(By.Id($"comments_{deviceId}")).SendKeys(comment);
        }

        public void SelectDeviceRating(string rating, int deviceId)
        {
            SelectElement selectElement = new SelectElement(_driver.FindElement(By.Id($"rating_{deviceId}")));
            selectElement.SelectByValue(rating);
        }

        public void FillInReviewItem(int deviceId, string rating, string comment)
        {
            SelectDeviceRating(rating, deviceId);
            if (!string.IsNullOrEmpty(comment))
            {
                FillInDeviceComment(comment, deviceId);
            }
        }

        public void PressPublishReview()
        {
            WaitForBeingClickable(By.Id("Submit"));
            _submitButton().Click();
        }

        public void PressModifyDevices()
        {
            _modifyDevicesButton().Click();
        }

        public bool CheckListOfReviewItems(List<string[]> expectedReviewItems)
        {
            return CheckBodyTable(expectedReviewItems, By.Id("TableOfReviewItems"));
        }

        public bool CheckValidationError(string expectedError)
        {
            System.Threading.Thread.Sleep(2000); // Esperar a que aparezca el error
            var pageSource = _driver.PageSource;
            
            _output?.WriteLine($"[DEBUG] Buscando error: '{expectedError}'");
            
            // Buscar el error en el ValidationSummary
            if (pageSource.Contains(expectedError))
            {
                _output?.WriteLine($"[DEBUG] ✓ Error encontrado en PageSource");
                return true;
            }
            
            // Buscar en el elemento de errores específico
            try
            {
                var errorElement = _driver.FindElement(By.Id("ErrorsShown"));
                var errorText = errorElement.Text;
                _output?.WriteLine($"[DEBUG] ErrorsShown text: '{errorText}'");
                if (errorText.Contains(expectedError))
                    return true;
            }
            catch (Exception ex) 
            { 
                _output?.WriteLine($"[DEBUG] ErrorsShown no encontrado: {ex.Message}");
            }
            
            // Buscar en alerts de peligro
            try
            {
                var alerts = _driver.FindElements(By.ClassName("alert-danger"));
                _output?.WriteLine($"[DEBUG] Found {alerts.Count} alert-danger elements");
                foreach (var alert in alerts)
                {
                    var alertText = alert.Text;
                    _output?.WriteLine($"[DEBUG] Alert text: '{alertText}'");
                    if (alertText.Contains(expectedError))
                        return true;
                }
            }
            catch (Exception ex)
            {
                _output?.WriteLine($"[DEBUG] Error checking alerts: {ex.Message}");
            }

            // Print primeras líneas del HTML después del submit para debugging
            _output?.WriteLine($"[DEBUG] === FULL PAGE SOURCE (primeros 5000 chars) ===");
            _output?.WriteLine(pageSource.Substring(0, Math.Min(5000, pageSource.Length)));
            _output?.WriteLine($"[DEBUG] === FIN PAGE SOURCE ===");
            
            return false;
        }

        public bool CheckPublishButtonDisabled()
        {
            return !_submitButton().Enabled;
        }
    }
}