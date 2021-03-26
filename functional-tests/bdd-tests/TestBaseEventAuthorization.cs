using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request an event authorization (.*)")]
        public void RequestEventAuthorization(string eventType)
        {
            /* 
            Page Title: Licences & Authorizations
            Subtitle:   Catering Licences
            */

            /*
            Temporary workaround for LCSD-3867 - start
            */

            // sign out
            SignOut();

            // log back in as same user
            ReturnLogin();

            // navigate to Licences tab
            ClickLicencesTab();

            /*
            Temporary workaround for LCSD-3867 - end
            */

            var requestEventAuthorization = "Request Catered Event Authorization";

            // click on the request event authorization link
            var uiRequestEventAuthorization = ngDriver.FindElement(By.LinkText(requestEventAuthorization));
            uiRequestEventAuthorization.Click();

            /* 
            Page Title: Catered Event Authorization Request
            */

            // create event authorization data
            var eventContactName = "AutoTestEventContactName";
            var eventContactPhone = "2500000000";

            var eventDescription = "Automated test event description added here.";
            var eventClientOrHostName = "Automated test event";
            var maximumAttendance = "100";
            var maximumStaffAttendance = "25";
            var maximumAttendanceApproval = "300";
            var maximumStaffAttendanceApproval = "300";

            var venueNameDescription = "Automated test venue name or description";
            var venueAdditionalInfo = "Automated test additional venue information added here.";
            var physicalAddStreetAddress1 = "Automated test street address 1";
            var physicalAddStreetAddress2 = "Automated test street address 2";
            var physicalAddCity = "Victoria";
            var physicalAddPostalCode = "V9A 6X5";

            // enter event contact name
            var uiEventContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            uiEventContactName.SendKeys(eventContactName);

            // enter event contact phone
            var uiEventContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            uiEventContactPhone.SendKeys(eventContactPhone);

            if (eventType == "for a community event after 2am")
            {
                // select community event type
                var uiEventType =
                    ngDriver.FindElement(By.CssSelector("[formcontrolname='eventType'] [value='1: 845280001']"));
                uiEventType.Click();
            }
            else
            {
                // select corporate event type
                var uiEventType =
                    ngDriver.FindElement(By.CssSelector("[formcontrolname='eventType'] option[value='2: 845280002']"));
                uiEventType.Click();
            }

            // enter event description
            var uiEventDescription =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='eventTypeDescription']"));
            uiEventDescription.SendKeys(eventDescription);

            // enter event client or host name
            var uiEventClientOrHostName =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='clientHostname']"));
            uiEventClientOrHostName.SendKeys(eventClientOrHostName);

            if (eventType == "with more than 500 people")
            {
                // enter maximum attendance
                var uiMaxAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxAttendance']"));
                uiMaxAttendance.SendKeys(maximumAttendanceApproval);

                // enter maximum staff attendance
                var uiMaxStaffAttendance =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxStaffAttendance']"));
                uiMaxStaffAttendance.SendKeys(maximumStaffAttendanceApproval);
            }
            else
            {
                // enter maximum attendance
                var uiMaxAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxAttendance']"));
                uiMaxAttendance.SendKeys(maximumAttendance);

                // enter maximum staff attendance
                var uiMaxStaffAttendance =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxStaffAttendance']"));
                uiMaxStaffAttendance.SendKeys(maximumStaffAttendance);
            }

            // select whether minors are attending - yes
            var uiMinorsAttending =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='minorsAttending'] option[value='true']"));
            uiMinorsAttending.Click();

            // select type of food service provided
            var uiFoodServiceProvided =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='foodService'] option[value='0: 845280000']"));
            uiFoodServiceProvided.Click();

            // select type of entertainment provided
            var uiEntertainmentProvided =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='entertainment'] option[value='1: 845280001']"));
            uiEntertainmentProvided.Click();

            // enter venue name description
            var uiVenueNameDescription =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='venueDescription']"));
            uiVenueNameDescription.SendKeys(venueNameDescription);

            if (eventType == "for an outdoor location")
            {
                // select outdoor venue location
                var uiVenueLocation =
                    ngDriver.FindElement(
                        By.CssSelector("[formcontrolname='specificLocation'] option[value='1: 845280001']"));
                uiVenueLocation.Click();
            }
            else if (eventType == "for an indoor and outdoor location")
            {
                // select both indoor/outdoor venue location
                var uiVenueLocation =
                    ngDriver.FindElement(
                        By.CssSelector("[formcontrolname='specificLocation'] option[value='2: 845280002']"));
                uiVenueLocation.Click();
            }
            else
            {
                // select indoor venue location
                var uiVenueLocation =
                    ngDriver.FindElement(
                        By.CssSelector("[formcontrolname='specificLocation'] option[value='0: 845280000']"));
                uiVenueLocation.Click();
            }

            // enter venue additional info
            var uiVenueAdditionalInfo =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            uiVenueAdditionalInfo.SendKeys(venueAdditionalInfo);

            // enter physical address - street address 1
            var uiPhysicalAddStreetAddress1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street1']"));
            uiPhysicalAddStreetAddress1.SendKeys(physicalAddStreetAddress1);

            // enter physical address - street address 2 
            var uiPhysicalAddStreetAddress2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street2']"));
            uiPhysicalAddStreetAddress2.SendKeys(physicalAddStreetAddress2);

            // enter physical address - city
            var uiPhysicalAddCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='city']"));
            uiPhysicalAddCity.SendKeys(physicalAddCity);

            // enter physical address - postal code
            var uiPhysicalAddPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='postalCode']"));
            uiPhysicalAddPostalCode.SendKeys(physicalAddPostalCode);

            // select terms and conditions checkbox
            var uiTermsAndConditions =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            JavaScriptClick(uiTermsAndConditions);

            // select start date
            var uiVenueStartDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='startDate']"));
            uiVenueStartDate1.Click();

            try
            {
                var uiVenueStartDate2 =
                    ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
                uiVenueStartDate2.Click();
            }

            catch
            {
                var uiVenueStartDate2 =
                    ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
                uiVenueStartDate2.Click();
            }

            // select end date
            var uiEndDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='endDate']"));
            uiEndDate1.Click();

            // click on the next button
            var uiOpenCalendarNext = ngDriver.FindElement(By.CssSelector(".mat-calendar .mat-calendar-next-button"));
            uiOpenCalendarNext.Click();

            try
            {
                // click on the first day
                var uiOpenCalendarFirstDay =
                    ngDriver.FindElement(
                        By.CssSelector(".mat-calendar-content .mat-calendar-body-cell-content:first-child"));
                JavaScriptClick(uiOpenCalendarFirstDay);
            }

            catch
            {
                // click on the first day
                var uiOpenCalendarFirstDay =
                    ngDriver.FindElement(
                        By.CssSelector(".mat-calendar-content .mat-calendar-body-cell-content:first-child"));
                JavaScriptClick(uiOpenCalendarFirstDay);
            }

            // select event and liquor end time after 2am
            if (eventType == "for after 2am" || eventType == "for a community event after 2am")
            {
                var uiEventCloseTime = ngDriver.FindElement(By.CssSelector(
                    ".col-md-2:nth-child(3) .ngb-tp-minute .ng-star-inserted:nth-child(1) .ngb-tp-chevron"));
                JavaScriptClick(uiEventCloseTime);

                var uiLiquorCloseTime =
                    ngDriver.FindElement(
                        By.CssSelector(".col-md-2:nth-child(5) .ngb-tp-minute .btn-link:nth-child(1) .ngb-tp-chevron"));
                JavaScriptClick(uiLiquorCloseTime);
            }

            // click on the terms and conditions checkbox
            var uiAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            uiAgreement.Click();

            if (eventType == "for a draft" || eventType == "being validated")
            {
                // click on the Save For Later button
                var uiSaveForLater = ngDriver.FindElement(By.CssSelector(".btn-primary:nth-child(1)"));
                uiSaveForLater.Click();
            }
            else
            {
                // click on the Submit button
                var uiSubmit = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
                uiSubmit.Click();
            }
        }


        [And(@"the event history is updated correctly for an application (.*)")]
        public void EventHistoryIsUpdatedCorrectly(string eventType)
        {
            /* 
            Page Title: Licences & Authorizations
            Subtitle:   Catering Licences
            */

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Licences')]")).Displayed);

            try
            {
                var uiExpandEventHistory =
                    ngDriver.FindElement(
                        By.CssSelector("mat-expansion-panel-header[role='button'] #expand-history-button-0"));
                uiExpandEventHistory.Click();
            }
            catch
            {
                var uiExpandEventHistory =
                    ngDriver.FindElement(
                        By.CssSelector("mat-expansion-panel-header[role='button'] #expand-history-button-0"));
                uiExpandEventHistory.Click();
            }

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Automated test event')]")).Displayed);

            // confirm that the correct status based on application type is present
            if (eventType == "for a draft" || eventType == "being validated")
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Draft')]")).Displayed);

            if (eventType == "for after 2am" || eventType == "for an indoor and outdoor location" ||
                eventType == "with more than 500 people" || eventType == "for an outdoor location")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'In Review')]")).Displayed);

                Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'Download Authorization'))]"))
                    .Displayed);
            }

            if (eventType == "for a community event after 2am" || eventType == "without approval")
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Approved')]")).Displayed);
        }


        [And(@"the saved event authorization details are correct")]
        public void SavedEventHistoryIsCorrect()
        {
            /* 
            Page Title: Catered Event Authorization Request
            */

            // create event authorization data
            var eventContactName = "AutoTestEventContactName";
            var eventContactPhone = "(250) 000-0000";

            var eventDescription = "Automated test event description added here.";
            var eventClientOrHostName = "Automated test event";
            var maximumAttendance = "100";
            var maximumStaffAttendance = "25";

            var venueNameDescription = "Automated test venue name or description";
            var venueAdditionalInfo = "Automated test additional venue information added here.";
            var physicalAddStreetAddress1 = "Automated test street address 1";
            var physicalAddStreetAddress2 = "Automated test street address 2";
            var physicalAddCity = "Victoria";
            var physicalAddPostalCode = "V9A 6X5";

            // check event contact name
            var uiEventContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            Assert.True(uiEventContactName.GetAttribute("value") == eventContactName);

            // check event contact phone
            var uiEventContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            Assert.True(uiEventContactPhone.GetAttribute("value") == eventContactPhone);

            // check corporate event type selected
            var uiEventType = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventType']"));
            Assert.True(uiEventType.GetAttribute("value") == "2: 845280002");

            // check event description
            var uiEventDescription =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='eventTypeDescription']"));
            Assert.True(uiEventDescription.GetAttribute("value") == eventDescription);

            // check event client or host name
            var uiEventClientOrHostName =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='clientHostname']"));
            Assert.True(uiEventClientOrHostName.GetAttribute("value") == eventClientOrHostName);

            // check maximum attendance
            var uiMaxAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxAttendance']"));
            Assert.True(uiMaxAttendance.GetAttribute("value") == maximumAttendance);

            // check maximum staff attendance
            var uiMaxStaffAttendance =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxStaffAttendance']"));
            Assert.True(uiMaxStaffAttendance.GetAttribute("value") == maximumStaffAttendance);

            // check whether minors are attending - yes
            var uiMinorsAttending = ngDriver.FindElement(By.CssSelector("[formcontrolname='minorsAttending']"));
            Assert.True(uiMinorsAttending.GetAttribute("value") == "true");

            // check type of food service provided
            var uiFoodServiceProvided = ngDriver.FindElement(By.CssSelector("[formcontrolname='foodService']"));
            Assert.True(uiFoodServiceProvided.GetAttribute("value") == "0: 845280000");

            // check type of entertainment provided
            var uiEntertainmentProvided = ngDriver.FindElement(By.CssSelector("[formcontrolname='entertainment']"));
            Assert.True(uiEntertainmentProvided.GetAttribute("value") == "1: 845280001");

            // check venue name description
            var uiVenueNameDescription =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='venueDescription']"));
            Assert.True(uiVenueNameDescription.GetAttribute("value") == venueNameDescription);

            // check venue location - indoors
            var uiVenueLocation = ngDriver.FindElement(By.CssSelector("[formcontrolname='specificLocation']"));
            Assert.True(uiVenueLocation.GetAttribute("value") == "0: 845280000");

            // check venue additional info
            var uiVenueAdditionalInfo =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            Assert.True(uiVenueAdditionalInfo.GetAttribute("value") == venueAdditionalInfo);

            // check physical address - street address 1
            var uiPhysicalAddStreetAddress1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street1']"));
            Assert.True(uiPhysicalAddStreetAddress1.GetAttribute("value") == physicalAddStreetAddress1);

            // check physical address - street address 2 
            var uiPhysicalAddStreetAddress2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street2']"));
            Assert.True(uiPhysicalAddStreetAddress2.GetAttribute("value") == physicalAddStreetAddress2);

            // check physical address - city
            var uiPhysicalAddCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='city']"));
            Assert.True(uiPhysicalAddCity.GetAttribute("value") == physicalAddCity);

            // check physical address - postal code
            var uiPhysicalAddPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='postalCode']"));
            Assert.True(uiPhysicalAddPostalCode.GetAttribute("value") == physicalAddPostalCode);

            // check event start and end times     
            var uiEventStartTimeHours =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='startTime'] input[aria-label='Hours']"));
            Assert.True(uiEventStartTimeHours.GetAttribute("value") == "09");

            var uiEventStartTimeMinutes =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='startTime'] input[aria-label='Minutes']"));
            Assert.True(uiEventStartTimeMinutes.GetAttribute("value") == "00");

            var uiEventCloseTimeHours =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='endTime'] input[aria-label='Hours']"));
            Assert.True(uiEventCloseTimeHours.GetAttribute("value") == "02");

            var uiEventCloseTimeMinutes =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='endTime'] input[aria-label='Minutes']"));
            Assert.True(uiEventCloseTimeMinutes.GetAttribute("value") == "00");

            // check liquor start and end times 
            var uiLiquorStartTimeHours =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorStartTime'] input[aria-label='Hours']"));
            Assert.True(uiLiquorStartTimeHours.GetAttribute("value") == "09");

            var uiLiquorStartTimeMinutes =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorStartTime'] input[aria-label='Minutes']"));
            Assert.True(uiLiquorStartTimeMinutes.GetAttribute("value") == "00");

            var uiLiquorCloseTimeHours =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorEndTime'] input[aria-label='Hours']"));
            Assert.True(uiLiquorCloseTimeHours.GetAttribute("value") == "02");

            var uiLiquorCloseTimeMinutes =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='endTime'] input[aria-label='Minutes']"));
            Assert.True(uiLiquorCloseTimeMinutes.GetAttribute("value") == "00");
        }


        [And(@"I do not complete the event authorization application correctly")]
        public void EventAuthorizationValidation()
        {
            /* 
            Page Title: Catered Event Authorization Request
            */

            // remove event contact name
            var uiEventContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            uiEventContactName.Clear();

            // remove event contact phone
            var uiEventContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            uiEventContactPhone.Clear();

            // remove event description
            var uiEventDescription =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='eventTypeDescription']"));
            uiEventDescription.Clear();

            // remove event client or host name
            var uiEventClientOrHostName =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='clientHostname']"));
            uiEventClientOrHostName.Clear();

            // remove maximum attendance
            var uiMaxAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxAttendance']"));
            uiMaxAttendance.Clear();

            // remove maximum staff attendance
            var uiMaxStaffAttendance =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxStaffAttendance']"));
            uiMaxStaffAttendance.Clear();

            // remove venue name description
            var uiVenueNameDescription =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='venueDescription']"));
            uiVenueNameDescription.Clear();

            // remove venue additional info
            var uiVenueAdditionalInfo =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            uiVenueAdditionalInfo.Clear();

            // remove physical address - street address 1
            var uiPhysicalAddStreetAddress1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street1']"));
            uiPhysicalAddStreetAddress1.Clear();

            // remove physical address - street address 2 
            var uiPhysicalAddStreetAddress2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street2']"));
            uiPhysicalAddStreetAddress2.Clear();

            // remove physical address - city
            var uiPhysicalAddCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='city']"));
            uiPhysicalAddCity.Clear();

            // remove physical address - postal code
            var uiPhysicalAddPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='postalCode']"));
            uiPhysicalAddPostalCode.Clear();

            // deselect terms and conditions checkbox
            var uiTermsAndConditions =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            uiTermsAndConditions.Click();
        }
    }
}