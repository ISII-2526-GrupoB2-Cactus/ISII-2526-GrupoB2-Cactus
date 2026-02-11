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
            WaitForBeingVisible(By.Id("ReviewTitle"));
            bool result = true;
            result = result && _driver.FindElement(By.Id("NameSurname")).Text.Contains(name);
            result = result && _driver.FindElement(By.Id("Country")).Text.Contains(country);
            result = result && _driver.FindElement(By.Id("ReviewTitle")).Text.Contains(reviewTitle);

            result = result && _driver.FindElement(By.Id("ReviewDate")).Text.Contains(DateTime.Now.Year.ToString());

            return result;
        }

        public bool CheckListOfDevices(List<string[]> expectedReviewItems)
        {
            return CheckBodyTable(expectedReviewItems, By.Id("ReviewdDevices"));
        }
    }
}