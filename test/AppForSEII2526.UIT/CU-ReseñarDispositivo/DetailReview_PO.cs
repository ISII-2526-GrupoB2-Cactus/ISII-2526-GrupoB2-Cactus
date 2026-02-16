using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;

namespace AppForSEII2526.UIT.ReviewDevices
{
    internal class DetailReviewPO : PageObject
    {
        public DetailReviewPO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public bool CheckReviewDetail(string name, string reviewTitle, string country)
        {
            try
            {
                // Esperar a que la página se navegue completamente
                System.Threading.Thread.Sleep(3000);
                
                _output.WriteLine($"URL actual: {_driver.Url}");
                _output.WriteLine($"Esperando elemento ReviewTitle...");

                // Intentar esperar a que ReviewTitle sea visible (máx 10 segundos)
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        WaitForBeingVisible(By.Id("ReviewTitle"));
                        break;
                    }
                    catch
                    {
                        if (i == 4)
                        {
                            _output.WriteLine($"⚠ ReviewTitle no se hizo visible después de múltiples intentos");
                        }
                        System.Threading.Thread.Sleep(1000);
                    }
                }
                
                System.Threading.Thread.Sleep(2000);
                
                // Verificar si hay un mensaje de error
                try
                {
                    var errorElement = _driver.FindElement(By.TagName("p"));
                    if (errorElement.Displayed && !string.IsNullOrEmpty(errorElement.Text))
                    {
                        _output.WriteLine($"✗ Error en la página: {errorElement.Text}");
                        return false;
                    }
                }
                catch { }

                bool result = true;

                // Verificar NameSurname
                try
                {
                    var nameSurnameElement = _driver.FindElement(By.Id("NameSurname"));
                    var nameSurnameText = nameSurnameElement.Text;
                    _output.WriteLine($"NameSurname encontrado: '{nameSurnameText}'");
                    if (string.IsNullOrEmpty(nameSurnameText))
                    {
                        _output.WriteLine($"⚠ NameSurname está vacío, reintentando en 2 segundos...");
                        System.Threading.Thread.Sleep(2000);
                        nameSurnameText = nameSurnameElement.Text;
                        _output.WriteLine($"NameSurname después de retry: '{nameSurnameText}'");
                    }
                    if (!nameSurnameText.Contains(name))
                    {
                        _output.WriteLine($"✗ NameSurname mismatch. Esperado: {name}, Encontrado: {nameSurnameText}");
                        result = false;
                    }
                    else
                    {
                        _output.WriteLine($"✓ NameSurname correcto");
                    }
                }
                catch (NoSuchElementException ex)
                {
                    _output.WriteLine($"✗ Elemento NameSurname no encontrado: {ex.Message}");
                    result = false;
                }

                // Verificar Country
                try
                {
                    var countryElement = _driver.FindElement(By.Id("Country"));
                    var countryText = countryElement.Text;
                    _output.WriteLine($"Country encontrado: '{countryText}'");
                    if (string.IsNullOrEmpty(countryText))
                    {
                        _output.WriteLine($"⚠ Country está vacío, reintentando en 2 segundos...");
                        System.Threading.Thread.Sleep(2000);
                        countryText = countryElement.Text;
                        _output.WriteLine($"Country después de retry: '{countryText}'");
                    }
                    if (!countryText.Contains(country))
                    {
                        _output.WriteLine($"✗ Country mismatch. Esperado: {country}, Encontrado: {countryText}");
                        result = false;
                    }
                    else
                    {
                        _output.WriteLine($"✓ Country correcto");
                    }
                }
                catch (NoSuchElementException ex)
                {
                    _output.WriteLine($"✗ Elemento Country no encontrado: {ex.Message}");
                    result = false;
                }

                // Verificar ReviewTitle
                try
                {
                    var reviewTitleElement = _driver.FindElement(By.Id("ReviewTitle"));
                    var reviewTitleText = reviewTitleElement.Text;
                    _output.WriteLine($"ReviewTitle encontrado: '{reviewTitleText}'");
                    if (string.IsNullOrEmpty(reviewTitleText))
                    {
                        _output.WriteLine($"⚠ ReviewTitle está vacío, reintentando en 2 segundos...");
                        System.Threading.Thread.Sleep(2000);
                        reviewTitleText = reviewTitleElement.Text;
                        _output.WriteLine($"ReviewTitle después de retry: '{reviewTitleText}'");
                    }
                    if (!reviewTitleText.Contains(reviewTitle))
                    {
                        _output.WriteLine($"✗ ReviewTitle mismatch. Esperado: {reviewTitle}, Encontrado: {reviewTitleText}");
                        result = false;
                    }
                    else
                    {
                        _output.WriteLine($"✓ ReviewTitle correcto");
                    }
                }
                catch (NoSuchElementException ex)
                {
                    _output.WriteLine($"✗ Elemento ReviewTitle no encontrado: {ex.Message}");
                    result = false;
                }

