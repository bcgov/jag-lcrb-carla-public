using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"the correct data is displayed for a(.*)")]
        public void AccountProfileData(string bizType)
        {
            /* 
            Page Title: Account Profile
            */

            // check legal name field is populated
            var uiLegalName = ngDriver.FindElement(By.CssSelector(
                "div:nth-of-type(2) > div:nth-of-type(1) > div > div > div:nth-of-type(1) > app-field:nth-of-type(1) > section > div > section > input"));
            var fieldValueLegalName = uiLegalName.GetAttribute("value");
            Assert.True(fieldValueLegalName != null);

            // check business number is correct
            var uiBusinessNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='businessNumber']"));
            Assert.True(uiBusinessNumber.GetAttribute("value") == "111111111");
        }
    }
}