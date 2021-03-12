using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a before midnight temporary change to hours of sale")]
        public void TemporaryChangeToHoursOfSale()
        {
            /* 
            Page Title: Temporary Change to Hours of Sale (Before Midnight)
            */

            // create test data
            string description = "Test automation event details";

            // enter the event details
            NgWebElement uiEventDetails = ngDriver.FindElement(By.CssSelector("textarea#description2"));
            uiEventDetails.SendKeys(description);

            // add a date from
            NgWebElement uiDateFrom = ngDriver.FindElement(By.CssSelector("input#tempDateFrom"));
            uiDateFrom.Click();

            // select the date
            SharedCalendarDate();

            // add a date to
            NgWebElement uiDateTo = ngDriver.FindElement(By.CssSelector("input#tempDateTo"));
            uiDateTo.Click();

            // select the date
            SharedCalendarDate();

            // select authorizedToSubmit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // select signatureAgreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();
        }
    }
}
