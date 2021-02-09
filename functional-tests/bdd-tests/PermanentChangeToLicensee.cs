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
Feature: PermanentChangeToLicensee
    As a logged in business user
    I want to submit a licensee changes for different business types

@catering @licenseechanges @release
Scenario: DEV Catering Licensee Changes (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Dashboard tab
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a private corporation
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

@catering @licenseechanges @release
Scenario: DEV Catering Licensee Changes (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I complete the Catering application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Dashboard tab
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a partnership
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

@catering @licenseechanges @release
Scenario: DEV Catering Licensee Changes (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I complete the Catering application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Dashboard tab
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a sole proprietorship
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

@catering @licenseechanges @release
Scenario: DEV Catering Licensee Changes (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I complete the Catering application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Dashboard tab
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a society
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

@catering @licenseechanges @release
Scenario: DEV Catering Licensee Changes (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I complete the Catering application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Dashboard tab
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a public corporation
    And I click on the Submit button
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./PermanentChangeToLicensee.feature")]
    [Collection("Cannabis")]
    public sealed class PermanentChangeToLicensee : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsMaps();

            CheckFeatureFlagsLGIN();

            CheckFeatureFlagsIN();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureFlagsSecurityScreening();

            CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
