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
    I want to confirm the functionality on the Notices tab

@notices
Scenario: No Notices (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Notices
    And there are no notices attached to the account
    And the account is deleted
    Then I see the login page

# @notices @manualonly
# Scenario: Upload a Notice (Private Corporation)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the link for Notices
#    And there are no notices attached to the account
#    And I navigate to the Dynamics Account page for this account
#    And I click on the Open Document Location link
#    And I upload a file prefixed Notice__
#    And I return to the portal
#    And I click on the link for Notices
#    And the uploaded file is displayed
#    And I click on the uploaded file
#    And the file is successfully downloaded
#    And the account is deleted
#    Then I see the login page
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

            // CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
