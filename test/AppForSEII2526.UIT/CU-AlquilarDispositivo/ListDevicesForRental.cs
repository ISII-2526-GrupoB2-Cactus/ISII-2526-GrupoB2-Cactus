using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_AlquilarDispositivo
{
    internal class ListDevicesForRental_PO : PageObject
    {
        private By _searchDevicesBy = By.Id("searchDevices");
        private By _rentButtonBy = By.Id("purchaseDeviceButton");
        private By _tableOfDevicesBy = By.Id("TableOfDevices");

        public ListDevicesForRental_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output) { }

        public void FilterDevices(string model, string price, DateTime from, DateTime to)
        {
            _driver.FindElement(By.Id("inputModel")).SendKeys(model);
            _driver.FindElement(By.Id("inputPrice")).SendKeys(price);
            InputDateInDatePicker(By.Id("fromDate"), from);
            InputDateInDatePicker(By.Id("toDate"), to);
            _driver.FindElement(_searchDevicesBy).Click();
            Thread.Sleep(1500);
        }

        public void SelectDevices(List<string> deviceNames)
        {
            foreach (var name in deviceNames)
            {
                var safe = name.Replace(" ", "_");
                var locator = By.Id($"deviceToRent_{safe}");
                WaitForBeingVisible(locator);
                _driver.FindElement(locator).Click();
            }
        }

        public void ModifyRentingCart(string deviceName)
        {
            var safe = deviceName.Replace(" ", "_");
            var locator = By.Id($"removeDevice_{safe}");
            WaitForBeingVisible(locator);
            _driver.FindElement(locator).Click();
        }

        public void RentDevices()
        {
            _driver.FindElement(_rentButtonBy).Click();
        }

        public bool CheckListOfDevices(List<string[]> expected)
        {
            return CheckBodyTable(expected, _tableOfDevicesBy);
        }

        public bool CheckShoppingCart(string price)
        {
            // El precio se muestra en los items del carrito, no en el botón
            // Buscamos el precio en el PageSource
            return _driver.PageSource.Contains(price);
        }

        public bool CheckRentDevicesDisabled()
        {
            try
            {
                var button = _driver.FindElement(_rentButtonBy);
                // Si el botón existe, verificar si está deshabilitado o no está visible
                return !button.Enabled || !button.Displayed;
            }
            catch (NoSuchElementException)
            {
                // Si el botón no existe, se considera como "deshabilitado"
                return true;
            }
        }

        public bool CheckMessageError(string error)
        {
            return _driver.PageSource.Contains(error);
        }

        public bool CheckMessageErrorNotAvailableDevices(string error)
        {
            return _driver.PageSource.Contains(error);
        }
    }
}
