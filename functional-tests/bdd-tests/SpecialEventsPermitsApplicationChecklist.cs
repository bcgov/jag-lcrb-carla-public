using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: SpecialEventsPermitsApplicationChecklist
    As a logged in business user
    I want to view the application checklist for Special Events Permits

Scenario: SEP Application Checklist (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for See Checklist 
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./SpecialEventsPermitsApplicationChecklist.feature")]
    [Collection("Liquor")]
    public sealed class SpecialEventsPermitsApplicationChecklist : TestBase
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