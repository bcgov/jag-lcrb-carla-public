using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a valid store name or branding change for (.*)")]
        public void RequestNameBrandingChange(string changeType)
        {
            /*
            Page Title: Submit a Name or Branding Change Application
            */

            var newEstablishmentName = "Point Ellis Conservatory";

            // enter a new establishment name
            var uiEstablishmentName =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentName']"));
            uiEstablishmentName.Clear();
            uiEstablishmentName.SendKeys(newEstablishmentName);

            // click on the authorized to submit checkbox
            var uiAuthSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthSubmit.Click();

            // click on the signature agreement checkbox
            var uiSigAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSigAgreement.Click();

            // upload a supporting document
            FileUpload("signage.pdf", "(//input[@type='file'])[2]");


            if (changeType == "Cannabis")
            {
                // click on the store exterior change button	
                var uiStoreExterior = ngDriver.FindElement(By.Id("mat-button-toggle-2-button"));
                uiStoreExterior.Click();
            }

            // retrieve the application ID
            // string[] parsedURL = URL.Split('/');

            // string[] tempFix = parsedURL[5].Split(';');

            // applicationID = tempFix[0];

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            // pay for the branding change application
            MakePayment();
        }
    }
}