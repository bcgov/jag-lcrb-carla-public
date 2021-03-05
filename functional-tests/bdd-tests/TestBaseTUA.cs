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
        [And(@"I complete the TUA event application")]
        public void TUAEventApplication(string bizType)
        {
            /* 
            Page Title: Temporary Use Area Event
            */

            string eventName = "Test Event";
            string contactName = "Contact Name";
            string contactPhone = "2501811818";
            string locationName = "Location name";
            string attendanceCount = "18";
            string eventTotalAttendance = "180";
            string description = "Sample description.";

            // enter the event name
            NgWebElement uiEventName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='eventName']"));
            uiEventName.SendKeys(eventName);

            // enter the contact name
            NgWebElement uiContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            uiContactName.SendKeys(contactName);

            // enter the contact phone
            NgWebElement uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            uiContactPhone.SendKeys(contactPhone);

            // click add location button
            NgWebElement uiAddLocation = ngDriver.FindElement(By.CssSelector("button.btn-secondary"));
            uiAddLocation.Click();

            // select the location ID
            NgWebElement uiLocationID = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceAreaId']"));
            uiLocationID.Click();

            // select the option
            NgWebElement uiLocationIDOption = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceAreaId'] option[value='0: e0828de4-327d-eb11-b824-00505683fbf4']"));
            uiLocationIDOption.Click();

            // enter the location name
            NgWebElement uiLocationName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='name']"));
            uiLocationName.SendKeys(locationName);

            // enter the attendance count - no save button?
            NgWebElement uiAttendanceCount = ngDriver.FindElement(By.CssSelector("input[formcontrolname='attendance']"));
            uiAttendanceCount.SendKeys(attendanceCount);

            // select the start date
            NgWebElement uiStartDate = ngDriver.FindElement(By.CssSelector(".mat-datepicker-input[formcontrolname='startDate']"));
            JavaScriptClick(uiStartDate);

            NgWebElement uiStartDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            JavaScriptClick(uiStartDate2);

            // select the end date
            NgWebElement uiEndDate = ngDriver.FindElement(By.CssSelector(".mat-datepicker-input[formcontrolname='endDate']"));
            JavaScriptClick(uiEndDate);

            NgWebElement uiEndDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            JavaScriptClick(uiEndDate2);

            // increment the event start hour
            NgWebElement uiEventStartHour = ngDriver.FindElement(By.CssSelector(".time-picker-title+ .col-md-2 .ngb-tp-hour .ng-star-inserted:nth-child(1)"));
            JavaScriptClick(uiEventStartHour);

            // increment the event start minute
            NgWebElement uiEventStartMinute = ngDriver.FindElement(By.CssSelector(".time-picker-title+ .col-md-2 .ngb-tp-minute .ng-star-inserted:nth-child(1) .ngb-tp-chevron"));
            JavaScriptClick(uiEventStartMinute);

            // decrement the event end hour
            NgWebElement uiEventEndHour = ngDriver.FindElement(By.CssSelector(".ng-pristine .ngb-tp-hour .bottom"));
            JavaScriptClick(uiEventEndHour);

            // decrement the event end minute
            NgWebElement uiEventEndMinute = ngDriver.FindElement(By.CssSelector(".ng-pristine .ngb-tp-minute .bottom"));
            JavaScriptClick(uiEventEndMinute);

            // enter total attendance
            NgWebElement uiTotalAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxAttendance']"));
            uiTotalAttendance.SendKeys(eventTotalAttendance);

            // select 'Yes' for minors attending
            NgWebElement uiMinorsAttendingYes = ngDriver.FindElement(By.CssSelector("select[formcontrolname='minorsAttending'] option[value='true']"));
            JavaScriptClick(uiMinorsAttendingYes);

            // select event type
            NgWebElement uiEventType = ngDriver.FindElement(By.CssSelector("select[formcontrolname='tuaEventType'] option[value='0: 845280000']"));
            JavaScriptClick(uiEventType);

            // select 'Yes' for 'Will your permanent licensed establishment also be closed to the public during the event hours?'
            NgWebElement uiClosedToPublic = ngDriver.FindElement(By.CssSelector("select[formcontrolname='isClosedToPublic']"));
            JavaScriptClick(uiClosedToPublic);

            // enter the description
            NgWebElement uiDescription = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='eventTypeDescription']"));
            uiDescription.SendKeys(description);

            // select all the event items
            NgWebElement uiIsWedding = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isWedding']"));
            JavaScriptClick(uiIsWedding);

            NgWebElement uiIsNetworkingParty = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isNetworkingParty']"));
            JavaScriptClick(uiIsNetworkingParty);

            NgWebElement uiIsConcert = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isConcert']"));
            JavaScriptClick(uiIsConcert);

            NgWebElement uiIsNoneOfTheAbove = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isNoneOfTheAbove']"));
            JavaScriptClick(uiIsNoneOfTheAbove);

            NgWebElement uiIsBanquet = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isBanquet']"));
            JavaScriptClick(uiIsBanquet);

            NgWebElement uiIsAmplifiedSound = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isAmplifiedSound']"));
            JavaScriptClick(uiIsAmplifiedSound);

            NgWebElement uiIsDancing = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isDancing']"));
            JavaScriptClick(uiIsDancing);

            NgWebElement uiIsReception = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isReception']"));
            JavaScriptClick(uiIsReception);

            NgWebElement uiIsLiveEntertainment = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isLiveEntertainment']"));
            JavaScriptClick(uiIsLiveEntertainment);

            NgWebElement uiIsGambling = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isGambling']"));
            JavaScriptClick(uiIsGambling);

            // enter the event email address
            NgWebElement uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactEmail']"));
            JavaScriptClick(uiContactEmail);

            // re-enter the event email address
            NgWebElement uiContactEmailConfirmation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactEmailConfirmation']"));
            JavaScriptClick(uiContactEmailConfirmation);

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();
        }
    }
}
