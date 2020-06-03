using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using System.IO;
using Xunit;

/*
Feature: Create_CRSApplication_partnership
    As a logged in business user
    I want to submit a CRS Application for a partnership
    To be used as test data

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
    And I click on the Pay for Application button
    And I enter the payment information
    Then I return to the dashboard   
*/

namespace bdd_tests
{
    [FeatureFile("./Create_CRSApplication_partnership.feature")]
    public sealed class CreateCRSApplicationPartnership : TestBase
    {

        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            //CheckFeatureFlagsCannabis();

            CarlaLoginNoCheck();
        }

        [And(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLoginWithUser(businessType);
        }

        [And(@"I click on the Start Application button for a Cannabis Retail Store")]
        public void I_start_application()
        {
            /* 
            Page Title: Welcome to Cannabis Licensing
            */

            NgWebElement startApp_button = ngDriver.FindElement(By.XPath("//button[text()='START APPLICATION']"));
            startApp_button.Click();
        }

        [And(@"I review the organization structure")]
        public void review_org_structure()
        {
            ReviewOrgStructure();
        }

        [And(@"I submit the organization structure")]
        public void submit_org_structure()
        {
            SubmitOrgInfoButton();
        }

        [And(@"I complete the Cannabis Retail Store application")]
        public void I_complete_the_application()
        {
            CRSApplication();
        }

        [And(@"I click on the Pay for Application button")]
        public void click_on_pay()
        {
            NgWebElement pay_button = ngDriver.FindElement(By.XPath("//button[contains(.,'Pay for Application')]"));
            pay_button.Click();
        }

        [And(@"I enter the payment information")]
        public void enter_payment_info()
        {
            MakePayment();
        }

        [Then(@"I return to the dashboard")]
        public void return_to_dashboard()
        {
            CRSReturnToDashboard();
        }

        [And(@"the account is deleted")]
        public void Delete_my_account()
        {
            this.CarlaDeleteCurrentAccount();
        }
    }
}
