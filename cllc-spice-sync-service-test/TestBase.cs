using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Security;

using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using OpenQA.Selenium.Support.Events;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;



using OpenQA.Selenium;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace CllcSpiceSyncServiceTest
{


    public abstract class TestBase
    {
        protected IConfigurationRoot attrs;

        protected Browser XrmTestBrowser;

        public bool HasData { get; set; }

        protected TestBase()
        {
            attrs = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets("dc6f3b78-5234-4b46-96e3-75849fde4479")
                .Build();

            string xrmUriStr = attrs["D365_URL"] ?? "http://acme.crm.dynamics.com";
            string usernameStr = attrs["D365_USER"] ?? "admin@acme.onmicrosoft.com";
            string passwordStr = attrs["D365_PWD"] ?? "Password@12345";

            Uri xrmUri = new Uri(xrmUriStr);
            var username = usernameStr.ToSecureString();
            var password = passwordStr.ToSecureString();

            XrmTestBrowser = new Browser(new BrowserOptions()
            {
                BrowserType = BrowserType.Chrome,
                Headless = false,
                PrivateMode = true,
                EnableRecording = true
            });

            XrmTestBrowser.LoginPage.Login(xrmUri, username, password);

            XrmTestBrowser.ThinkTime(200);
            if (XrmTestBrowser.Driver.IsVisible(By.XPath(Elements.Xpath[Reference.Login.StaySignedIn])))
            {
                XrmTestBrowser.Driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Login.StaySignedIn]));
                XrmTestBrowser.Driver.FindElement(By.XPath(Elements.Xpath[Reference.Login.StaySignedIn])).Submit();
            }

            XrmTestBrowser.GuidedHelp.CloseGuidedHelp();
        }

        
        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            XrmTestBrowser.Dispose();
        }
        
       

        public void OpenEntity(string area, string subArea, string view = null)
        {
            XrmTestBrowser.ThinkTime(500);
            XrmTestBrowser.Navigation.OpenSubArea(area, subArea);
            if (view != null) XrmTestBrowser.Grid.SwitchView(view);
        }

        public bool OpenEntityGrid()
        {
            var entityGrid = XrmTestBrowser.Grid.GetGridItems(20).Value;
            if (entityGrid?.FirstOrDefault() != null)
            {
                XrmTestBrowser.Entity.OpenEntity(entityGrid[0].EntityName, entityGrid[0].Id);
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}