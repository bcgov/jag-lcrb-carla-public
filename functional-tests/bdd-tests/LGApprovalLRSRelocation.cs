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
Feature: LGApprovalLRSRelocation
    As a logged in business user
    I want to submit a LRS Relocation Application for review and approval

@e2e @catering @privatecorporation @picnic @lgapproval
Scenario: Local Government Approval for LRS Relocation
    Given I am logged in as a private corporation
    And an LRS licence has been created
    And I click on the Licences tab 
    And I click on the link for Relocation Application
    And I click on the Continue to Application button
    And I complete the LRS application
    And I log in as local government for Saanich
    And I specify that the zoning allows the endorsement
    And I specify my contact details
    And I log in as the LRS
    And I review the local government
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page 
*/

namespace bdd_tests
{
    [FeatureFile("./LGApprovalLRSRelocation.feature")]
    public sealed class LGApprovalLRSRelocation : TestBase
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