using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.ReviewDevices
{
    public class CreateReview_PO : PageObject
    {
        private By _reviewTitleBy = By.Id("ReviewTitle");
        private IWebElement _reviewTitle() => _driver.FindElement(_reviewTitleBy);
        private IWebElement _customerCountry() => _driver.FindElement(By.Id("CustomerCountry"));
        private IWebElement _customerUserName() => _driver.FindElement(By.Id("CustomerUserName"));
        private IWebElement _submitButton() => _driver.FindElement(By.Id("Submit"));
        private IWebElement _modifyDevicesButton() => _driver.FindElement(By.Id("ModifyDevices"));

        public CreateReview_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public void FillInReviewInfo(string reviewTitle, string customerCountry, string customerUserName = "")
        {
            WaitForBeingVisible(_reviewTitleBy);
            _reviewTitle().SendKeys(reviewTitle);
            _customerCountry().SendKeys(customerCountry);

            if (!string.IsNullOrEmpty(customerUserName))
            {
                _customerUserName().SendKeys(customerUserName);
            }
        }

        public void FillInDeviceComment(string comment, int deviceId)
        {
            _driver.FindElement(By.Id($"comments_{deviceId}")).SendKeys(comment);
        }

        public void SelectDeviceRating(string rating, int deviceId)
        {
            SelectElement selectElement = new SelectElement(_driver.FindElement(By.Id($"rating_{deviceId}")));
            selectElement.SelectByText(rating);
        }

        public void PressPublishReview()
        {
            _submitButton().Click();
        }

        public void PressModifyDevices()
        {
            _modifyDevicesButton().Click();
        }

        public bool CheckListOfReviewItems(List<string[]> expectedReviewItems)
        {
            return CheckBodyTable(expectedReviewItems, By.Id("TableOfReviewItems"));
        }

        public bool CheckValidationError(string expectedError)
        {
            return _driver.PageSource.Contains(expectedError);
        }

        public bool CheckPublishButtonDisabled()
        {
            return !_submitButton().Enabled;
        }
    }
}