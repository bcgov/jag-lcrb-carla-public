using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: ManufacturerLoungeAreaEndorsement
    As a logged in business user
    I want to request lounge area endorsement for a manufacturer licence

@manufacturer @winery @loungearea
Scenario: Lounge Area Endorsement Application (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I request a lounge area endorsement
    And the account is deleted
    Then I see the login page

@manufacturer @brewery @loungearea
Scenario: Lounge Area Endorsement Application (Brewery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I request a lounge area endorsement
    And the account is deleted
    Then I see the login page

@manufacturer @distillery @loungearea
Scenario: Lounge Area Endorsement Application (Distillery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I request a lounge area endorsement
    And the account is deleted
    Then I see the login page

@manufacturer @copacker @loungearea
Scenario: Lounge Area Endorsement Application (Co-packer)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I request a lounge area endorsement
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ManufacturerLoungeAreaEndorsement.feature")]
    [Collection("Liquor")]
    public sealed class ManufacturerLoungeAreaEndorsement : TestBase
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