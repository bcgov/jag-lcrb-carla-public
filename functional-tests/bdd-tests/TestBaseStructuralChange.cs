using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a structural change")]
        public void RequestStructuralChange()
        {
            /* 
            Page Title: Licences & Authorizations
            Subtitle:   Cannabis Retail Store Licences
            */

            var structuralChange = "Request a Structural Change";

            // click on the request structural change link
            var uiStructuralChange = ngDriver.FindElement(By.LinkText(structuralChange));
            uiStructuralChange.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Submit the Cannabis Retail Store Structural Change Application
            */

            // create test data
            var description = "Test automation outline of the proposed change.";

            // enter the description of the change
            var uiDescriptionOfChange = ngDriver.FindElement(By.Id("description1"));
            uiDescriptionOfChange.SendKeys(description);

            // select not visible from outside checkbox
            var uiVisibleFromOutside = ngDriver.FindElement(By.CssSelector(".mat-checkbox-inner-container"));
            JavaScriptClick(uiVisibleFromOutside);

            // upload a floor plan document
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[2]");

            // upload a site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[5]");

            // select authorizedToSubmit checkbox
            var uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // select signatureAgreement checkbox
            var uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            // pay for the structural change application
            MakePayment();
        }


        [And(@"I confirm the structural change request is displayed on the dashboard")]
        public void RequestedStructuralChangeOnDashboard()
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // confirm that a structural change request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Structural Change')]")).Displayed);
        }
    }
}