using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: ManufacturerTemporaryChangeToHoursOfSale
    As a logged in business user
    I want to request a Manufacturer licence temporary change to hours of sale 

@manufacturer @privatecorporation
Scenario: Manufacturer Temp Change to Hours of Sale (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the link for Temporary Change to Hours of Sale
    And I click on the Continue to Application button
    And I request a temporary change to hours of sale
    And I click on the Submit button
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ManufacturerTemporaryChangeToHoursOfSale.feature")]
    [Collection("Liquor")]
    public sealed class ManufacturerTemporaryChangeToHoursOfSale : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLiquorOne();

            CheckFeatureFlagsLiquorTwo();

            CheckFeatureFlagsLiquorThree();

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