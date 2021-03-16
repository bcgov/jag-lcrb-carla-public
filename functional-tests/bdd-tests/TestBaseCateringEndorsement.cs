using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a catering endorsement application")]
        public void CompleteCateringEndorsement()
        {
            /* 
            Page Title: Please Review Your Account Profile
            */

            // click on the Continue to Application button
            ContinueToApplicationButton();

            /* 
            Page Title: Catering Endorsement Application
            */

            // click on the authorized to submit checkbox
            var uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();
        }
    }
}