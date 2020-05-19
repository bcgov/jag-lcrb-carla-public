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
Feature: COVID_temporary_extension_validation.feature
    As a business user who is not logged in
    I want to confirm the required field messages for a COVID temporary extension application

Scenario: Validate COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I do not complete the temporary extension application
    And I click on the Submit button
    Then the required field messages are displayed
*/

namespace bdd_tests
{
    [FeatureFile("./COVID_temporary_extension_validation.feature")]
    public sealed class COVIDTemporaryExtensionValidation : TestBase
    {
        public void CheckFeatureFlagsCOVIDTempExtension()
        {
            // navigate to the feature flags page
            driver.Navigate().GoToUrl($"{baseUri}api/features");

            // confirm that the COVID Temporary Extension feature flag is enabled during this test - TODO
            Assert.True(driver.FindElement(By.XPath("//body[contains(.,'CovidApplication')]")).Displayed);
        }

        [Given(@"I am not logged in to the Liquor and Cannabis Portal")]
        public void not_logged_in()
        {
            CheckFeatureFlagsCOVIDTempExtension();
        }

        [And(@"I click on the COVID Temporary Extension link")]
        public void click_on_covid_temp()
        {
            ngDriver.Navigate().GoToUrl($"{baseUri}covid-temporary-extension");
        }

        [And(@"I do not complete the temporary extension application")]
        public void do_not_complete_application()
        {
            /* 
            Page Title: Covid Temporary Extension Application
            */

            // create unacceptable entry to generate error messages
            string licencenumber = " ";

            // enter the licence number and then clear it
            NgWebElement uiLicenceNumber = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiLicenceNumber.SendKeys(licencenumber);
            NgWebElement uiBody = ngDriver.FindElement(By.XPath("//app-application-covid-temporary-extension/div/div[2]/div[2]/section[2]/app-field[1]/section/div[1]"));
            uiBody.Click();
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Licence Number is a required field')]")).Displayed);
        }

        [And(@"I click on the Submit button")]
        public void submit_button()
        {
            NgWebElement submitButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT APPLICATION')]"));
            submitButton.Click();
        }

        [Then(@"the required field messages are displayed")]
        public void required_field_messages_displayed()
        {
            // confirm that specific required field error messages are displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Licence Type is a required field')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Establishment Name is a required field')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please identify your Local Goverment status')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'The following fields are not valid:')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Selection of the LG Option')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Declaration checkbox')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Missing Floor Plan Documents')]")).Displayed);
        }
    }
}
