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
Feature: OnSiteEndorsementRenewal
    As a logged in business user
    I want to renew a licence that expired yesterday and has an on-site endorsement

@e2e @onsiteendorsementrenewalwinery 
Scenario: Winery On-Site Endorsement Licence Renewal
Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I click on the Dashboard tab
    And I click on the link for Pay First Year Fee
    And I enter the payment information
    And I click on the Licences tab
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @onsiteendorsementrenewalbrewery 
Scenario: Brewery On-Site Endorsement Licence Renewal
Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I click on the Dashboard tab
    And I click on the link for Pay First Year Fee
    And I enter the payment information
    And I click on the Licences tab
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @onsiteendorsementrenewaldistiller 
Scenario: Distillery On-Site Endorsement Licence Renewal
Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I click on the Dashboard tab
    And I click on the link for Pay First Year Fee
    And I enter the payment information
    And I click on the Licences tab
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @onsiteendorsementrenewalcopacker 
Scenario: Co-packer On-Site Endorsement Licence Renewal
Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I click on the Dashboard tab
    And I click on the link for Pay First Year Fee
    And I enter the payment information
    And I click on the Licences tab
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a co-packer
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./OnSiteEndorsementRenewal.feature")]
    [Collection("Liquor")]
    public sealed class OnSiteEndorsementRenewal : TestBase
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

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}