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
  Feature: CateringApplicationEventAuthorizationTransfer
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit an event authorization and transfer of ownership request for different business types

 @e2e @catering @indigenousnation @cateringeventtransfer2
 Scenario: Indigenous Nation Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    #And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @partnership @cateringeventtransfer
 Scenario: Partnership Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @privatecorporation @cateringeventtransfer
 Scenario: Private Corporation Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @publiccorporation @cateringeventtransfer2
 Scenario: Public Corporation Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @society @cateringeventtransfer2
 Scenario: Society Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

  @e2e @catering @soleproprietorship @cateringeventtransfer
  Scenario: Sole Proprietorship Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CateringApplicationEventAuthorizationTransfer.feature")]
    public sealed class CateringApplicationEventAuthorizationTransfer : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void Given_I_view_the_dashboard_IN(string businessType)
        {
            CheckFeatureFlagsLiquor();

            CarlaLogin(businessType);
        }
    }
}