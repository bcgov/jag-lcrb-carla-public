using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using Xunit;

/*
Feature: WorkerApplication
    As a logged in worker applicant
    I want to submit a cannabis worker application

Scenario: Worker Application
    Given I login with no terms
    And the account is deleted
    And I am logged in to the dashboard
    And I click on my name
    And I complete Step 1 of the application
    And I complete Step 2 of the application
    And I click on the Submit & Pay button
    And I enter the payment information
    And I return to the dashboard
    And the dashboard has a new status
    And the account is deleted
    Then I see the login page

Scenario: Validation for Worker Application
    Given I login with no terms
    And the account is deleted
    And I am logged in to the dashboard
    And I click on my name
    And I do not complete Step 1 of the application
    And the Step 1 error messages are displayed
    And I complete Step 1 of the application
    And I do not complete Step 2 of the application
    And I click on the Submit & Pay button
    And the Step 2 error messages are displayed
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./WorkerApplication.feature")]
    public sealed class WorkerApplication : TestBaseWorker
    {

        [Given(@"I login with no terms")]
        public void LoginNoTerms()
        {
            CarlaLoginWorkerNoTerms();
        }

        [Given(@"I am logged in to the dashboard")]
        public void ViewDashboard()
        {
            CarlaLoginWorker();
        }

        [And(@"I am logged in to the dashboard")]
        public void ViewDashboard2()
        {
            CarlaLoginWorker();
        }

        [And(@"I click on my name")]
        public void ClickOnMyName()
        {
            /* 
            Page Title: Worker Dashboard
            */

            NgWebElement uiNameLink = ngDriver.FindElement(By.CssSelector("section section a"));
            uiNameLink.Click();
        }

        [And(@"I complete Step 1 of the application")]
        public void CompleteStep1OfApplication()
        {
            /* 
            Page Title: Worker Security Verification Application - Step 1
            */

            //string previousFirst = "First";
            //string previousMiddle = "Middle";
            //string previousLast = "Last";
            string birthCityCountry = "Victoria, Canada";
            string BCDL = "1234568";
            string BCID = "123456789";
            string primaryPhone = "2508888888";
            string email = "test@automation.com";
            string mailingStreet = "645 Tyee Road";
            string mailingCity = "Victoria";
            string mailingProvince = "BC";
            string postalCode = "V8V4Y3";

            /* - under development
            // click on link to add previous name
            NgWebElement uiPreviousNameLink = ngDriver.FindElement(By.CssSelector("div:nth-child(3) span a"));
            uiPreviousNameLink.Click();

            // enter the previous first name
            NgWebElement uiPreviousFirstName = ngDriver.FindElement(By.CssSelector("section:nth-child(2) input"));
            uiPreviousFirstName.Click();
            //uiPreviousFirstName.SendKeys(previousFirst);

            // enter the previous middle name
            NgWebElement uiPreviousMiddleName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='middlename']"));
            uiPreviousMiddleName.Click();
            //uiPreviousMiddleName.SendKeys(previousMiddle);

            // enter the previous last name
            NgWebElement uiPreviousLastName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastname']"));
            uiPreviousLastName.Click();
            //uiPreviousLastName.SendKeys(previousLast);
            */

            // enter the birth city and country
            NgWebElement uiBirthCityCountry = ngDriver.FindElement(By.CssSelector("input[formcontrolname='birthPlace']"));
            uiBirthCityCountry.SendKeys(birthCityCountry);

            // enter the BC driver's licence
            NgWebElement uiBCDL = ngDriver.FindElement(By.CssSelector("input[formcontrolname='primaryIdNumber']"));
            uiBCDL.SendKeys(BCDL);

            // enter the BCID
            NgWebElement uiBCID = ngDriver.FindElement(By.CssSelector("input[formcontrolname='secondaryIdNumber']"));
            uiBCID.SendKeys(BCID);

            // enter the primary phone number
            NgWebElement uiPrimaryPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mobilePhone']"));
            uiPrimaryPhone.SendKeys(primaryPhone);

            // enter the email address
            NgWebElement uiEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='email']"));
            uiEmail.SendKeys(email);

            // select the start date textbox (Date: From) for the current address
            NgWebElement openCalendar = ngDriver.FindElement(By.CssSelector("input[formcontrolname='fromdate']"));
            openCalendar.Click();

            // select the calendar period button                                                                    
            NgWebElement nextCalendar = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/mat-calendar-header/div/div/button/span"));
            nextCalendar.Click();

            // select the year
            NgWebElement nextCalendar2 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/div/mat-multi-year-view/table/tbody/tr[3]/td[2]/div"));
            nextCalendar2.Click();

            // select the month
            NgWebElement nextCalendar3 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/div/mat-year-view/table/tbody/tr[3]/td[3]/div"));
            nextCalendar3.Click();

            // select the day
            NgWebElement nextCalendar4 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[4]/td[3]/div"));
            nextCalendar4.Click();

            // enter the street of the mailing address
            NgWebElement uiMailingStreet = ngDriver.FindElement(By.CssSelector("input[formcontrolname='address2_line1']"));
            uiMailingStreet.SendKeys(mailingStreet);

            // enter the city of the mailing address
            NgWebElement uiMailingCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='address2_city']"));
            uiMailingCity.SendKeys(mailingCity);

            // enter the province of the mailing address
            NgWebElement uiMailingProvince = ngDriver.FindElement(By.CssSelector("input[formcontrolname='address2_stateorprovince']"));
            uiMailingProvince.SendKeys(mailingProvince);

            // enter the postal code of the mailing address
            NgWebElement uiPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='address2_postalcode']"));
            uiPostalCode.SendKeys(postalCode);

            // click on save and continue button
            NgWebElement saveAndContinueButton = ngDriver.FindElement(By.CssSelector("span button.btn-primary.btn"));
            saveAndContinueButton.Click();
        }

        [And(@"I complete Step 2 of the application")]
        public void CompleteStep2OfApplication()
        {
            /* 
            Page Title: Consent for Cannabis Security Screening - Step 2
            */

            // select consent and disclosure checkbox
            NgWebElement uiNoWetSignature1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='consentToCollection']"));
            uiNoWetSignature1.Click();
        }

        [And(@"I click on the Submit & Pay button")]
        public void ClickOnSubmitAndPay()
        {
            /* 
            Page Title: Consent for Cannabis Security Screening - Step 2
            */

            // click on the Submit and Pay button
            NgWebElement submitPayButton = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
            submitPayButton.Click();
        }

        [And(@"I enter the payment information")]
        public void EnterPaymentInfo()
        {
            /* 
            Page Title: Internet Payments Program (Bambora)
            */

            MakeWorkerPayment();
        }

        [And(@"I return to the dashboard")]
        public void ReturnToDashboard()
        {
            /* 
            Page Title: Payment Approved
            */

            string retDash = "Return to Dashboard";

            System.Threading.Thread.Sleep(3000);

            // confirm that payment receipt is for $100.00
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$100.00')]")).Displayed);

            // click on the Return to Dashboard link
            NgWebElement returnDash = ngDriver.FindElement(By.LinkText(retDash));
            returnDash.Click();
        }

        [And(@"the dashboard has a new status")]
        public void DashboardHasNewStatus()
        {
            /* 
            Page Title: Worker Dashboard
            */

            // confirm Pending Review status is displayed
            Assert.True (ngDriver.FindElement(By.XPath("//body[contains(.,'Pending Review')]")).Displayed);
        }

        [Then(@"I see the login page")]
        public void I_see_login()
        {
            /* 
            Page Title: Apply for a cannabis licence
            */

            //Assert.True(ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }

        [Then(@"I sign out")]
        public void SignOut()
        {
            CarlaDeleteCurrentAccount();
        }
    }
}
