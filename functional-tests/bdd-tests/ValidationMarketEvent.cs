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
Feature: ValidationMarketEvent
    As a logged in business user
    I want to confirm the validation messages for a market event application

@privatecorporation @marketevent @validation
Scenario: Validation for Market Event Application 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee
    And I request an on-site store endorsement
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
    [FeatureFile("./ValidationMarketEvent.feature")]
    [Collection("Cannabis")]
    public sealed class ValidationMarketEvent : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LoggedInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLGIN();

            CheckFeatureFlagsIN();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureFlagsSecurityScreening();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
