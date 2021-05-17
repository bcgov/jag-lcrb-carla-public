using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: SpecialEventsPermitsApplication
    As a logged in business user
    I want to submit a Special Events Permits application

Scenario: SEP Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    # Start New Application button
    And I click on the Submit button
    # Start Application button (splash)
    And I click on the button for SEP Start Application
    And I complete the special events permits applicant info
    # Next button
    And I click on the Submit button
    And I complete the SEP eligibility form
    # Next button
    And I click on the Submit button
    And I complete the SEP event form
    # Next button
    And I click on the Submit button
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