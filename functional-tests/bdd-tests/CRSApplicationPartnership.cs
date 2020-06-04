using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using System.IO;
using Xunit;

/*
Feature: CRSApplication_partnership
    As a logged in business user
    I want to submit a CRS Application for a partnership

Scenario: Start Application
    Given I am logged in to the dashboard as a partnership
    And the account is deleted
    And I am logged in to the dashboard as a partnership
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
    [FeatureFile("./CRSApplication_partnership.feature")]
    public sealed class CRSApplicationPartnership : TestBase
    {
        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CheckFeatureFlagsCannabis();
            
            CarlaLogin(businessType);
        }

        [And(@"I click on the Start Application button for a Cannabis Retail Store")]
        public void I_start_application()
        {
            StartCRSApplication();
        }

        [And(@"I submit the organization structure")]
        public void submit_org_structure()
        {
            SubmitOrgInfoButton();
        }

        [And(@"I click on the Pay for Application button")]
        public void click_on_pay()
        {
            NgWebElement pay_button = ngDriver.FindElement(By.XPath("//button[contains(.,'Pay for Application')]"));
            pay_button.Click();
        }

        [And(@"I return to the dashboard")]
        public void return_to_dashboard()
        {
            CRSReturnToDashboard();
        }
    }
}
