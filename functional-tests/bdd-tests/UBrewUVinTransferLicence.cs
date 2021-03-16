using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: UbrewUvinTransferLicence
    As a logged in business user
    I want to submit a UBrew / UVin Licence transfer for different business types

@ubrewuvintransferlicence @partnership 
Scenario: UBrew / UVin Application Transfer Licence (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a partnership
    And I complete the UBrew / UVin application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I request a transfer of ownership for UBrew / UVin
    And the account is deleted
    Then I see the login page

@ubrewuvintransferlicence @privatecorporation 
Scenario: UBrew / UVin Application Transfer Licence (Private Corporation)
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
    And I click on the Licences tab
    And I request a transfer of ownership for UBrew / UVin
    And the account is deleted
    Then I see the login page

@ubrewuvintransferlicence @publiccorporation 
Scenario: UBrew / UVin Application Transfer Licence (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a public corporation
    And I complete the UBrew / UVin application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee
    And I click on the Licences tab
    And I request a transfer of ownership for UBrew / UVin
    And the account is deleted
    Then I see the login page

@ubrewuvintransferlicence @soleproprietorship 
Scenario: UBrew / UVin Application Transfer Licence (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a sole proprietorship
    And I complete the UBrew / UVin application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I request a transfer of ownership for UBrew / UVin
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./UbrewUvinTransferLicence.feature")]
    [Collection("Liquor")]
    public sealed class UbrewUvinTransferLicence : TestBase
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