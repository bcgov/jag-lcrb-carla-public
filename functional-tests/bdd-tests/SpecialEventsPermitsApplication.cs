using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: SpecialEventsPermitsApplication
    As a logged in business user
    I want to submit a Special Event Permit application

Scenario: SEP Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for Start New Application
    And I click on the button for Start Application
    And I complete the applicant info
    And I click on the Submit button
    And I click on the Submit button
    # To be completed
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./SpecialEventsPermitsApplication.feature")]
    [Collection("Liquor")]
    public sealed class SpecialEventsPermitsApplication : TestBase
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