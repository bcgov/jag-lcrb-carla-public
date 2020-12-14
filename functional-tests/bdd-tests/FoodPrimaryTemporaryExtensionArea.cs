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
Feature: FoodPrimaryTemporaryExtensionArea
    As a logged in business user
    I want to submit a Temporary Extension of Licensed Area Application for a Food Primary licence

@foodprimarytemporaryext @partnership 
Scenario: Partnership Food Primary Temporary Extension of Licensed Area
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Food Primary
    And I review the account profile for a partnership
    And I complete the Food Primary application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I submit a temporary extension of licensed area application
    And the account is deleted
    Then I see the login page

@foodprimarytemporaryext @privatecorporation
Scenario: Private Corporation Food Primary Temporary Extension of Licensed Area
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a private corporation
    And I complete the Food Primary application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I submit a temporary extension of licensed area application
    And the account is deleted
    Then I see the login page

@foodprimarytemporaryext @publiccorporation
Scenario: Public Corporation Food Primary Temporary Extension of Licensed Area
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a public corporation
    And I complete the Food Primary application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I submit a temporary extension of licensed area application
    And the account is deleted
    Then I see the login page

@foodprimarytemporaryext @society
Scenario: Society Food Primary Temporary Extension of Licensed Area
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Food Primary
    And I review the account profile for a society
    And I complete the Food Primary application for a society
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee
    And I submit a temporary extension of licensed area application
    And the account is deleted
    Then I see the login page

@foodprimarytemporaryext @soleproprietorship
Scenario: Sole Proprietorship Food Primary Temporary Extension of Licensed Area
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Food Primary
    And I review the account profile for a sole proprietorship
    And I complete the Food Primary application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I submit a temporary extension of licensed area application
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./FoodPrimaryTemporaryExtensionArea.feature")]
    [Collection("Liquor")]
    public sealed class FoodPrimaryTemporaryExtensionArea : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLiquorOne();

            CheckFeatureFlagsLiquorTwo();

            CheckFeatureFlagsLiquorThree();

            CheckFeatureFlagsLGIN();

            CheckFeatureFlagsIN();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureFlagsSecurityScreening();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}