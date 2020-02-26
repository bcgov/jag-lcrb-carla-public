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
Feature: CRSApplication
    As a logged in business user
    I want to submit a CRS Application

Scenario: Start Application
    Given I SEE the Dashboard
    And I CLICK Start Application
    And I CLICK on Continue to Application
    And I COMPLETE the Application
    And I CLICK on 'SUBMIT & PAY'
    And I enter the payment information
    Then I return to the dashboard
*/

namespace bdd_tests
{
    [FeatureFile("./CRSApplication.feature")]
    public sealed class CRSApplication : TestBase
    {

        // Dashboard related common actions

        [Given(@"I SEE the Dashboard")]
        public void I_view_the_dashboard()
        {
            CarlaLogin();
        }

        [And(@"I am not a marketer")]
        public void check_marketer()
        {
        }

        [And(@"I CLICK Start Application")]
        public void I_start_application()
        {
            ngDriver.WaitForAngular();
            NgWebElement startApp_button = ngDriver.FindElement(By.XPath("//button[text()='START APPLICATION']"));
            startApp_button.Click();
        }

        [And(@"I CLICK on Continue to Application")]
        public void I_continue_to_application()
        {
            ngDriver.WaitForAngular();
            NgWebElement continueApp_button = ngDriver.FindElement(By.Id("continueToApp"));
            continueApp_button.Click();
            ngDriver.WaitForAngular();
        }

        [And(@"I COMPLETE the Application")]
        public void I_complete_the_application()
        {
            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A6X5";
            string estPID = "012345678";
            string estEmail = "test@test.com";
            string estPhone = "2505555555";
            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";

            NgWebElement estabName = ngDriver.FindElement(By.Id("establishmentName"));
            estabName.SendKeys(estName);

            NgWebElement estabAddress = ngDriver.FindElement(By.Id("establishmentAddressStreet"));
            estabAddress.SendKeys(estAddress);

            NgWebElement estabCity = ngDriver.FindElement(By.Id("establishmentAddressCity"));
            estabCity.SendKeys(estCity);

            NgWebElement estabPostal = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            estabPostal.SendKeys(estPostal);

            NgWebElement estabPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            estabPID.SendKeys(estPID);

            NgWebElement estabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            estabEmail.SendKeys(estEmail);

            NgWebElement estabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            estabPhone.SendKeys(estPhone);

            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;
            
            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSignage.SendKeys(signagePath);

            string validInterestPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "valid_interest.pdf");
            NgWebElement uploadValidInterest = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uploadValidInterest.SendKeys(validInterestPath);

            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uploadFloorplan.SendKeys(floorplanPath);

            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
            uploadSitePlan.SendKeys(sitePlanPath);

            string finIntegrityPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "fin_integrity.pdf");
            NgWebElement uploadFinIntegrity = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uploadFinIntegrity.SendKeys(finIntegrityPath);

            NgWebElement contactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            contactGiven.SendKeys(conGiven);

            NgWebElement contactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            contactSurname.SendKeys(conSurname);
            
            NgWebElement contactRole = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonRole]"));
            contactRole.SendKeys(conRole);

            NgWebElement contactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonPhone]"));
            contactPhone.SendKeys(conPhone);

            NgWebElement authorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            authorizedSubmit.Click();

            NgWebElement signatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            signatureAgree.Click();
        }

        [And(@"I CLICK on 'SUBMIT & PAY'")]
        public void click_on_submit_and_pay()
        {
            NgWebElement submitpay_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT & PAY')]"));
            System.Threading.Thread.Sleep(7000);

            submitpay_button.Click();
            System.Threading.Thread.Sleep(7000);
        }

        [Then(@"I CLICK on 'SAVE FOR LATER'")]
        public void click_on_save_for_later()
        {
            NgWebElement saveforlater_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SAVE FOR LATER')]"));
            saveforlater_button.Click();
        }

        [And(@"I enter the payment information")]
        public void enter_payment_info()
        {
            string testCC = "4030000010001234";
            string testCVD = "123";

            //find out what browser equivalent is in this context
            //browser sync - don't wait for Angular
            ngDriver.IgnoreSynchronization = true;

            driver.FindElementByName("trnCardNumber").SendKeys(testCC);

            driver.FindElementByName("trnCardCvd").SendKeys(testCVD);

            driver.FindElementByName("submitButton").Click();
            System.Threading.Thread.Sleep(10000);

            //turn back on when returning to Angular
            ngDriver.IgnoreSynchronization = false;
        }

        [Then(@"I return to the dashboard")]
        public void return_to_dashboard()
        {
            string retDash = "Return to Dashboard";
            NgWebElement returnDash = ngDriver.FindElement(By.LinkText(retDash));
            returnDash.Click();
        }

        [And(@"I SEE Review Account Profile")]
        public void see_account_profile()
        {
            // on the update account profile page
        }

        [And(@"a CRS Application exists")]
        public void application_exists()
        {
            // click start application
        }

        [And(@"I CLICK Complete Application")]
        public void I_complete_application()
        {
            // click the link for the application
        }

        [And(@"I CLICK Cancel Application")]
        public void I_cancel_application()
        {
            // click the link to cancel from the dashboard
            // confirm popup
        }

        [Then(@"the Application is Cancelled")]
        public void cancel_from_dashboard()
        {
            // application is removed from dashboard
            // application is marked as terminated
        }

        /*
        // Account Profile common activities


        [Given(@"I SEE the Account Profile")]
        public void I_view_the_dashboard()
        {
            // navigate to the map page.
            //ngDriver.Navigate().GoToUrl($"{baseUri}/map");
        }


        [And(@"I CLICK Continue to Application")]
        public void I_continue_application()
        {
            // navigate to the map page.
            //ngDriver.Navigate().GoToUrl($"{baseUri}/map");
        }

        [Then(@"I am navigated the Application Page")]
        public void See_Application_page()
        {
            // application is removed from dashboard
            // application is marked as terminated
        }


        // Application common activities

        [Given(@"I SEE the Application Form")]
        public void I_see_application()
        {
            // navigate to the map page.
            //ngDriver.Navigate().GoToUrl($"{baseUri}/map");
        }

        //[And(@"I CLICK Save for Later")]
        //public void I_save_for_later(string search)
        //{
        //    //var elements = ngDriver.FindElements(NgBy.Repeater("todo in todoList.todos"));
        //}

        //[And(@"I CLICK Cancel Application")]
        //public void I_cancel_application(string search)
        //{
        //    //var elements = ngDriver.FindElements(NgBy.Repeater("todo in todoList.todos"));
        //}

        [And(@"I UPLOAD an Associate Form")]
        public void upload_associate_form(string search)
        {
            //var elements = ngDriver.FindElements(NgBy.Repeater("todo in todoList.todos"));
        }

        [And(@"I CLICK Submit and Pay")]
        public void I_submit_application(string search)
        {
            //var elements = ngDriver.FindElements(NgBy.Repeater("todo in todoList.todos"));
        }

        [Then(@"the page shows a map")]
        public void The_page_shows_a_map()
        {
            // verify that the results are in the right area
            //var elements = ngDriver.FindElements(NgBy.Repeater("todo in todoList.todos"));
            //Assert.Equal("build an angular app", elements[1].Text);
        }

        [Then(@"the page shows search results including (.*)")]
        public void The_page_shows_search_results_including(string expectedResult)
        {
            // verify that the results are in the right area
            //var elements = ngDriver.FindElements(NgBy.Repeater("todo in todoList.todos"));
            //Assert.Equal("build an angular app", elements[1].Text);
        }
        */
    }
}
