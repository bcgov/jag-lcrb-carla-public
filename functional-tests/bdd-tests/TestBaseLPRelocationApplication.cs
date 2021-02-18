using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete a liquor primary relocation request")]
        public void LPRelocationApplication()
        {
            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Application to Request Relocation
            */
            
            // create test data
            string areaDescription = "Sample area description";
            string patioPerimeter = "Sample patio perimeter";
            string patioLocation = "Sample patio location";
            string patioAccess = "Sample patio access";
            string liquorCarried = "Sample liquor carried details";
            string patioAccessControl = "Sample patio access control";
            string occupantLoad = "180";
            string establishmentType = "Military mess";

            // click zoning checkbox
            NgWebElement uiIsPermittedInZoning = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isPermittedInZoning']"));
            uiIsPermittedInZoning.Click();

            // select 'yes' for Treaty First Nation Land
            NgWebElement uiIsTreatyFirstNationLand = ngDriver.FindElement(By.CssSelector("[formcontrolname='isOnINLand'] mat-radio-button#mat-radio-5"));
            uiIsTreatyFirstNationLand.Click();

            // upload a letter of intent
            FileUpload("letter_of_intent.pdf", "(//input[@type='file'])[2]");

            // select 'yes' for patio
            NgWebElement uiHasPatioYes = ngDriver.FindElement(By.CssSelector("[formcontrolname='isHasPatio'] mat-radio-button#mat-radio-2"));
            uiHasPatioYes.Click();

            // enter patio perimeter details
            NgWebElement uiPatioComp = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioComp.SendKeys(patioPerimeter);

            // enter patio location details
            NgWebElement uiPatioLocation = ngDriver.FindElement(By.CssSelector("textarea#patioLocationDescription"));
            uiPatioLocation.SendKeys(patioLocation);

            // enter patio access details
            NgWebElement uiPatioAccess = ngDriver.FindElement(By.CssSelector("textarea#patioAccessDescription"));
            uiPatioAccess.SendKeys(patioAccess);

            // check liquor carried checkbox
            NgWebElement uiPatioLiquorCarried = ngDriver.FindElement(By.CssSelector("mat-checkbox#patioIsLiquorCarried"));
            uiPatioLiquorCarried.Click();

            // enter liquor carried details
            NgWebElement uiLiquorCarriedDetails = ngDriver.FindElement(By.CssSelector("textarea#patioLiquorCarriedDescription"));
            uiLiquorCarriedDetails.SendKeys(liquorCarried);

            // enter patio access control description
            NgWebElement uiPatioAccessControl = ngDriver.FindElement(By.CssSelector("textarea#patioAccessControlDescription"));
            uiPatioAccessControl.SendKeys(patioAccessControl);

            // click Fixed option
            NgWebElement uiFixedOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-1-button"));
            uiFixedOption.Click();

            // click Portable option
            NgWebElement uiPortableOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-2-button"));
            uiPortableOption.Click();

            // click Interior option
            NgWebElement uiInteriorOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-3-button"));
            uiInteriorOption.Click();

            // enter the establishment type
            NgWebElement uiEstablishmentType = ngDriver.FindElement(By.CssSelector("input[formcontrolname='description1']"));
            uiEstablishmentType.SendKeys(establishmentType);

            // upload the signage document
            FileUpload("signage.pdf", "(//input[@type='file'])[5]");

            // upload the floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[8]");

            // click on service area button
            NgWebElement uiServiceAreas = ngDriver.FindElement(By.CssSelector("[formcontrolname= 'serviceAreas'] button"));
            uiServiceAreas.Click();

            // enter area description
            NgWebElement uiAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
            uiAreaDescription.SendKeys(areaDescription);

            // enter occupant load
            NgWebElement uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiOccupantLoad.SendKeys(occupantLoad);

            // upload the site plan 
            FileUpload("site_plan.pdf", "(//input[@type='file'])[11]");

            // enter the hours of sales
            NgWebElement uiServiceHoursSundayOpen = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursSundayOpen'] option[value='09:00']"));
            uiServiceHoursSundayOpen.Click();

            NgWebElement uiServiceHoursSundayClose = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursSundayClose'] option[value='21:00']"));
            uiServiceHoursSundayClose.Click();

            NgWebElement uiServiceHoursMondayOpen = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursMondayOpen'] option[value='11:00']"));
            uiServiceHoursMondayOpen.Click();

            NgWebElement uiServiceHoursMondayClose = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursMondayClose'] option[value='23:00']"));
            uiServiceHoursMondayClose.Click();

            NgWebElement uiServiceHoursTuesdayOpen = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursTuesdayOpen'] option[value='09:15']"));
            uiServiceHoursTuesdayOpen.Click();

            NgWebElement uiServiceHoursTuesdayClose = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursTuesdayClose'] option[value='10:30']"));
            uiServiceHoursTuesdayClose.Click();

            NgWebElement uiServiceHoursWednesdayOpen = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursWednesdayOpen'] option[value='12:30']"));
            uiServiceHoursWednesdayOpen.Click();

            NgWebElement uiServiceHoursWednesdayClose = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursWednesdayClose'] option[value='21:30']"));
            uiServiceHoursWednesdayClose.Click();

            NgWebElement uiServiceHoursThursdayOpen = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursThursdayOpen'] option[value='14:30']"));
            uiServiceHoursThursdayOpen.Click();

            NgWebElement uiServiceHoursThursdayClose = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursThursdayClose'] option[value='19:00']"));
            uiServiceHoursThursdayClose.Click();

            NgWebElement uiServiceHoursFridayOpen = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursFridayOpen'] option[value='17:00']"));
            uiServiceHoursFridayOpen.Click();

            NgWebElement uiServiceHoursFridayClose = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursFridayClose'] option[value='19:45']"));
            uiServiceHoursFridayClose.Click();

            NgWebElement uiServiceHoursSaturdayOpen = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursSaturdayOpen'] option[value='09:45']"));
            uiServiceHoursSaturdayOpen.Click();

            NgWebElement uiServiceHoursSaturdayClose = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceHoursSaturdayClose'] option[value='20:00']"));
            uiServiceHoursSaturdayClose.Click();

            // select the owner checkbox
            NgWebElement uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            NgWebElement uiFutureValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiFutureValidInterest.Click();

            // select the authorized to submit checkbox
            NgWebElement uiAuthToSubmit = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSigAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSigAgreement.Click();
        }
    }
}
