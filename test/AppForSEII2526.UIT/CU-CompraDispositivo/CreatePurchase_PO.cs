using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.CU_CompraDispositivo
{
    public class CreatePurchase_PO:PageObject
    {
        private By _nameBy = By.Id("Name");
        private By _surnameBy = By.Id("Surname");
        private By _deliveryAddressBy = By.Id("DeliveryAddress");
        private By _paymentMethodBy = By.Id("PaymentMethod");
        private By _submitBy = By.Id("Submit");
        private By _modifyDevicesBy = By.Id("ModifyDevices");
        private By _tableOfItemsBy = By.Id("TableOfPurchaseItems");

        private IWebElement _name() => _driver.FindElement(_nameBy);
        private IWebElement _surname() => _driver.FindElement(_surnameBy);

        private IWebElement _deliveryAddress() => _driver.FindElement(By.Id("DeliveryAddress"));
        private IWebElement _paymentMethod() => _driver.FindElement(By.Id("PaymentMethod"));

        public CreatePurchase_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void FillInPurchaseInfo(string name, string surname, string deliveryAddress, string paymentMethod)
        {
            WaitForBeingVisible(_nameBy);

            _name().Clear();
            _name().SendKeys(name);

            _surname().Clear();
            _surname().SendKeys(surname);

            _deliveryAddress().Clear();
            _deliveryAddress().SendKeys(deliveryAddress);

            SelectElement selectElement = new SelectElement(_paymentMethod());
            selectElement.SelectByText(paymentMethod);
        }

        public void PressPurchaseDevices()
        {
            _driver.FindElement(_submitBy).Click();
        }

        public void PressModifyDevices()
        {
            _driver.FindElement(_modifyDevicesBy).Click();
        }


        public bool CheckListOfPurchaseItems(List<string[]> expectedItems)
        {
            return CheckBodyTable(expectedItems, _tableOfItemsBy);
        }

        public bool CheckValidationError(string expectedError)
        {
            return _driver.PageSource.Contains(expectedError);
        }








    }
}
