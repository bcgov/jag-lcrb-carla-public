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
Feature: ReviewOrgStructureData
    As a logged in business user
    I want to confirm the data saved for the org structure

@e2e @cannabis @partnership @orgstructure
Scenario: Data for Partnership Org Structure
    Given I am logged in to the dashboard as a partnership
    And I click on the Complete Organization Information button
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a partnership
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @orgstructure
Scenario: Data for Private Corporation Org Structure
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a private corporation
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @orgstructure
Scenario: Data for Public Corporation Org Structure
    Given I am logged in to the dashboard as a public corporation
    And I click on the Complete Organization Information button
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a public corporation
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @orgstructure
Scenario: Data for Society Org Structure
    Given I am logged in to the dashboard as a society
    And I click on the Complete Organization Information button
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a society
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @orgstructure
Scenario: Data for Sole Proprietorship Org Structure
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Complete Organization Information button
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a sole proprietorship
    And the account is deleted
    Then I see the login page

 @e2e @cannabis @indigenousnation @orgstructure
 Scenario: Data for Indigenous Nation Org Structure
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Complete Organization Information button
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for an indigenous nation
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ReviewOrgStructureData.feature")]
    [Collection("Cannabis")]
    public sealed class ReviewOrgStructureData : TestBase
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
