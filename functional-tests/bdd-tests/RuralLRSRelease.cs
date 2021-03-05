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
Feature: RuralLRSRelease
    As a logged in business user
    I want to run a release test for a rural LRS application

@privatecorporation @ruralLRS @release4
Scenario: Rural LRS Release (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a private corporation
    And I complete the Rural LRS application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Download Licence
    And I request a licensee representative
    And I click on the Licences tab
    And I request a valid store name or branding change for Rural RLS
    And I click on the Licences tab
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And I click on the Licences tab
    And I click on the link for Sales to Hospitality Licensees and Special Event Permittees
    And I click on the Continue to Application button
    And I request a sales to hospitality licensees and special event permittees application
    And I click on the Submit button
    And I enter the payment information
    And I click on the Licences tab
    And I click on the link for Structural Alteration Application
    And I click on the Continue to Application button
    And I request a Rural LRS structural alteration application
    And I click on the Submit button
    And I enter the payment information
    And I click on the Licences tab
    And I request a third party operator
    And I click on the Licences tab
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./RuralLRSRelease.feature")]
    [Collection("Cannabis")]
    public sealed class RuralLRSRelease : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
