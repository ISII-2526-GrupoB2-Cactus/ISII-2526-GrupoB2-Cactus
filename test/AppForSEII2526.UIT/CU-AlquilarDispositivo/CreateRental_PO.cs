using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_AlquilarDispositivo
{
    internal class CreateRental_PO : PageObject
    {
        private By _surnameBy = By.Id("Surname"); // "Surname" en HTML  no "NameSurname"
        private IWebElement _surname() => _driver.FindElement(_surnameBy);
        private IWebElement _deliveryAddress() => _driver.FindElement(By.Id("DeliveryAddress"));
        private IWebElement _paymentMethod() => _driver.FindElement(By.Id("PaymentMethod"));
        private IWebElement _customerUserName() => _driver.FindElement(By.Id("Name"));

        public CreateRental_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public void FillInRentalInfo(string customerUserName, string nameSurname, string deliveryAddress, string paymentMethod)
        {
            WaitForBeingVisible(_surnameBy);
            _customerUserName().Clear();
            _customerUserName().SendKeys(customerUserName);
            _surname().Clear();
            _surname().SendKeys(nameSurname);
            _deliveryAddress().Clear();
            _deliveryAddress().SendKeys(deliveryAddress);

            //create select element object 
            SelectElement selectElement = new SelectElement(_paymentMethod());

            //select Action from the dropdown menu
            selectElement.SelectByText(paymentMethod);
        }

        

        public void PressRentYourDevices()
        {
            _driver.FindElement(By.Id("Submit")).Click();
        }

        public void PressModifyDevices()
        {
            var modifyDevicesBy = By.Id("ModifyDevices");
            WaitForBeingVisible(modifyDevicesBy);
            _driver.FindElement(modifyDevicesBy).Click();
        }

        public bool CheckListOfRentalItems(List<string[]> expectedRentalItems)
        {
            return CheckBodyTable(expectedRentalItems, By.Id("TableOfRentalItems"));
        }

        public bool CheckValidationError(string expectedError)
        {
            return _driver.PageSource.Contains(expectedError);
        }

        public new void PressOkModalDialog()
        {
            base.PressOkModalDialog();
            // Wait for the navigation to detail page
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            wait.Until(d => d.Url.Contains("detailrental"));
        }
    }
}