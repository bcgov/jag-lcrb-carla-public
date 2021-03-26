using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

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

            // upload the associates form
            FileUpload("associates.pdf", "(//input[@type='file'])[3]");

            // upload the financial integrity form
            FileUpload("fin_integrity.pdf", "(//input[@type='file'])[6]");

            // upload the supporting document
            FileUpload("business_plan.pdf", "(//input[@type='file'])[8]");

            // select authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}