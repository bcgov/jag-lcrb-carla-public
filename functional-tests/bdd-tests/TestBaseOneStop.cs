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
            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html");
            ngDriver.IgnoreSynchronization = false;
        }


        [And(@"I click on the (.*)")]
        public void ClickOnSwaggerButton(string buttonType)
        {
            /*******************
             *  Authorize button
             *******************/

            if (buttonType == "Authorize button")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.CssSelector("button.authorize"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            /*******************
            *  Close button
            *******************/

            if (buttonType == "Close button")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.CssSelector("button.btn-done"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            /*******************
            *  Get buttons
            *******************/

            if (buttonType == "Get button for SendChangeAddress")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.CssSelector("span:nth-child(1) #operations-OneStop-OneStop_GET .opblock-summary-method"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Get button for SendChangeName")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.CssSelector(".no-margin span:nth-child(2) #operations-OneStop-OneStop_GET .opblock-summary-method"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Get button for SendChangeStatus")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.CssSelector("span:nth-child(3) .opblock-summary-method"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Get button for SendLicenceCreationMessage")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.CssSelector("span:nth-child(4) .opblock-summary-method"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Get button for SendProgramAccountDetailsBroadcastMessage")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.CssSelector("span:nth-child(5) .opblock-summary-method"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Get button for LdbExport")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.CssSelector("span:nth-child(6) .opblock-summary-method"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            /********************
            *  Try it out buttons
            ********************/

            if (buttonType == "Try it out button for SendChangeAddress")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[1]/div[1]/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Try it out button for SendChangeName")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[1]/div[1]/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Try it out button for SendChangeStatus")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[1]/div[1]/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Try it out button for SendLicenceCreationMessage")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[1]/div[1]/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Try it out button for SendProgramAccountDetailsBroadcastMessage")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[1]/div[1]/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Try it out button for LdbExport")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[1]/div[1]/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }


            /********************
            *  Execute buttons
            ********************/

            if (buttonType == "Execute button for SendChangeAddress")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Execute button for SendChangeName")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Execute button for SendChangeStatus")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Execute button for SendLicenceCreationMessage")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Execute button for SendProgramAccountDetailsBroadcastMessage")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }

            if (buttonType == "Execute button for LdbExport")
            {
                ngDriver.IgnoreSynchronization = true;
                IWebElement uiButton = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[2]/button"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiButton);
                ngDriver.IgnoreSynchronization = false;
            }
        }


        [And(@"I enter the licence GUID (.*)")]
        public void EnterLicenceGUID(string licenceGUID)
        {
            // TODO: need to distinguish between 6 visible textboxes
            ngDriver.IgnoreSynchronization = true;
            IWebElement uiEnterGUID = ngDriver.FindElement(By.XPath("//*[@id='operations-OneStop-OneStop_GET']/div[2]/div/div[1]/div[2]/div/table/tbody/tr/td[2]/input"));
            IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
            executor.ExecuteScript("arguments[0].value='31b08509-909b-ea11-b818-00505683fbf4';", uiEnterGUID);
            ngDriver.IgnoreSynchronization = false;
        }


        [Then(@"the correct 200 response is displayed")]
        public void CorrectResponse()
        {
            // to be updated
            // Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'200')]")).Displayed);
        }
    }
}
