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
        [And(@"I request a market event")]
        public void MarketEvents()
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

            // select frequency
            NgWebElement uiFrequency = ngDriver.FindElement(By.CssSelector("[formcontrolname='marketDuration'] option[value='2: 845280002']"));
            uiFrequency.Click();

            // enter additional information
            NgWebElement uiAdditionalInformation = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            uiAdditionalInformation.SendKeys(additionalInformation);

            // select start date
            NgWebElement uiStartDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='startDate']"));
            uiStartDate1.Click();

            NgWebElement uiStartDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            uiStartDate2.Click();
            
            // select end date
            NgWebElement uiEndDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='startDate']"));
            uiEndDate1.Click();

            NgWebElement uiEndDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            uiEndDate2.Click();

            // select day of the week
            NgWebElement uiDayOfWeek = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='sunday']"));
            uiDayOfWeek.Click();

            // select week of the month
            NgWebElement uiWeekOfMonth = ngDriver.FindElement(By.CssSelector("[formcontrolname='weekOfMonth'] #mat-radio-2[tabindex='-1']"));
            uiWeekOfMonth.Click();

            // enter event start hour
            NgWebElement uiEventStartHour = ngDriver.FindElement(By.CssSelector(".time-picker-title+ .col-md-2 .ngb-tp-hour .form-control"));
            uiEventStartHour.SendKeys(eventStartHour);

            // enter event start minute
            NgWebElement uiEventStartMinute = ngDriver.FindElement(By.CssSelector(".time-picker-title+ .col-md-2 .ngb-tp-minute .form-control"));
            uiEventStartMinute.SendKeys(eventStartMinute);

            // enter event end hour
            NgWebElement uiEventEndHour = ngDriver.FindElement(By.CssSelector(".col-md-2:nth-child(3) .ngb-tp-hour .form-control"));
            uiEventEndHour.SendKeys(eventEndHour);

            // enter event end minute
            NgWebElement uiEventEndMinute = ngDriver.FindElement(By.CssSelector(".col-md-2:nth-child(3) .ngb-tp-minute .form-control"));
            uiEventEndMinute.SendKeys(eventEndMinute);

            // enter liquor sale start hour
            NgWebElement uiLiquorStartHour = ngDriver.FindElement(By.CssSelector(".col-md-2~ .col-md-2+ .col-md-2 .ng-dirty .ngb-tp-hour .form-control"));
            uiLiquorStartHour.SendKeys(liquorStartHour);

            // enter liquor sale start minute
            NgWebElement uiLiquorStartMinute = ngDriver.FindElement(By.CssSelector(".col-md-2~ .col-md-2+ .col-md-2 .ng-dirty .ngb-tp-minute .form-control"));
            uiLiquorStartMinute.SendKeys(liquorStartMinute);

            // enter liquor sale end hour
            NgWebElement uiLiquorEndHour = ngDriver.FindElement(By.CssSelector(".col-md-2:nth-child(5) .ngb-tp-hour .form-control"));
            uiLiquorEndHour.SendKeys(liquorEndHour);

            // enter liquor sale end minute
            NgWebElement uiLiquorEndMinute = ngDriver.FindElement(By.CssSelector(".col-md-2:nth-child(5) .ngb-tp-minute .form-control"));
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
            ClickOnSubmitButton();

            MakePayment();
        }
    }
}
