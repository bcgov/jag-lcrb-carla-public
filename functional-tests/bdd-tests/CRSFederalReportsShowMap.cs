using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: CrsFederalReportsShowMap
    As a logged in business user
    I want to submit a CRS Application for different business types
    And review the federal reports and show the store as open on the map for the approved application
    
@cannabis @crsfedreports
Scenario: Federal Reports and Show Map (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee
    And I show the store as open on the map
    And I review the federal reports
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CrsFederalReportsShowMap.feature")]
    [Collection("Cannabis")]
    public sealed class CrsFederalReportsShowMap : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsMaps();

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