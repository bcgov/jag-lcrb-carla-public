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
Feature: Catering_transfer_ownership
    As a logged in business user
    I want to transfer the Catering ownership

Scenario: Transfer Catering Ownership
    Given the Catering application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And I click on the Transfer Ownership link
    And I identify the proposed licensee
    And I complete the consent and declarations
    And I submit the transfer
    And I click on the Licences tab
    Then the transfer application is displayed
*/

namespace bdd_tests
{
    [FeatureFile("./Catering_transfer_ownership.feature")]
    public sealed class CateringLicenceTransferOwnership : TestBase
    {
        [Given(@"the Catering application has been approved")]
        public void catering_application_approved()
        {
        }

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
        public void click_on_transfer_ownership_link()
        {
            /* 
            Page Title: 
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
        public void complete_consent_and_declarations()
        {
            /* 
            Page Title: 
            */
        }

        [And(@"I submit the transfer")]
        public void submit_transfer()
        {
            /* 
            Page Title: 
            */
        }

        [Then(@"the transfer application is displayed")]
        public void transfer_application_displayed()
        {
            /* 
            Page Title: 
            */
        }
    }
}