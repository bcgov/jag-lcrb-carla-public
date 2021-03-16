using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the TUA event application")]
        public void TUAEventApplication()
        {
            /* 
            Page Title: Temporary Use Area Event
            */

            var eventName = "Test Event";
            var contactName = "Contact Name";
            var contactPhone = "2501811818";
            var locationName = "Location name";
            var attendanceCount = "18";
            var eventTotalAttendance = "180";
            var description = "Sample description.";

            // enter the event name
            var uiEventName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='eventName']"));
            uiEventName.SendKeys(eventName);

            // enter the contact name
            var uiContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            uiContactName.SendKeys(contactName);

            // enter the contact phone
            var uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            uiContactPhone.SendKeys(contactPhone);

            // click add location button
            var uiAddLocation = ngDriver.FindElement(By.CssSelector("button.btn-secondary"));
            uiAddLocation.Click();

            // select the location ID
            var uiLocationID = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceAreaId']"));
            uiLocationID.Click();

            // select the option
            var uiLocationIDOption = ngDriver.FindElement(By.CssSelector(
                "[formcontrolname='serviceAreaId'] option[value='0: e0828de4-327d-eb11-b824-00505683fbf4']"));
            uiLocationIDOption.Click();

            // enter the location name
            var uiLocationName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='name']"));
            uiLocationName.SendKeys(locationName);

            // enter the attendance count - no save button?
            var uiAttendanceCount = ngDriver.FindElement(By.CssSelector("input[formcontrolname='attendance']"));
            uiAttendanceCount.SendKeys(attendanceCount);

            // select the start date
            var uiStartDate =
                ngDriver.FindElement(By.CssSelector(".mat-datepicker-input[formcontrolname='startDate']"));
            JavaScriptClick(uiStartDate);

            var uiStartDate2 =
                ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            JavaScriptClick(uiStartDate2);

            // select the end date
            var uiEndDate = ngDriver.FindElement(By.CssSelector(".mat-datepicker-input[formcontrolname='endDate']"));
            JavaScriptClick(uiEndDate);

            var uiEndDate2 =
                ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            JavaScriptClick(uiEndDate2);

            // increment the event start hour
            var uiEventStartHour =
                ngDriver.FindElement(
                    By.CssSelector(".time-picker-title+ .col-md-2 .ngb-tp-hour .ng-star-inserted:nth-child(1)"));
            JavaScriptClick(uiEventStartHour);

            // increment the event start minute
            var uiEventStartMinute = ngDriver.FindElement(By.CssSelector(
                ".time-picker-title+ .col-md-2 .ngb-tp-minute .ng-star-inserted:nth-child(1) .ngb-tp-chevron"));
            JavaScriptClick(uiEventStartMinute);

            // decrement the event end hour
            var uiEventEndHour = ngDriver.FindElement(By.CssSelector(".ng-pristine .ngb-tp-hour .bottom"));
            JavaScriptClick(uiEventEndHour);

            // decrement the event end minute
            var uiEventEndMinute = ngDriver.FindElement(By.CssSelector(".ng-pristine .ngb-tp-minute .bottom"));
            JavaScriptClick(uiEventEndMinute);

            // enter total attendance
            var uiTotalAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxAttendance']"));
            uiTotalAttendance.SendKeys(eventTotalAttendance);

            // select 'Yes' for minors attending
            var uiMinorsAttendingYes =
                ngDriver.FindElement(By.CssSelector("select[formcontrolname='minorsAttending'] option[value='true']"));
            JavaScriptClick(uiMinorsAttendingYes);

            // select event type
            var uiEventType =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='tuaEventType'] option[value='0: 845280000']"));
            JavaScriptClick(uiEventType);

            // select 'Yes' for 'Will your permanent licensed establishment also be closed to the public during the event hours?'
            var uiClosedToPublic = ngDriver.FindElement(By.CssSelector("select[formcontrolname='isClosedToPublic']"));
            JavaScriptClick(uiClosedToPublic);

            // enter the description
            var uiDescription =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='eventTypeDescription']"));
            uiDescription.SendKeys(description);

            // select all the event items
            var uiIsWedding = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isWedding']"));
            JavaScriptClick(uiIsWedding);

            var uiIsNetworkingParty =
                ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isNetworkingParty']"));
            JavaScriptClick(uiIsNetworkingParty);

            var uiIsConcert = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isConcert']"));
            JavaScriptClick(uiIsConcert);

            var uiIsNoneOfTheAbove =
                ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isNoneOfTheAbove']"));
            JavaScriptClick(uiIsNoneOfTheAbove);

            var uiIsBanquet = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isBanquet']"));
            JavaScriptClick(uiIsBanquet);

            var uiIsAmplifiedSound =
                ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isAmplifiedSound']"));
            JavaScriptClick(uiIsAmplifiedSound);

            var uiIsDancing = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isDancing']"));
            JavaScriptClick(uiIsDancing);

            var uiIsReception = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isReception']"));
            JavaScriptClick(uiIsReception);

            var uiIsLiveEntertainment =
                ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isLiveEntertainment']"));
            JavaScriptClick(uiIsLiveEntertainment);

            var uiIsGambling = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isGambling']"));
            JavaScriptClick(uiIsGambling);

            // enter the event email address
            var uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactEmail']"));
            JavaScriptClick(uiContactEmail);

            // re-enter the event email address
            var uiContactEmailConfirmation =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactEmailConfirmation']"));
            JavaScriptClick(uiContactEmailConfirmation);

            // click on the authorized to submit checkbox
            var uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();
        }
    }
}