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
Feature: RuralLRSRequestRelocation
    As a logged in business user
    I want to request a relocation for a rural LRS application

@privatecorporation @ruralLRS 
Scenario: Rural LRS Request Relocation (Private Corporation)
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
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

@publiccorporation @ruralLRS 
Scenario: Rural LRS Request Relocation (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a public corporation
    And I complete the Rural LRS application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

@partnership @ruralLRS 
Scenario: Rural LRS Request Relocation (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a partnership
    And I complete the Rural LRS application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

@society @ruralLRS 
Scenario: Rural LRS Request Relocation (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a society
    And I complete the Rural LRS application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

@soleproprietorship @ruralLRS
Scenario: Rural LRS Request Relocation (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a sole proprietorship
    And I complete the Rural LRS application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./RuralLRSRequestRelocation.feature")]
    [Collection("Cannabis")]
    public sealed class RuralLRSRequestRelocation : TestBase
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
