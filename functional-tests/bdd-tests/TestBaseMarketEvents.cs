using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a market event (.*)")]
        public void MarketEvents(string frequency)
        {
            /* 
            Page Title: Market Authorization Request
            */

            // create test data
            var contactName = "Test Automation";
            var contactPhoneNumber = "(222) 222-2222";
            var contactEmail = "test@automation.com";
            var marketName = "Point Ellis Market";
            var marketWebsite = "http://www.pointellismarketisamazing.com";
            var bizLegalName = "Point Ellis Market Cooperative";
            var marketBizNumber = "2222222222222222";
            var incorporationNumber = "1234567";
            var address1 = "645 Tyee Road";
            var address2 = "West";
            var city = "Victoria";
            var postalCode = "V9A 6X5";
            var additionalDetails = "Additional details for automated test.";
            var additionalInformation = "Additional information for automated test.";

            // select preventing sale of liquor checkbox
            var uiPreventingSaleOfLiquor =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isNoPreventingSaleofLiquor']"));
            uiPreventingSaleOfLiquor.Click();

            // select market managed or carried checkbox
            var uiMarketManagedOrCarried =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketManagedorCarried']"));
            uiMarketManagedOrCarried.Click();

            // select market only vendors checkbox
            var uiIsMarketOnlyVendors =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketOnlyVendors']"));
            uiIsMarketOnlyVendors.Click();

            // select six vendors checkbox
            var uiIsMarketHostsSixVendors =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketHostsSixVendors']"));
            uiIsMarketHostsSixVendors.Click();

            // select max amount or duration checkbox
            var uiIsMarketMaxAmountorDuration =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketMaxAmountorDuration']"));
            uiIsMarketMaxAmountorDuration.Click();

            // enter contact name
            var uiContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            uiContactName.SendKeys(contactName);

            // enter contact phone number
            var uiContactPhoneNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'contactPhone']"));
            uiContactPhoneNumber.SendKeys(contactPhoneNumber);

            // enter contact email
            var uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'contactEmail']"));
            uiContactEmail.SendKeys(contactEmail);

            // enter market name
            var uiMarketName = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'marketName']"));
            uiMarketName.SendKeys(marketName);

            // enter market website
            var uiMarketWebsite = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'marketWebsite']"));
            uiMarketWebsite.SendKeys(marketWebsite);

            // enter business legal name
            var uiClientHostname = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'clientHostname']"));
            uiClientHostname.SendKeys(bizLegalName);

            // select market event type (annual)
            var uiMarketEventType =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname= 'marketEventType'] option[value = '2: 845280002']"));
            uiMarketEventType.Click();

            // enter market business number
            var uiBusinessNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'businessNumber']"));
            uiBusinessNumber.SendKeys(marketBizNumber);

            // enter incorporation/registration number
            var uiRegistrationNumber =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'registrationNumber']"));
            uiRegistrationNumber.SendKeys(incorporationNumber);

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

            // enter additional details
            var uiAdditionalDetails =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='venueDescription']"));
            uiAdditionalDetails.SendKeys(additionalDetails);

            // select 'Once' from the frequency dropdown
            if (frequency == "for one date only")
            {
                // select frequency
                var uiFrequency =
                    ngDriver.FindElement(
                        By.CssSelector("[formcontrolname='marketDuration'] option[value='3: 845280003']"));
                uiFrequency.Click();
            }

            // select 'Weekly' from the frequency dropdown
            if (frequency == "weekly")
            {
                // select frequency
                var uiFrequency =
                    ngDriver.FindElement(
                        By.CssSelector("[formcontrolname='marketDuration'] option[value='0: 845280000']"));
                uiFrequency.Click();
            }

            // select 'Bi-Weekly' from the frequency dropdown
            if (frequency == "bi-weekly")
            {
                // select frequency
                var uiFrequency =
                    ngDriver.FindElement(
                        By.CssSelector("[formcontrolname='marketDuration'] option[value='1: 845280001']"));
                uiFrequency.Click();
            }

            // select 'Monthly' from the frequency dropdown
            if (frequency == "monthly")
            {
                // select frequency
                var uiFrequency =
                    ngDriver.FindElement(
                        By.CssSelector("[formcontrolname='marketDuration'] option[value='2: 845280002']"));
                uiFrequency.Click();
            }

            // enter additional information
            var uiAdditionalInformation =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            uiAdditionalInformation.SendKeys(additionalInformation);

            // select end date
            var uiEndDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='endDate']"));
            uiEndDate1.Click();

            // click on the next button
            var uiOpenCalendarNext = ngDriver.FindElement(By.CssSelector(".mat-calendar .mat-calendar-next-button"));
            JavaScriptClick(uiOpenCalendarNext);

            if (frequency == "monthly")
            {
                // click on the next button again
                var uiOpenCalendarNext2 =
                    ngDriver.FindElement(By.CssSelector(".mat-calendar .mat-calendar-next-button"));
                JavaScriptClick(uiOpenCalendarNext2);
            }

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

            if (frequency != "for one date only")
            {
                // confirm that all days are available for selection; de-selection is required due to days per week limit
                var uiSunday = ngDriver.FindElement(By.CssSelector("#mat-checkbox-10 .mat-checkbox-inner-container"));
                JavaScriptClick(uiSunday);
                var uiSunday2 = ngDriver.FindElement(By.CssSelector("#mat-checkbox-10 .mat-checkbox-inner-container"));
                JavaScriptClick(uiSunday2);

                var uiMonday = ngDriver.FindElement(By.CssSelector("#mat-checkbox-11 .mat-checkbox-inner-container"));
                JavaScriptClick(uiMonday);
                var uiMonday2 = ngDriver.FindElement(By.CssSelector("#mat-checkbox-11 .mat-checkbox-inner-container"));
                JavaScriptClick(uiMonday2);

                var uiTuesday = ngDriver.FindElement(By.CssSelector("#mat-checkbox-12 .mat-checkbox-inner-container"));
                JavaScriptClick(uiTuesday);
                var uiTuesday2 = ngDriver.FindElement(By.CssSelector("#mat-checkbox-12 .mat-checkbox-inner-container"));
                JavaScriptClick(uiTuesday2);

                var uiWednesday =
                    ngDriver.FindElement(By.CssSelector("#mat-checkbox-13 .mat-checkbox-inner-container"));
                JavaScriptClick(uiWednesday);
                var uiWednesday2 =
                    ngDriver.FindElement(By.CssSelector("#mat-checkbox-13 .mat-checkbox-inner-container"));
                JavaScriptClick(uiWednesday2);

                var uiThursday = ngDriver.FindElement(By.CssSelector("#mat-checkbox-14 .mat-checkbox-inner-container"));
                JavaScriptClick(uiThursday);
                var uiThursday2 =
                    ngDriver.FindElement(By.CssSelector("#mat-checkbox-14 .mat-checkbox-inner-container"));
                JavaScriptClick(uiThursday2);

                var uiFriday = ngDriver.FindElement(By.CssSelector("#mat-checkbox-15 .mat-checkbox-inner-container"));
                JavaScriptClick(uiFriday);
                var uiFriday2 = ngDriver.FindElement(By.CssSelector("#mat-checkbox-15 .mat-checkbox-inner-container"));
                JavaScriptClick(uiFriday2);

                var uiSaturday = ngDriver.FindElement(By.CssSelector("#mat-checkbox-16 .mat-checkbox-inner-container"));
                JavaScriptClick(uiSaturday);
                var uiSaturday2 =
                    ngDriver.FindElement(By.CssSelector("#mat-checkbox-16 .mat-checkbox-inner-container"));
                JavaScriptClick(uiSaturday2);

                if (frequency == "weekly" || frequency == "bi-weekly")
                {
                    // make final selection re days of the week
                    var uiThursdayFinal =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-14 .mat-checkbox-inner-container"));
                    JavaScriptClick(uiThursdayFinal);

                    var uiFridayFinal =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-15 .mat-checkbox-inner-container"));
                    JavaScriptClick(uiFridayFinal);

                    var uiSaturdayFinal =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-16 .mat-checkbox-inner-container"));
                    JavaScriptClick(uiSaturdayFinal);
                }

                if (frequency == "monthly")
                {
                    // select day of the week
                    var uiSaturday3 =
                        ngDriver.FindElement(By.CssSelector("#mat-checkbox-16 .mat-checkbox-inner-container"));
                    JavaScriptClick(uiSaturday3);

                    // select first week of the month
                    var uiWeekOfMonth1 =
                        ngDriver.FindElement(
                            By.CssSelector("[formcontrolname='weekOfMonth'] [for='mat-radio-2-input']"));
                    JavaScriptClick(uiWeekOfMonth1);

                    // select second week of the month
                    var uiWeekOfMonth2 =
                        ngDriver.FindElement(
                            By.CssSelector("[formcontrolname='weekOfMonth'] [for='mat-radio-3-input']"));
                    JavaScriptClick(uiWeekOfMonth2);

                    // select third week of the month
                    var uiWeekOfMonth3 =
                        ngDriver.FindElement(
                            By.CssSelector("[formcontrolname='weekOfMonth'] [for='mat-radio-4-input']"));
                    JavaScriptClick(uiWeekOfMonth3);

                    // select fourth week of the month
                    var uiWeekOfMonth4 =
                        ngDriver.FindElement(
                            By.CssSelector("[formcontrolname='weekOfMonth'] [for='mat-radio-5-input']"));
                    JavaScriptClick(uiWeekOfMonth4);
                }
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

            // select agreement checkbox
            var uiAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            uiAgreement.Click();

            // select serving it right/minors checkbox
            var uiServingItRight =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isAllStaffServingitRight']"));
            uiServingItRight.Click();

            // select sample sizes checkbox
            var uiSampleSizes =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isSampleSizeCompliant']"));
            uiSampleSizes.Click();
        }


        [And(@"I click on the market event save button")]
        public void MarketEventsSave()
        {
            var uiSaveForLater = ngDriver.FindElement(By.CssSelector(".btn-primary:nth-child(1)"));
            uiSaveForLater.Click();
        }


        [And(@"I click on the event history for markets")]
        public void MarketEventsHistory()
        {
            var uiExpandEventHistory =
                ngDriver.FindElement(
                    By.CssSelector("mat-expansion-panel-header[role='button'] #expand-history-button-0"));
            uiExpandEventHistory.Click();
        }


        [And(@"the market event data is correct for (.*)")]
        public void MarketEventDataCorrect(string frequency)
        {
            // confirm preventing sale of liquor checkbox is selected
            var uiPreventingSaleOfLiquor =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isNoPreventingSaleofLiquor']"));
            Assert.Contains("mat-checkbox-checked", uiPreventingSaleOfLiquor.GetAttribute("class"));

            // confirm market managed or carried checkbox is selected
            var uiMarketManagedOrCarried =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketManagedorCarried']"));
            Assert.Contains("mat-checkbox-checked", uiMarketManagedOrCarried.GetAttribute("class"));

            // confirm market only vendors checkbox is selected
            var uiIsMarketOnlyVendors =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketOnlyVendors']"));
            Assert.Contains("mat-checkbox-checked", uiIsMarketOnlyVendors.GetAttribute("class"));

            // confirm six vendors checkbox is selected
            var uiIsMarketHostsSixVendors =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketHostsSixVendors']"));
            Assert.Contains("mat-checkbox-checked", uiIsMarketHostsSixVendors.GetAttribute("class"));

            // confirm max amount or duration checkbox is selected
            var uiIsMarketMaxAmountorDuration =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isMarketMaxAmountorDuration']"));
            Assert.Contains("mat-checkbox-checked", uiIsMarketMaxAmountorDuration.GetAttribute("class"));

            // confirm contact name is correct
            var uiContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            Assert.True(uiContactName.GetAttribute("value") == "Test Automation");

            // confirm contact phone number is correct
            var uiContactPhoneNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'contactPhone']"));
            Assert.True(uiContactPhoneNumber.GetAttribute("value") == "(222) 222-2222");

            // confirm contact email is correct
            var uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'contactEmail']"));
            Assert.True(uiContactEmail.GetAttribute("value") == "test@automation.com");

            // confirm market name is correct
            var uiMarketName = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'marketName']"));
            Assert.True(uiMarketName.GetAttribute("value") == "Point Ellis Market");

            // confirm market website is correct
            var uiMarketWebsite = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'marketWebsite']"));
            Assert.True(uiMarketWebsite.GetAttribute("value") == "http://www.pointellismarketisamazing.com");

            // confirm business legal name is correct
            var uiClientHostname = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'clientHostname']"));
            Assert.True(uiClientHostname.GetAttribute("value") == "Point Ellis Market Cooperative");

            // confirm market event type is correct
            var uiMarketEventType = ngDriver.FindElement(By.CssSelector("[formcontrolname= 'marketEventType']"));
            Assert.True(uiMarketEventType.GetAttribute("value") == "2: 845280002");

            // confirm market business number is correct
            var uiBusinessNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'businessNumber']"));
            Assert.True(uiBusinessNumber.GetAttribute("value") == "2222222222222222");

            // confirm incorporation/registration number is correct
            var uiRegistrationNumber =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'registrationNumber']"));
            Assert.True(uiRegistrationNumber.GetAttribute("value") == "1234567");

            // confirm address 1 is correct
            var uiStreet1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'street1']"));
            Assert.True(uiStreet1.GetAttribute("value") == "645 Tyee Road");

            // confirm address 2 is correct
            var uiStreet2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'street2']"));
            Assert.True(uiStreet2.GetAttribute("value") == "West");

            // confirm city is correct
            var uiCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'city']"));
            Assert.True(uiCity.GetAttribute("value") == "Victoria");

            // confirm postal code is correct
            var uiPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'postalCode']"));
            Assert.True(uiPostalCode.GetAttribute("value") == "V9A 6X5");

            // confirm additional details are correct
            var uiAdditionalDetails =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='venueDescription']"));
            Assert.True(uiAdditionalDetails.GetAttribute("value") == "Additional details for automated test.");

            if (frequency == "a one day event" || frequency == "one day event saved for later")
            {
                var uiFrequency = ngDriver.FindElement(By.CssSelector("[formcontrolname='marketDuration']"));
                Assert.True(uiFrequency.GetAttribute("value") == "3: 845280003");
            }

            if (frequency == "a weekly event" || frequency == "a weekly event saved for later")
            {
                var uiFrequency = ngDriver.FindElement(By.CssSelector("[formcontrolname='marketDuration']"));
                Assert.True(uiFrequency.GetAttribute("value") == "0: 845280000");
            }

            if (frequency == "a monthly event" || frequency == "a monthly event saved for later")
            {
                var uiFrequency = ngDriver.FindElement(By.CssSelector("[formcontrolname='marketDuration']"));
                Assert.True(uiFrequency.GetAttribute("value") == "2: 845280002");
            }

            if (frequency == "a bi-weekly event" || frequency == "a bi-weekly event saved for later")
            {
                var uiFrequency = ngDriver.FindElement(By.CssSelector("[formcontrolname='marketDuration']"));
                Assert.True(uiFrequency.GetAttribute("value") == "1: 845280001");
            }

            // confirm additional information is correct
            var uiAdditionalInformation =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            Assert.True(uiAdditionalInformation.GetAttribute("value") == "Additional information for automated test.");

            if (frequency != "a one day event" || frequency != "a one day event saved for later")
            {
                if (frequency == "a weekly event" || frequency == "a weekly event saved for later" ||
                    frequency == "a bi-weekly event" || frequency == "a bi-weekly event saved for later")
                {
                    // confirm selection re days of the week
                    var uiThursdayFinal =
                        ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='thursday']"));
                    Assert.Contains("mat-checkbox-checked", uiThursdayFinal.GetAttribute("class"));

                    var uiFridayFinal = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='friday']"));
                    Assert.Contains("mat-checkbox-checked", uiFridayFinal.GetAttribute("class"));

                    var uiSaturdayFinal =
                        ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='saturday']"));
                    Assert.Contains("mat-checkbox-checked", uiSaturdayFinal.GetAttribute("class"));
                }

                if (frequency == "a monthly event" || frequency == "a monthly event saved for later")
                {
                    // confirm selected day of the week
                    var uiSaturday3 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='saturday']"));
                    Assert.Contains("mat-checkbox-checked", uiSaturday3.GetAttribute("class"));

                    // confirm selected week of the month
                    var uiWeekOfMonth4 =
                        ngDriver.FindElement(
                            By.CssSelector("[formcontrolname='weekOfMonth'] mat-radio-button#mat-radio-10"));
                    Assert.Contains("mat-radio-checked", uiWeekOfMonth4.GetAttribute("class"));
                }

                if (frequency == "an approved monthly event")
                {
                    // confirm selected day of the week
                    var uiSaturday3 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='saturday']"));
                    Assert.Contains("mat-checkbox-checked", uiSaturday3.GetAttribute("class"));

                    // confirm selected week of the month
                    var uiWeekOfMonth4 =
                        ngDriver.FindElement(
                            By.CssSelector("[formcontrolname='weekOfMonth'] mat-radio-button#mat-radio-15"));
                    Assert.Contains("mat-radio-checked", uiWeekOfMonth4.GetAttribute("class"));
                }
            }

            // confirm event start hour
            var uiEventStartHour =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='startTime'] input[aria-label='Hours']"));
            Assert.True(uiEventStartHour.GetAttribute("value") == "07");

            // confirm event start minute
            var uiEventStartMinute =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='startTime'] input[aria-label='Minutes']"));
            Assert.True(uiEventStartMinute.GetAttribute("value") == "59");

            // confirm event end hour
            var uiEventEndHour =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='endTime'] input[aria-label='Hours']"));
            Assert.True(uiEventEndHour.GetAttribute("value") == "09");

            // confirm event end minute
            var uiEventEndMinute =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='endTime'] input[aria-label='Minutes']"));
            Assert.True(uiEventEndMinute.GetAttribute("value") == "59");

            // confirm liquor sale start hour
            var uiLiquorStartHour =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorStartTime'] input[aria-label='Hours']"));
            Assert.True(uiLiquorStartHour.GetAttribute("value") == "10");

            // confirm liquor sale start minute
            var uiLiquorStartMinute =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorStartTime'] input[aria-label='Minutes']"));
            Assert.True(uiLiquorStartMinute.GetAttribute("value") == "01");

            // confirm liquor sale end hour
            var uiLiquorEndHour1 =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorEndTime'] input[aria-label='Hours']"));
            Assert.True(uiLiquorEndHour1.GetAttribute("value") == "08");

            // confirm liquor sale end minute
            var uiLiquorEndMinute1 =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='liquorEndTime'] input[aria-label='Minutes']"));
            Assert.True(uiLiquorEndMinute1.GetAttribute("value") == "58");

            // confirm serving it right/minors checkbox is selected
            var uiServingItRight =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isAllStaffServingitRight']"));
            Assert.Contains("mat-checkbox-checked", uiServingItRight.GetAttribute("class"));

            // confirm sample sizes checkbox is selected
            var uiSampleSizes =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isSampleSizeCompliant']"));
            Assert.Contains("mat-checkbox-checked", uiSampleSizes.GetAttribute("class"));

            if (frequency == "a one day event" || frequency == "a weekly event" || frequency == "a bi-weekly event" ||
                frequency == "a monthly event")
            {
                // confirm agreement checkbox is selected
                var uiAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
                Assert.Contains("mat-checkbox-checked", uiAgreement.GetAttribute("class"));
            }

            // confirm that LCSD-4211 error is no longer happening
            Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'Please enter the start date'))]"))
                .Displayed);
        }
    }
}