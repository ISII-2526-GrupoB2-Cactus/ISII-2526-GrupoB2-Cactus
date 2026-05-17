using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class DetailReview_PO : PageObject
    {
        public DetailReview_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public bool CheckReviewDetail(string customerName, string customerCountry,
            string reviewTitle, DateTime reviewDate, string averageRating)
        {
            WaitForBeingVisible(By.Id("CustomerName"));
            bool result = true;

            result = result && _driver.FindElement(By.Id("CustomerName")).Text.Contains(customerName);
            result = result && _driver.FindElement(By.Id("CustomerCountry")).Text.Contains(customerCountry);
            result = result && _driver.FindElement(By.Id("ReviewTitle")).Text.Contains(reviewTitle);
            result = result && _driver.FindElement(By.Id("AverageRating")).Text.Contains(averageRating);

            var actualReviewDate = DateTime.Parse(_driver.FindElement(By.Id("ReviewDate")).Text);
            result = result && ((actualReviewDate - reviewDate) < new TimeSpan(0, 1, 0));

            return result;
        }

        public bool CheckListOfReviewedDevices(List<string[]> expectedDevices)
        {
            return CheckBodyTable(expectedDevices, By.Id("ReviewedDevices"));
        }
    }
}