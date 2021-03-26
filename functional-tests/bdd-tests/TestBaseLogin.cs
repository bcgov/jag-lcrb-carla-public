using System;
using System.Threading;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        public void Dispose()
        {
            ngDriver.Quit();

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }


        [And(@"I click on Home page")]
        public void ClickOnHomePage()
        {
            ngDriver.Navigate().GoToUrl($"{baseUri}");
            ngDriver.WaitForAngular();
        }

        private void DoLogin(string businessType)
        {
            ngDriver.Navigate().GoToUrl($"{baseUri}dashboard");

            /* 
            Page Title: Terms of Use
            */

            // select the acceptance checkbox
            var uiTermsOfUseCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1"));
            uiTermsOfUseCheckbox.Click();

            // click on the Continue button
            var uiContinueButton = ngDriver.FindElement(By.CssSelector("button.termsAccept"));
            uiContinueButton.Click();

            /* 
            Page Title: Please confirm the business or organization name associated to the Business BCeID.
            */

            // click on the Yes button
            var uiConfirmationButton = ngDriver.FindElement(By.CssSelector("button.confirmYes"));
            uiConfirmationButton.Click();

            /* 
            Page Title: Please confirm the organization type associated with the Business BCeID:
            */

            string businessTypeValue;
            switch (businessType.Trim())
            {
                case "co-op":
                    businessTypeValue = "Coop";
                    break;
                case "private corporation":
                    businessTypeValue = "PrivateCorporation";
                    break;
                case "public corporation":
                    businessTypeValue = "PrivateCorporation";
                    break;
                case "sole proprietorship":
                    businessTypeValue = "SoleProprietorship";
                    break;
                case "partnership":
                    businessTypeValue = "Partnership";
                    break;
                case "society":
                    businessTypeValue = "Society";
                    break;
                case "university":
                    businessTypeValue = "University";
                    break;
                case "n indigenous nation":
                    businessTypeValue = "IndigenousNation";
                    break;
                case "local government":
                    businessTypeValue = "LocalGovernment";
                    break;
                case "military mess":
                    businessTypeValue = "MilitaryMess";
                    break;
                default:
                    businessTypeValue = "ERROR - unknown business type.";
                    break;
            }

            var uiPartnershipRadio =
                ngDriver.FindElement(By.CssSelector($"[value='{businessTypeValue}'][type='radio']"));
            JavaScriptClick(uiPartnershipRadio);


            // click on the Next button
            var uiNextButton = ngDriver.FindElement(By.CssSelector("button.mat-primary"));
            uiNextButton.Click();

            /* 
            Page Title: Please confirm the name associated with the Business BCeID login provided.
            */

            // click on the Yes button
            var uiConfirmNameButton = FindFirstElementByCssWithRetry("button.btn-primary");
            uiConfirmNameButton.Click();
        }


        public void CarlaLogin(string businessType)
        {
            var random = new Random();
            var test_start = "login/token/AT" + DateTime.Now.Ticks + random.Next(0, 999);
            returnUser = test_start;
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");

            DoLogin(businessType);
        }


        public void CarlaLoginNoCheck(string businessType)
        {
            // load the dashboard page
            var test_start = configuration["test_start"];
            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");
            ngDriver.IgnoreSynchronization = false;

            DoLogin(businessType);
        }


        public void CarlaLoginWithUser(string businessType)
        {
            // load the dashboard page
            var test_start = configuration["test_start"];
            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");
            ngDriver.IgnoreSynchronization = false;

            DoLogin(businessType);
        }


        [And(@"I am logged in to the dashboard as a(.*)")]
        public void ViewDashboard(string businessType)
        {
            CarlaLoginNoCheck(businessType);
        }


        [Then(@"I see the login page")]
        public void SeeLogin()
        {
            Assert.True(ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }


        [And(@"I log in as local government for Parksville")]
        public void LocalGovernmentLogin()
        {
            Thread.Sleep(5000);

            var localGovt = "login/token/Parksville";
            ngDriver.Navigate().GoToUrl($"{baseUri}{localGovt}");
        }


        [And(@"I log in as a return user")]
        public void ReturnLogin()
        {
            ngDriver.Navigate().GoToUrl($"{baseUri}{returnUser}");
        }


        public void SignOut()
        {
            ClickOnLink("Sign Out");
        }


        [And(@"the account is deleted")]
        public void DeleteMyAccount()
        {
            CarlaDeleteCurrentAccount();
        }


        public void NavigateToFeatures()
        {
            ngDriver.IgnoreSynchronization = true;
            ngDriver.WrappedDriver.Navigate().GoToUrl($"{baseUri}api/features");
        }


        public void IgnoreSynchronizationFalse()
        {
            ngDriver.IgnoreSynchronization = false;
        }


        public void CheckFeatureFlag(string flag)
        {
            // confirm that the correct flag is enabled during this test
            var found = false;
            var maxTries = 10;
            var tries = 0;
            do
            {
                try
                {
                    found = ngDriver.FindElement(By.XPath($"//body[contains(.,'{flag}')]")).Displayed;
                }
                catch
                {
                    // wait for the feature list response
                    Thread.Sleep(500);
                }
            } while (!found && tries < maxTries);

            Assert.True(found);
        }


        public void CheckFeatureFlagsCOVIDTempExtension()
        {
            CheckFeatureFlag("CovidApplication");
        }


        public void CheckFeatureFlagsLiquorOne()
        {
            CheckFeatureFlag("LiquorOne");
        }


        public void CheckFeatureFlagsLiquorTwo()
        {
            CheckFeatureFlag("LiquorTwo");
        }


        public void CheckFeatureFlagsLiquorThree()
        {
            CheckFeatureFlag("LiquorThree");
        }


        public void CheckFeatureFlagsMaps()
        {
            CheckFeatureFlag("Maps");
        }


        public void CheckFeatureFlagsLGIN()
        {
            CheckFeatureFlag("LGApprovals");
        }


        public void CheckFeatureFlagsIN()
        {
            CheckFeatureFlag("IndigenousNation");
        }


        public void CheckFeatureFlagsLicenseeChanges()
        {
            CheckFeatureFlag("LicenseeChanges");
        }


        public void CheckFeatureFlagsSecurityScreening()
        {
            CheckFeatureFlag("SecurityScreening");
        }


        public void CheckFeatureFlagsMarketEvents()
        {
            CheckFeatureFlag("MarketEvents");
        }

        public void CheckFeatureLEConnections()
        {
            CheckFeatureFlag("LEConnections");
        }

        public void CarlaDeleteCurrentAccount()
        {
            ngDriver.IgnoreSynchronization = true;
            var tempTimeout = ngDriver.WrappedDriver.Manage().Timeouts().PageLoad;
            ngDriver.WrappedDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60 * 5);
            // using wrapped driver as this call is not angular
            ngDriver.Navigate().GoToUrl($"{baseUri}api/accounts/delete/current");

            ngDriver.IgnoreSynchronization = false;

            ngDriver.Navigate().GoToUrl($"{baseUri}logout");
            ngDriver.WrappedDriver.Manage().Timeouts().PageLoad = tempTimeout;
        }
    }
}