using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: UbrewUvinEstablishmentNameChange
    As a logged in business user
    I want to submit a UBrew / UVin Licence name change application for different business types

@ubrewuvinnamechange @partnership 
Scenario: UBrew / UVin Application Establishment Name Change (Partnership)
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
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for UBrew
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@ubrewuvinnamechange @privatecorporation 
Scenario: UBrew / UVin Application Establishment Name Change (Private Corporation)
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
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for UBrew
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@ubrewuvinnamechange @publiccorporation 
Scenario: UBrew / UVin Application Establishment Name Change (Public Corporation)
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
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for UBrew
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@ubrewuvinnamechange @soleproprietorship 
Scenario: UBrew / UVin Application Establishment Name Change (Sole Proprietorship)
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
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for UBrew
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./UbrewUvinEstablishmentNameChange.feature")]
    [Collection("Liquor")]
    public sealed class UbrewUvinEstablishmentNameChange : TestBase
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