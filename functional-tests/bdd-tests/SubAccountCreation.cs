using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: SubAccountCreation
    As a logged in business user
    I want to confirm that I am a sub-account user with correct account details

@cannabis @privatecorporation @subaccount
Scenario: Sub-Account Log In (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I review the account profile for a private corporation
    And I log in as the sub-account user
    And I click on the link for Edit Account Profile
    And the user details for sub-account user is displayed
    And the sub-account is deleted
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./SubAccountCreation.feature")]
    [Collection("Cannabis")]
    public sealed class SubAccountCreation : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

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