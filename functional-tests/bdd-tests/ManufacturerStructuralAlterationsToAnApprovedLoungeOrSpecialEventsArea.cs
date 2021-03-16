using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: ManufacturerStructuralAlterationsToAnApprovedLoungeOrSpecialEventsArea
    As a logged in business user
    I want to request structural alterations to an approved lounge or special events area for a manufacturer licence

@manufacturer @winery @structuralalterations
Scenario: Structural Alterations (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I request structural alterations to an approved lounge or special events area
    And the account is deleted
    Then I see the login page

@manufacturer @brewery @structuralalterations
Scenario: Structural Alterations (Brewery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I request structural alterations to an approved lounge or special events area
    And the account is deleted
    Then I see the login page

@manufacturer @distillery @structuralalterations
Scenario: Structural Alterations (Distillery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I request structural alterations to an approved lounge or special events area
    And the account is deleted
    Then I see the login page

@manufacturer @copacker @structuralalterations
Scenario: Structural Alterations (Co-packer)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I request structural alterations to an approved lounge or special events area
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ManufacturerStructuralAlterationsToAnApprovedLoungeOrSpecialEventsArea.feature")]
    [Collection("Liquor")]
    public sealed class ManufacturerStructuralAlterationsToAnApprovedLoungeOrSpecialEventsArea : TestBase
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