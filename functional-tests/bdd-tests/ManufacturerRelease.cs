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
Feature: CateringRelease
    As a logged in business user
    I want to confirm that the Catering functionality is ready for release

@validation @privatecorporation @release
Scenario: Private Corporation Catering Release
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for Catering
    And I click on the button for Catering terms and conditions
    And the correct terms and conditions are displayed for Catering
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Catering
    And I click on the Licences tab
    And I request a licensee representative
    And I click on the link for Dashboard
    And I request a personnel name change for a private corporation
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And I click on the Licences tab
    And I request a store relocation for Catering
    And I click on the Licences tab
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    # And I click on the Licences tab
    # And I request a transfer of ownership
    And the expiry date is changed to today
    And I renew the licence with positive responses for Catering
    And I click on the Licences tab
    And I request a third party operator
    And the account is deleted
    Then I see the login page

@validation @soleproprietorship @release
Scenario: Sole Proprietorship Catering Release
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for Catering
    And I click on the button for Catering terms and conditions
    And the correct terms and conditions are displayed for Catering
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Catering
    And I click on the Licences tab
    And I request a licensee representative
    And I click on the link for Dashboard
    And I request a personnel name change for a sole proprietorship
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And I click on the Licences tab
    And I request a store relocation for Catering
    And I click on the Licences tab
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    # And I click on the Licences tab
    # And I request a transfer of ownership
    And the expiry date is changed to today
    And I renew the licence with positive responses for Catering
    And I click on the Licences tab
    And I request a third party operator
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ManufacturerRelease.feature")]
    [Collection("Liquor")]
    public sealed class ManufacturerRelease : TestBase
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
