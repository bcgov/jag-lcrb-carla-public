using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I review the federal reports")]
        public void ReviewFederalReports()
        {
            /* 
            Page Title: Federal Reporting
            */

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Federal Reporting')]")).Displayed);
        }
    }
}