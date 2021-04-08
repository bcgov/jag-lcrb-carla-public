using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a picnic area endorsement")]
        public void PicnicAreaEndorsement()
        {
            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Manufacturer Picnic Area Endorsement Application
            */

            // create test data
            var proposedChange =
                "Description of proposed change(s) such as moving, adding or changing approved picnic area(s)";
            var otherBizDetails = "Description of other business details";
            var patioCompositionDescription = "Description of patio composition";
            var capacity = "99999";

            // enter the description of the proposed change
            var uiProposedChange = ngDriver.FindElement(By.CssSelector("textarea#description1"));
            uiProposedChange.SendKeys(proposedChange);

            // enter the other business details
            var uiOtherBizDetails = ngDriver.FindElement(By.CssSelector("textarea#otherBusinessesDetails"));
            uiOtherBizDetails.SendKeys(otherBizDetails);

            // enter the patio composition description
            var uiPatioCompDesc = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioCompDesc.SendKeys(patioCompositionDescription);

            // select 'Grass' for patio location
            var uiGrass = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-125-button"));
            uiGrass.Click();

            // select 'Earth' for patio location
            var uiEarth = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-126-button"));
            uiEarth.Click();

            // select 'Gravel' for patio location
            var uiGravel = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-127-button"));
            uiGravel.Click();

            // select 'Finished Flooring' for patio location
            var uiFinishedFlooring = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-128-button"));
            uiFinishedFlooring.Click();

            // select 'Cement Sidewalk' for patio location
            var uiCementSidewalk = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-129-button"));
            uiCementSidewalk.Click();

            // select 'Other' for patio location
            var uiOther = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-130-button"));
            uiOther.Click();

            // enter the capacity
            var uiCapacity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiCapacity.Clear();
            uiCapacity.SendKeys(capacity);

            // upload the site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[2]");

            // upload the exterior photos
            FileUpload("exterior_photos.jpg", "(//input[@type='file'])[5]");

            // click on the authorized to submit checkbox
            var uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgree.Click();

            ClickOnSubmitButton();

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Pending External Review ')]")).Displayed);
        }
    }
}