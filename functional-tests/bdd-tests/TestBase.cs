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
        // Protractor driver
        protected NgWebDriver ngDriver;

        protected IConfigurationRoot configuration;

        protected string baseUri;

        protected string applicationID;

        protected string licenceID;

        protected string endorsementID;

        protected string returnUser;

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
                options.AddArguments("headless", "no-sandbox", "disable-web-security", "no-zygote", "disable-gpu", "disable-dev-shm-usage", "disable-infobars", "start-maximized", "hide-scrollbars", "window-size=1920,1080");
                if (!string.IsNullOrEmpty(configuration["CHROME_BINARY_LOCATION"]))
                {
                    options.BinaryLocation = configuration["CHROME_BINARY_LOCATION"];
                }
            }
            else
            {
                options.AddArguments("start-maximized");
            }

            // setup ChromeDriver with a command timeout of 3 minutes.
            var driver = new ChromeDriver(path, options, TimeSpan.FromMinutes(3));

            double timeout = 200.0;

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout * 2);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);

            ngDriver = new NgWebDriver(driver);

            ngDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);
            ngDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
            ngDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout * 2);

            baseUri = configuration["baseUri"] ?? "https://dev.justice.gov.bc.ca/cannabislicensing";

        }

        protected bool IsIdPresent(string id)
        {
            bool result = true;
            try
            {
                var x = ngDriver.FindElements(By.Id(id));
                if (x.Count == 0)
                {
                    result = false;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;

        }
    }
}
