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
Feature: ReviewAccountProfileData
    As a logged in business user
    I want to confirm the saved data for the account profile

@privatecorporation @reviewaccountdata
Scenario: Data for Private Corporation Review Account Profile
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Edit Account Profile
    And I review the account profile for a private corporation
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a private corporation account profile
    And the account is deleted
    Then I see the login page

@partnership @reviewaccountdata
Scenario: Data for Partnership Review Account Profile
    Given I am logged in to the dashboard as a partnership
    And I click on the link for Edit Account Profile
    And I review the account profile for a partnership
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a partnership account profile
    And the account is deleted
    Then I see the login page

@publiccorporation @reviewaccountdata
Scenario: Data for Public Corporation Review Account Profile
    Given I am logged in to the dashboard as a public corporation
    And I click on the link for Edit Account Profile
    And I review the account profile for a public corporation
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a public corporation account profile
    And the account is deleted
    Then I see the login page

@society @reviewaccountdata
Scenario: Data for Society Review Account Profile
    Given I am logged in to the dashboard as a society
    And I click on the link for Edit Account Profile
    And I review the account profile for a society
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a society account profile
    And the account is deleted
    Then I see the login page

@soleproprietorship @reviewaccountdata
Scenario: Data for Sole Proprietorship Review Account Profile
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the link for Edit Account Profile
    And I review the account profile for a sole proprietorship
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a sole proprietorship account profile
    And the account is deleted
    Then I see the login page

 @indigenousnation @reviewaccountdata
 Scenario: Data for Indigenous Nation Review Account Profile
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the link for Edit Account Profile
    And I review the account profile for an indigenous nation
    And I click on the link for Edit Account Profile
    And the correct data is displayed for an indigenous nation account profile
    And the account is deleted
    Then I see the login page

 @localgovernment @reviewaccountdata
 Scenario: Data for Local Government Review Account Profile
    Given I am logged in to the dashboard as a local government
    And I click on the link for Edit Account Profile
    And I review the account profile for a local government
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a local government account profile
    And the account is deleted
    Then I see the login page

 @university @reviewaccountdata
 Scenario: Data for University Review Account Profile
    Given I am logged in to the dashboard as a university
    And I click on the link for Edit Account Profile
    And I review the account profile for a university
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a university account profile
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ReviewAccountProfileData.feature")]
    [Collection("Cannabis")]
    public sealed class ReviewAccountProfileData : TestBase
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
