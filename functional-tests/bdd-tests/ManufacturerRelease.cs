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
Feature: ManufacturerRelease
    As a logged in business user
    I want to confirm that the Manufacturer functionality is ready for release

@manufacturer @winery @release 
Scenario: Private Corporation Manufacturer Release 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Manufacturing
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And I click on the Licences tab
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for a winery
    And I click on the Licences tab
    And I request a facility structural change
    And I click on the Licences tab
    And I request a location change
    And I click on the Licences tab
    And I request a lounge area endorsement
    And I click on the Licences tab
    And I request an on-site store endorsement
    And I click on the Licences tab
    And I request a picnic area endorsement
    And I click on the Licences tab
    And I request a special event area endorsement
    And I click on the Licences tab
    And I request structural alterations to an approved lounge or special events area
    And I click on the Licences tab
    And I complete the change hours application for a lounge area within service hours
    And I click on the link for Manage Off-Site Storage
    And I complete the offsite storage application
    And I request a third party operator
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@manufacturer @winery
Scenario: Sole Proprietorship Manufacturer Release 
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Manufacturing
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And I click on the Licences tab
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for a winery
    And I click on the Licences tab
    And I request a facility structural change
    And I click on the Licences tab
    And I request a location change
    And I click on the Licences tab
    And I request a lounge area endorsement
    And I click on the Licences tab
    And I request an on-site store endorsement
    And I click on the Licences tab
    And I request a picnic area endorsement
    And I click on the Licences tab
    And I request a special event area endorsement
    And I click on the Licences tab
    And I request structural alterations to an approved lounge or special events area
    And I click on the Licences tab
    And I complete the change hours application for a lounge area within service hours
    And I click on the link for Manage Off-Site Storage
    And I complete the offsite storage application
    And I request a third party operator
    And I request a transfer of ownership
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
