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
Feature: CheckOrgStructure
    As a logged in business user
    I want to confirm that the organization structure page displays

@validation @privatecorporation @checkorgstructure
Scenario: Check Organization Structure
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And the organization structure page is displayed
    And I click on the link for Dashboard
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CheckOrgStructure.feature")]
    [Collection("Liquor")]
    public sealed class CheckOrgStructure : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLicenseeChanges();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
