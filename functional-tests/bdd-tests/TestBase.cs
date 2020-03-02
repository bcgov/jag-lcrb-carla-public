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
                options.AddArguments("headless", "no-sandbox", "disable-web-security",  "no-zygote", "disable-gpu");
            }
            
            driver = new ChromeDriver(path, options);
            
            //driver = new FirefoxDriver(FirefoxDriverService);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(10);  // max 10 seconds for any page, to allow for slow test servers

            ngDriver = new NgWebDriver(driver);

            

            baseUri = configuration["baseUri"] ?? "https://dev.justice.gov.bc.ca/cannabislicensing";
        }

        public void CarlaLogin()
        {
            // load the dashboard page
            string test_start = configuration["test_start"];
            
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");

            //ngDriver.WaitForAngular();

            //ngDriver.Navigate().GoToUrl($"{baseUri}");
            //NgWebElement butt = ngDriver.FindElement(By.XPath("(.//*[normalize-space(text()) and normalize-space(.)='OR'])[1]/following::strong[1]"));

            //butt.Click();

            //IWebElement username = driver.FindElement(By.Id("user"));
            //username.SendKeys(configuration["testUser1"]);
            //IWebElement password = driver.FindElement(By.Id("password"));

            //password.SendKeys(configuration["testPass1"]);

            //IWebElement sub = driver.FindElement(By.Name("btnSubmit"));

            //sub.Click();
        }

        public void CarlaDeleteCurrentAccount()
        {
            string deleteAccountURL = $"{baseUri}api/accounts/delete/current";
            string script = $"fetch(\"{deleteAccountURL}\", {{method: \"POST\", body: {{}}}})";

            ngDriver.ExecuteScript(script);

            // note that the above call to delete the account will take a period of time to execute.

            ngDriver.Navigate().GoToUrl($"{baseUri}logout");
        }
    }
}
