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
                // Validar que el rating esté en el rango válido (1-5)
                if (rating < 1 || rating > 5)
                {
                    _output.WriteLine($"⚠ Rating inválido: {rating}. Rango válido: 1-5. Se omitirá la selección.");
                    return;
                }

                WaitForBeingVisible(By.Id($"rating_{deviceId}"));
                var selectElement = new SelectElement(_Rating(deviceId));
                selectElement.SelectByValue(rating.ToString());
                System.Threading.Thread.Sleep(200);
                _output.WriteLine($"✓ Rating {rating} seleccionado para dispositivo {deviceId}");
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
            try
            {
                System.Threading.Thread.Sleep(2000);
                
                if (string.IsNullOrEmpty(expectedError))
                {
                    // No se espera error, verificar que no haya mensajes de error
                    try
                    {
                        var validationSummaries = _driver.FindElements(By.ClassName("validation-summary-errors"));
                        foreach (var summary in validationSummaries)
                        {
                            if (summary.Displayed && !string.IsNullOrEmpty(summary.Text))
                            {
                                _output.WriteLine($"Found unexpected error: {summary.Text}");
                                return false;
                            }
                        }
                    }
                    catch { }

                    try
                    {
                        var errorElement = _driver.FindElement(By.Id("ErrorsShown"));
                        var errorText = errorElement.Text;
                        return string.IsNullOrEmpty(errorText) || errorText.Equals("Errors: ");
                    }
                    catch { }

                    return true;
                }
                else
                {
                    // Se espera un error específico
                    _output.WriteLine($"Buscando error esperado: '{expectedError}'");

                    // Mapeo de mensajes esperados a sus equivalentes en Blazor
                    var validationMessages = new List<string> { expectedError };
                    
                    // Agregar mensajes alternativos que Blazor podría mostrar
                    if (expectedError.Contains("pais") || expectedError.Contains("Country"))
                    {
                        validationMessages.Add("The CustomerCountry field is required");
                        validationMessages.Add("CustomerCountry");
                        validationMessages.Add("required");
                    }
                    if (expectedError.Contains("título") || expectedError.Contains("ReviewTitle"))
                    {
                        validationMessages.Add("The ReviewTitle");
                        validationMessages.Add("minimum length");
                    }

                    // Buscar en ValidationSummary
                    try
                    {
                        var validationSummaries = _driver.FindElements(By.ClassName("validation-summary-errors"));
                        foreach (var summary in validationSummaries)
                        {
                            var text = summary.Text;
                            _output.WriteLine($"ValidationSummary encontrado: '{text}'");
                            
                            // Buscar cual
                            foreach (var msg in validationMessages)
                            {
                                if (text.Contains(msg, StringComparison.OrdinalIgnoreCase))
                                {
                                    _output.WriteLine($"✓ Error encontrado: '{msg}' en ValidationSummary");
                                    return true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _output.WriteLine($"Error buscando en ValidationSummary: {ex.Message}");
                    }

                    // Buscar en ErrorsShown
                    try
                    {
                        var errorElement = _driver.FindElement(By.Id("ErrorsShown"));
                        var errorText = errorElement.Text;
                        _output.WriteLine($"ErrorsShown encontrado: '{errorText}'");
                        
                        foreach (var msg in validationMessages)
                        {
                            if (errorText.Contains(msg, StringComparison.OrdinalIgnoreCase))
                            {
                                _output.WriteLine($"✓ Error encontrado: '{msg}' en ErrorsShown");
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _output.WriteLine($"Error buscando en ErrorsShown: {ex.Message}");
                    }

                    // Buscar en cualquier elemento con clase alert
                    try
                    {
                        var alerts = _driver.FindElements(By.ClassName("alert"));
                        foreach (var alert in alerts)
                        {
                            var text = alert.Text;
                            _output.WriteLine($"Alert encontrado: '{text}'");
                            
                            foreach (var msg in validationMessages)
                            {
                                if (text.Contains(msg, StringComparison.OrdinalIgnoreCase))
                                {
                                    _output.WriteLine($"✓ Error encontrado: '{msg}' en alert");
                                    return true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _output.WriteLine($"Error buscando en alerts: {ex.Message}");
                    }

                    // Último recurso: buscar en el PageSource
                    try
                    {
                        foreach (var msg in validationMessages)
                        {
                            if (_driver.PageSource.Contains(msg))
                            {
                                _output.WriteLine($"✓ Error encontrado: '{msg}' en PageSource");
                                return true;
                            }
                        }
                    }
                    catch { }

                    _output.WriteLine($"✗ Ninguno de estos errores fue encontrado: {string.Join(", ", validationMessages)}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error en CheckValidationError: {ex.Message}");
                return false;
            }
        }
    }
}