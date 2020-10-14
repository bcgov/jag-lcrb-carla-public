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
Feature: CRSApplicationStoreRelocation
    As a logged in business user
    I want to submit a CRS Application for different business types
    And request a store relocation for the approved application

@e2e @cannabis @indigenousnation @crsstorerelocationIN
Scenario: Indigenous Nation Cannabis Store Relocation
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I request a store relocation for Cannabis
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @crsstorerelocationpartnership
Scenario: Partnership Cannabis Store Relocation
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I request a store relocation for Cannabis
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @crsstorerelocationprivcorp
Scenario: Private Corporation Cannabis Store Relocation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I request a store relocation for Cannabis
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @crsstorerelocationpubcorp
Scenario: Public Corporation Cannabis Store Relocation
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I request a store relocation for Cannabis
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @crsstorerelocationsociety
Scenario: Society Cannabis Store Relocation
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I request a store relocation for Cannabis
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @crsstorerelocationsoleprop
Scenario: Sole Proprietorship Cannabis Store Relocation
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I request a store relocation for Cannabis
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CRSApplicationStoreRelocation.feature")]
    [Collection("Cannabis")]
    public sealed class CRSApplicationStoreRelocation : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLGIN();

            CheckFeatureFlagsIN();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureFlagsSecurityScreening();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
