using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request structural alterations to an approved lounge or special events area")]
        public void StructuralAlterations()
        {
            // create test data
            var outdoorAreaDescription = "Sample outdoor area description";
            var outdoorAreaCapacity = "99999";
            var capacityAreaOccupants = "99999";

            // upload the floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[2]");

            // add outside area
            var uiOutdoorArea = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] button"));
            uiOutdoorArea.Click();

            // enter the outdooor area description
            var uiOutdoorAreaDescription =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='outsideAreas'] input[formcontrolname='areaLocation']"));
            uiOutdoorAreaDescription.SendKeys(outdoorAreaDescription);

            // enter the outdoor area occupant load
            var uiOutdoorAreaOccupantLoad =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='outsideAreas'] input[formcontrolname='capacity']"));
            uiOutdoorAreaOccupantLoad.SendKeys(outdoorAreaCapacity);

            // enter capacity area occupant load
            var uiCapacityAreaOccupantLoad =
                ngDriver.FindElement(
                    By.CssSelector("[formgroupname='capacityArea'] input[formcontrolname='capacity']"));
            uiCapacityAreaOccupantLoad.SendKeys(capacityAreaOccupants);

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
        }
    }
}