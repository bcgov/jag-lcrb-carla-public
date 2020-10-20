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
Feature: CRSRelease
    As a logged in business user
    I want to confirm that the CRS functionality is ready for release

@validation @privatecorporation @release
Scenario: Check CRS Release status
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I click on the Licences tab
    And I click on the button for CRS terms and conditions
    And the correct terms and conditions are displayed for CRS
    And I request a valid store name or branding change for Cannabis
    And I click on the Licences tab
    And I click on the link for Download Licence
    And I show the store as open on the map
    And I review the federal reports
    And I click on the Licences tab
    And the expiry date is changed to today
    And I renew the licence with negative responses
    And I click on the link for Dashboard
    And I request a personnel name change for a private corporation
    And I confirm the correct personnel name change fee for a Cannabis licence
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And I click on the Licences tab
    And I request a store relocation for Cannabis
    And I request a structural change
    And I request a transfer of ownership
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And I confirm the structural change request is displayed on the dashboard
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CRSRelease.feature")]
    [Collection("Liquor")]
    public sealed class CRSRelease : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLicenseeChanges();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
