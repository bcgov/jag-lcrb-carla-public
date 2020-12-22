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
Feature: ReviewOrgStructureDeletion
    As a logged in business user
    I want to confirm the successful deletion of personnel from the org structure

@cannabis @partnership @orgstructuredeletion
Scenario: Deletion from Partnership Org Structure
    Given I am logged in to the dashboard as a partnership
    And I click on the Complete Organization Information button
    And I add personnel to the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete the personnel for a partnership
    And I click on the button for Save For Later
    And the org structure data is successfully deleted for a partnership
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @orgstructuredeletion
Scenario: Deletion from Private Corporation Org Structure
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I add personnel to the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete the personnel for a private corporation
    And I click on the button for Save For Later
    And the org structure data is successfully deleted for a private corporation
    And the account is deleted
    Then I see the login page

@cannabis @publiccorporation @orgstructuredeletion
Scenario: Deletion from Public Corporation Org Structure
    Given I am logged in to the dashboard as a public corporation
    And I click on the Complete Organization Information button
    And I add personnel to the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete the personnel for a public corporation
    And I click on the button for Save For Later
    And the org structure data is successfully deleted for a public corporation
    And the account is deleted
    Then I see the login page

@cannabis @society @orgstructuredeletion
Scenario: Deletion from Society Org Structure
    Given I am logged in to the dashboard as a society
    And I click on the Complete Organization Information button
    And I add personnel to the organization structure for a society
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete the personnel for a society
    And I click on the button for Save For Later
    And the org structure data is successfully deleted for a society
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @orgstructuredeletion
Scenario: Deletion from Sole Proprietorship Org Structure
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Complete Organization Information button
    And I add personnel to the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete the personnel for a sole proprietorship
    And I click on the button for Save For Later
    And the org structure data is successfully deleted for a sole proprietorship
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./ReviewOrgStructureDeletion.feature")]
    [Collection("Cannabis")]
    public sealed class ReviewOrgStructureDeletion : TestBase
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
