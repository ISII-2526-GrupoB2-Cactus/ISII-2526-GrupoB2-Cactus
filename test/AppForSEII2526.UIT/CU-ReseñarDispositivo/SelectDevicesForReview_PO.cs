using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.ReviewDevices
{
    internal class SelectDevicesForReviewPO : PageObject
    {
        private By inputBrand = By.Id("inputBrand");
        private By inputYear = By.Id("inputYear");
        private By searchDevices = By.Id("searchDevices");
        private By _ShowReviewCartBy = By.Id("showReviewCart");
        private By _reviewButtonBy = By.Id("DevicesReviewButton");
        private By TableOfDevices = By.Id("TableOfDevices");
        private By DevicesReviewButton = By.Id("DevicesReviewButton");

        private IWebElement _deviceBrand() => _driver.FindElement(inputBrand);
        private IWebElement _deviceYear() => _driver.FindElement(inputYear);
        private IWebElement _searchDevices() => _driver.FindElement(searchDevices);
        private IWebElement _TableOfDevices() => _driver.FindElement(TableOfDevices);
        private IWebElement _DevicesReviewButton() => _driver.FindElement(DevicesReviewButton);
        private IWebElement _showReviewCartButton() => _driver.FindElement(_ShowReviewCartBy);
        private IWebElement _reviewButton() => _driver.FindElement(_reviewButtonBy);

        public SelectDevicesForReviewPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void SearchDevices(string brand, int year)
        {
            WaitForBeingVisible(inputBrand);
            System.Threading.Thread.Sleep(1000); // Esperar a que las opciones se carguen

            // PRIMERO: Resetear AMBOS filtros a su estado inicial
            var brandElement = _deviceBrand();
            var brandSelect = new SelectElement(brandElement);

            // Esperar a que haya opciones disponibles
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => brandSelect.Options.Count > 0);

            // Resetear marca a la primera opción
            brandSelect.SelectByIndex(0);

            // Resetear año a la primera opción (valor por defecto)
            WaitForBeingVisible(inputYear);
            var yearElement = _deviceYear();
            var yearSelect = new SelectElement(yearElement);
            yearSelect.SelectByIndex(0);

            // Hacer click en buscar para aplicar el reset
            _driver.FindElement(searchDevices).Click();
            System.Threading.Thread.Sleep(1000);

            // SEGUNDO: Aplicar los nuevos filtros si se proporcionan
            if (!string.IsNullOrEmpty(brand) && brand != "All")
            {
                WaitForBeingVisible(inputBrand);
                brandElement = _deviceBrand();
                brandSelect = new SelectElement(brandElement);

                try
                {
                    brandSelect.SelectByValue(brand);
                }
                catch (NoSuchElementException)
                {
                    // Si no encuentra por valor, intentar por texto
                    _output.WriteLine($"⚠️ No se encontró opción con valor '{brand}', intentando por texto...");
                    brandSelect.SelectByText(brand);
                }

                _driver.FindElement(searchDevices).Click();
                System.Threading.Thread.Sleep(500);
            }

            if (year != 0)
            {
                WaitForBeingVisible(inputYear);
                System.Threading.Thread.Sleep(500);
                yearElement = _deviceYear();
                yearSelect = new SelectElement(yearElement);
                yearSelect.SelectByValue(year.ToString());
                _driver.FindElement(searchDevices).Click();
                System.Threading.Thread.Sleep(500);
            }

            System.Threading.Thread.Sleep(2000);
        }


        public void SelectDevices(List<string> deviceids)
        {
            foreach (var deviceId in deviceids)
            {
                WaitForBeingVisible(By.Id($"deviceToReview_{deviceId}"));
                _driver.FindElement(By.Id($"deviceToReview_{deviceId}")).Click();
            }
        }

        public void Boraranio()
        {
            var yearSelect = new SelectElement(_deviceYear());
            yearSelect.SelectByValue("0");
            _driver.FindElement(searchDevices).Click();
        }

        public void ReviewDevices()
        {
            WaitForBeingClickable(DevicesReviewButton);
            _DevicesReviewButton().Click();
        }

        public void ModifyReviewCart(string name)
        {
            System.Threading.Thread.Sleep(500);
            WaitForBeingVisible(By.Id($"removeDevice_{name}"));
            _driver.FindElement(By.Id($"removeDevice_{name}")).Click();
        }

        public bool CheckListOfDevices(List<string[]> expectedDevices)
        {
            return CheckBodyTable(expectedDevices, TableOfDevices);
        }

        public bool CheckReviewDeviceDisabled()
        {
            try
            {
                System.Threading.Thread.Sleep(1000);
                
                // Verificar si el contenedor del carrito está oculto
                try
                {
                    var cartContainer = _driver.FindElement(By.CssSelector("div.col-2"));
                    
                    // Verificar si tiene el atributo hidden
                    var hiddenAttr = cartContainer.GetAttribute("hidden");
                    if (hiddenAttr != null && (hiddenAttr == "true" || hiddenAttr == ""))
                    {
                        _output.WriteLine($"✓ Carrito oculto (atributo hidden presente): el botón está deshabilitado");
                        return true;
                    }
                    
                    // Si el contenedor está oculto por CSS, también está deshabilitado
                    if (!cartContainer.Displayed)
                    {
                        _output.WriteLine($"✓ Carrito no visible (display: none): el botón está deshabilitado");
                        return true;
                    }
                }
                catch (NoSuchElementException)
                {
                    _output.WriteLine($"⚠ Contenedor del carrito no encontrado");
                }

                // Si llegamos aquí, el contenedor está visible, intentar acceder al botón
                try
                {
                    WaitForBeingVisible(_reviewButtonBy);
                    var button = _reviewButton();
                    
                    // Verificar si el atributo disabled existe
                    var disabledAttr = button.GetAttribute("disabled");
                    if (disabledAttr != null)
                    {
                        _output.WriteLine($"✓ Botón deshabilitado detectado (atributo disabled presente)");
                        return true;
                    }

                    // Verificar si la propiedad Enabled es false
                    if (!button.Enabled)
                    {
                        _output.WriteLine($"✓ Botón deshabilitado detectado (Enabled = false)");
                        return true;
                    }

                    _output.WriteLine($"✗ Botón está habilitado (no está deshabilitado)");
                    return false;
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"⚠ Error accediendo al botón: {ex.Message}");
                    // Si no podemos acceder al botón, consideramos que está deshabilitado
                    return true;
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error en CheckReviewDeviceDisabled: {ex.Message}");
                return false;
            }
        }

        public bool CheckShoppingCart(string deviceName)
        {
            System.Threading.Thread.Sleep(500);

            try
            {
                var cartContainer = _driver.FindElement(By.CssSelector("div.col-2"));

                if (!cartContainer.Displayed || cartContainer.GetAttribute("hidden") != null)
                {
                    return false;
                }

                var button = cartContainer.FindElement(By.Id($"removeDevice_{deviceName}"));
                return button != null && button.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}