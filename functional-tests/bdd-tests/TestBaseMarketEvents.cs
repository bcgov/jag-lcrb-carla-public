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
        [And(@"I request a market event (.*)")]
        public void MarketEvents(string frequency)
        {
            /* 
            Page Title: Licences & Authorizations
            */

            string marketEvent = "Request Market Event Authorization";

            // click on the Request Market Event Authorization link
            NgWebElement uiOnSiteStoreEndorsement = ngDriver.FindElement(By.LinkText(marketEvent));
            uiOnSiteStoreEndorsement.Click();

            /* 
            Page Title: Market Authorization Request
            */

            // create test data
            string contactName = "Test Automation";
            string contactPhoneNumber = "(222) 222-2222";
            string contactEmail = "test@automation.com";
            string marketName = "Point Ellis Market";
            string marketWebsite = "http://www.pointellismarketisamazing.com";
            string bizLegalName = "Point Ellis Market Cooperative";
            string marketBizNumber = "2222222222222222";
            string incorporationNumber = "1234567";
            string address1 = "645 Tyee Road";
            string address2 = "West";
            string city = "Victoria";
            string postalCode = "V9A 6X5";
            string additionalDetails = "Additional details for automated test.";
            string additionalInformation = "Additional information for automated test.";
            string eventStartHour = "10";
            string eventStartMinute = "30";
            string eventEndHour = "11";
            string eventEndMinute = "05";
            string liquorStartHour = "22";
            string liquorStartMinute = "30";
            string liquorEndHour = "14";
            string liquorEndMinute = "04";

            // select preventing sale of liquor checkbox
            NgWebElement uiPreventingSaleOfLiquor = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isNoPreventingSaleofLiquor']"));
            uiPreventingSaleOfLiquor.Click();

            // select market managed or carried checkbox
            NgWebElement uiMarketManagedOrCarried = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketManagedorCarried']"));
            uiMarketManagedOrCarried.Click();

            // select market only vendors checkbox
            NgWebElement uiIsMarketOnlyVendors = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketOnlyVendors']"));
            uiIsMarketOnlyVendors.Click();

            // select imported goods checkbox
            NgWebElement uiIsNoImportedGoods = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isNoImportedGoods']"));
            uiIsNoImportedGoods.Click();

            // select six vendors checkbox
            NgWebElement uiIsMarketHostsSixVendors = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketHostsSixVendors']"));
            uiIsMarketHostsSixVendors.Click();

            // select max amount or duration checkbox
            NgWebElement uiIsMarketMaxAmountorDuration = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketMaxAmountorDuration']"));
            uiIsMarketMaxAmountorDuration.Click();

            // enter contact name
            NgWebElement uiContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            uiContactName.SendKeys(contactName);

            // enter contact phone number
            NgWebElement uiContactPhoneNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'contactPhone']"));
            uiContactPhoneNumber.SendKeys(contactPhoneNumber);

            // enter contact email
            NgWebElement uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'contactEmail']"));
            uiContactEmail.SendKeys(contactEmail);

            // enter market name
            NgWebElement uiMarketName = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'marketName']"));
            uiMarketName.SendKeys(marketName);

            // enter market website
            NgWebElement uiMarketWebsite = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'marketWebsite']"));
            uiMarketWebsite.SendKeys(marketWebsite);

            // enter business legal name
            NgWebElement uiClientHostname = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'clientHostname']"));
            uiClientHostname.SendKeys(bizLegalName);

            // select market event type
            NgWebElement uiMarketEventType = ngDriver.FindElement(By.CssSelector("[formcontrolname= 'marketEventType'] option[value = '2: 845280002']"));
            uiMarketEventType.Click();

            // enter market business number
            NgWebElement uiBusinessNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'businessNumber']"));
            uiBusinessNumber.SendKeys(marketBizNumber);

            // enter incorporation/registration number
            NgWebElement uiRegistrationNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'registrationNumber']"));
            uiRegistrationNumber.SendKeys(incorporationNumber);

            // enter address 1
            NgWebElement uiStreet1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'street1']"));
            uiStreet1.SendKeys(address1);

            // enter address 2
            NgWebElement uiStreet2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'street2']"));
            uiStreet2.SendKeys(address2);

            // enter city
            NgWebElement uiCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'city']"));
            uiCity.SendKeys(city);

            // enter postal code
            NgWebElement uiPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'postalCode']"));
            uiPostalCode.SendKeys(postalCode);

            // enter additional details
            NgWebElement uiAdditionalDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='venueDescription']"));
            uiAdditionalDetails.SendKeys(additionalDetails);

            // select 'Once' from the frequency dropdown
            if (frequency == "for one date only")
            {
                // select frequency
                NgWebElement uiFrequency = ngDriver.FindElement(By.CssSelector("[formcontrolname='marketDuration'] option[value='3: 845280003']"));
                uiFrequency.Click();
            }

            // select 'Weekly' from the frequency dropdown
            if (frequency == "weekly")
            {
                // select frequency
                NgWebElement uiFrequency = ngDriver.FindElement(By.CssSelector("[formcontrolname='marketDuration'] option[value='0: 845280000']"));
                uiFrequency.Click();
            }

            // select 'Bi-Weekly' from the frequency dropdown
            if (frequency == "bi-weekly")
            {
                // select frequency
                NgWebElement uiFrequency = ngDriver.FindElement(By.CssSelector("[formcontrolname='marketDuration'] option[value='1: 845280001']"));
                uiFrequency.Click();
            }

            // select 'Monthly' from the frequency dropdown
            if (frequency == "monthly")
            {
                // select frequency
                NgWebElement uiFrequency = ngDriver.FindElement(By.CssSelector("[formcontrolname='marketDuration'] option[value='2: 845280002']"));
                uiFrequency.Click();
            }

            // enter additional information
            NgWebElement uiAdditionalInformation = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            uiAdditionalInformation.SendKeys(additionalInformation);

            // select start date
            NgWebElement uiStartDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='startDate']"));
            uiStartDate1.Click();

            NgWebElement uiStartDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            uiStartDate2.Click();

            // select end date
            NgWebElement uiEndDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='endDate']"));
            uiEndDate1.Click();

            NgWebElement uiEndDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            uiEndDate2.Click();

            if (frequency != "for one date only")
            {
                // confirm that all days are available for selection; de-selection is required due to days per week limit
                NgWebElement uiSunday = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='sunday']"));
                uiSunday.Click();
                NgWebElement uiSunday2 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='sunday']"));
                uiSunday2.Click();

                NgWebElement uiMonday = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='monday']"));
                uiMonday.Click();
                NgWebElement uiMonday2 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='monday']"));
                uiMonday2.Click();

                NgWebElement uiTuesday = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='tuesday']"));
                uiTuesday.Click();
                NgWebElement uiTuesday2 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='tuesday']"));
                uiTuesday2.Click();

                NgWebElement uiWednesday = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='wednesday']"));
                uiWednesday.Click();
                NgWebElement uiWednesday2 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='wednesday']"));
                uiWednesday2.Click();

                NgWebElement uiThursday = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='thursday']"));
                uiThursday.Click();
                NgWebElement uiThursday2 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='thursday']"));
                uiThursday2.Click();

                NgWebElement uiFriday = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='friday']"));
                uiFriday.Click();
                NgWebElement uiFriday2 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='friday']"));
                uiFriday2.Click();

                NgWebElement uiSaturday = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='saturday']"));
                uiSaturday.Click();
                NgWebElement uiSaturday2 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='saturday']"));
                uiSaturday2.Click();

                if ((frequency == "weekly") || (frequency == "bi-weekly"))
                {
                    // make final selection re days of the week
                    NgWebElement uiThursdayFinal = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='thursday']"));
                    uiThursdayFinal.Click();

                    NgWebElement uiFridayFinal = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='friday']"));
                    uiFridayFinal.Click();

                    NgWebElement uiSaturdayFinal = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='saturday']"));
                    uiSaturdayFinal.Click();
                }

                if (frequency == "monthly")
                {
                    // select first week of the month
                    NgWebElement uiWeekOfMonth1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='weekOfMonth'] #mat-radio-7[tabindex='-1']"));
                    uiWeekOfMonth1.Click();

                    // select second week of the month
                    NgWebElement uiWeekOfMonth2 = ngDriver.FindElement(By.CssSelector("[formcontrolname='weekOfMonth'] #mat-radio-8[tabindex='-1']"));
                    uiWeekOfMonth2.Click();

                    // select third week of the month
                    NgWebElement uiWeekOfMonth3 = ngDriver.FindElement(By.CssSelector("[formcontrolname='weekOfMonth'] #mat-radio-9[tabindex='-1']"));
                    uiWeekOfMonth3.Click();

                    // select fourth week of the month
                    NgWebElement uiWeekOfMonth4 = ngDriver.FindElement(By.CssSelector("[formcontrolname='weekOfMonth'] #mat-radio-10[tabindex='-1']"));
                    uiWeekOfMonth4.Click();
                }
            }

            // enter event start hour
            NgWebElement uiEventStartHour = ngDriver.FindElement(By.CssSelector("[formcontrolname='startTime'] input[aria-label='Hours']"));
            uiEventStartHour.SendKeys(eventStartHour);

            // enter event start minute
            NgWebElement uiEventStartMinute = ngDriver.FindElement(By.CssSelector("[formcontrolname='startTime'] input[aria-label='Minutes']"));
            uiEventStartMinute.SendKeys(eventStartMinute);

            // enter event end hour
            NgWebElement uiEventEndHour = ngDriver.FindElement(By.CssSelector("[formcontrolname='endTime'] input[aria-label='Hours']"));
            uiEventEndHour.SendKeys(eventEndHour);

            // enter event end minute
            NgWebElement uiEventEndMinute = ngDriver.FindElement(By.CssSelector("[formcontrolname='endTime'] input[aria-label='Minutes']"));
            uiEventEndMinute.SendKeys(eventEndMinute);

            // enter liquor sale start hour
            NgWebElement uiLiquorStartHour = ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorStartTime'] input[aria-label='Hours']"));
            uiLiquorStartHour.SendKeys(liquorStartHour);

            // enter liquor sale start minute
            NgWebElement uiLiquorStartMinute = ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorStartTime'] input[aria-label='Minutes']"));
            uiLiquorStartMinute.SendKeys(liquorStartMinute);

            // enter liquor sale end hour
            NgWebElement uiLiquorEndHour = ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorEndTime'] input[aria-label='Hours']"));
            uiLiquorEndHour.SendKeys(liquorEndHour);

            // enter liquor sale end minute
            NgWebElement uiLiquorEndMinute = ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorEndTime'] input[aria-label='Minutes']"));
            uiLiquorEndMinute.SendKeys(liquorEndMinute);

            // select serving it right/minors checkbox
            NgWebElement uiServingItRight = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isAllStaffServingitRight']"));
            uiServingItRight.Click();

            // select sample sizes checkbox
            NgWebElement uiSampleSizes = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isSampleSizeCompliant']"));
            uiSampleSizes.Click();

            // select agreement checkbox
            NgWebElement uiAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            uiAgreement.Click();

            // click on the Submit & Pay button
            NgWebElement uiSubmitandPay = ngDriver.FindElement(By.CssSelector(".btn-primary+ .ng-star-inserted"));
            uiSubmitandPay.Click();

            MakePayment();
        }


        [And(@"I click on the event history for markets")]
        public void MarketEventsHistory()
        {
            NgWebElement uiExpandEventHistory = ngDriver.FindElement(By.CssSelector("mat-expansion-panel-header[role='button'] #expand-history-button-0"));
            uiExpandEventHistory.Click();
        }
        

        [And(@"the market event is approved")]
        public void MarketEventsApproved()
        {
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Approved')]")).Displayed);
        }
    }
}
