using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a special event area endorsement")]
        public void SpecialEventAreaEndorsement()
        {
            /* 
            Page Title: Special Event Area Endorsement Application
            */

            // creeate test data
            var serviceAreaDescription = "Service area description";
            var serviceAreaOccupantLoad = "100";
            var outdoorAreaDescription = "Outdoor area description";
            var outdoorAreaCapacity = "150";
            var contactTitle = "Tester";

            // select the zoning checkbox
            var uiZoningCheckbox =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isPermittedInZoning']"));
            uiZoningCheckbox.Click();

            // upload the letter of intent
            FileUpload("letter_of_intent.pdf", "(//input[@type='file'])[2]");

            // upload the floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[5]");

            // add a service area
            var uiServiceArea = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceAreas'] button"));
            ScrollToElement(uiServiceArea);
            uiServiceArea.Click();

            // enter the service area description
            var uiServiceAreaDescription =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceAreas'] input[formcontrolname='areaLocation']"));
            uiServiceAreaDescription.SendKeys(serviceAreaDescription);

            // enter the service area occupant load
            var uiServiceAreaOccupantLoad =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceAreas'] input[formcontrolname='capacity']"));
            uiServiceAreaOccupantLoad.SendKeys(serviceAreaOccupantLoad);

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

            // upload the site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[8]");

            // select the Sunday opening time
            var uiSundayOpen =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursSundayOpen'] option[value='10:00']"));
            uiSundayOpen.Click();

            // select the Sunday closing time
            var uiSundayClose =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursSundayClose'] option[value='16:00']"));
            uiSundayClose.Click();

            // select the Monday opening time
            var uiMondayOpen =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursMondayOpen'] option[value='09:00']"));
            uiMondayOpen.Click();

            // select the Monday closing time
            var uiMondayClose =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursMondayClose'] option[value='23:00']"));
            uiMondayClose.Click();

            // select the Tuesday opening time
            var uiTuesdayOpen =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursTuesdayOpen'] option[value='09:15']"));
            uiTuesdayOpen.Click();

            // select the Tuesday closing time
            var uiTuesdayClose =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursTuesdayClose'] option[value='22:45']"));
            uiTuesdayClose.Click();

            // select the Wednesday opening time
            var uiWednesdayOpen =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursWednesdayOpen'] option[value='09:30']"));
            uiWednesdayOpen.Click();

            // select the Wednesday closing time
            var uiWednesdayClose =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursWednesdayClose'] option[value='12:00']"));
            uiWednesdayClose.Click();

            // select the Thursday opening time
            var uiThursdayOpen =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursThursdayOpen'] option[value='13:00']"));
            uiThursdayOpen.Click();

            // select the Thursday closing time
            var uiThursdayClose =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursThursdayClose'] option[value='14:00']"));
            uiThursdayClose.Click();

            // select the Friday opening time
            var uiFridayOpen =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursFridayOpen'] option[value='12:15']"));
            uiFridayOpen.Click();

            // select the Friday closing time
            var uiFridayClose =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursFridayClose'] option[value='21:15']"));
            uiFridayClose.Click();

            // select the Saturday opening time
            var uiSaturdayOpen =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursSaturdayOpen'] option[value='10:00']"));
            uiSaturdayOpen.Click();

            // select the Saturday closing time
            var uiSaturdayClose =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='serviceHoursSaturdayClose'] option[value='22:00']"));
            uiSaturdayClose.Click();

            // enter the contact title
            var uiContactTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonRole']"));
            uiContactTitle.SendKeys(contactTitle);

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