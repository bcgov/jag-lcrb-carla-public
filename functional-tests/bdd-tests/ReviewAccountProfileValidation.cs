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
Feature: ReviewAccountProfileValidation
    As a logged in business user
    I want to confirm the validation messages for the account profile

@privatecorporation @reviewaccount
Scenario: Validation for Review Account Profile (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

@partnership @reviewaccount
Scenario: Validation for Review Account Profile (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

@publiccorporation @reviewaccount
Scenario: Validation for Review Account Profile (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

@society @reviewaccount
Scenario: Validation for Review Account Profile (Society)
    Given I am logged in to the dashboard as a society
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

@soleproprietorship @reviewaccount
Scenario: Validation for Review Account Profile (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the link for Edit Account Profile
    And I click on the Submit button   
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

 @indigenousnation @reviewaccount
 Scenario: Validation for Review Account Profile (Indigenous Nation)
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the link for Edit Account Profile
    And I click on the Submit button   
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

 @localgovernment @reviewaccount
 Scenario: Validation for Review Account Profile (Local Government)
    Given I am logged in to the dashboard as a local government
    And I click on the link for Edit Account Profile
    And I click on the Submit button   
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ReviewAccountProfileValidation.feature")]
    [Collection("Cannabis")]
    public sealed class ReviewAccountProfileValidation : TestBase
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
