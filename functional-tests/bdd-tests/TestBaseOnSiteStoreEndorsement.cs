using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request an on-site store endorsement")]
        public void OnSiteStoreEndorsement()
        {
            /* 
            Page Title: Licences & Authorizations
            */

            var onSiteStoreEndorsement = "On-Site Store Endorsement Application";

            // click on the On-Site Store Endorsement Application link
            var uiOnSiteStoreEndorsement = ngDriver.FindElement(By.LinkText(onSiteStoreEndorsement));
            uiOnSiteStoreEndorsement.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Manufacturer On-Site Store Endorsement Application
            */

            // select the zoning checkbox
            var uiZoningCheckbox =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isPermittedInZoning']"));
            uiZoningCheckbox.Click();

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

            // retrieve the current URL to get the application ID (needed downstream)
            var URL = ngDriver.Url;

            // retrieve the application ID
            var parsedURL = URL.Split('/');

            endorsementID = parsedURL[5];

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            MakePayment();
        }
    }
}