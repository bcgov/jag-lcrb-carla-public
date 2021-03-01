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
        [And(@"I complete the tied house exemption request")]
        public void TiedHouseExemptionRequest()
        {
            /* 
            Page Title: Tied House Exemption Application Invitation
            */

            string licence = "450170 - The Smoking Gun";

            // search for and select the licence
            NgWebElement uiLicence = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiLicence.SendKeys(licence);

            NgWebElement uiLicence2 = ngDriver.FindElement(By.CssSelector("#mat-option-0 span"));
            uiLicence2.Click();

            // click on the consent checkbox
            NgWebElement uiConsent = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1[formcontrolname='consent']"));
            uiConsent.Click();

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedSubmit = ngDriver.FindElement(By.Id("mat-checkbox-2"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.Id("mat-checkbox-3"));
            uiSignatureAgree.Click();
        }
    }
}
