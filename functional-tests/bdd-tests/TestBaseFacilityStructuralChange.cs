using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a facility structural change")]
        public void FacilityStructuralChange()
        {
            /* 
            Page Title: Manufacturing Facility Structural Change Application
            */

            // create test data
            var applicationDetails = "Sample application details.";
            var proposedChange = "Sample proposed change";

            // enter the application details into the text area
            var uiApplicationDetails = ngDriver.FindElement(By.CssSelector("textarea#otherBusinessesDetails"));
            uiApplicationDetails.SendKeys(applicationDetails);

            // enter the proposed change into the text area
            var uiProposedChange = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='description1']"));
            uiProposedChange.SendKeys(proposedChange);

            // upload the floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[2]");

            // upload the site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[5]");

            // select the authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            MakePayment();
        }
    }
}