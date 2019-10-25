using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
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

        protected IConfigurationRoot configuration;

        protected string baseUri;

        protected TestBase()
        {
            string path = Directory.GetCurrentDirectory();

            driver = new ChromeDriver(path);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(5);

            ngDriver = new NgWebDriver(driver);

            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets("dc6f3b78-5234-4b46-96e3-75849fde4479")
                .Build();

            baseUri = configuration["baseUri"] ?? "https://dev.justice.gov.bc.ca/cannabislicensing";            
        }
    }
}
