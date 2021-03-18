using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: FoodPrimaryRelocation
    As a logged in business user
    I want to submit a relocation request for a Food Primary licence

@foodprimaryrelocation @privatecorporation
Scenario: Food Primary Relocation Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a private corporation
    And I complete the Food Primary application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I click on the link for Relocation Application
    And I request a Food Primary relocation application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./FoodPrimaryRelocation.feature")]
    [Collection("Liquor")]
    public sealed class FoodPrimaryRelocation : TestBase
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