using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using System.IO;
using Xunit;

/*
Feature: CreateCRSApplicationPartnership
    As a logged in business user
    I want to submit a CRS Application for a partnership
    To be used as test data

Scenario: Create CRS Application Partnership
    Given I am logged in to the dashboard as a partnership
    And the account is deleted
    And I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    Then I confirm the payment receipt for a Cannabis Retail Store application
*/

namespace bdd_tests
{
    [FeatureFile("./CreateCRSApplicationPartnership.feature")]
    public sealed class CreateCRSApplicationPartnership : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLicenseeChanges();

            IgnoreSynchronizationFalse();

            CarlaLoginNoCheck(businessType);
        }
    }
}
