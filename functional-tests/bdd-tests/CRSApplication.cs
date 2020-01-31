using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;

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
        }

        [Then(@"I COMPLETE the Application")]
        public void I_complete_the_application()
        {
            ngDriver.WaitForAngular();
            
            //skipping down to end of page to troubleshoot timeout issue

            NgWebElement submitpay_button = ngDriver.FindElement(By.XPath("//button[text()='SUBMIT & PAY']"));
            
            submitpay_button.Click();

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
