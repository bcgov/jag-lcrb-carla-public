using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the Application to Allow Family Food Service")]
        public void AllowFamilyFoodService()
        {
            /* 
            Page Title: Application to Allow Family Food Service
            */

            // upload the associates form
            FileUpload("associates.pdf", "(//input[@type='file'])[3]");

            // upload the financial integrity form
            FileUpload("fin_integrity.pdf", "(//input[@type='file'])[6]");

            // upload the supporting document
            FileUpload("business_plan.pdf", "(//input[@type='file'])[8]");

            // select authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}