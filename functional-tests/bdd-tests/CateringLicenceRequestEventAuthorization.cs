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
Feature: Catering_request_event_authorization
    As a logged in business user
    I would like to request a Catering event authorization

Scenario: Request Catering Event Authorization
    Given the Catering application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Request Event Authorization link
    And I submit a catered event authorization request
    And I click on the Event History form
    Then the event is displayed
*/

namespace bdd_tests
{
    [FeatureFile("./Catering_request_event_authorization.feature")]
    public sealed class CateringLicenceRequestEventAuthorization : TestBase
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

        [And(@"I click on the Request Event Authorization link")]
        public void click_request_event_authorization()
        {
            /* 
            Page Title: 
            */
        }

        [And(@"I submit a catered event authorization request")]
        public void submit_event_request()
        {
            /* 
            Page Title: 
            */
        }

        [And(@"I click on the Event History form")]
        public void click_event_history_form()
        {
            /* 
            Page Title: 
            */
        }

        [Then(@"the event is displayed")]
        public void event_is_displayed()
        {
            /* 
            Page Title: 
            */
        }

    }
}