using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.CU_AlquilarDispositivo
{
    internal class DetailRental_PO : PageObject
    {
        public DetailRental_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output) { }

        public bool CheckRentalDetail(string name, string address, string payment,
            DateTime rentalDate, DateTime from, DateTime to, string total)
        {
            WaitForBeingVisible(By.Id("TotalPrice"));

            return _driver.FindElement(By.Id("NameSurname")).Text.Contains(name)
                && _driver.FindElement(By.Id("DeliveryAddress")).Text.Contains(address)
                && _driver.FindElement(By.Id("PaymentMethod")).Text.Contains(payment)
                && _driver.FindElement(By.Id("TotalPrice")).Text.Contains(total);
        }

        public bool CheckListOfDevices(List<string[]> expected)
        {
            return CheckBodyTable(expected, By.Id("RentedDevices"));
        }
    }
}
