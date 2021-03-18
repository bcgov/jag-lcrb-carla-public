using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a change in terms and conditions application")]
        public void ChangeInTermsApplication()
        {
            /* 
            Page Title: Request of Change in Terms and Conditions/Request for Discretion
            */

            // upload letter of intent
            FileUpload("letter_of_intent.pdf", "(//input[@type='file'])[3]");

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