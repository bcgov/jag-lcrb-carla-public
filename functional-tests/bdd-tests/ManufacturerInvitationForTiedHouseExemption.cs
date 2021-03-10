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
Feature: ManufacturerInvitationForTiedHouseExemption
    As a logged in business user
    I want to submit a tied house exemption for different manufacturer types

@manufacturer @winery @mfglicencetiedhouse
Scenario: Manufacturer Tied House Exemption (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Invitation for Tied House Exemption
    And I complete the tied house exemption request
    And I click on the secondary Submit button
    And the account is deleted
    Then I see the login page

@manufacturer @brewery @mfglicencetiedhouse
Scenario: Manufacturer Tied House Exemption (Brewery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Invitation for Tied House Exemption
    And I complete the tied house exemption request
    And I click on the secondary Submit button
    And the account is deleted
    Then I see the login page

@manufacturer @distillery @mfglicencetiedhouse
Scenario: Manufacturer Tied House Exemption (Distillery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Invitation for Tied House Exemption
    And I complete the tied house exemption request
    And I click on the secondary Submit button
    And the account is deleted
    Then I see the login page

@manufacturer @copacker @mfglicencetiedhouse
Scenario: Manufacturer Tied House Exemption (Co-packer)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Invitation for Tied House Exemption
    And I complete the tied house exemption request
    And I click on the secondary Submit button
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ManufacturerInvitationForTiedHouseExemption.feature")]
    [Collection("Liquor")]
    public sealed class ManufacturerInvitationForTiedHouseExemption : TestBase
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
