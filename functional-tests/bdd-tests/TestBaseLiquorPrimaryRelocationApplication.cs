using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete a liquor primary relocation request")]
        public void LPRelocationApplication()
        {
            /* 
            Page Title: Liquor Primary and Liquor Primary Club Relocation Application
            */

            // create test data
            var patioPerimeter = "Sample patio perimeter";
            var patioLocation = "Sample patio location";
            var patioAccess = "Sample patio access";
            var liquorCarried = "Sample liquor carried details";
            var patioAccessControl = "Sample patio access control";
            var establishmentType = "Military mess";

            // click zoning checkbox
            var uiIsPermittedInZoning =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isPermittedInZoning']"));
            uiIsPermittedInZoning.Click();

            // select 'yes' for Treaty First Nation Land
            var uiIsTreatyFirstNationLand =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='isOnINLand'] mat-radio-button#mat-radio-5"));
            uiIsTreatyFirstNationLand.Click();

            // upload a letter of intent
            FileUpload("letter_of_intent.pdf", "(//input[@type='file'])[2]");

            // select 'yes' for patio
            var uiHasPatioYes =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='isHasPatio'] mat-radio-button#mat-radio-2"));
            uiHasPatioYes.Click();

            // enter patio perimeter details
            var uiPatioComp = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioComp.SendKeys(patioPerimeter);

            // enter patio location details
            var uiPatioLocation = ngDriver.FindElement(By.CssSelector("textarea#patioLocationDescription"));
            uiPatioLocation.SendKeys(patioLocation);

            // enter patio access details
            var uiPatioAccess = ngDriver.FindElement(By.CssSelector("textarea#patioAccessDescription"));
            uiPatioAccess.SendKeys(patioAccess);

            // check liquor carried checkbox
            var uiPatioLiquorCarried = ngDriver.FindElement(By.CssSelector("mat-checkbox#patioIsLiquorCarried"));
            uiPatioLiquorCarried.Click();

            // enter liquor carried details
            var uiLiquorCarriedDetails = ngDriver.FindElement(By.CssSelector("textarea#patioLiquorCarriedDescription"));
            uiLiquorCarriedDetails.SendKeys(liquorCarried);

            // enter patio access control description
            var uiPatioAccessControl = ngDriver.FindElement(By.CssSelector("textarea#patioAccessControlDescription"));
            uiPatioAccessControl.SendKeys(patioAccessControl);

            // click Fixed option
            var uiFixedOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-1-button"));
            uiFixedOption.Click();

            // click Portable option
            var uiPortableOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-2-button"));
            uiPortableOption.Click();

            // click Interior option
            var uiInteriorOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-3-button"));
            uiInteriorOption.Click();

            // enter the establishment type
            var uiEstablishmentType = ngDriver.FindElement(By.CssSelector("input[formcontrolname='description1']"));
            uiEstablishmentType.SendKeys(establishmentType);

            // upload the signage document
            FileUpload("signage.pdf", "(//input[@type='file'])[5]");

            // upload the floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[8]");

            // upload the site plan 
            FileUpload("site_plan.pdf", "(//input[@type='file'])[11]");

            /*
            // click on service area button
            NgWebElement uiServiceAreas = ngDriver.FindElement(By.CssSelector("[formcontrolname= 'serviceAreas'] button"));
            uiServiceAreas.Click();

            // enter area description
            NgWebElement uiAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
            uiAreaDescription.SendKeys(areaDescription);

            // enter occupant load
            NgWebElement uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiOccupantLoad.SendKeys(occupantLoad);
            */

            // enter the hours of sales
            var uiServiceHoursSundayOpen =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursSundayOpen'] option[value='09:00']"));
            uiServiceHoursSundayOpen.Click();

            var uiServiceHoursSundayClose =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursSundayClose'] option[value='21:00']"));
            uiServiceHoursSundayClose.Click();

            var uiServiceHoursMondayOpen =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursMondayOpen'] option[value='11:00']"));
            uiServiceHoursMondayOpen.Click();

            var uiServiceHoursMondayClose =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursMondayClose'] option[value='23:00']"));
            uiServiceHoursMondayClose.Click();

            var uiServiceHoursTuesdayOpen =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursTuesdayOpen'] option[value='09:15']"));
            uiServiceHoursTuesdayOpen.Click();

            var uiServiceHoursTuesdayClose =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursTuesdayClose'] option[value='10:30']"));
            uiServiceHoursTuesdayClose.Click();

            var uiServiceHoursWednesdayOpen =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursWednesdayOpen'] option[value='12:30']"));
            uiServiceHoursWednesdayOpen.Click();

            var uiServiceHoursWednesdayClose =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursWednesdayClose'] option[value='21:30']"));
            uiServiceHoursWednesdayClose.Click();

            var uiServiceHoursThursdayOpen =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursThursdayOpen'] option[value='14:30']"));
            uiServiceHoursThursdayOpen.Click();

            var uiServiceHoursThursdayClose =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursThursdayClose'] option[value='19:00']"));
            uiServiceHoursThursdayClose.Click();

            var uiServiceHoursFridayOpen =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursFridayOpen'] option[value='17:00']"));
            uiServiceHoursFridayOpen.Click();

            var uiServiceHoursFridayClose =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursFridayClose'] option[value='19:45']"));
            uiServiceHoursFridayClose.Click();

            var uiServiceHoursSaturdayOpen =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursSaturdayOpen'] option[value='09:45']"));
            uiServiceHoursSaturdayOpen.Click();

            var uiServiceHoursSaturdayClose =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='serviceHoursSaturdayClose'] option[value='20:00']"));
            uiServiceHoursSaturdayClose.Click();

            // select the owner checkbox
            var uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            var uiValidInterest =
                ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            var uiFutureValidInterest =
                ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiFutureValidInterest.Click();

            // select the authorized to submit checkbox
            var uiAuthToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthToSubmit.Click();

            // select the signature agreement checkbox
            var uiSigAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSigAgreement.Click();
        }
    }
}