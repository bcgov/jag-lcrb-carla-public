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
        [And(@"I request a special event area endorsement")]
        public void SpecialEventAreaEndorsement()
        {
            /* 
            Page Title: Licences & Authorizations
            */

            string specialEventAreaEndorsement = "Special Event Area Endorsement Application";

            // click on the Special Event Area Endorsement Application link
            NgWebElement uiSpecialEventAreaEndorsement = ngDriver.FindElement(By.LinkText(specialEventAreaEndorsement));
            uiSpecialEventAreaEndorsement.Click();

            ContinueToApplicationButton();

            /* 
            Page Title: Special Event Area Endorsement Application
            */

            // creeate test data
            string serviceAreaDescription = "Service area description";
            string serviceAreaOccupantLoad = "100";
            string outdoorAreaDescription = "Outdoor area description";
            string outdoorAreaCapacity = "150";
            string contactTitle = "Tester";

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1"));
            uiZoningCheckbox.Click();

            // upload the letter of intent
            FileUpload("letter_of_intent.pdf","(//input[@type='file'])[2]");

            // upload the floor plan
            FileUpload("floor_plan.pdf","(//input[@type='file'])[5]");

            // add a service area
            NgWebElement uiServiceArea = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceAreas'] button"));
            uiServiceArea.Click();

            // enter the service area description
            NgWebElement uiServiceAreaDescription = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceAreas'] input[formcontrolname='areaLocation']"));
            uiServiceAreaDescription.SendKeys(serviceAreaDescription);

            // enter the service area occupant load
            NgWebElement uiServiceAreaOccupantLoad = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceAreas'] input[formcontrolname='capacity']"));
            uiServiceAreaOccupantLoad.SendKeys(serviceAreaOccupantLoad);

            // add outside area
            NgWebElement uiOutdoorArea = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] button"));
            uiOutdoorArea.Click();

            // enter the outdooor area description
            NgWebElement uiOutdoorAreaDescription = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] input[formcontrolname='areaLocation']"));
            uiOutdoorAreaDescription.SendKeys(outdoorAreaDescription);

            // enter the outdoor area occupant load
            NgWebElement uiOutdoorAreaOccupantLoad = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] input[formcontrolname='capacity']"));
            uiOutdoorAreaOccupantLoad.SendKeys(outdoorAreaCapacity);

            // upload the site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[8]");

            // select the Sunday opening time
            NgWebElement uiSundayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSundayOpen'] option[value='10:00']"));
            uiSundayOpen.Click();

            // select the Sunday closing time
            NgWebElement uiSundayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSundayClose'] option[value='16:00']"));
            uiSundayClose.Click();

            // select the Monday opening time
            NgWebElement uiMondayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursMondayOpen'] option[value='09:00']"));
            uiMondayOpen.Click();

            // select the Monday closing time
            NgWebElement uiMondayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursMondayClose'] option[value='23:00']"));
            uiMondayClose.Click();

            // select the Tuesday opening time
            NgWebElement uiTuesdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursTuesdayOpen'] option[value='09:15']"));
            uiTuesdayOpen.Click();

            // select the Tuesday closing time
            NgWebElement uiTuesdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursTuesdayClose'] option[value='22:45']"));
            uiTuesdayClose.Click();

            // select the Wednesday opening time
            NgWebElement uiWednesdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursWednesdayOpen'] option[value='09:30']"));
            uiWednesdayOpen.Click();

            // select the Wednesday closing time
            NgWebElement uiWednesdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursWednesdayClose'] option[value='12:00']"));
            uiWednesdayClose.Click();

            // select the Thursday opening time
            NgWebElement uiThursdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursThursdayOpen'] option[value='13:00']"));
            uiThursdayOpen.Click();

            // select the Thursday closing time
            NgWebElement uiThursdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursThursdayClose'] option[value='14:00']"));
            uiThursdayClose.Click();

            // select the Friday opening time
            NgWebElement uiFridayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursFridayOpen'] option[value='12:15']"));
            uiFridayOpen.Click();

            // select the Friday closing time
            NgWebElement uiFridayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursFridayClose'] option[value='21:15']"));
            uiFridayClose.Click();

            // select the Saturday opening time
            NgWebElement uiSaturdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSaturdayOpen'] option[value='10:00']"));
            uiSaturdayOpen.Click();

            // select the Saturday closing time
            NgWebElement uiSaturdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSaturdayClose'] option[value='22:00']"));
            uiSaturdayClose.Click();

            // enter the contact title
            NgWebElement uiContactTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonRole']"));
            uiContactTitle.SendKeys(contactTitle);

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();
        }
    }
}
