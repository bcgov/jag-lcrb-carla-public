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
        [And(@"I request an event authorization (.*)")]
        public void RequestEventAuthorization(string eventType)
        {
            /* 
            Page Title: Licences
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

            string requestEventAuthorization = "Request Event Authorization";

            // click on the request event authorization link
            NgWebElement uiRequestEventAuthorization = ngDriver.FindElement(By.LinkText(requestEventAuthorization));
            uiRequestEventAuthorization.Click();

            /* 
            Page Title: Catered Event Authorization Request
            */

            // create event authorization data
            string eventContactName = "AutoTestEventContactName";
            string eventContactPhone = "2500000000";

            string eventDescription = "Automated test event description added here.";
            string eventClientOrHostName = "Automated test event";
            string maximumAttendance = "100";
            string maximumStaffAttendance = "25";
            string maximumAttendanceApproval = "300";
            string maximumStaffAttendanceApproval = "300";

            string venueNameDescription = "Automated test venue name or description";
            string venueAdditionalInfo = "Automated test additional venue information added here.";
            string physicalAddStreetAddress1 = "Automated test street address 1";
            string physicalAddStreetAddress2 = "Automated test street address 2";
            string physicalAddCity = "Victoria";
            string physicalAddPostalCode = "V9A 6X5";

            // enter event contact name
            NgWebElement uiEventContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            uiEventContactName.SendKeys(eventContactName);

            // enter event contact phone
            NgWebElement uiEventContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            uiEventContactPhone.SendKeys(eventContactPhone);

            if (eventType == "for a community event after 2am")
            {
                // select community event type
                NgWebElement uiEventType = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventType'] [value='1: 845280001']"));
                uiEventType.Click();
            }
            else
            {
                // select corporate event type
                NgWebElement uiEventType = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventType'] option[value='2: 845280002']"));
                uiEventType.Click();
            }

            // enter event description
            NgWebElement uiEventDescription = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='eventTypeDescription']"));
            uiEventDescription.SendKeys(eventDescription);

            // enter event client or host name
            NgWebElement uiEventClientOrHostName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='clientHostname']"));
            uiEventClientOrHostName.SendKeys(eventClientOrHostName);

            if (eventType == "with more than 500 people")
            {
                // enter maximum attendance
                NgWebElement uiMaxAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxAttendance']"));
                uiMaxAttendance.SendKeys(maximumAttendanceApproval);

                // enter maximum staff attendance
                NgWebElement uiMaxStaffAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxStaffAttendance']"));
                uiMaxStaffAttendance.SendKeys(maximumStaffAttendanceApproval);
            }
            else
            {
                // enter maximum attendance
                NgWebElement uiMaxAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxAttendance']"));
                uiMaxAttendance.SendKeys(maximumAttendance);

                // enter maximum staff attendance
                NgWebElement uiMaxStaffAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxStaffAttendance']"));
                uiMaxStaffAttendance.SendKeys(maximumStaffAttendance);
            }

            // select whether minors are attending - yes
            NgWebElement uiMinorsAttending = ngDriver.FindElement(By.CssSelector("[formcontrolname='minorsAttending'] option[value='true']"));
            uiMinorsAttending.Click();

            // select type of food service provided
            NgWebElement uiFoodServiceProvided = ngDriver.FindElement(By.CssSelector("[formcontrolname='foodService'] option[value='0: 845280000']"));
            uiFoodServiceProvided.Click();

            // select type of entertainment provided
            NgWebElement uiEntertainmentProvided = ngDriver.FindElement(By.CssSelector("[formcontrolname='entertainment'] option[value='1: 845280001']"));
            uiEntertainmentProvided.Click();

            // enter venue name description
            NgWebElement uiVenueNameDescription = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='venueDescription']"));
            uiVenueNameDescription.SendKeys(venueNameDescription);

            if (eventType == "for an outdoor location")
            {
                // select outdoor venue location
                NgWebElement uiVenueLocation = ngDriver.FindElement(By.CssSelector("[formcontrolname='specificLocation'] option[value='1: 845280001']"));
                uiVenueLocation.Click();
            }
            else if (eventType == "for an indoor and outdoor location")
            {
                // select both indoor/outdoor venue location
                NgWebElement uiVenueLocation = ngDriver.FindElement(By.CssSelector("[formcontrolname='specificLocation'] option[value='2: 845280002']"));
                uiVenueLocation.Click();
            }
            else
            {
                // select indoor venue location
                NgWebElement uiVenueLocation = ngDriver.FindElement(By.CssSelector("[formcontrolname='specificLocation'] option[value='0: 845280000']"));
                uiVenueLocation.Click();
            }

            // enter venue additional info
            NgWebElement uiVenueAdditionalInfo = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            uiVenueAdditionalInfo.SendKeys(venueAdditionalInfo);

            // enter physical address - street address 1
            NgWebElement uiPhysicalAddStreetAddress1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street1']"));
            uiPhysicalAddStreetAddress1.SendKeys(physicalAddStreetAddress1);

            // enter physical address - street address 2 
            NgWebElement uiPhysicalAddStreetAddress2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street2']"));
            uiPhysicalAddStreetAddress2.SendKeys(physicalAddStreetAddress2);

            // enter physical address - city
            NgWebElement uiPhysicalAddCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='city']"));
            uiPhysicalAddCity.SendKeys(physicalAddCity);

            // enter physical address - postal code
            NgWebElement uiPhysicalAddPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='postalCode']"));
            uiPhysicalAddPostalCode.SendKeys(physicalAddPostalCode);

            // select start date
            NgWebElement uiVenueStartDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='startDate']"));
            uiVenueStartDate1.Click();

            NgWebElement uiVenueStartDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            uiVenueStartDate2.Click();

            // select end date
            NgWebElement uiVenueEndDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='endDate']"));
            uiVenueEndDate1.Click();

            NgWebElement uiVenueEndDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            uiVenueEndDate2.Click();

            // select event and liquor end time after 2am
            if ((eventType == "for after 2am") || (eventType == "for a community event after 2am"))
            {
                NgWebElement uiEventCloseTime = ngDriver.FindElement(By.CssSelector(".col-md-2:nth-child(3) .ngb-tp-minute .ng-star-inserted:nth-child(1) .ngb-tp-chevron"));
                uiEventCloseTime.Click();

                NgWebElement uiLiquorCloseTime = ngDriver.FindElement(By.CssSelector(".col-md-2:nth-child(5) .ngb-tp-minute .btn-link:nth-child(1) .ngb-tp-chevron"));
                uiLiquorCloseTime.Click();
            }

            // select terms and conditions checkbox
            NgWebElement uiTermsAndConditions = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            uiTermsAndConditions.Click();

            if (eventType == "as a draft")
            {
                // click on the Save For Later button
                NgWebElement uiSaveForLater = ngDriver.FindElement(By.CssSelector(".btn-primary:nth-child(1) span"));
                uiSaveForLater.Click();

                System.Threading.Thread.Sleep(4000);
            }
            else
            {
                // click on the Submit button
                NgWebElement uiSubmit = ngDriver.FindElement(By.CssSelector(".btn-primary~ .btn-primary+ .btn-primary"));
                uiSubmit.Click();

                System.Threading.Thread.Sleep(4000);
            }
        }


        [And(@"the event history is updated correctly for an application (.*)")]
        public void EventHistoryIsUpdatedCorrectly(string eventType)
        {
            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Licences')]")).Displayed);

            // click on the Event History bar
            NgWebElement uiExpandEventHistory = ngDriver.FindElement(By.CssSelector(".mat-expansion-panel #mat-expansion-panel-header-2[role='button']"));
            uiExpandEventHistory.Click();

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Automated test event')]")).Displayed);

            // confirm that the correct status based on application type is present
            if (eventType == "as a draft")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Draft')]")).Displayed);
            }

            if ((eventType == "for after 2am") || (eventType == "for an indoor and outdoor location") || (eventType == "with more than 500 people") || (eventType == "for an outdoor location"))
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'In Review')]")).Displayed);

                Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'Download Authorization'))]")).Displayed);
            }

            if ((eventType == "for a community event after 2am") || (eventType == "without approval"))
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Approved')]")).Displayed);
            }
        }


        [And(@"the saved event authorization details are correct")]
        public void SavedEventHistoryIsCorrect()
        {
            /* 
            Page Title: Catered Event Authorization Request
            */

            // create event authorization data
            string eventContactName = "AutoTestEventContactName";
            string eventContactPhone = "(250) 000-0000";

            string eventDescription = "Automated test event description added here.";
            string eventClientOrHostName = "Automated test event";
            string maximumAttendance = "100";
            string maximumStaffAttendance = "25";

            string venueNameDescription = "Automated test venue name or description";
            string venueAdditionalInfo = "Automated test additional venue information added here.";
            string physicalAddStreetAddress1 = "Automated test street address 1";
            string physicalAddStreetAddress2 = "Automated test street address 2";
            string physicalAddCity = "Victoria";
            string physicalAddPostalCode = "V9A 6X5";

            // check event contact name
            NgWebElement uiEventContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            Assert.True(uiEventContactName.GetAttribute("value") == eventContactName);

            // check event contact phone
            NgWebElement uiEventContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            Assert.True(uiEventContactPhone.GetAttribute("value") == eventContactPhone);

            // check community event type selected - TODO
            NgWebElement uiEventType = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventType'] [value='1: 845280001']"));

            // check event description
            NgWebElement uiEventDescription = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='eventTypeDescription']"));
            Assert.True(uiEventDescription.GetAttribute("value") == eventDescription);

            // check event client or host name
            NgWebElement uiEventClientOrHostName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='clientHostname']"));
            Assert.True(uiEventClientOrHostName.GetAttribute("value") == eventClientOrHostName);

            // check maximum attendance
            NgWebElement uiMaxAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxAttendance']"));
            Assert.True(uiMaxAttendance.GetAttribute("value") == maximumAttendance);

            // check maximum staff attendance
            NgWebElement uiMaxStaffAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxStaffAttendance']"));
            Assert.True(uiMaxStaffAttendance.GetAttribute("value") == maximumStaffAttendance);

            // check whether minors are attending - yes
            NgWebElement uiMinorsAttending = ngDriver.FindElement(By.CssSelector("[formcontrolname='minorsAttending'] option[value='true']"));
            Assert.True(uiMinorsAttending.Selected);

            // check type of food service provided - TODO
            NgWebElement uiFoodServiceProvided = ngDriver.FindElement(By.CssSelector("[formcontrolname='foodService'] option[value='0: 845280000']"));

            // check type of entertainment provided - TODO
            NgWebElement uiEntertainmentProvided = ngDriver.FindElement(By.CssSelector("[formcontrolname='entertainment'] option[value='1: 845280001']"));

            // check venue name description
            NgWebElement uiVenueNameDescription = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='venueDescription']"));
            Assert.True(uiVenueNameDescription.GetAttribute("value") == venueNameDescription);

            // check venue location - TODO
            NgWebElement uiVenueLocation = ngDriver.FindElement(By.CssSelector("[formcontrolname='specificLocation'] option[value='1: 845280001']"));

            // check venue additional info
            NgWebElement uiVenueAdditionalInfo = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            Assert.True(uiVenueAdditionalInfo.GetAttribute("value") == venueAdditionalInfo);

            // check physical address - street address 1
            NgWebElement uiPhysicalAddStreetAddress1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street1']"));
            Assert.True(uiPhysicalAddStreetAddress1.GetAttribute("value") == physicalAddStreetAddress1);

            // check physical address - street address 2 
            NgWebElement uiPhysicalAddStreetAddress2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street2']"));
            Assert.True(uiPhysicalAddStreetAddress2.GetAttribute("value") == physicalAddStreetAddress2);

            // check physical address - city
            NgWebElement uiPhysicalAddCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='city']"));
            Assert.True(uiPhysicalAddCity.GetAttribute("value") == physicalAddCity);

            // check physical address - postal code
            NgWebElement uiPhysicalAddPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='postalCode']"));
            Assert.True(uiPhysicalAddPostalCode.GetAttribute("value") == physicalAddPostalCode);

            // check start date is selected - TODO
            NgWebElement uiVenueStartDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='startDate']"));

            // check end date is selected - TODO
            NgWebElement uiVenueEndDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='endDate']"));

            // check event end time after 2am - TODO           
            NgWebElement uiEventCloseTime = ngDriver.FindElement(By.CssSelector(".col-md-2:nth-child(3) .ngb-tp-minute .ng-star-inserted:nth-child(1) .ngb-tp-chevron"));

            // check liquor end time after 2am - TODO           
            NgWebElement uiLiquorCloseTime = ngDriver.FindElement(By.CssSelector(".col-md-2:nth-child(5) .ngb-tp-minute .btn-link:nth-child(1) .ngb-tp-chevron"));
        }


        [And(@"I do not complete the event authorization application correctly")]
        public void EventAuthorizationValidation()
        {
            /* 
            Page Title: Catered Event Authorization Request
            */

            // remove event contact name
            NgWebElement uiEventContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            uiEventContactName.Clear();

            // remove event contact phone
            NgWebElement uiEventContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            uiEventContactPhone.Clear();

            // remove event description
            NgWebElement uiEventDescription = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='eventTypeDescription']"));
            uiEventDescription.Clear();

            // remove event client or host name
            NgWebElement uiEventClientOrHostName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='clientHostname']"));
            uiEventClientOrHostName.Clear();

            // remove maximum attendance
            NgWebElement uiMaxAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxAttendance']"));
            uiMaxAttendance.Clear();

            // remove maximum staff attendance
            NgWebElement uiMaxStaffAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxStaffAttendance']"));
            uiMaxStaffAttendance.Clear();

            // remove venue name description
            NgWebElement uiVenueNameDescription = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='venueDescription']"));
            uiVenueNameDescription.Clear();

            // remove venue additional info
            NgWebElement uiVenueAdditionalInfo = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            uiVenueAdditionalInfo.Clear();

            // remove physical address - street address 1
            NgWebElement uiPhysicalAddStreetAddress1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street1']"));
            uiPhysicalAddStreetAddress1.Clear();

            // remove physical address - street address 2 
            NgWebElement uiPhysicalAddStreetAddress2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street2']"));
            uiPhysicalAddStreetAddress2.Clear();

            // remove physical address - city
            NgWebElement uiPhysicalAddCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='city']"));
            uiPhysicalAddCity.Clear();

            // remove physical address - postal code
            NgWebElement uiPhysicalAddPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='postalCode']"));
            uiPhysicalAddPostalCode.Clear();

            // deselect event and liquor service times are different on specific dates checkbox
            NgWebElement uiEventLiquorServiceTimesDifferent = ngDriver.FindElement(By.Id("mat-checkbox-1"));
            uiEventLiquorServiceTimesDifferent.Click();

            // deselect terms and conditions checkbox
            NgWebElement uiTermsAndConditions = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            uiTermsAndConditions.Click();
        }
    }
}
