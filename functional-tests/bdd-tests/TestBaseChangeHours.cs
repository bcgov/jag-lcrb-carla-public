using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

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
                var uiLoungeAreaWithinHours =
                    ngDriver.FindElement(
                        By.LinkText("Change to Hours of Liquor Service (Lounge Area, within Service Hours)"));
                var executor = (IJavaScriptExecutor) ngDriver.WrappedDriver;
                executor.ExecuteScript("arguments[0].scrollIntoView(true);", uiLoungeAreaWithinHours);
                uiLoungeAreaWithinHours.Click();
            }

            if (hoursType == "a lounge area outside of service hours")
            {
                var uiLoungeAreaOutsideHours =
                    ngDriver.FindElement(
                        By.LinkText("Change to Hours of Liquor Service (Lounge Area, outside Service Hours)"));
                var executor = (IJavaScriptExecutor) ngDriver.WrappedDriver;
                executor.ExecuteScript("arguments[0].scrollIntoView(true);", uiLoungeAreaOutsideHours);
                uiLoungeAreaOutsideHours.Click();
            }

            if (hoursType == "a special event area within service hours")
            {
                var uiSpecialEventAreaWithinHours =
                    ngDriver.FindElement(By.CssSelector(".ng-star-inserted:nth-child(15) span"));
                uiSpecialEventAreaWithinHours.Click();
            }

            if (hoursType == "a special event area outside of service hours")
            {
                var uiSpecialEventAreaOutsideHours =
                    ngDriver.FindElement(By.CssSelector(".ng-star-inserted:nth-child(14) span"));
                uiSpecialEventAreaOutsideHours.Click();
            }

            ContinueToApplicationButton();

            // select the proposed new hours
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

            // select the authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }


        [And(@"I complete the change hours application for liquor service within service hours")]
        public void RequestLiquorChangeHours(string hoursType)
        {
            // select the proposed new hours
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

            // select the authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }


        [And(@"I complete the change hours application for liquor service outside service hours")]
        public void RequestLiquorChangeHoursOutside(string hoursType)
        {
            // select the proposed new hours
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

            // select the authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}