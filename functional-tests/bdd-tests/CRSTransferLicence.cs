using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: CrsTransferLicence
    As a logged in business user
    I want to submit a CRS Application for different business types
    And request a transfer of ownership for the approved application

@cannabis @privatecorporation @crstransferownership
Scenario: Cannabis Transfer Ownership (Private Corporation)
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
    And I request a transfer of ownership for Cannabis
    And the account is deleted
    Then I see the login page

@cannabis @society @crstransferownership
Scenario: Cannabis Transfer Ownership (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a society
    And I complete the Cannabis Retail Store application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I request a transfer of ownership for Cannabis
    And the account is deleted
    Then I see the login page

@cannabis @partnership @crstransferownership
Scenario: Cannabis Transfer Ownership (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee
    And I request a transfer of ownership for Cannabis
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @crstransferownership
Scenario: Cannabis Transfer Ownership (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee
    And I request a transfer of ownership for Cannabis
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CrsTransferLicence.feature")]
    [Collection("Cannabis")]
    public sealed class CrsTransferLicence : TestBase
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