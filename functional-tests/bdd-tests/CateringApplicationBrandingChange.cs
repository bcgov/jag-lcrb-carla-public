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
Feature: CateringApplicationBrandingChange
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a name branding change for different business types

@e2e @catering @indigenousnation @cateringbranding1
Scenario: Catering Indigenous Nation Branding Change
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for Catering
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @cateringbranding2
Scenario: Catering Partnership Branding Change
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for Catering
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @cateringbranding3
Scenario: Catering Private Corporation Branding Change
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for Catering
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @cateringbranding4
Scenario: Catering Public Corporation Branding Change
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for Catering
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @society @cateringbranding5
Scenario: Catering Society Branding Change
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for Catering
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @cateringbranding6
Scenario: Catering Sole Proprietorship Branding Change
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for Catering
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @validation
Scenario: Validation for Catering Branding Change 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for Catering
    And I click on the branding change link for Catering
    And I click on the Continue to Application button
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Branding Change application
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CateringApplicationBrandingChange.feature")]
    public sealed class CateringApplicationBrandingChange : TestBase
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