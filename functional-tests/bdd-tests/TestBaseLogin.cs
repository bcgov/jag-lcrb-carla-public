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
            NgWebElement uiTermsOfUseCheckbox = ngDriver.FindElement(By.CssSelector("input.terms-cb[type='checkbox']"));
            uiTermsOfUseCheckbox.Click();

            // click on the Continue button
            NgWebElement uiContinueButton = ngDriver.FindElement(By.CssSelector("button.termsAccept"));
            uiContinueButton.Click();

            /* 
            Page Title: Please confirm the business or organization name associated to the Business BCeID.
            */

            // click on the Yes button
            NgWebElement uiConfirmationButton = ngDriver.FindElement(By.CssSelector("button.confirmYes"));
            uiConfirmationButton.Click();

            /* 
            Page Title: Please confirm the organization type associated with the Business BCeID:
            */

            // if this is a private corporation, click the radio button
            if (businessType == " private corporation")
            {
                NgWebElement uiPrivateCorporationRadio = ngDriver.FindElement(By.CssSelector("input[value='PrivateCorporation'][type = 'radio']"));
                uiPrivateCorporationRadio.Click();
            }

            // if this is a public corporation, click the radio button
            if (businessType == " public corporation")
            {
                NgWebElement uiPublicCorporationRadio = ngDriver.FindElement(By.CssSelector("[value='PublicCorporation'][type='radio']"));
                uiPublicCorporationRadio.Click();
            }

            // if this is a sole proprietorship, click the radio button
            if (businessType == " sole proprietorship")
            {
                NgWebElement uiSoleProprietorshipRadio = ngDriver.FindElement(By.CssSelector("[value='SoleProprietorship'][type='radio']"));
                uiSoleProprietorshipRadio.Click();
            }

            // if this is a partnership, click the radio button
            if (businessType == " partnership")
            {
                NgWebElement uiPartnershipRadio = ngDriver.FindElement(By.CssSelector("[value='Partnership'][type='radio']"));
                uiPartnershipRadio.Click();
            }

            // if this is a society, click the radio button
            if (businessType == " society")
            {
                NgWebElement uiSocietyRadio = ngDriver.FindElement(By.CssSelector("[type='radio'][value='Society']"));
                uiSocietyRadio.Click();
            }

            // if this is a university, click the radio button
            if (businessType == " university")
            {
                NgWebElement uiUniversityRadio = ngDriver.FindElement(By.CssSelector("[type='radio'][value='University']"));
                uiUniversityRadio.Click();
            }

            // if this is an indigenous nation, click the radio button
            if (businessType == "n indigenous nation")
            {
                NgWebElement uiIndigenousNationRadio = ngDriver.FindElement(By.CssSelector("[value='IndigenousNation'][type='radio']"));
                uiIndigenousNationRadio.Click();
            }

            // if this is a local government, click the radio button
            if (businessType == " local government")
            {
                NgWebElement uiLocalGovernmentRadio = ngDriver.FindElement(By.CssSelector("[value='LocalGovernment'][type='radio']"));
                uiLocalGovernmentRadio.Click();
            }

            // click on the Next button
            NgWebElement uiNextButton = ngDriver.FindElement(By.CssSelector(".btn-primary"));
            uiNextButton.Click();

            /* 
            Page Title: Please confirm the name associated with the Business BCeID login provided.
            */

            // click on the Yes button
            NgWebElement uiConfirmNameButton = ngDriver.FindElement(By.CssSelector("app-bceid-confirmation .btn-primary"));
            uiConfirmNameButton.Click();
        }


        public void CarlaLogin(string businessType)
        {
            Random random = new Random();
            var tempTimeout = ngDriver.WrappedDriver.Manage().Timeouts().PageLoad;
            ngDriver.WrappedDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60 * 5);
            // load the dashboard page
            string test_start = "login/token/AT" + DateTime.Now.Ticks.ToString() + random.Next(0, 999).ToString();
            returnUser = test_start;
            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");
            ngDriver.IgnoreSynchronization = false;

            DoLogin(businessType);

            ngDriver.WrappedDriver.Manage().Timeouts().PageLoad = tempTimeout;
        }


        public void CarlaLoginNoCheck(string businessType)
        {
            // load the dashboard page
            string test_start = configuration["test_start"];
            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");
            ngDriver.IgnoreSynchronization = false;

            DoLogin(businessType);
        }


        public void CarlaLoginWithUser(string businessType)
        {
            // load the dashboard page
            string test_start = configuration["test_start"];
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


        [And(@"I log in as local government for Saanich")]
        public void LocalGovernmentLogin()
        {
            string localGovt = "login/token/Saanich";
            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"{baseUri}{localGovt}");
            ngDriver.IgnoreSynchronization = false;

            System.Threading.Thread.Sleep(2000);
        }


        [And(@"I log in as a return user")]
        public void ReturnLogin()
        {
            System.Threading.Thread.Sleep(2000);

            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"{baseUri}{returnUser}");
            ngDriver.IgnoreSynchronization = false;

            System.Threading.Thread.Sleep(2000);
        }


        public void SignOut()
        {
            ClickOnLink("Sign Out");
        }


        public void Dispose()
        {
            ngDriver.Quit();

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }


        [And(@"the account is deleted")]
        public void DeleteMyAccount()
        {
            this.CarlaDeleteCurrentAccount();
        }


        public void NavigateToFeatures()
        {
            ngDriver.IgnoreSynchronization = true;

            // navigate to the feature flags page
            ngDriver.WrappedDriver.Navigate().GoToUrl($"{baseUri}api/features");
            System.Threading.Thread.Sleep(1000);
        }


        public void IgnoreSynchronizationFalse()
        {
            ngDriver.IgnoreSynchronization = false;
        }


        public void CheckFeatureFlag(string flag)
        {
            // confirm that the correct flag is enabled during this test
            bool found;
            try
            {
                found = ngDriver.FindElement(By.XPath($"//body[contains(.,'{flag}')]")).Displayed;
            }
            catch (Exception) // handle cases where nginx did not show features.
            {
                ngDriver.WrappedDriver.Navigate().GoToUrl($"{baseUri}api/features");
                System.Threading.Thread.Sleep(5000);
                found = ngDriver.FindElement(By.XPath($"//body[contains(.,'{flag}')]")).Displayed;
            }

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
