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
Feature: E2ECateringApplication_partnership_store_relocation
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a store relocation request for a partnership

Scenario: Pay First Year Catering Licence and Submit Store Relocation Request
    Given I am logged in to the dashboard as a partnership
    And the account is deleted
    And I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee
    And I request a store relocation
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./E2ECateringApplication_partnership_store_relocation.feature")]
    public sealed class E2ECateringApplicationLicencePartnershipStoreRelocation : TestBase
    {
        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void Given_I_view_the_dashboard(string businessType)
        {
            CheckFeatureFlagsLiquor();

            CarlaLogin(businessType);
        }


        [And(@"I pay the licensing fee")]
        public void click_pay_first_year_licensing_fee()
        {
            PayCateringLicenceFee();
        }
    }
}