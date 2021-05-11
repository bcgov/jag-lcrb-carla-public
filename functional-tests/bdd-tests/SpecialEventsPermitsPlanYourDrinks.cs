using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: SpecialEventsPermitsPlanYourDrinks
    As a logged in business user
    I want to plan my drinks for my Special Events Permits applications

Scenario: SEP Plan Your Drinks (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for Plan Your Drinks
    And the Plan Your Drinks label is displayed
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./SpecialEventsPermitsPlanYourDrinks.feature")]
    [Collection("Liquor")]
    public sealed class SpecialEventsPermitsPlanYourDrinks : TestBase
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