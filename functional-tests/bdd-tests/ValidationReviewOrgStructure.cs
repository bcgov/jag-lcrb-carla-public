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
Feature: ValidationReviewOrgStructure
    As a logged in business user
    I want to confirm the validation messages for the org structure

@e2e @cannabis @partnership @orgstructure
Scenario: Validation for Partnership Org Structure
    Given I am logged in to the dashboard as a partnership
    And I click on the Complete Organization Information button
    And I click on the button for Submit Organization Information
    And the expected validation errors are thrown for a partnership org structure
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @orgstructure
Scenario: Validation for Private Corporation Org Structure
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I click on the button for Submit Organization Information
    And the expected validation errors are thrown for a private corporation org structure
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @orgstructure
Scenario: Validation for Public Corporation Org Structure
    Given I am logged in to the dashboard as a public corporation
    And I click on the Complete Organization Information button
    And I click on the button for Submit Organization Information
    And the expected validation errors are thrown for a public corporation org structure
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @orgstructure
Scenario: Validation for Society Org Structure
    Given I am logged in to the dashboard as a society
    And I click on the Complete Organization Information button
    And I click on the button for Submit Organization Information
    And the expected validation errors are thrown for a society org structure
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @orgstructure
Scenario: Validation for Sole Proprietorship Org Structure
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Complete Organization Information button
    And I click on the button for Submit Organization Information
    And the expected validation errors are thrown for a sole proprietorship org structure
    And the account is deleted
    Then I see the login page

# Note: There is no validation configured for this biz type.
# @e2e @cannabis @indigenousnation @orgstructure
# Scenario: Validation for Indigenous Nation Org Structure
#    Given I am logged in to the dashboard as an indigenous nation
#    And I click on the Complete Organization Information button
#    And I click on the button for Submit Organization Information
#    And the expected validation errors are thrown for an indigenous nation org structure
#    And the account is deleted
#    Then I see the login page

# Note: There is no validation configured for this biz type.
# @e2e @cannabis @localgovernment @orgstructure
# Scenario: Validation for Local Government Org Structure
#    Given I am logged in to the dashboard as a local government
#    And I click on the Complete Organization Information button
#    And I click on the button for Submit Organization Information
#    And the expected validation errors are thrown for a local government org structure
#    And the account is deleted
#    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ValidationReviewOrgStructure.feature")]
    [Collection("Cannabis")]
    public sealed class ValidationReviewOrgStructure : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LoggedInToDashboard(string businessType)
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
