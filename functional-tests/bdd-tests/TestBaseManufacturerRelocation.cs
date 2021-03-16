using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a relocation change")]
        public void RelocationChange()
        {
            /* 
            Page Title: Licences & Authorizations
            */

            var locationChange = "Request Relocation";

            // click on the Request Relocation link
            var uiLocationChange = ngDriver.FindElement(By.LinkText(locationChange));
            uiLocationChange.Click();

            ContinueToApplicationButton();

            /* 
            Page Title: Manufacturer Relocation Application
            */

            // create test data
            var additionalPIDs = "012345678, 343434344";
            var proposedChanges = "Details of proposed changes.";

            // enter additional PIDs
            var uiAdditionalPIDs = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='pidList']"));
            uiAdditionalPIDs.SendKeys(additionalPIDs);

            // select the zoning checkbox
            var uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-2"));
            uiZoningCheckbox.Click();

            // enter the proposed changes
            var uiProposedChanges = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='description1']"));
            uiProposedChanges.SendKeys(proposedChanges);

            // upload the signage document
            FileUpload("signage.pdf", "(//input[@type='file'])[2]");

            // upload the floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[5]");

            // upload the site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[8]");

            // upload the exterior photos
            FileUpload("exterior_photos.jpg", "(//input[@type='file'])[11]");

            // upload the ownership details
            FileUpload("ownership_details.pdf", "(//input[@type='file'])[15]");

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