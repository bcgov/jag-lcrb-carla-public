using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a temporary change to hours of sale")]
        public void TemporaryChangeToHoursOfSale()
        {
            /* 
            Page Title: Temporary Change to Hours of Sale
            */

            // create test data
            var description = "Test automation event details";

            // enter the event details
            var uiEventDetails = ngDriver.FindElement(By.CssSelector("textarea#description2"));
            uiEventDetails.SendKeys(description);

            // add a date from
            var uiDateFrom = ngDriver.FindElement(By.CssSelector("input#tempDateFrom"));
            uiDateFrom.Click();

            // select the date
            SharedCalendarDate();

            // add a date to
            var uiDateTo = ngDriver.FindElement(By.CssSelector("input#tempDateTo"));
            uiDateTo.Click();

            // select the date
            SharedCalendarDate();

            // select authorizedToSubmit checkbox
            var uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // select signatureAgreement checkbox
            var uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();
        }


        [And(@"I request a before midnight temporary change to hours of sale")]
        public void TemporaryChangeToHoursOfSaleBeforeMidnight()
        {
            /* 
            Page Title: Temporary Change to Hours of Sale (Before Midnight)
            */

            TemporaryChangeToHoursOfSale();
        }


        [And(@"I request an after midnight temporary change to hours of sale")]
        public void TemporaryChangeToHoursOfSaleAfterMidnight()
        {
            /* 
            Page Title: Temporary Change to Hours of Sale (Before Midnight)
            */

            TemporaryChangeToHoursOfSale();
        }
    }
}