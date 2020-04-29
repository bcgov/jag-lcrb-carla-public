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
    And I click on the Catering Start Application button
    And I review the account profile
    And I review the organization structure
    And I do not complete the catering application correctly
    Then the expected error messages are displayed
*/

namespace bdd_tests
{
    [FeatureFile("./CateringApplication_validation.feature")]
    public sealed class CateringApplicationValidation : TestBase
    {
        public void CheckFeatureFlagsLiquor()
        {
            string feature_flags = configuration["featureFlags"];

            // navigate to the feature flags page
            driver.Navigate().GoToUrl($"{baseUri}{feature_flags}");

            // confirm that the LiquorOne flag is enabled during this test
            Assert.True(driver.FindElement(By.XPath("//body[contains(.,'LiquorOne')]")).Displayed);
        }

        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CheckFeatureFlagsLiquor();

            CarlaLoginNoCheck();
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

        [And(@"I click on the Catering Start Application button")]
        public void I_start_application()
        {
            /* 
            Page Title: 
            */

            // click on the Catering Start Application button
            NgWebElement startApp_button = ngDriver.FindElement(By.Id("startCatering"));
            startApp_button.Click();
        }

        [And(@"I review the account profile")]
        public void review_account_profile()
        {
            ReviewAccountProfile();
        }

        [And(@"I review the organization structure")]
        public void I_continue_to_organization_review()
        {
            /*
            Page Title: [client name] Detailed Organization Information
            */
            
            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document
            string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
            uploadSignage.SendKeys(noticeOfArticles);

            // upload a central securities register document
            string centralSecuritiesRegister = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uploadCentralSecReg = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uploadCentralSecReg.SendKeys(centralSecuritiesRegister);

            // upload a special rights and restrictions document
            string specialRightsRestrictions = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uploadSpecialRightsRes = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
            uploadSpecialRightsRes.SendKeys(specialRightsRestrictions);

            /********** Key Personnel *********/

            // create the key personnel data
            string keyPersonnelFirstName = "Jane";
            string keyPersonnelLastName = "Bond";
            string keyPersonnelTitle = "Adventurer";
            string keyPersonnelEmail = "jane@bond.com";

            // open key personnel form   
            NgWebElement openKeyPersonnelForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[4]/section/app-associate-list/div/button"));
            openKeyPersonnelForm.Click();

            // enter key personnel first name
            NgWebElement uiKeyPersonFirst = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiKeyPersonFirst.SendKeys(keyPersonnelFirstName);

            // enter key personnel last name
            NgWebElement uiKeyPersonLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiKeyPersonLast.SendKeys(keyPersonnelLastName);

            // select key personnel role
            NgWebElement uiKeyPersonRole = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[3]"));
            uiKeyPersonRole.Click();
            
            // enter key personnel title
            NgWebElement uiKeyPersonTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiKeyPersonTitle.SendKeys(keyPersonnelTitle);

            // enter key personnel email
            NgWebElement uiKeyPersonEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiKeyPersonEmail.SendKeys(keyPersonnelEmail);

            // enter key personnel DOB
            NgWebElement openKeyPersonnelDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            openKeyPersonnelDOB.Click();

            NgWebElement openKeyPersonnelDOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-1']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            openKeyPersonnelDOB1.Click();

            /********** Individual Shareholder *********/

            // create the individual shareholder data
            string shareholderFirstName = "Jacqui";
            string shareholderLastName = "Chan";
            string shareholderVotingShares = "500";
            string shareholderEmail = "jacqui@chan.com";

            // open individual shareholder form    
            NgWebElement uiOpenShare = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section/app-associate-list/div/button"));
            uiOpenShare.Click();

            // enter individual shareholder first name
            NgWebElement uiShareFirst = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiShareFirst.SendKeys(shareholderFirstName);

            // enter individual shareholder last name
            NgWebElement uiShareLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiShareLast.SendKeys(shareholderLastName);

            // enter individual number of voting shares
            NgWebElement uiShareVotes = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiShareVotes.SendKeys(shareholderVotingShares);

            // enter individual shareholder email
            NgWebElement uiShareEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            uiShareEmail.SendKeys(shareholderEmail);

            // enter individual shareholder DOB
            NgWebElement uiCalendarS1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiCalendarS1.Click();

            NgWebElement uiCalendarS2 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-2']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiCalendarS2.Click();
                      
            // click on Submit Organization Info button
            NgWebElement submitOrgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT ORGANIZATION INFORMATION')]"));
            submitOrgInfoButton.Click();
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
            NgWebElement otherBusiness = ngDriver.FindElement(By.Id("mat-button-toggle-10-button"));
            otherBusiness.Click();

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