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

namespace bdd_tests
{
    public abstract class TestBase : Feature
    {
        protected RemoteWebDriver driver;
        // Protractor driver
        protected NgWebDriver ngDriver;

        //protected FirefoxDriverService driverService;

        protected IConfigurationRoot configuration;

        protected string baseUri;

        protected TestBase()
        {
            string path = Directory.GetCurrentDirectory();

            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets("a004e634-29c7-48b6-becc-87fe16be7538")
                .Build();

            //bool runlocal = true;
            ChromeOptions options = new ChromeOptions();
            // run headless when in CI
            if (!string.IsNullOrEmpty(configuration["OPENSHIFT_BUILD_COMMIT"]) || !string.IsNullOrEmpty(configuration["Build.BuildNumber"]))
            {
                Console.Out.WriteLine("Enabling Headless Mode");
                options.AddArguments("headless", "no-sandbox", "disable-web-security", "no-zygote", "disable-gpu");
            }
            else
            {
                options.AddArguments("--start-maximized");
            }

            driver = new ChromeDriver(path, options);

            //driver = new FirefoxDriver(FirefoxDriverService);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(60);

            ngDriver = new NgWebDriver(driver);

            baseUri = configuration["baseUri"] ?? "https://dev.justice.gov.bc.ca/cannabislicensing";
        }

        public void CarlaLoginNoCheck(string businessType)
        {
            // load the dashboard page
            string test_start = configuration["test_start"];

            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");

            ngDriver.WaitForAngular();
        }

        public void CarlaLogin(string businessType)
        {
            // load the dashboard page
            string test_start = configuration["test_start"];

            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");

            ngDriver.WaitForAngular();

            /* 
            Page Title: Terms of Use
            */

            // select the acceptance checkbox
            NgWebElement termsOfUseCheckbox = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            termsOfUseCheckbox.Click();

            // click on the Continue button
            NgWebElement continueButton = ngDriver.FindElement(By.XPath("//section[3]/button"));
            continueButton.Click();

            /* 
            Page Title: Please confirm the business or organization name associated to the Business BCeID.
            */

            // click on the Yes button
            NgWebElement confirmationButton = ngDriver.FindElement(By.XPath("//button"));
            confirmationButton.Click();

            /* 
            Page Title: Please confirm the organization type associated with the Business BCeID:
            */

            // if this is a private corporation, click the radio button
            if (businessType == "private corporation")
            {
                NgWebElement privateCorporationRadio = ngDriver.FindElement(By.Name("InitialBusinessType"));
                privateCorporationRadio.Click();
            }

            // if this is a public corporation, click the radio button
            if (businessType == "public corporation")
            {
                NgWebElement publicCorporationRadio = ngDriver.FindElement(By.XPath("(//input[@name='InitialBusinessType'])[2]"));
                publicCorporationRadio.Click();
            }

            // if this is a sole proprietorship, click the radio button
            if (businessType == "sole proprietorship")
            {
                NgWebElement soleProprietorshipRadio = ngDriver.FindElement(By.XPath("(//input[@name='InitialBusinessType'])[3]"));
                soleProprietorshipRadio.Click();
            }

            // if this is a partnership, click the radio button
            if (businessType == "partnership")
            {
                NgWebElement partnershipRadio = ngDriver.FindElement(By.XPath("(//input[@name='InitialBusinessType'])[4]"));
                partnershipRadio.Click();
            }

            // if this is a society, click the radio button
            if (businessType == "society")
            {
                NgWebElement societyRadio = ngDriver.FindElement(By.XPath("(//input[@name='InitialBusinessType'])[5]"));
                societyRadio.Click();
            }

            // if this is an indigenous nation, click the radio button
            if (businessType == "indigenous nation")
            {
                NgWebElement indigenousNationRadio = ngDriver.FindElement(By.XPath("(//input[@name='InitialBusinessType'])[6]"));
                indigenousNationRadio.Click();
            }

            // click on the Next button
            NgWebElement nextButton = ngDriver.FindElement(By.XPath("//button[contains(.,'Next')]"));
            nextButton.Click();

            /* 
            Page Title: Please confirm the name associated with the Business BCeID login provided.
            */

            // click on the Yes button
            NgWebElement confirmNameButton = ngDriver.FindElement(By.XPath("//button"));
            confirmNameButton.Click();
        }

        public void MakePayment()
        {
            string testCC = configuration["test_cc"];
            string testCVD = configuration["test_ccv"];

            System.Threading.Thread.Sleep(7000);

            //browser sync - don't wait for Angular
            ngDriver.IgnoreSynchronization = true;

            /* 
            Page Title: Internet Payments Program (Bambora)
            */

            driver.FindElementByName("trnCardNumber").SendKeys(testCC);

            driver.FindElementByName("trnCardCvd").SendKeys(testCVD);

            driver.FindElementByName("submitButton").Click();

            System.Threading.Thread.Sleep(3000);

            //turn back on when returning to Angular
            ngDriver.IgnoreSynchronization = false;
        }
        public void CarlaDeleteCurrentAccount()
        {
            
            string deleteAccountURL = $"{baseUri}api/accounts/delete/current";
            string script = $"return fetch(\"{deleteAccountURL}\", {{method: \"POST\", body: {{}}}})";

            var  deleteResult = ngDriver.ExecuteScript(script);
            var obj = JsonConvert.SerializeObject(deleteResult);
            var json = JsonConvert.DeserializeObject<Dictionary<string,object>>(obj);
            //bool success = (Int64)json["status"] == 404 || (Newtonsoft.Json.Linq.JObject)json["text"] == new Newtonsoft.Json.Linq.JObject("OK");
            //bool success = (Int64)json["status"] != 500;
            //Assert.True(success);

            // note that the above call to delete the account will take a period of time to execute.            
            ngDriver.Navigate().GoToUrl($"{baseUri}logout");
        }
    }
}
