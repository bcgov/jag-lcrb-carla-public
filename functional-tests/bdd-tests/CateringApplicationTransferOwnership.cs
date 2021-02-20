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
 Feature: CateringApplicationTransferOwnership
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a transfer of ownership request for different business types

 @catering @privatecorporation @cateringeventtransfer
 Scenario: Catering Transfer Ownership Request (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @catering @society @cateringeventtransfer
 Scenario: Catering Transfer Ownership Request (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I complete the Catering application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @catering @partnership @cateringeventtransfer
 Scenario: Catering Transfer Ownership Request (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I complete the Catering application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

  @catering @soleproprietorship @cateringeventtransfer
  Scenario: Catering Transfer Ownership Request (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I complete the Catering application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CateringApplicationTransferOwnership.feature")]
    [Collection("Liquor")]
    public sealed class CateringApplicationTransferOwnership : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLiquorOne();

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