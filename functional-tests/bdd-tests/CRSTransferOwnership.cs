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
Feature: CRS_transfer_ownership
    As a logged in business user
    I would like to transfer the Cannabis Retail Store ownership

Scenario: Transfer CRS Ownership
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Transfer Ownership link
    And I identify the proposed licensee
    And I complete the consent and declarations
    And I submit the transfer
    And I return to the dashboard
    Then the transfer application is displayed
*/

namespace bdd_tests
{
    [FeatureFile("./CRS_transfer_ownership.feature")]
    public sealed class CRSTransferOwnership : TestBase
    {
        [Given(@"the CRS application has been approved")]
        public void CRS_application_is_approved()
        {       
        }

        //[Given(@"I am logged in to the dashboard as a (.*)")]
        [And(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLogin(businessType);
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

        [And(@"the licence fee has been paid")]
        public void licence_fee_paid()
        {
            /* 
            Page Title: 
            */
        }

        [And(@"I click on the Transfer Ownership link")]
        public void click_transfer_ownership_link()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */
        }

        [And(@"I identify the proposed licensee")]
        public void identify_proposed_licensee()
        {
            /* 
            Page Title: 
            */
        }

        [And(@"I complete the consent and declarations")]
        public void complete_consent_declarations()
        {
            /* 
            Page Title:
            */
        }

        [And(@"I submit the transfer")]
        public void submit_transfer()
        {
        }

        [And(@"I return to the dashboard")]
        public void return_to_dashboard()
        {
            /* 
            Page Title: Payment Approved
            */

            string retDash = "Return to Dashboard";

            // click on the Return to Dashboard link
            NgWebElement returnDash = ngDriver.FindElement(By.LinkText(retDash));
            returnDash.Click();
        }

        [Then(@"the transfer application is displayed")]
        public void transfer_application_displayed()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            //Assert.True (ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }
    }
}
