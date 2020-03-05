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

/*
Feature: WorkerApplication
    As a logged in worker applicant
    I want to submit a cannabis worker application

Scenario: Worker Application
    Given I am logged in to the dashboard
    And I click on my name
    And I complete Step 1 of the application
    And I complete Step 2 of the application
    And I click on the Submit & Pay button
    And I enter the payment information
    And I return to the dashboard
    Then the dashboard has a new status
*/

namespace bdd_tests
{
    [FeatureFile("./WorkerApplication.feature")]
    public sealed class WorkerApplication : TestBaseWorker
    {

        [Given(@"I am logged in to the dashboard")]
        public void I_view_the_dashboard()
        {
            CarlaLoginWorker();
        }

        [And(@"I complete the Terms of Use")]
        public void I_complete_the_terms_of_use()
        {
            NgWebElement uiCheckOne = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiCheckOne.Click();

            NgWebElement uiCheckTwo = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'][2]"));
            uiCheckTwo.Click();

            NgWebElement continueButton = ngDriver.FindElement(By.XPath("//button[contains(.,'CONTINUE')]");
            continueButton.Click();
        }

        [And(@"I click on my name")]
        public void I_click_on_my_name()
        {
            NgWebElement uiNameLink = ngDriver.FindElement(By.XPath("(/html/body/app-root/div/div/div/main/div/app-dashboard/div/div[2]/div[1]/div/section/div/section/div/a"));
            uiNameLink.Click();
        }

        [And(@"I complete Step 1 of the application")]
        public void I_complete_step_1_of_the_application()
        {
            string birthCityCountry = "Victoria, Canada";
            string BCDL = "1234568";
            string BCID = "123456789";
            string primaryPhone = "2508888888";
            string email = "test@automation.com";
            string mailingStreet = "645 Tyee Road";
            string mailingCity = "Victoria";
            string mailingProvince = "BC";
            string postalCode = "V8V4Y3";

            NgWebElement uiBirthCityCountry = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiBirthCityCountry.SendKeys(birthCityCountry);

            NgWebElement uiBCDL = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiBCDL.SendKeys(BCDL);

            NgWebElement uiBCID = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiBCID.SendKeys(BCID);

            NgWebElement uiPrimaryPhone = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            uiPrimaryPhone.SendKeys(primaryPhone);

            NgWebElement uiEmail = ngDriver.FindElement(By.XPath("//input[@type='email']"));
            uiEmail.SendKeys(email);

            NgWebElement uiCalendarYear = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/mat-calendar-header/div/div/button/span"));
            uiCalendarYear.Click();

            NgWebElement uiCalendarMonth = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/div/mat-multi-year-view/table/tbody/tr/td/div"));
            uiCalendarMonth.Click();

            NgWebElement uiCalendarDay = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/div/mat-year-view/table/tbody/tr[2]/td/div"));
            uiCalendarDay.Click();

            NgWebElement uiMailingStreet = ngDriver.FindElement(By.XPath("(//input[@type='text'])[17]"));
            uiMailingStreet.SendKeys(mailingStreet);

            NgWebElement uiMailingCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[18]"));
            uiMailingCity.SendKeys(mailingCity);

            NgWebElement uiMailingProvince = ngDriver.FindElement(By.XPath("(//input[@type='text'])[19]"));
            uiMailingProvince.SendKeys(mailingProvince);

            NgWebElement uiPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[20]"));
            uiPostalCode.SendKeys(postalCode);

            //todo: sort out label value
            NgWebElement uiCountry = ngDriver.FindElement(By.XPath("(//select"));
            uiCountry.Click();
        }

        [And(@"I complete Step 2 of the application")]
        public void I_complete_step_2_of_the_application()
        {
            string applicantName = "Automated Test";
            
            NgWebElement uiSelfDisclosure = ngDriver.FindElement(By.XPath("((//input[@name='selfDisclosure'])[2]"));
            uiSelfDisclosure.Click();

            NgWebElement uiNoWetSignature1 = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiNoWetSignature1.Click();

            NgWebElement uiNoWetSignature2 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[2]"));
            uiNoWetSignature2.Click();

            NgWebElement uiApplicantName = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiApplicantName.SendKeys(applicantName);

            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            string signatureFormPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "associates.pdf");
            NgWebElement uploadSignatureForm = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
            uploadSignatureForm.SendKeys(signatureFormPath);
        }

        [And(@"I click on the Submit & Pay button")]
        public void click_on_submit_and_pay()
        {
            NgWebElement submitpay_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT & PAY')]"));
            System.Threading.Thread.Sleep(7000);

            submitpay_button.Click();
            System.Threading.Thread.Sleep(7000);
        }

        [And(@"I enter the payment information")]
        public void enter_payment_info()
        {
            MakeWorkerPayment();
        }

        [And(@"I return to the dashboard")]
        public void return_to_dashboard()
        {
            string retDash = "Return to Dashboard";
            NgWebElement returnDash = ngDriver.FindElement(By.LinkText(retDash));
            returnDash.Click();
        }

        [Then(@"the dashboard has a new status")]
        public void dashboard_has_new_status()
        {
             //to do 
        }
    }
}
