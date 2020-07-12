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
Feature: CateringApplicationTransferOfOwnership
    As a logged in business user
    I want to pay the first year catering licence fee
    And request a transfer of ownership for different business types

 @e2e @catering @indigenousnation @cateringtransfer
 Scenario: Indigenous Nation Catering Request a Transfer of Ownership
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @partnership @cateringtransfer
 Scenario: Partnership Catering Request a Transfer of Ownership
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page
     
 @e2e @catering @privatecorporation @cateringtransfer
 Scenario: Private Corporation Catering Request a Transfer of Ownership
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @publiccorporation @cateringtransfer2
 Scenario: Public Corporation Catering Request a Transfer of Ownership
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @society @cateringtransfer2
 Scenario: Society Catering Request a Transfer of Ownership
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @soleproprietorship @cateringtransfer
 Scenario: Sole Proprietorship Catering Request a Transfer of Ownership
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CateringApplicationTransferOfOwnership.feature")]
    public sealed class CateringApplicationTransferOfOwnership : TestBase
    {
        [Given(@"I am logged in to the dashboard as an (.*)")]
        public void Given_I_view_the_dashboard_IN(string businessType)
        {
            CheckFeatureFlagsLiquor();

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