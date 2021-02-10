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
        [And(@"I complete the Application to Allow Family Food Service")]
        public void AllowFamilyFoodService()
        {
            /* 
            Page Title: Application to Allow Family Food Service
            */

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

            // upload the associates form
            FileUpload("associates.pdf", "(//input[@type='file'])[3]");

            // upload the financial integrity form
            FileUpload("fin_integrity.pdf", "(//input[@type='file'])[6]");

            // upload the supporting document
            FileUpload("business_plan.pdf", "(//input[@type='file'])[8]");

            // select authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}
