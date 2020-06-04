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
Feature: CRSApplication_indigenousnation
    As a logged in business user
    I want to submit a CRS Application for an indigenous nation

Scenario: Start Application
    Given I am logged in to the dashboard as an indigenous nation
    And the account is deleted
    And I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Cannabis Retail Store application
    And I review the security screening requirements
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CRSApplication_indigenousnation.feature")]
    public sealed class CRSApplicationIndigenousNation : TestBase
    {
        [Given(@"I am logged in to the dashboard as an (.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CheckFeatureFlagsCannabis();

            CarlaLogin(businessType);
        }

        [And(@"I return to the dashboard")]
        public void return_to_dashboard()
        {
            CRSReturnToDashboard();
        }
    }
}
