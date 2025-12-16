using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class SelectDevicesForReview_PO : PageObject
    {
        // Locators EXACTAMENTE como en tu HTML
        private By _brandFilterBy = By.Id("selectBrand");
        private By _yearFilterBy = By.Id("selectYear");
        private By _searchDevicesButtonBy = By.Id("searchDevices");
        private By _createReviewButtonBy = By.Id("createReviewButton");
        private By _devicesTableBy = By.Id("TableOfDevices");
        private By _errorAlertBy = By.ClassName("alert-danger");
        private By _errorsShownBy = By.Id("ErrorsShown");

        // Métodos shortcut (igual que tus profesoras)
        private IWebElement _brandFilter() => _driver.FindElement(_brandFilterBy);
        private IWebElement _yearFilter() => _driver.FindElement(_yearFilterBy);
        private IWebElement _searchDevicesButton() => _driver.FindElement(_searchDevicesButtonBy);
        private IWebElement _createReviewButton() => _driver.FindElement(_createReviewButtonBy);

        public SelectDevicesForReview_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public void FilterDevices(string brandFilter, string yearFilter)
        {
            WaitForBeingVisible(_brandFilterBy);

            // Filtrar por marca (según tu HTML: "All" = "Todas")
            if (brandFilter == "" || brandFilter == "All") brandFilter = "Todas";
            SelectElement brandSelect = new SelectElement(_brandFilter());
            brandSelect.SelectByText(brandFilter);

            // Filtrar por año (según tu HTML: "0" = "Todos")
            SelectElement yearSelect = new SelectElement(_yearFilter());
            if (yearFilter == "0" || yearFilter == "")
            {
                yearSelect.SelectByText("Todos");
            }
            else
            {
                // Tu HTML muestra números (2023, 2022, etc.)
                yearSelect.SelectByText(yearFilter);
            }

            // Hacer clic en buscar
            _searchDevicesButton().Click();

            // Esperar recarga de tabla (igual que tus profesoras)
            System.Threading.Thread.Sleep(2000);
        }

        public void SelectDevices(List<string> deviceNames)
        {
            foreach (var deviceName in deviceNames)
            {
                // Buscar por el texto en la primera columna (Nombre)
                WaitForBeingVisible(By.XPath($"//table[@id='TableOfDevices']//tr[td[1][text()='{deviceName}']]"));
                var row = _driver.FindElement(By.XPath($"//table[@id='TableOfDevices']//tr[td[1][text()='{deviceName}']]"));
                var addButton = row.FindElement(By.XPath(".//button[contains(@id, 'deviceToReview_')]"));
                addButton.Click();
            }
        }

        public void RemoveDeviceFromCart(string deviceName)
        {
            By locator = By.XPath($"//button[starts-with(@id, 'removeDevice_') and contains(., '{deviceName}')]");
            WaitForBeingVisible(locator);
            _driver.FindElement(locator).Click();
        }

        public void NavigateToCreateReview()
        {
            WaitForBeingClickable(_createReviewButtonBy);
            _createReviewButton().Click();
        }

        public bool CheckListOfDevices(List<string[]> expectedDevices)
        {
            return CheckBodyTable(expectedDevices, _devicesTableBy);
        }

        public bool CheckCreateReviewButtonDisabled()
        {
            try
            {
                // El botón está oculto cuando hideReviewCart = true (carrito vacío)
                var button = _createReviewButton();
                return !button.Displayed || !button.Enabled;
            }
            catch (NoSuchElementException)
            {
                return true; // Si no existe el botón, está "deshabilitado"
            }
        }

        public bool CheckReviewCartHasDevices(int expectedCount)
        {
            try
            {
                // Contar botones de remover (cada uno empieza con "removeDevice_")
                var removeButtons = _driver.FindElements(By.XPath("//button[starts-with(@id, 'removeDevice_')]"));
                return removeButtons.Count == expectedCount;
            }
            catch
            {
                return false;
            }
        }

        public bool CheckMessageError(string expectedError)
        {
            try
            {
                WaitForBeingVisible(_errorAlertBy);
                var errorText = _driver.FindElement(_errorsShownBy).Text;
                return errorText.Contains(expectedError);
            }
            catch
            {
                return false;
            }
        }
        // Añade estos métodos si no los tienes en tu PO:

        public void SelectDeviceByName(string deviceName)
        {
            // Buscar el botón "Añadir" para el dispositivo específico
            WaitForBeingVisible(By.XPath($"//table[@id='TableOfDevices']//tr[td[1][text()='{deviceName}']]"));
            var row = _driver.FindElement(By.XPath($"//table[@id='TableOfDevices']//tr[td[1][text()='{deviceName}']]"));
            var addButton = row.FindElement(By.XPath(".//button[contains(@id, 'deviceToReview_')]"));
            addButton.Click();
        }

        public bool IsDeviceInCart(string deviceName)
        {
            try
            {
                // NO usar WaitForBeingVisible aquí porque el elemento puede no existir
                // Solo intentar encontrarlo sin espera
                By locator = By.XPath($"//button[starts-with(@id, 'removeDevice_') and contains(., '{deviceName}')]");
                var removeButton = _driver.FindElement(locator);
                return removeButton.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

    }
}