using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I submit a liquor primary temporary extension of licensed area application")]
        public void LPTempExtensionAreaApplication()
        {
            /* 
            Page Title: Temporary Extension of Licensed Area
            */

            // create test data
            var description = "Test automation event details";
            var capacity = "180";

            // enter the event details
            var uiEventDetails = ngDriver.FindElement(By.CssSelector("textarea#description2"));
            uiEventDetails.SendKeys(description);

            // upload a floor plan document
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[2]");

            // enter the capacity
            var uiCapacity = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'capacity']"));
            uiCapacity.SendKeys(capacity);

            // select authorizedToSubmit checkbox
            var uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // select signatureAgreement checkbox
            var uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();

            // click on the Submit button
            ClickOnSubmitButton();
        }
    }
}