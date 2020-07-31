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
Feature: ManufacturerRequestChanges.feature
    As a logged in business user
    I want to request changes for a manufacturer licence

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Transfer Licence
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Third Party Operator
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a third party operator
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Picnic Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a picnic area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery On-Site Store Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Lounge Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a lounge area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Establishment Name Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request an establishment name change 
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Facility Structural Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a facility structural change
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Location Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a location change
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Special Event Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a special event area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery New Outdoor Patio Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a new outdoor patio endorsement
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ManufacturerRequestChanges.feature")]
    public sealed class ManufacturerRequestChanges : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CarlaLogin(businessType);
        }
    }
}
