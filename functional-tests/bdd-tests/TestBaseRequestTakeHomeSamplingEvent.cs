using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the Take Home Sampling Event Authorization request")]
        public void RequestTakeHomeSamplingEvent()
        {
            /* 
            Page Title: Take-Home Sampling Event
            */

            // create test data
            var contactName = "Test Automation";
            var contactPhoneNumber = "(222) 222-2222";
            var eventName = "Test Event";
            var eventDescription = "Sample event description";
            var clientOrHostName = "Client or host name";
            var venueNameDescription = "Venue Name/Description";
            var additionalInfo = "Additional info";
            var contactEmail = "contact@test.com";

            // enter contact name
            var uiContactName =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            uiContactName.SendKeys(contactName);

            // enter contact phone number
            var uiContactNumber =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            uiContactNumber.SendKeys(contactPhoneNumber);

            // select event type
            var uiEventType =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='eventType'] option[value='0: 845280000']"));
            uiEventType.Click();

            // enter event name
            var uiEventName =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='eventName']"));
            uiEventName.SendKeys(eventName);

            // enter event description
            var uiEventDescription =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='eventTypeDescription']"));
            uiEventDescription.SendKeys(eventDescription);

            // enter client or host name
            var uiClientOrHostName =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='clientHostname']"));
            uiClientOrHostName.SendKeys(clientOrHostName);

            // enter venue name/description
            var uiVenueNameDescription =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='venueDescription']"));
            uiVenueNameDescription.SendKeys(venueNameDescription);

            // select the location
            var uiLocation =
                    ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='specificLocation'] [value='1: 845280001']"));
            uiLocation.Click();

            // enter additional info
            var uiAdditionalInfo =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            uiAdditionalInfo.SendKeys(additionalInfo);

            // enter address 1
            var uiStreet1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'street1']"));
            uiStreet1.SendKeys(address1);

            // enter address 2
            var uiStreet2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'street2']"));
            uiStreet2.SendKeys(address2);

            // enter city
            var uiCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'city']"));
            uiCity.SendKeys(city);

            // enter postal code
            var uiPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'postalCode']"));
            uiPostalCode.SendKeys(postalCode);

            // select end date
            var uiEndDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='endDate']"));
            uiEndDate1.Click();

            // click on the next button
            var uiOpenCalendarNext = ngDriver.FindElement(By.CssSelector(".mat-calendar .mat-calendar-next-button"));
            JavaScriptClick(uiOpenCalendarNext);

            // click on the first day
            var uiOpenCalendarYear =
                ngDriver.FindElement(
                    By.CssSelector(".mat-calendar-content .mat-calendar-body-cell-content:first-child"));
            JavaScriptClick(uiOpenCalendarYear);

            // select start date
            var uiStartDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='startDate']"));
            uiStartDate1.Click();

            try
            {
                var uiStartDate2 =
                    ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
                uiStartDate2.Click();
            }
            catch
            {
                // retry if failed once
                var uiStartDate2 =
                    ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
                uiStartDate2.Click();
            }

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

            // increment liquor sale start hour
            var uiLiquorStartHour =
                ngDriver.FindElement(
                    By.CssSelector(
                        ".col-md-2:nth-child(4) .ngb-tp-hour .ng-star-inserted:nth-child(1) .ngb-tp-chevron"));
            JavaScriptClick(uiLiquorStartHour);

            // increment liquor sale start minute
            var uiLiquorStartMinute =
                ngDriver.FindElement(By.CssSelector(
                    ".col-md-2:nth-child(4) .ngb-tp-minute .ng-star-inserted:nth-child(1) .ngb-tp-chevron"));
            JavaScriptClick(uiLiquorStartMinute);

            // double decrement liquor sale end hour
            var uiLiquorEndHour1 =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorEndTime'] .ngb-tp-hour span.bottom"));
            JavaScriptClick(uiLiquorEndHour1);
            JavaScriptClick(uiLiquorEndHour1);

            // double decrement liquor sale end minute
            var uiLiquorEndMinute1 =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorEndTime'] .ngb-tp-minute span.bottom"));
            JavaScriptClick(uiLiquorEndMinute1);
            JavaScriptClick(uiLiquorEndMinute1);

            // enter contact email
            var uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'contactEmail']"));
            uiContactEmail.SendKeys(contactEmail);

            // re-enter contact email
            var uiContactEmail2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactEmailConfirmation']"));
            uiContactEmail2.SendKeys(contactEmail);

            // select agreement checkbox
            var uiAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            uiAgreement.Click();
        }
    }
}