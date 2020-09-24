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

@e2e @cannabis @partnership @orgstructure
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

@e2e @cannabis @privatecorporation @orgstructure
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

@e2e @cannabis @publiccorporation @orgstructure
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

@e2e @cannabis @society @orgstructure
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

@e2e @cannabis @soleproprietorship @orgstructure
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

# @e2e @cannabis @indigenousnation @orgstructure
# Scenario: Deletion from Indigenous Nation Org Structure
#    Given I am logged in to the dashboard as an indigenous nation
#    And I click on the Complete Organization Information button
#    And I add personnel to the organization structure for an indigenous nation
#    And I click on the button for Submit Organization Information
#    And I click on the Complete Organization Information button
#    And I delete the personnel for an indigenous nation
#    And I click on the button for Submit Organization Information
#    And I click on the Complete Organization Information button
#    And the org structure data is successfully deleted for an indigenous nation
#    And the account is deleted
#    Then I see the login page

# @e2e @cannabis @localgovernment @orgstructure
# Scenario: Deletion from Local Government Org Structure
#    Given I am logged in to the dashboard as a local government
#    And I click on the Complete Organization Information button
#    And I add personnel to the organization structure for a local government
#    And I click on the button for Submit Organization Information
#    And I click on the Complete Organization Information button
#    And I delete the personnel for a local government
#    And I click on the button for Submit Organization Information
#    And I click on the Complete Organization Information button
#    And the org structure data is successfully deleted for a local government
#    And the account is deleted
#    Then I see the login page
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
