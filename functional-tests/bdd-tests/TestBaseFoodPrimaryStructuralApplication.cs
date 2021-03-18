using System;
using OpenQA.Selenium;
using Protractor;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I submit a Food Primary structural change application")]
        public void FPStructuralChange()
        {
            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Food Primary Structural Change Application
            */

            // create test data
            var description1 = "Test automation outline of the proposed change.";
            var description2 = "Test automation patio location.";
            var areaDescription = "Test automation area description.";
            var occupantLoad = "180";

            // enter the establishment name
            NgWebElement uiDescriptionOfChange = null;
            for (var i = 0; i < 30; i++)
                try
                {
                    var names = ngDriver.FindElements(By.CssSelector("textarea#description1"));
                    if (names.Count > 0)
                    {
                        uiDescriptionOfChange = names[0];
                        break;
                    }
                }
                catch (Exception)
                {
                }

            uiDescriptionOfChange.SendKeys(description1);

            // enter the location of the patio
            var uiPatioLocation = ngDriver.FindElement(By.CssSelector("textarea#description2"));
            uiPatioLocation.SendKeys(description2);

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

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            // pay for the structural change application
            MakePayment();
        }
    }
}