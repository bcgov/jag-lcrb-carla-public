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
        [And(@"I complete the temporary suspension request")]
        public void TempSuspensionRequest()
        {
            /* 
            Page Title: Temporary Suspension Request
            */

            // create test data
            string estEmail = "test@test.com";
            string estPhone = "250-123-4567";
            string eventDetails = "Patio liquor carried description.";

            // enter establishment email
            NgWebElement uiEstablishmentEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentEmail']"));
            uiEstablishmentEmail.SendKeys(estEmail);

            // enter establishment email
            NgWebElement uiEstablishmentPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentPhone']"));
            uiEstablishmentPhone.SendKeys(estPhone);

            // enter event details
            NgWebElement uiEventDetails = ngDriver.FindElement(By.CssSelector("textarea#description1"));
            uiEventDetails.SendKeys(eventDetails);

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}
