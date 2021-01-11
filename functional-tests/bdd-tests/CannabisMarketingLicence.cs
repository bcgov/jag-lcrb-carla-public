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
Feature: CannabisMarketingLicence
    As a logged in business user
    I want to submit a Cannabis Marketing Licence for different business types

@cannabismktg @indigenousnation 
Scenario: Cannabis Marketing Application (Indigenous Nation)
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Marketing application for an indigenous nation
    And I click on the Submit button
    And I review the security screening requirements for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Marketing Licence
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@cannabismktg @partnership 
Scenario: Cannabis Marketing Application (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Marketing application for a partnership
    And I click on the Submit button
    # And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application for Cannabis Marketing
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Marketing Licence
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@cannabismktg @privatecorporation
Scenario: Cannabis Marketing Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Marketing application for a private corporation
    And I click on the Submit button
    # And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application for Cannabis Marketing
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Marketing Licence
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@cannabismktg @publiccorporation 
Scenario: Cannabis Marketing Application (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Marketing application for a public corporation
    And I click on the Submit button
    # And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application for Cannabis Marketing
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Marketing Licence
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@cannabismktg @society 
Scenario: Cannabis Marketing Application (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Marketing application for a society
    And I click on the Submit button
    # And I review the security screening requirements for a society
    And I click on the button for Pay for Application for Cannabis Marketing
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Marketing Licence
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@cannabismktg @soleproprietorship 
Scenario: Cannabis Marketing Application (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Marketing application for a sole proprietorship
    And I click on the Submit button
    # And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application for Cannabis Marketing
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Marketing Licence
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CannabisMarketingLicence.feature")]
    [Collection("Liquor")]
    public sealed class CannabisMarketingLicence : TestBase
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

            // CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}