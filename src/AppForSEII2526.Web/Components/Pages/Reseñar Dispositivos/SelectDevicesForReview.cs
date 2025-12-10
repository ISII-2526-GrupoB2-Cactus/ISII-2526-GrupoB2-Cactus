using AppForSEII2526.UIT.Shared;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.Web.Components.Pages.Resenar_Dispositivos
{
    public class SelectDevicesForReview : PageObject
    {
        public SelectDevicesForReview(IWebDriver driver, ITestOutputHelper output): base(driver,output)
        {
        } //Se filtra por marca y año y se da al boton de buscar
    }
}
