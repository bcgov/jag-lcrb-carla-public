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
Feature: LiquorPrimaryStructuralChangeNoCapacity
    As a logged in business user
    I want to request a Structural Change (No Capacity) for a Liquor Primary Application

@liquorprimary
Scenario: Liquor Primary Structural Change No Capacity (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary application for a private corporation
    And I click on the Submit button
    And the dashboard status is updated as Pending External Review
    # TODO
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./LiquorPrimaryStructuralChangeNoCapacity.feature")]
    [Collection("Liquor")]
    public sealed class LiquorPrimaryStructuralChangeNoCapacity : TestBase
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
