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
Feature: CRSApplicationPersonnelEmailChanges
    As a logged in business user
    I want to submit a CRS Application for different business types
    And request personnel email changes for the approved application

@cannabis @partnership @crsemail
Scenario: UAT CRS Personnel Email Changes (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Dashboard
    And I change a personnel email address for a partnership
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @crsemail
Scenario: UAT CRS Personnel Email Changes (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I change a personnel email address for a private corporation
    And the account is deleted
    Then I see the login page

@cannabis @publiccorporation @crsemail
Scenario: UAT CRS Personnel Email Changes (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Dashboard
    And I change a personnel email address for a public corporation
    And the account is deleted
    Then I see the login page

@cannabis @society @crsemail
Scenario: UAT CRS Personnel Email Changes (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Dashboard
    And I change a personnel email address for a society
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @crsemail
Scenario: UAT CRS Personnel Email Changes (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I change a personnel email address for a sole proprietorship
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CRSApplicationPersonnelEmailChanges_archived.feature")]
    [Collection("Cannabis")]
    public sealed class _archive_CRSApplicationPersonnelEmailChanges : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLGIN();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureFlagsSecurityScreening();

            // CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
