using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a new outdoor patio application")]
        public void NewOutdoorPatioApplication()
        {
            /* 
            Page Title: New Outdoor Patio
            */

            // create test data
            var patioCompDescription = "Sample patio composition description.";
            var patioLocationDescription = "Sample patio location description.";
            var patioAccessDescription = "Sample patio access description.";
            var patioLiquorCarriedDescription = "Sample liquor carried description.";
            var patioAccessControlDescription = "Sample patio access control description.";
            var areaDescription = "Sample area description";
            var occupantLoad = "99999";

            // enter patio composition description
            var uiPatioCompDescription = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioCompDescription.SendKeys(patioCompDescription);

            // enter patio location description
            var uiPatioLocationDescription = ngDriver.FindElement(By.CssSelector("textarea#patioLocationDescription"));
            uiPatioLocationDescription.SendKeys(patioLocationDescription);

            // enter patio access description
            var uiPatioAccessDescription = ngDriver.FindElement(By.CssSelector("textarea#patioAccessDescription"));
            uiPatioAccessDescription.SendKeys(patioAccessDescription);

            // click on Is Liquor Carried checkbox
            var uiPatioIsLiquorCarried = ngDriver.FindElement(By.CssSelector("mat-checkbox#patioIsLiquorCarried"));
            JavaScriptClick(uiPatioIsLiquorCarried);

            // enter Is Liquor Carried description
            var uiPatioLiquorCarriedDescription =
                ngDriver.FindElement(By.CssSelector("textarea#patioLiquorCarriedDescription"));
            uiPatioLiquorCarriedDescription.SendKeys(patioLiquorCarriedDescription);

            // enter patio access control description
            var uiPatioAccessControlDescription =
                ngDriver.FindElement(By.CssSelector("textarea#patioAccessControlDescription"));
            uiPatioAccessControlDescription.SendKeys(patioAccessControlDescription);

            // click Fixed option
            var uiFixedOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-25-button"));
            uiFixedOption.Click();

            // click Portable option
            var uiPortableOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-26-button"));
            uiPortableOption.Click();

            // click Interior option
            var uiInteriorOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-27-button"));
            uiInteriorOption.Click();

            // upload floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[2]");

            // click on service area button
            var uiServiceAreas = ngDriver.FindElement(By.CssSelector("[formcontrolname= 'serviceAreas'] button"));
            uiServiceAreas.Click();

            // enter area description
            var uiAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
            uiAreaDescription.SendKeys(areaDescription);

            // enter occupant load
            var uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiOccupantLoad.SendKeys(occupantLoad);

            // upload exterior photos
            FileUpload("exterior_photos.pdf", "(//input[@type='file'])[5]");

            // click on the isOwnerBusiness checkbox
            var uiIsOwnerBusiness =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiIsOwnerBusiness.Click();

            // click on the hasValidInterest checkbox
            var uiHasValidInterest =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='hasValidInterest']"));
            uiHasValidInterest.Click();

            // click on the willHaveValidInterest checkbox
            var uiWillHaveValidInterest =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiWillHaveValidInterest.Click();

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