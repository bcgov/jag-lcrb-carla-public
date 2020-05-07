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
Feature: LicenceCRS_society.feature
    As a logged in business user
    I want to pay the Cannabis Retail Store Licence Fee
    And complete the available application types

Scenario: Pay CRS Licence Fee and Complete Applications
    # Given the CRS application has been approved
    # And I am logged in to the dashboard as a society
    Given I am logged in to the dashboard as a society
    And I click on the Licences tab
    And I pay the licensing fee
    And I click on the licence download link
    And I plan the store opening
    And I request a store relocation
    And I request a valid store name or branding change
    And I request a structural change
    And I review the federal reports
    And I show the store as open on the map
    And I request a transfer of ownership
    And I request a personnel name change
    And I change a personnel email address
    Then the requested applications are visible on the dashboard
*/

namespace bdd_tests
{
    [FeatureFile("./LicenceCRS_society.feature")]
    public sealed class LicenceCRSSociety : TestBase
    {
        /*[Given(@"the CRS application has been approved")]
        public void CRS_application_is_approved()
        {
        }*/

        [Given(@"I am logged in to the dashboard as a (.*)")]
        //[And(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLoginNoCheck();
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
        public void pay_licence_fee()
        {
            PayLicenceFee();
        }

        [And(@"I click on the licence download link")]
        public void click_licence_download_link()
        {
            DownloadLicence();
        }

        [And(@"I plan the store opening")]
        public void plan_store_opening()
        {
            PlanStoreOpening();
        }

        [And(@"I request a store relocation")]
        public void request_store_relocation()
        {
            RequestRelocation();
        }

        [And(@"I request a valid store name or branding change")]
        public void request_name_branding_change()
        {
            StoreNameBrandingChange();
        }

        [And(@"I request a structural change")]
        public void request_structural_change()
        {
            RequestStructuralChange();
        }

        [And(@"I review the federal reports")]
        public void review_federal_reports()
        {
            ReviewFederalReports();
        }   

        [And(@"I request a transfer of ownership")]
        public void request_ownership_transfer()
        {
            RequestTransferOwnership();
        }

        [And(@"I show the store as open on the map")]
        public void show_store_open_on_map()
        {
            ShowStoreOpen();
        }

        [And(@"I request a personnel name change")]
        public void request_personnel_name_change()
        {
            RequestPersonnelNameChange();
        }

        [And(@"I change a personnel email address")]
        public void request_personnel_email_change()
        {
            RequestPersonnelEmailChange();
        }

        [Then(@"the requested applications are visible on the dashboard")]
        public void licences_tab_updated()
        {
            RequestedApplicationsOnDashboard();
        }
    }
}
