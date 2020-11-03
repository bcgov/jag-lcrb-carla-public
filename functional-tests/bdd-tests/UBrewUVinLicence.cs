using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using Xunit;

/*
Feature: UBrewUVinLicence
    As a logged in business user
    I want to submit a UBrew / UVin Licence for different business types

#-----------------------
# Application Only
#-----------------------

@e2e @ubrewuvinapplication @partnership 
Scenario: Partnership UBrew / UVin Application
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a partnership
    And I click on the Submit button
    And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvinapplication @privatecorporation 
Scenario: Private Corporation UBrew / UVin Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a private corporation
    And I click on the Submit button
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvinapplication @publiccorporation 
Scenario: Public Corporation UBrew / UVin Application
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a public corporation
    And I click on the Submit button
    And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvinapplication @soleproprietorship 
Scenario: Sole Proprietorship UBrew / UVin Application
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a sole proprietorship
    And I click on the Submit button
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

#-----------------------
# Post-Approval
#-----------------------

@e2e @ubrewuvinapproved @partnership 
Scenario: Partnership UBrew / UVin Application Approved
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a partnership
    And I click on the Submit button
    And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    # Application approval and request change options to be confirmed
    And I request a valid store name or branding change for Catering
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvinapproved @privatecorporation 
Scenario: Private Corporation UBrew / UVin Application Approved
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a private corporation
    And I click on the Submit button
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    # Application approval and request change options to be confirmed
    And I request a valid store name or branding change for Catering
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvinapproved @publiccorporation 
Scenario: Public Corporation UBrew / UVin Application Approved
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a public corporation
    And I click on the Submit button
    And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    # Application approval and request change options to be confirmed
    And I request a valid store name or branding change for Catering
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvinapproved @soleproprietorship 
Scenario: Sole Proprietorship UBrew / UVin Application Approved
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a sole proprietorship
    And I click on the Submit button
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    # Application approval and request change options to be confirmed
    And I request a valid store name or branding change for Catering
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./UBrewUVinLicence.feature")]
    [Collection("Liquor")]
    public sealed class UBrewUVinLicence : TestBase
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

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}