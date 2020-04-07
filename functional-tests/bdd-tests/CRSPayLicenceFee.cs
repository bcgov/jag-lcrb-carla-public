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
Feature: CRS_pay_licence_fee
    As a logged in business user
    I want to pay the Cannabis Retail Store Licence Fee

Scenario: Pay CRS Licence Fee
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And I click on the Pay Licence Fee and Plan Store Opening link
    And I enter the estimated opening date and the opening date reason
    And I click on the payment button
    And I complete the payment
    And I click on the Licences tab
    Then the Licences tab has been updated with expiry date, download link, and change jobs
*/

namespace bdd_tests
{
    [FeatureFile("./CRS_pay_licence_fee.feature")]
    public sealed class CRSPayLicenceFee : TestBase
    {
        /*[Given(@"the CRS application has been approved")]
        public void CRS_application_is_approved()
        {
        }*/

        [Given(@"I am logged in to the dashboard as a (.*)")]
        //[And(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLoginNoCheck(businessType);
        }

        [And(@"I click on the Licences tab")]
        public void click_on_licences_tab()
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }

        [And(@"I click on the Pay Licence Fee and Plan Store Opening link")]
        public void click_pay_licence_fee_store_opening()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            string licenceFee = "Pay Licence Fee and Plan Store Opening";

            // click on the pay licence fee link
            NgWebElement uiLicenceFee = ngDriver.FindElement(By.LinkText(licenceFee));
            uiLicenceFee.Click();
        }

        [And(@"I enter the estimated opening date and the opening date reason")]
        public void enter_date_and_reason()
        {
            /* 
            Page Title: Plan Your Store Opening
            */

            string reasonDay = "The store will be opened on this day for many reasons.";

            // enter the estimated opening date
            NgWebElement uiCalendar1 = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-application-and-licence-fee/div/div[2]/div[2]/section[2]/div/app-field[1]/section/div[1]/section/input"));
            uiCalendar1.Click();

            NgWebElement uiCalendar2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[2]/td[3]/div"));
            uiCalendar2.Click();

            // enter the reason for the opening date
            NgWebElement uiLicenceFee = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-application-and-licence-fee/div/div[2]/div[2]/section[2]/div/app-field[2]/section/div/section/textarea"));
            uiLicenceFee.SendKeys(reasonDay);
        }

        [And(@"I click on the payment button")]
        public void click_on_payment_button()
        {
            /* 
            Page Title: Plan Your Store Opening
            */

            NgWebElement paymentButton = ngDriver.FindElement(By.XPath("//button[contains(.,' PAY LICENCE FEE AND RECEIVE LICENCE')]"));
            paymentButton.Click();

            System.Threading.Thread.Sleep(3000);
        }

        [And(@"I complete the payment")]
        public void complete_the_payment()
        {
            MakePayment();
        }

        [Then(@"the Licences tab has been updated with expiry date, download link, and change jobs")]
        public void licences_tab_updated()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            //Assert.True (ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }
    }
}
