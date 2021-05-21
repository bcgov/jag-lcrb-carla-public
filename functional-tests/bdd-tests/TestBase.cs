using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Protractor;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        protected string applicationID;
        protected string baseUri;
        protected IConfigurationRoot configuration;
        protected string endorsementID;

        protected string licenceID;

        // Protractor driver
        protected NgWebDriver ngDriver;
        protected string returnUser;

        protected TestBase()
        {
            var path = Directory.GetCurrentDirectory();

            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets("a004e634-29c7-48b6-becc-87fe16be7538")
                .Build();

            //bool runlocal = true;
            var options = new ChromeOptions();
            // run headless when in CI
            if (!string.IsNullOrEmpty(configuration["OPENSHIFT_BUILD_COMMIT"]) ||
                !string.IsNullOrEmpty(configuration["Build.BuildNumber"]))
            {
                Console.Out.WriteLine("Enabling Headless Mode");
                // could try --shm-size=1gb "disable-dev-shm-usage"
                options.AddArguments("headless", "no-sandbox", "disable-web-security", "no-zygote", "disable-gpu",
                    "disable-dev-shm-usage", "disable-infobars", "start-maximized", "hide-scrollbars",
                    "window-size=1920,1080");
                if (!string.IsNullOrEmpty(configuration["CHROME_BINARY_LOCATION"]))
                    options.BinaryLocation = configuration["CHROME_BINARY_LOCATION"];
            }
            else
            {
                options.AddArguments("start-maximized");
            }

            // setup ChromeDriver with a command timeout of 2 minutes.
            var driver = new ChromeDriver(path, options, TimeSpan.FromMinutes(2));

            // var timeout = 45.0;
            // temp change to explore pipeline impact
            var timeout = 45.0;

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
            //driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout * 2);
            //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);

            ngDriver = new NgWebDriver(driver);

            //ngDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);
            ngDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
            //ngDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout * 2);

            baseUri = configuration["baseUri"] ?? "https://dev.justice.gov.bc.ca/lcrb";
        }

        protected bool IsIdPresent(string id)
        {
            var result = true;
            try
            {
                var x = ngDriver.FindElements(By.Id(id));
                if (x.Count == 0) result = false;
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        protected void ScrollToElement(NgWebElement element)
        {
            var executor = (IJavaScriptExecutor) ngDriver.WrappedDriver;
            executor.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        /// <summary>
        ///     Helper function to populate the Local Government / Indigenous Nation and Police Jurisdiction fields.
        ///     This function is centralized as the same code is used on all application types.
        ///     Past issues that have been attempted to be resolved with this code include the item being found but not clicked on
        ///     properly.
        /// </summary>
        /// <param name="lgin">Name of the lgin.  Defaults to "Parksville" in most cases</param>
        /// <param name="police">Name of the police jurisdiction</param>
        private void FillLginAndPolice(string lgin, string police)
        {
            // search for and select Parksville as the local government
            var uiIndigenousNation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='indigenousNation']"));
            uiIndigenousNation.SendKeys(lgin);

            var uiIndigenousNation2 = FindFirstElementByCssWithRetry("span[class='mat-option-text']");
            if (uiIndigenousNation2 != null) JavaScriptClick(uiIndigenousNation2);

            // search for and select Oceanside RCMP as the police jurisdiction
            var uiPoliceJurisdiction =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='policeJurisdiction']"));
            uiPoliceJurisdiction.SendKeys(police);

            var uiPoliceJurisdiction2 = FindFirstElementByCssWithRetry("span[class='mat-option-text']");
            if (uiIndigenousNation2 != null) JavaScriptClick(uiPoliceJurisdiction2);
        }

        /// <summary>
        ///     Find a given css selector, with a retry.  Useful for cases where a given control may not have been loaded at the
        ///     time the selector is used.
        /// </summary>
        /// <param name="cssSelector">The Css Selector</param>
        /// <returns>The found element, or null if not found.</returns>
        protected NgWebElement FindFirstElementByCssWithRetry(string cssSelector)
        {
            NgWebElement result = null;
            var retry = 5;
            while (retry > 0)
            {
                var elements = ngDriver.FindElements(By.CssSelector(cssSelector));
                if (elements != null && elements.Count > 0)
                {
                    result = elements[0];
                    retry = 0;
                }
                else
                {
                    retry--;
                }
            }

            if (result == null)
                // attempt to save a screenshot.
                try
                {
                    ((ITakesScreenshot) ngDriver.WrappedDriver).GetScreenshot().SaveAsFile("error.png");
                }
                catch (Exception)
                {
                    // ignore any errors that occur when saving the screenshot.
                }

            return result;
        }
    }
}