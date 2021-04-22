using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: SpecialEventsPermitsManageAccount
    As a logged in business user
    I want to manage my account for Special Events Permits

Scenario: Manage SEP Account (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for Edit Account Profile
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./SpecialEventsPermitsManageAccount.feature")]
    [Collection("Liquor")]
    public sealed class SpecialEventsPermitsManageAccount : TestBase
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