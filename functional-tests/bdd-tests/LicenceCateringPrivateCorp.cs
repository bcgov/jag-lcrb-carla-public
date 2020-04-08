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
Feature: LicenceCateringPrivateCorp
    As a logged in business user
    I want to pay the first year catering licence fee
    And complete the available application types

Scenario: Pay First Year Catering Licence and Complete Applications
    #Given the Catering application has been approved
    #And I am logged in to the dashboard as a private corporation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And I pay the licensing fee
    And I request an event authorization
    And I request a valid store name or branding change
    And I request a store relocation
    And I request a third party operator
    And I request a transfer of ownership
    Then the requested applications are visible on the dashboard
*/

namespace bdd_tests
{
    [FeatureFile("./LicenceCateringPrivateCorp.feature")]
    public sealed class LicenceCateringPrivateCorp : TestBase
    {
        [Given(@"the Catering application has been approved")]
        public void catering_application_approved()
        {
        }

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

        [And(@"I pay the licensing fee")]
        public void click_pay_first_year_licensing_fee()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */
            
            MakePayment();

        }

        [And(@"I request an event authorization")]
        public void request_event_authorization()
        {          
        }

        [And(@"I request a valid store name or branding change")]
        public void name_branding_change()
        {
        }

        [And(@"I request a store relocation")]
        public void request_store_relocation()
        {
        }
   
        [And(@"I request a third party operator")]
        public void request_third_party_operator()
        {
        }


        [And(@"I request a transfer of ownership")]
        public void request_transfer_of_ownership()
        {
        }
 

        [Then(@"the requested applications are visible on the dashboard")]
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