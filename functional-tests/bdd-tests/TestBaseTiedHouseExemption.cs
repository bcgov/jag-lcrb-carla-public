using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

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

            var licence = "450170 - The Smoking Gun";

            // search for and select the licence
            var uiLicence = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiLicence.SendKeys(licence);

            var uiLicence2 = ngDriver.FindElement(By.CssSelector("#mat-option-0 span"));
            uiLicence2.Click();

            // click on the consent checkbox
            var uiConsent =
                ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1[formcontrolname='consent']"));
            uiConsent.Click();

            // click on the authorized to submit checkbox
            var uiAuthorizedSubmit = ngDriver.FindElement(By.Id("mat-checkbox-2"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgree = ngDriver.FindElement(By.Id("mat-checkbox-3"));
            uiSignatureAgree.Click();
        }
    }
}