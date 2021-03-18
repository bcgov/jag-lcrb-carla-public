using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: FoodPrimaryPatronParticipation
    As a logged in business user
    I want to request a Patron Participation Entertainment Endorsement Application for a Food Primary licence

@foodprimarypatronparticipation @privatecorporation
Scenario: Food Primary Patron Participation Entertainment Endorsement (Private Corporation)
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
    And I click on the link for Patron Participation Entertainment Endorsement
    And I click on the Continue to Application button
    And I request a Patron Participation Entertainment Endorsement application
    And I click on the Submit button
    And the dashboard status is updated as Pending External Review
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./FoodPrimaryPatronParticipation.feature")]
    [Collection("Liquor")]
    public sealed class FoodPrimaryPatronParticipation : TestBase
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