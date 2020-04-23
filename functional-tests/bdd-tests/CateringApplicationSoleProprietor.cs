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
Feature: CateringApplication_soleproprietor
    As a logged in business user
    I want to submit a Catering Application for a sole proprietor

Scenario: Start Application
    Given I am logged in to the dashboard as a sole proprietorship
    And the account is deleted
    And I am logged in to the dashboard as a sole proprietorship
    And I click on the Catering Start Application button
    And I review the account profile
    And I review the organization structure
    And I complete the application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CateringApplication_soleproprietor.feature")]
    public sealed class CateringApplicationSoleProprietor : TestBase
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
            /* 
            Page Title: Please Review the Account Profile
            */

            // create the account data
            string bizNumber = "012345678";
            string streetAddress1 = "645 Tyee Road";
            string streetAddress2 = "Point Ellis";
            string city = "Victoria";
            string postalCode = "V8V4Y3";
            string bizPhoneNumber = "2501811818";
            string bizEmail = "test@automation.com";
            string solepropContactGiven = "CateringSolePropGiven";
            string solepropContactSurname = "CateringSolePropSurname";
            string soleProprietorContactTitle = "Owner";
            string soleProprietorContactPhone = "7781811818";
            string soleProprietorContactEmail = "automated@test.com";

            string mailStreet1 = "P.O. Box 123";
            string mailStreet2 = "303 Prideaux St.";
            string mailCity = "Nanaimo";
            string mailProvince = "B.C.";
            string mailPostalCode = "V9R2N3";
            string mailCountry = "Switzerland";

            // enter the business number
            NgWebElement uiBizNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiBizNumber.SendKeys(bizNumber);

            // enter the physical street address 1
            NgWebElement uiStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiStreetAddress1.SendKeys(streetAddress1);

            // enter the physical street address 2
            NgWebElement uiStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            uiStreetAddress2.SendKeys(streetAddress2);

            // enter the physical city
            NgWebElement uiCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiCity.SendKeys(city);

            // enter the physical postal code
            NgWebElement uiPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiPostalCode.SendKeys(postalCode);

            /* switching off use of checkbox "Same as physical address" in order to test mailing address fields
            NgWebElement uiSameAsMailingAddress = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiSameAsMailingAddress.Click(); */

            // enter the mailing street address 1
            NgWebElement uiMailingStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiMailingStreetAddress1.SendKeys(mailStreet1);

            // enter the mailing street address 2
            NgWebElement uiMailingStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[11]"));
            uiMailingStreetAddress2.SendKeys(mailStreet2);

            // enter the mailing city
            NgWebElement uiMailingCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
            uiMailingCity.SendKeys(mailCity);

            // enter the mailing province
            NgWebElement uiMailingProvince = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
            uiMailingProvince.SendKeys(mailProvince);

            // enter the mailing postal code
            NgWebElement uiMailingPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[14]"));
            uiMailingPostalCode.SendKeys(mailPostalCode);

            // enter the mailing country
            NgWebElement uiMailingCountry = ngDriver.FindElement(By.XPath("(//input[@type='text'])[15]"));
            uiMailingCountry.SendKeys(mailCountry);

            // enter the business phone number
            NgWebElement uiBizPhoneNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[16]"));
            uiBizPhoneNumber.SendKeys(bizPhoneNumber);

            // enter the business email
            NgWebElement uiBizEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[17]"));
            uiBizEmail.SendKeys(bizEmail);

            // (re)enter the sole proprietor contact first name
            NgWebElement uiSolePropContactGiven = ngDriver.FindElement(By.XPath("(//input[@type='text'])[18]"));
            uiSolePropContactGiven.SendKeys(solepropContactGiven);

            // (re)enter the sole proprietor contact surname
            NgWebElement uiSolePropContactSurname = ngDriver.FindElement(By.XPath("(//input[@type='text'])[19]"));
            uiSolePropContactSurname.SendKeys(solepropContactSurname);

            // enter the sole proprietor contact title
            NgWebElement uiSolePropContactTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[20]"));
            uiSolePropContactTitle.SendKeys(soleProprietorContactTitle);

            // enter the sole proprietor contact phone number
            NgWebElement uiSolePropContactPhone = ngDriver.FindElement(By.XPath("(//input[@type='text'])[21]"));
            uiSolePropContactPhone.SendKeys(soleProprietorContactPhone);

            // enter the sole proprietor contact phone email
            NgWebElement uiSolePropContactEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[22]"));
            uiSolePropContactEmail.SendKeys(soleProprietorContactEmail);

            // select 'No' for sole proprietor's connection to a federal producer
            NgWebElement corpConnectionFederalProducer = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[2]"));
            corpConnectionFederalProducer.Click();

            // select 'No' for federal producer's connection to sole proprietor
            NgWebElement federalProducerConnectionToCorp = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[4]"));
            federalProducerConnectionToCorp.Click();

            // click on Continue to Organization Review button
            NgWebElement continueApp_button = ngDriver.FindElement(By.Id("continueToApp"));
            continueApp_button.Click();
        }

        [And(@"I review the organization structure")]
        public void I_continue_to_organization_review()
        {
            // open the leader row                                                           
            NgWebElement openLeaderForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/app-associate-list/div/button"));
            openLeaderForm.Click();

            // create the leader info
            string firstName = "Jane";
            string lastName = "Bond";
            //string title = "Adventurer";
            string email = "jane@bond.com";

            // enter the leader first name
            NgWebElement uiFirstName = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiFirstName.SendKeys(firstName);

            // enter the leader last name
            NgWebElement uiLastName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiLastName.SendKeys(lastName);

            // enter the leader email
            NgWebElement uiEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiEmail.SendKeys(email);

            // select the leader DOB
            NgWebElement openLeaderDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            openLeaderDOB.Click();

            NgWebElement openLeaderDOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            openLeaderDOB1.Click();

            // click on the Submit Organization Information button
            NgWebElement submitOrgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT ORGANIZATION INFORMATION')]"));
            submitOrgInfoButton.Click();
        }

        [And(@"I complete the application")]
        public void I_complete_the_application()
        {
            CateringApplication();
        }

        [And(@"I click on the Submit button")]
        public void click_on_submit()
        {
            NgWebElement submit_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT')]"));
            submit_button.Click();
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

        [And(@"I return to the dashboard")]
        public void return_to_dashboard()
        {
            CateringReturnToDashboard();
        }

        [And(@"the account is deleted")]
        public void Delete_my_account()
        {
            this.CarlaDeleteCurrentAccount();
        }

        [Then(@"I see the login page")]
        public void I_see_login()
        {
            /* 
            Page Title: 
            */

            Assert.True(ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }
    }
}