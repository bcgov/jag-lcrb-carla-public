using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: ManufacturerChangeToHoursOfLiquorService
    As a logged in business user
    I want to update the liquor hours of service for lounge areas and special events

@manufacturer @changehours @release3
Scenario: Lounge Area Within Service Hours (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I click on the Licences tab
    And I complete the change hours application for a lounge area within service hours
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

@manufacturer @changehours @release3
Scenario: Lounge Area Outside Service Hours (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the Licences tab
    And I complete the change hours application for a lounge area outside of service hours
    And I click on the LG Submit button
    And the account is deleted
    Then I see the login page

@manufacturer @changehours @release3
Scenario: Special Event Area Within Service Hours (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I click on the Licences tab
    And I complete the change hours application for a special event area within service hours
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

@manufacturer @changehours @release3
Scenario: Special Event Area Outside Service Hours (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the Licences tab
    And I complete the change hours application for a special event area outside of service hours
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ManufacturerChangeToHoursOfLiquorService.feature")]
    [Collection("Liquor")]
    public sealed class ManufacturerChangeToHoursOfLiquorService : TestBase
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