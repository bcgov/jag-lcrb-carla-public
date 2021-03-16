using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a capacity increase structural change")]
        public void RequestStructuralChangeCapacityIncrease()
        {
            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Structural Change Application (Capacity Increase)
            */

            // create test data
            var description = "Test automation outline of the proposed change.";
            var patioLocation = "Location of the patio";
            var areaDescription = "Description of area";
            var occupantLoad = "180";

            // enter the description of the change
            var uiDescriptionOfChange = ngDriver.FindElement(By.CssSelector("textarea#description1"));
            uiDescriptionOfChange.SendKeys(description);

            // enter the patio location 
            var uiPatioLocation = ngDriver.FindElement(By.CssSelector("textarea#description2"));
            uiPatioLocation.SendKeys(patioLocation);

            // upload a floor plan document
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