using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using Xunit;

/*


*/

namespace bdd_tests
{
    [FeatureFile("./***.feature")]
    public sealed class TemplateCS : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

           // check correct feature flags

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }

        [And(@"I complete the step function")]
        public void SampleMethod()
        {
            /* 
            Page Title: 
            */

            // create test data
            string sampleTestData = " ";

            // sample input call
            NgWebElement uiInputElement = ngDriver.FindElement(By.CssSelector("  "));
            uiInputElement.SendKeys(sampleTestData);

            // sample selection call
            NgWebElement uiSelectElement = ngDriver.FindElement(By.CssSelector("  "));
            uiSelectElement.Click();
        }
    }
}