                // Verificar ReviewDate contiene el año actual
                try
                {
                    var reviewDateElement = _driver.FindElement(By.Id("ReviewDate"));
                    var reviewDateText = reviewDateElement.Text;
                    _output.WriteLine($"ReviewDate encontrado: '{reviewDateText}'");
                    if (!reviewDateText.Contains(DateTime.Now.Year.ToString()))
                    {
                        _output.WriteLine($"✗ ReviewDate no contiene el año actual");
                        result = false;
                    }
                    else
                    {
                        _output.WriteLine($"✓ ReviewDate correcto");
                    }
                }
                catch (NoSuchElementException ex)
                {
                    _output.WriteLine($"⚠ Elemento ReviewDate no encontrado: {ex.Message}");
                }

                if (result)
                {
                    _output.WriteLine($"✓ Todos los detalles de la reseña son correctos");
                }
                else
                {
                    _output.WriteLine($"\n=== DEBUG INFO ===");
                    _output.WriteLine($"URL: {_driver.Url}");
                    var pageSource = _driver.PageSource;
                    _output.WriteLine($"Page length: {pageSource.Length}");
                    if (pageSource.Contains("Error") || pageSource.Contains("error"))
                    {
                        _output.WriteLine($"⚠ La palabra 'Error' está en el HTML");
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error en CheckReviewDetail: {ex.Message}");
                _output.WriteLine($"StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public bool CheckListOfDevices(List<string[]> expectedReviewItems)
        {
            try
            {
                WaitForBeingVisible(By.Id("ReviewdDevices"));
                
                var table = _driver.FindElement(By.Id("ReviewdDevices"));
                var tbody = table.FindElement(By.TagName("tbody"));
                var rows = tbody.FindElements(By.TagName("tr")).ToList();

                if (rows.Count != expectedReviewItems.Count)
                {
                    _output.WriteLine($"✗ Número de filas incorrecto. Esperado: {expectedReviewItems.Count}, Actual: {rows.Count}");
                    return false;
                }

                bool result = true;

                for (int i = 0; i < rows.Count; i++)
                {
                    var cells = rows[i].FindElements(By.TagName("td")).ToList();
                    
                    // Esperamos: [Nombre, Modelo, Año, Rating, Comentario]
                    if (cells.Count < 5)
                    {
                        _output.WriteLine($"✗ Fila {i}: Número de celdas incorrecto. Esperado: 5, Actual: {cells.Count}");
                        result = false;
                        continue;
                    }

                    string deviceName = cells[0].Text.Trim();
                    string deviceModel = cells[1].Text.Trim();
                    string deviceYear = cells[2].Text.Trim();
                    string ratingText = cells[3].Text.Trim(); // Contiene estrellas o número
                    string comment = cells[4].Text.Trim();

                    // Verificar campos (ignorando el rating por ahora)
                    if (deviceName != expectedReviewItems[i][0])
                    {
                        _output.WriteLine($"✗ Fila {i}: Device name mismatch. Esperado: {expectedReviewItems[i][0]}, Actual: {deviceName}");
                        result = false;
                    }

                    if (deviceModel != expectedReviewItems[i][1])
                    {
                        _output.WriteLine($"✗ Fila {i}: Device model mismatch. Esperado: {expectedReviewItems[i][1]}, Actual: {deviceModel}");
                        result = false;
                    }

                    if (deviceYear != expectedReviewItems[i][2])
                    {
                        _output.WriteLine($"✗ Fila {i}: Device year mismatch. Esperado: {expectedReviewItems[i][2]}, Actual: {deviceYear}");
                        result = false;
                    }

                    // El rating puede estar en formato numérico o con estrellas, ambos son válidos
                    // Verificar que contiene el número esperado (puede estar entre paréntesis)
                    string expectedRating = expectedReviewItems[i][3];
                    if (!ratingText.Contains(expectedRating) && !ratingText.Contains(expectedRating + " / 5"))
                    {
                        _output.WriteLine($"✗ Fila {i}: Rating mismatch. Esperado: {expectedRating}, Actual: {ratingText}");
                        result = false;
                    }

                    if (comment != expectedReviewItems[i][4])
                    {
                        _output.WriteLine($"✗ Fila {i}: Comment mismatch. Esperado: {expectedReviewItems[i][4]}, Actual: {comment}");
                        result = false;
                    }
                }

                if (result)
                {
                    _output.WriteLine("✓ Todos los items de la reseña son correctos");
                }

                return result;
            }
            catch (Exception ex)
            {
                _output.WriteLine($"✗ Error en CheckListOfDevices: {ex.Message}");
                return false;
            }
        }
    }
}