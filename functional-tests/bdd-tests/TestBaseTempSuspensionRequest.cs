using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

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
            //string estEmail = "test@test.com";
            var estPhone = "250-123-4567";
            var eventDetails = "Patio liquor carried description.";

            // enter establishment email
            var uiEstablishmentPhone =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentPhone']"));
            uiEstablishmentPhone.SendKeys(estPhone);

            // enter event details
            var uiEventDetails = ngDriver.FindElement(By.CssSelector("textarea#description1"));
            uiEventDetails.SendKeys(eventDetails);

            // click on the authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}