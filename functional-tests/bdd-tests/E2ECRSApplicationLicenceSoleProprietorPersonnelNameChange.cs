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
Feature: E2E_CRSApplication_Licence_soleproprietor_personnel_name_change
    As a logged in business user
    I want to submit a CRS Application for a sole proprietorship
    And request a personnel name change for the approved application

Scenario: Start Application
    Given I am logged in to the dashboard as a sole proprietorship
    And the account is deleted
    And I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Cannabis Retail Store application
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for a Cannabis Retail Store
    And I pay the licensing fee
    And I request a personnel name change
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./E2E_CRSApplication_Licence_soleproprietor_personnel_name_change.feature")]
    public sealed class E2ECRSApplicationLicenceSoleProprietorPersonnelNameChange : TestBase
    {
        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CheckFeatureFlagsCannabis();

            CarlaLogin(businessType);
        }

        [And(@"I pay the licensing fee")]
        public void pay_licence_fee()
        {
            PayCRSLicenceFee();
        }
    }
}
