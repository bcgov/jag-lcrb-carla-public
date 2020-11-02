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
Feature: ManufacturerLicensingRenewal
    As a logged in business user
    I want to pay the first year licensing fee for an approved Manucturer Application
    And renew the licence

#-----------------------
# Expiry = Today
#-----------------------

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Negative Private Corporation Winery Licence Renewal
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
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Positive Private Corporation Winery Licence Renewal
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
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Negative Private Corporation Brewery Licence Renewal
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
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with negative responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Positive Private Corporation Brewery Licence Renewal
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
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with positive responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Negative Private Corporation Distillery Licence Renewal
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
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with negative responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Positive Private Corporation Distillery Licence Renewal
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
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with positive responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Negative Private Corporation Co-packer Licence Renewal
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
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with negative responses for a co-packer
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Positive Private Corporation Co-packer Licence Renewal
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
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with positive responses for a co-packer
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ManufacturerLicensingRenewal.feature")]
    [Collection("Liquor")]
    public sealed class ManufacturerLicensingRenewal : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLiquorTwo();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureFlagsSecurityScreening();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
