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
 Feature: CateringApplicationEventAuthorization
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit an event authorization request for different business types

 Scenario: Indigenous Nation Event Authorization Request
    Given I am logged in to the dashboard as an indigenous nation
    And the account is deleted
    And I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request an event authorization
    And the account is deleted
    Then I see the login page

 Scenario: Partnership Event Authorization Request
    Given I am logged in to the dashboard as a partnership
    And the account is deleted
    And I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request an event authorization
    And the account is deleted
    Then I see the login page

 Scenario: Private Corporation Event Authorization Request
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request an event authorization
    And the account is deleted
    Then I see the login page

 Scenario: Public Corporation Event Authorization Request
    Given I am logged in to the dashboard as a public corporation
    And the account is deleted
    And I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request an event authorization
    And the account is deleted
    Then I see the login page

 Scenario: Society Event Authorization Request
    Given I am logged in to the dashboard as a society
    And the account is deleted
    And I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request an event authorization
    And the account is deleted
    Then I see the login page

  Scenario: Sole Proprietorship Event Authorization Request
    Given I am logged in to the dashboard as a sole proprietorship
    And the account is deleted
    And I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request an event authorization
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CateringApplicationEventAuthorization.feature")]
    public sealed class CateringApplicationEventAuthorization : TestBase
    {
        [Given(@"I am logged in to the dashboard as an (.*)")]
        public void Given_I_view_the_dashboard_IN(string businessType)
        {
            CheckFeatureFlagsLiquor();

            CarlaLogin(businessType);
        }


        [And(@"I am logged in to the dashboard as an (.*)")]
        public void And_I_view_the_dashboard_IN(string businessType)
        {
            CarlaLogin(businessType);
        }


        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void Given_I_view_the_dashboard(string businessType)
        {
            CheckFeatureFlagsLiquor();

            CarlaLogin(businessType);
        }
    }
}