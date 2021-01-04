using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [Given(@"I click on the Swagger link for OneStop")]
        public void ClickOnSwaggerLink()
        {
            ngDriver.Navigate().GoToUrl($"https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca");
        }


        [And(@"I click on the (.*)")]
        public void ClickOnSwaggerButton(string buttonType)
        {
            if (buttonType == "Authorize button")
            {
                NgWebElement uiSwaggerButton = ngDriver.FindElement(By.CssSelector(""));
                uiSwaggerButton.Click();
            }

            if (buttonType == "Close button")
            {
                NgWebElement uiSwaggerButton = ngDriver.FindElement(By.CssSelector(""));
                uiSwaggerButton.Click();
            }

            if (buttonType == "Get button for SendChangeAddress")
            {
                NgWebElement uiSwaggerButton = ngDriver.FindElement(By.CssSelector(""));
                uiSwaggerButton.Click();
            }

            if (buttonType == "Get button for SendChangeName")
            {
                NgWebElement uiSwaggerButton = ngDriver.FindElement(By.CssSelector(""));
                uiSwaggerButton.Click();
            }

            if (buttonType == "Get button for SendChangeStatus")
            {
                NgWebElement uiSwaggerButton = ngDriver.FindElement(By.CssSelector(""));
                uiSwaggerButton.Click();
            }

            if (buttonType == "Get button for SendLicenceCreationMessage")
            {
                NgWebElement uiSwaggerButton = ngDriver.FindElement(By.CssSelector(""));
                uiSwaggerButton.Click();
            }

            if (buttonType == "Get button for SendProgramAccountDetailsBroadcastMessage")
            {
                NgWebElement uiSwaggerButton = ngDriver.FindElement(By.CssSelector(""));
                uiSwaggerButton.Click();
            }

            if (buttonType == "Get button for LdbExport")
            {
                NgWebElement uiSwaggerButton = ngDriver.FindElement(By.CssSelector(""));
                uiSwaggerButton.Click();
            }

            if (buttonType == "Try it out button")
            {
                NgWebElement uiSwaggerButton = ngDriver.FindElement(By.CssSelector(""));
                uiSwaggerButton.Click();
            }

            if (buttonType == "Execute button")
            {
                NgWebElement uiSwaggerButton = ngDriver.FindElement(By.CssSelector(""));
                uiSwaggerButton.Click();
            }
        }


        [And(@"I enter the licence GUID (.*)")]
        public void EnterLicenceGUID(string licenceGUID)
        {
            NgWebElement uiEnterGUID = ngDriver.FindElement(By.CssSelector(""));
            uiEnterGUID.SendKeys(licenceGUID);
        }


        [Then(@"the correct 200 response is displayed")]
        public void CorrectResponse()
        {
            // to be updated
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'200')]")).Displayed);
        }
    }
}
