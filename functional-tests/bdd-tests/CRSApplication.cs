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
    Then I SEE Review Account Profile
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

            //NgWebElement welcomeMessage = ngDriver.FindElement(By.ClassName("dashboard-spacing"));

            //welcomeMessage.

            //IWebElement welcomeMessage = driver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-dashboard/div/h1"));

            //Assert.Contains("Welcome to Cannabis Licensing", welcomeMessage.Text);

        }

        [And(@"I am not a marketer")]
        public void check_marketer()
        {
            // see if there is a marketing licence or
            // see if there is an application for a marketing licence

        }

        [And(@"I CLICK Start Application")]
        public void I_start_application()
        {

            ngDriver.WaitForAngular();

            NgWebElement startApp_button = ngDriver.FindElement(By.XPath("//button[text()='START APPLICATION']"));

            startApp_button.Click();

            // click start CRS application button
            //NgWebElement butt = ngDriver.FindElement(By.TagName("button"));

            // for (b)


            //butt.Click();


            //IWebElement startApp = driver.FindElementByXPath("(.//*[normalize-space(text()) and normalize-space(.)='APPLICATIONS AND LICENCES'])[1]/following::div[1]");

            //IWebElement sub = driver.FindElement(By.XPath(""));

            //startApp.Click();

            //driver.


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
            string path1 = "C:/LCRB/associates.pdf";
            string path2 = "C:/LCRB/fin_integrity.pdf";
            string path3 = "C:/LCRB/checklist.pdf";

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

            //NgWebElement uploadAssociates = ngDriver.FindElement(By.CssSelector("input[type=file]"));
            //uploadAssociates.SendKeys(path1);

            //NgWebElement uploadFinIntegrity = ngDriver.FindElement(By.CssSelector("input[type=file]"));
            //uploadFinIntegrity.SendKeys(path2);

            //NgWebElement uploadChecklistDocs = ngDriver.FindElement(By.CssSelector("input[type=file]"));
            //uploadChecklistDocs.SendKeys(path3);
        }

        [Then(@"I CLICK on 'SUBMIT & PAY'")]
        public void click_on_submit_and_pay()
        {
            NgWebElement submitpay_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT & PAY')]"));
            submitpay_button.Click();
        }

        [Then(@"I CLICK on 'SAVE FOR LATER'")]
        public void click_on_save_for_later()
        {
            NgWebElement saveforlater_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SAVE FOR LATER')]"));
            saveforlater_button.Click();
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
