using System;
using System.Threading;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the special events permits applicant info")]
        public void SpecialEventsPermtsApplicantInfo()
        {
            /* 
            Page Title: Applicant Info
            */

            string eventName = "SEP test event";

            // enter the event name
            var uiEventName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='eventName']"));
            uiEventName.SendKeys(eventName);

            // click on the terms and conditions checkbox
            var uiTermsAndConditions = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreeToTnC']"));
            uiTermsAndConditions.Click();
        }

        [And(@"the SEP Checklist content is displayed")]
        public void SpecialEventsPermtsChecklist()
        {
            /* 
            Page Title: Application Checklist
            */

            Thread.Sleep(5000);

            Assert.True(ngDriver.FindElement(By.XPath("//mat-dialog-content[contains(.,'Application Checklist')]")).Displayed);
        }
    }
}