using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a sales to hospitality licensees and special event permittees application")]
        public void SalesSEPApplication()
        {
            /* 
            Page Title: Sales to Hospitality Licensees and Special Event Permittees
            */

            // upload a letter of intent
            FileUpload("letter_of_intent.pdf", "(//input[@type='file'])[2]");

            // click on the authorized to submit checkbox
            var uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();
        }
    }
}