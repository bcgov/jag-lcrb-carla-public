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
Feature: LicenceCateringPrivateCorp
    As a logged in business user
    I want to pay the first year catering licence fees
*/

namespace bdd_tests
{
    [FeatureFile("./LicenceCateringPrivateCorp.feature")]
    public sealed class LicenceCateringPrivateCorp : TestBase
    {
        [Given(@"the Catering application has been approved")]
        public void catering_application_approved()
        {
        }

        [And(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLogin(businessType);
        }

        [And(@"I click on the Licences tab")]
        public void click_on_licences_tab()
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }

        [And(@"I click on the Pay First Year Licensing Fee link")]
        public void click_pay_first_year_licensing_fee()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */
        }

        [And(@"I complete the payment")]
        public void complete_the_payment()
        {
            MakePayment();
        }

        [Then(@"the Licences tab has been updated with expiry date, download link, and change jobs")]
        public void licences_tab_updated()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            //Assert.True (ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }

    }
}