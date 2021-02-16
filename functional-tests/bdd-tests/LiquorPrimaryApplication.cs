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
Feature: LiquorPrimaryApplication
    As a logged in business user
    I want to submit Liquor Primary Applications for different business types

@liquorprimaryapp
Scenario: DEV Liquor Primary Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary application for a private corporation
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

@liquorprimaryapp 
Scenario: DEV Liquor Primary Application (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a society
    And I complete the Liquor Primary application for a society
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

@liquorprimaryapp 
Scenario: DEV Liquor Primary Application (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a partnership
    And I complete the Liquor Primary application for a partnership
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

@liquorprimaryapp
Scenario: DEV Liquor Primary Application (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a sole proprietorship
    And I complete the Liquor Primary application for a sole proprietorship
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority
    And I click on the Submit button
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./LiquorPrimaryApplication.feature")]
    [Collection("Liquor")]
    public sealed class LiquorPrimaryApplication : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLiquorTwo();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
