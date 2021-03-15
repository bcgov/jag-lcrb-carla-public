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
Feature: CombinedCateringCrsApplications
    As a logged in business user
    I want to submit a CRS and Catering application for the same account

@cannabis @catering @crsapp
Scenario: Combined Catering / CRS Applications (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And I click on the Dashboard tab
    And I click on the Start Application button for Catering
    And I click on the button for Continue to Organization Review
    And I complete the Catering application for a combined application
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CombinedCateringCrsApplications.feature")]
    [Collection("Cannabis")]
    public sealed class CombinedCateringCrsApplications : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLGIN();

            CheckFeatureFlagsIN();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureFlagsSecurityScreening();

            CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
