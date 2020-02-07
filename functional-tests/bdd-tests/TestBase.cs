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

            //bool runlocal = true;

            driver = new ChromeDriver(path);
            //driver = new FirefoxDriver(FirefoxDriverService);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(30);

            ngDriver = new NgWebDriver(driver);

            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets("a004e634-29c7-48b6-becc-87fe16be7538")
                .Build();

            baseUri = configuration["baseUri"] ?? "https://dev.justice.gov.bc.ca/cannabislicensing";
        }

        public void CarlaLogin()
        {
            // load the dashboard page
            string test_start = configuration["test_start"];
            
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");

            ngDriver.WaitForAngular();

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
    }
}
