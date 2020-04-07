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
Feature: CRS_plan_store_opening
    As a logged in business user
    I want to plan my Cannabis Retail Store opening 

Scenario: Plan CRS Opening
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Plan Store Opening link
    And I complete the planning details
    And I click on the Save button
    And I click on the Licences tab
    And I click on the Plan Store Opening link
    Then the store planning details have been saved
*/

namespace bdd_tests
{
    [FeatureFile("./CRS_plan_store_opening.feature")]
    public sealed class CRSPlanStoreOpening : TestBase
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

        [And(@"I click on the Plan Store Opening link")]
        public void click_plan_store_opening()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */
        }

        [And(@"I complete the planning details")]
        public void complete_planning_details()
        {
            /* 
            Page Title:
            */
        }

        [And(@"I click on the Save button")]
        public void click_on_save_button()
        {
            /* 
            Page Title: 
            */
        }

        [Then(@"the store planning details have been saved")]
        public void store_planning_details_saved()
        {
            /* 
            Page Title:
            */
        }
    }
}
