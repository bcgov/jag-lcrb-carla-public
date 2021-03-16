using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: UBrewUVinAddLicenseeRepresentative
    As a logged in business user
    I want to request a licensee representative for a UBrew / UVin licence

@ubrewuvinlicencedownload @privatecorporation 
Scenario: UBrew / UVin Application Licensee Representative (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a private corporation
    And I complete the UBrew / UVin application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I request a licensee representative
    And the account is deleted
    Then I see the login page

*/

namespace bdd_tests
{
    [FeatureFile("./UBrewUVinAddLicenseeRepresentative.feature")]
    [Collection("Liquor")]
    public sealed class UBrewUVinAddLicenseeRepresentative : TestBase
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