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

            for (var i = 0; i < 10; i++)
                try
                {
                    Thread.Sleep(3000);

                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Application Checklist')]")).Displayed);
                    break;
                }
                catch (Exception e)
                { }
        }


        [And(@"Account Profile is displayed")]
        public void AccountProfileDisplayed()
        {
            /* 
            Page Title: Account Profile
            */

            for (var i = 0; i < 10; i++)
                try
                {
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Account Profile ')]")).Displayed);
                    break;
                }
                catch (Exception e)
                { }
        }


        [And(@"the Current Applications label is displayed")]
        public void CurrentApplicationsDisplayed()
        {
            /* 
            Page Title: Current Applications
            */

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Current Applications')]")).Displayed);
        }


        [And(@"the Plan Your Drinks label is displayed")]
        public void DrinkPlannerDisplayed()
        {
            /* 
            Page Title: Current Applications
            */

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Drink Planner')]")).Displayed);
        }
    }
}