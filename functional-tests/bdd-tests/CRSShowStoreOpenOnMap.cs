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
Feature: CRS_show_CRS_open_on_map
    As a logged in business user
    I want to show the Cannabis Retail Store as open on the map 

Scenario: Show CRS as Open on Map
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And I select the Show Store as Open on Map checkbox
    And I click on the Show Store as Open on Map link
    And I click on the maps page
    And I search for my store
    Then my store is shown as open
*/

namespace bdd_tests
{
    [FeatureFile("./CRS_show_CRS_open_on_map.feature")]
    public sealed class CRSShowStoreOpenOnMap : TestBase
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

        [And(@"I select the Show Store as Open on Map checkbox")]
        public void select_store_open_checkbox()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */
        }

        [And(@"I click on the Show Store as Open on Map link")]
        public void click_store_open_link()
        {
            /* 
            Page Title: 
            */
        }

        [And(@"I click on the maps page")]
        public void click_on_maps_page()
        {
            /* 
            Page Title: 
            */
        }

        [And(@"I search for my store")]
        public void search_for_store()
        {
        }

        [Then(@"my store is shown as open")]
        public void store_is_open()
        {
            /* 
            Page Title:
            */

            //Assert.True (ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }
    }
}
