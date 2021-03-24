using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: RuralLicenseeRetailStoreTermsAndConditions
    As a logged in business user
    I want to confirm the Terms and Conditions for a Rural LRS licence

@rurallrs @privatecorporation
Scenario: Rural LRS Terms and Conditions (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a private corporation
    And I complete the Rural LRS application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    And I confirm the terms and conditions for a Rural LRS licence
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./RuralLicenseeRetailStoreTermsAndConditions.feature")]
    [Collection("Liquor")]
    public sealed class RuralLicenseeRetailStoreTermsAndConditions : TestBase
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