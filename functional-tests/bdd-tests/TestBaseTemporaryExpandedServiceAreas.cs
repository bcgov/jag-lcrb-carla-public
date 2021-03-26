using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the TESA application for a (.*)")]
        public void CompleteTESAApplication(string applicationType)
        {
            /* 
            Page Title: Temporary Expanded Service Areas Application
            */

            // upload letter of intent
            FileUpload("letter_of_intent.pdf", "(//input[@type='file'])[2]");

            // upload floorplan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[5]");

            // select the authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}