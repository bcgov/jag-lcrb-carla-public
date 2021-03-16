using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the All Ages Liquor Free Event request")]
        public void AllAgesLiquorFreeEvent()
        {
            /* 
            Page Title: All Ages Liquor Free Event
            */

            // create test data
            var eventName = "Test Event";
            var eventNote = "Sample test event note.";
            var contactName = "Contact Name";
            var contactPhoneNumber = "(222) 222-2222";
            var contactEmail = "test@automation.com";

            // enter the event name
            var uiEventName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='eventName']"));
            uiEventName.SendKeys(eventName);

            // enter the event note
            var uiEventNote = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='eventTypeDescription']"));
            uiEventNote.SendKeys(eventNote);

            // enter the contact name
            var uiContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            uiContactName.SendKeys(contactName);

            // enter the contact phone number
            var uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            uiContactPhone.SendKeys(contactPhoneNumber);

            // select end date
            var uiEndDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='endDate']"));
            uiEndDate1.Click();

            SharedCalendarDate();

            // select start date
            var uiStartDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='startDate']"));
            JavaScriptClick(uiStartDate1);

            SharedCalendarDate();

            // enter the contact email
            var uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactEmail']"));
            uiContactEmail.SendKeys(contactEmail);

            // confirm the contact email
            var uiContactEmailConfirmation =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactEmailConfirmation']"));
            uiContactEmailConfirmation.SendKeys(contactEmail);

            // decrement event start hour
            var uiEventStartHour =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='startTime'] .ngb-tp-hour [type='button'] span.bottom"));
            JavaScriptClick(uiEventStartHour);

            // decrement event start minute
            var uiEventStartMinute =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='startTime'] .ngb-tp-minute span.ngb-tp-chevron.bottom"));
            JavaScriptClick(uiEventStartMinute);

            // decrement event end hour
            var uiEventEndHour =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='endTime'] .ngb-tp-hour [type='button'] span.bottom"));
            JavaScriptClick(uiEventEndHour);

            // decrement event end minute
            var uiEventEndMinute =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='endTime'] .ngb-tp-minute [type='button'] span.bottom"));
            JavaScriptClick(uiEventEndMinute);

            // select agreement checkbox
            var uiAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox.mat-checkbox[formcontrolname='isAgreement1']"));
            JavaScriptClick(uiAgreement);

            // select review checkbox
            var uiReview =
                ngDriver.FindElement(By.CssSelector("mat-checkbox.mat-checkbox[formcontrolname='isAgreement2']"));
            JavaScriptClick(uiReview);
        }
    }
}