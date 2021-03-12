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
Feature: RuralLRSDownloadLicence
    As a logged in business user
    I want to download a licence for a rural LRS application

@privatecorporation @ruralLRS
Scenario: Rural LRS Download Licence (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a private corporation
    And I complete the Rural LRS application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Download Licence
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./RuralLRSDownloadLicence.feature")]
    [Collection("Cannabis")]
    public sealed class RuralLRSDownloadLicence : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
