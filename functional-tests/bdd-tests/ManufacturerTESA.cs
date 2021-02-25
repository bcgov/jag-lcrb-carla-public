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
Feature: ManufacturerTESA
    As a logged in business user
    I want to submit TESA applications for different manufacturer types

@manufacturer @winery @tesa
Scenario: Manufacturer TESA Application (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Temporary Expanded Service Areas Application
    And I click on the Continue to Application button
    And I complete the TESA application for a Manufacturer licence
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority
    And I click on the Submit button
    And I log in as a return user
    And I click on the link for Complete Application
    # Update re LP vs Mfg
    And I review the local government response for a liquor primary licence
    And the account is deleted
    Then I see the login page

@manufacturer @brewery @tesa
Scenario: Manufacturer TESA Application (Brewery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Temporary Expanded Service Areas Application
    And I click on the Continue to Application button
    And I complete the TESA application for a Manufacturer licence
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority
    And I click on the Submit button
    And I log in as a return user
    And I click on the link for Complete Application
    # Update re LP vs Mfg
    And I review the local government response for a liquor primary licence
    And the account is deleted
    Then I see the login page

@manufacturer @distillery @tesa
Scenario: Manufacturer TESA Application (Distillery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Temporary Expanded Service Areas Application
    And I click on the Continue to Application button
    And I complete the TESA application for a Manufacturer licence
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority
    And I click on the Submit button
    And I log in as a return user
    And I click on the link for Complete Application
    # Update re LP vs Mfg
    And I review the local government response for a liquor primary licence
    And the account is deleted
    Then I see the login page

@manufacturer @copacker @tesa
Scenario: Manufacturer TESA Application (Co-packer)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Temporary Expanded Service Areas Application
    And I click on the Continue to Application button
    And I complete the TESA application for a Manufacturer licence
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority
    And I click on the Submit button
    And I log in as a return user
    And I click on the link for Complete Application
    # Update re LP vs Mfg
    And I review the local government response for a liquor primary licence
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ManufacturerTESA.feature")]
    [Collection("Liquor")]
    public sealed class ManufacturerTESA : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLiquorTwo();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureFlagsSecurityScreening();

            CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
