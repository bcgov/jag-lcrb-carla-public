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
Feature: COVID_temporary_extension.feature
    As a business user who is not logged in
    I want to submit a COVID temporary extension application

Scenario: Complete COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application
    And I click on the Submit button
    Then the application is submitted
*/

namespace bdd_tests
{
    [FeatureFile("./COVID_temporary_extension.feature")]
    public sealed class COVIDTemporaryExtension : TestBase
    {
        public void CheckFeatureFlagsCOVIDTempExtension()
        {
            // navigate to the feature flags page
            driver.Navigate().GoToUrl($"{baseUri}api/features");

            // confirm that the COVID Temporary Extension feature flag is enabled during this test - TODO
            //Assert.True(driver.FindElement(By.XPath("//body[contains(.,'FEATURE_COVID_APPLICATION')]")).Displayed);
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

        [And(@"I complete the temporary extension application")]
        public void complete_application()
        {
            /* 
            Page Title: Covid Temporary Extension Application
            */

            // create test data

            string licencenumber = "1234";
            string licencename = "Point Ellis Operations";
            string estname = "Point Ellis Greenhouse";
            string eststreet = "645 Tyee Road";
            string estcity = "Victoria";
            string estpostal = "V8V4Y3";
            string contactfirst = "ContactFirst";
            string contactlast = "ContactLast";
            string contacttitle = "Director";
            string contacttel = "2501811181";
            string contactemail = "contact@email.com";
            string mailingstreet = "MailingStreet";
            string mailingcity = "MailingCity";
            string mailingpostal = "V8V4Y3";

            // complete text inputs

            NgWebElement uiLicenceNumber = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiLicenceNumber.SendKeys(licencenumber);
            
            NgWebElement uiLicenceType = ngDriver.FindElement(By.XPath("//mat-radio-button[@id='mat-radio-2']"));
            uiLicenceType.Click();

            NgWebElement uiLicenceName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiLicenceName.SendKeys(licencename);

            NgWebElement uiEstName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiEstName.SendKeys(estname);

            NgWebElement uiEstStreet = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiEstStreet.SendKeys(eststreet);

            NgWebElement uiEstCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            uiEstCity.SendKeys(estcity);

            NgWebElement uiEstPostal = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiEstPostal.SendKeys(estpostal);

            NgWebElement uiContactFirst = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiContactFirst.SendKeys(contactfirst);

            NgWebElement uiContactLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiContactLast.SendKeys(contactlast);

            NgWebElement uiContactTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            uiContactTitle.SendKeys(contacttitle);

            NgWebElement uiContactTel = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiContactTel.SendKeys(contacttel);

            NgWebElement uiContactEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[11]"));
            uiContactEmail.SendKeys(contactemail);

            NgWebElement uiMailingStreet = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
            uiMailingStreet.SendKeys(mailingstreet);

            NgWebElement uiMailingCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
            uiMailingCity.SendKeys(mailingcity);

            NgWebElement uiMailingPostal = ngDriver.FindElement(By.XPath("(//input[@type='text'])[14]"));
            uiMailingPostal.SendKeys(mailingpostal);

            NgWebElement uiLocalGovPerm = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[6]"));
            uiLocalGovPerm.Click();

            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[7]"));
            uiSignatureAgreement.Click();
        }

        [And(@"I click on the Submit button")]
        public void submit_button()
        {
            NgWebElement submitButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT APPLICATION')]"));
            submitButton.Click();
        }

        [Then(@"the application is submitted")]
        public void application_submitted()
        {
            //TODO
        }
    }
}
