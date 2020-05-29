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
Feature: CateringApplication_validation
    As a logged in business user
    I want to confirm the page validation for a Catering Application

Scenario: Start Application
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I do not complete the catering application correctly
    Then the expected error messages are displayed
*/

namespace bdd_tests
{
    [FeatureFile("./CateringApplication_validation.feature")]
    public sealed class CateringApplicationValidation : TestBase
    {
        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CheckFeatureFlagsLiquor();

            CarlaLogin(businessType);
        }

        [And(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLogin(businessType);
        }

        [And(@"the account is deleted")]
        public void Delete_my_account()
        {
            this.CarlaDeleteCurrentAccount();
        }

        [And(@"I click on the Start Application button for (.*)")]
        public void I_start_application(string application_type)
        {
            /* 
            Page Title: 
            */

            // click on the Catering Start Application button
            NgWebElement startApp_button = ngDriver.FindElement(By.Id("startCatering"));
            startApp_button.Click();

            applicationTypeShared = application_type;
        }

        [And(@"I review the account profile")]
        public void review_account_profile()
        {
            ReviewAccountProfile();
        }

        [And(@"I review the organization structure")]
        public void I_continue_to_organization_review()
        {
            ReviewOrgStructure();            
        }

        [And(@"I submit the organization structure")]
        public void submit_org_structure()
        {
            SubmitOrgInfoButton();
        }

        [And(@"I do not complete the catering application correctly")]
        public void I_do_not_complete_the_application_correctly()
        {
            /* 
            Page Title: Catering Licence Application
            */

            System.Threading.Thread.Sleep(9000);

            // select 'Yes' for previous liquor licence
            NgWebElement previousLicence = ngDriver.FindElement(By.Id("mat-button-toggle-1-button"));
            previousLicence.Click();

            // select 'Yes' for Rural Agency Store Appointment
            NgWebElement ruralStore = ngDriver.FindElement(By.Id("mat-button-toggle-4-button"));
            ruralStore.Click();

            // select 'Yes' for distillery, brewery or winery connections
            NgWebElement liquorProduction = ngDriver.FindElement(By.Id("mat-button-toggle-7-button"));
            liquorProduction.Click();

            /*
            The following fields are intentionally left empty:
            - the establishment name
            - the establishment address
            - the establishment city
            - the establishment postal code
            - the PID
            - the store phone number
            */

            // select 'Yes' for other business on premises
            //NgWebElement otherBusiness = ngDriver.FindElement(By.Id("mat-button-toggle-10-button"));
            //otherBusiness.Click();

            /*
            The following actions are intentionally left incomplete:
            - upload a store signage document
            - enter the first name of the application contact
            - enter the last name of the application contact
            - enter the role of the application contact
            - enter the phone number of the application contact
            - click on the authorized to submit checkbox
            - click on the signature agreement checkbox
            */

            NgWebElement submit_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT')]"));
            submit_button.Click();
        }

        [Then(@"the expected error messages are displayed")]
        public void expected_error_messages_displayed()
        {
            /* 
            Page Title: Catering Licence Application
            */

            // Expected error messages:
            // - At least one signage document is required.
            // - Establishment name is required.
            // - Some required fields have not been completed

            // check if signage document has been uploaded
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);

            // check if establishment name has been provided
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Establishment name is required.')]")).Displayed);

            // check if empty required fields have been flagged
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Some required fields have not been completed')]")).Displayed);
        }
    }
}