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

        public void MakePayment()
        {
            string testCC = configuration["test_cc"];
            string testCVD = configuration["test_ccv"];

            var tempWait = ngDriver.Manage().Timeouts().ImplicitWait;
            ngDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
            ngDriver.WrappedDriver.FindElement(By.Name("trnCardNumber")).SendKeys(testCC);

            ngDriver.WrappedDriver.FindElement(By.Name("trnCardCvd")).SendKeys(testCVD);

            ngDriver.WrappedDriver.FindElement(By.Name("submitButton")).Click();

            System.Threading.Thread.Sleep(2000);
            ngDriver.Manage().Timeouts().ImplicitWait = tempWait;

            
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


        [And(@"I do not complete Step 1 of the application")]
        public void Step1NotCompleted()
        {
            // click on link to add previous name
            // NgWebElement uiPreviousNameLink = ngDriver.FindElement(By.CssSelector("div:nth-child(3) span a"));
            // uiPreviousNameLink.Click();

            // click on the Save & Continue to Step 2 button to generate errors
            NgWebElement submitButton = ngDriver.FindElement(By.CssSelector("span .btn-primary"));
            submitButton.Click();
        }


        [And(@"the Step 1 error messages are displayed")]
        public void Step1ErrorMessagesDisplayed()
        {
            // check that missing city and country of birth error message is thrown
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' The city and country of your birth is required. ')]")).Displayed);

            // check that missing BCID/DL error messages are thrown
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' You must enter either a BCID number or a driver’s licence number. ')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Format is 9 digits for BCID and 9 digits for driver')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s licence number. ')]")).Displayed);

            // check that missing phone number error messages are thrown
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'(incl. area code) example: 1234567890')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'This field cannot contain letters.')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please provide a 10-digit phone number without a country code, spaces or special characters.*')]")).Displayed);

            // check that missing email error messages are thrown
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please include an “@” in the email address.')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Your address must include a domain at the end such as .com, .ca, or .net.')]")).Displayed);

            // check that missing postal code error message is thrown
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Your postal / zip code should be in one of the following formats:')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'X1X1X1, 12345, 12345-1234. ')]")).Displayed);

            // check that general error message is thrown
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Some required fields have not been completed. ')]")).Displayed);
        }


        [And(@"I do not complete Step 2 of the application")]
        public void Step2NotCompleted()
        {
            // click on the Save & Pay button to generate errors
            NgWebElement submitButton = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
            submitButton.Click();
        }


        [And(@"the Step 2 error messages are displayed")]
        public void Step2ErrorMessagesDisplayed()
        {
            // check that missing consent error message is thrown
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Please provide your consent to proceed -- a security screening is required in order to become a qualified cannabis worker. ')]")).Displayed);

            // check that missing required fields error message is thrown
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Please make sure that you have completed all required fields. ')]")).Displayed);
        }
    }
}
