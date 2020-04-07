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
Feature: Catering_licence_download
    As a logged in business user
    I would like to download a catering licence

Scenario: Download Catering Licence
    Given the Catering application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Download Licence link
    And the licence is downloaded
    Then the correct information is displayed
*/

namespace bdd_tests
{
    [FeatureFile("./Catering_licence_download.feature")]
    public sealed class CateringLicenceDownload : TestBase
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

        [And(@"I click on the Download Licence link")]
        public void click_download_licence_link()
        {
            /* 
            Page Title: 
            */
        }

        [And(@"the licence is downloaded")]
        public void licence_is_downloaded()
        {
            /* 
            Page Title: 
            */
        }

        [Then(@"the correct information is displayed")]
        public void correct_info_displayed()
        {
            /* 
            Page Title: 
            Subtitle:   
            */
        }
    }
}