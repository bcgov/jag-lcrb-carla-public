using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: SpecialEventsPermitsGetCertified
    As a logged in business user
    I want to get certified my Special Events Permits applications

Scenario: SEP Get Certified (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for Get Certified
    And the Responsible Service BC webpage is displayed
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./SpecialEventsPermitsGetCertified.feature")]
    [Collection("Liquor")]
    public sealed class SpecialEventsPermitsGetCertified : TestBase
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