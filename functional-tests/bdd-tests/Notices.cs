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
Feature: Notices
    As a logged in business user
    I want to view notices on the Notices tab

@notices
Scenario: Notices Download (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Notices
    And I click on the Notices file
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./Notices.feature")]
    [Collection("Liquor")]
    public sealed class Notices : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLiquorTwo();

            CheckFeatureFlagsLicenseeChanges();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
