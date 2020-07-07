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

namespace bdd_tests
{
    public abstract class TestBaseWorker : Feature
    {
        protected RemoteWebDriver driver;
        
        // Protractor driver
        protected NgWebDriver ngDriver;

        protected IConfigurationRoot configuration;

        protected string baseUri;

        protected TestBaseWorker()
        {
            string path = Directory.GetCurrentDirectory();

            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets("a004e634-29c7-48b6-becc-87fe16be7538")
                .Build();

            ChromeOptions options = new ChromeOptions();
            
            // run headless when in CI
            if (!string.IsNullOrEmpty(configuration["OPENSHIFT_BUILD_COMMIT"]) || !string.IsNullOrEmpty(configuration["Build.BuildNumber"]))
            {
                Console.Out.WriteLine("Enabling Headless Mode");
                options.AddArguments("headless", "no-sandbox", "disable-web-security",  "no-zygote", "disable-gpu");
            }
            else
            {
                options.AddArguments("--start-maximized");
            }

            driver = new ChromeDriver(path, options);
            
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(60);

            ngDriver = new NgWebDriver(driver);

            baseUri = configuration["baseUri"] ?? "https://dev.justice.gov.bc.ca/cannabislicensing";
        }

        public void CarlaLoginWorkerNoTerms()
        {
            // load the dashboard page
            string test_start = configuration["test_start_worker"];

            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");

            ngDriver.WaitForAngular();
            
        }

        public void CarlaLoginWorker()
        {
            Random random = new Random();

            string test_start = "bcservice/token/AT" + DateTime.Now.Ticks.ToString() + random.Next(0, 999).ToString();

            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");

            ngDriver.WaitForAngular();

            /* 
            Page Title: Terms of Use
            */

            NgWebElement uiCheckBox1 = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiCheckBox1.Click();

            NgWebElement uiCheckBox2 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[2]"));
            uiCheckBox2.Click();

            NgWebElement continueButton = ngDriver.FindElement(By.CssSelector(".buttons .btn-primary"));
            continueButton.Click();

            /* 
            Page Title: Please confirm the name belonging to the BC Services card you provided
            */

            NgWebElement yesButton = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
            yesButton.Click();
        }

        public void MakeWorkerPayment()
        {
            /* 
            Page Title: Internet Payments Program (Bambora)
            */

            string testCC = configuration["test_cc"];
            string testCVD = configuration["test_ccv"];

            System.Threading.Thread.Sleep(7000);

            ngDriver.IgnoreSynchronization = true;

            // enter the test credit card number
            driver.FindElementByName("trnCardNumber").SendKeys(testCC);

            // enter the test credit card CVD number
            driver.FindElementByName("trnCardCvd").SendKeys(testCVD);

            // click on the Pay Now button
            driver.FindElementByName("submitButton").Click();

            System.Threading.Thread.Sleep(2000);

            ngDriver.IgnoreSynchronization = false;
        }

        public void CarlaDeleteCurrentAccount()
        {
            string deleteAccountURL = $"{baseUri}api/accounts/delete/current";
            string script = $"fetch(\"{deleteAccountURL}\", {{method: \"POST\", body: {{}}}})";

            ngDriver.ExecuteScript(script);

            // note that the above call to delete the account will take a period of time to execute.

            ngDriver.Navigate().GoToUrl($"{baseUri}logout");
        }

        [And(@"the account is deleted")]
        public void Delete_my_account()
        {
            this.CarlaDeleteCurrentAccount();
        }
    }
}
