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
Feature: MarketEvent
    As a logged in business user
    I want to submit a market event for different manufacturer types

@e2e @privatecorporation @marketevent @winery
Scenario: Private Corporation Winery Market Event
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I request a market event
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @marketevent @distillery
Scenario: Private Corporation Distillery Market Event
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I request a market event
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @marketevent @brewery
Scenario: Private Corporation Brewery Market Event
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I request a market event
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @marketevent @copacker
Scenario: Private Corporation Co-packer Market Event
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I request a market event
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @marketevent @validation
Scenario: Validation for Market Event Application 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Manufacturing application
    And the application is approved
    And I click on the Licences tab
    And I click on the link for Request Market Event Authorization
    And I click on the Submit button
    And the expected validation errors are thrown for a market event
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./MarketEvent.feature")]
    [Collection("Liquor")]
    public sealed class MarketEvent : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLiquorTwo();

            CheckFeatureFlagsLicenseeChanges();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
