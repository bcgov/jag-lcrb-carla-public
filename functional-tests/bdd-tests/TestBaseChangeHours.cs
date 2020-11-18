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
using System.Diagnostics;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the change hours application for (.*)")]
        public void RequestChangeHours(string hoursType)
        {
            /* 
            Page Title: Licences & Authorizations
            */

            if (hoursType == "a lounge area within service hours")
            {
                NgWebElement uiLoungeAreaWithinHours = ngDriver.FindElement(By.CssSelector(".ng-star-inserted:nth-child(11) span"));
                uiLoungeAreaWithinHours.Click();
            }

            if (hoursType == "a lounge area outside of service hours")
            {

            }

            if (hoursType == "a special event area within service hours")
            {

            }

            if (hoursType == "a special event area outside of service hours")
            {

            }

            ContinueToApplicationButton();

            // select the proposed new hours
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

            // click on authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}