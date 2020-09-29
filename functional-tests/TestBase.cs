using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using System;
using System.Diagnostics;
using System.Linq;

namespace FunctionalTest
{


    public abstract class TestBase
    {
        protected IConfigurationRoot configuration;

        protected Browser XrmTestBrowser;

        public bool HasData { get; set; }

        protected TestBase()
        {
            // clean up any previous test runs that might be stalled.

            /*
             var chromeProcesses = Process.GetProcessesByName("chromedriver");
            foreach (var process in chromeProcesses)
            {
                process.Kill();
            }
            */

            /*
            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets("dc6f3b78-5234-4b46-96e3-75849fde4479")
                .Build();*/

            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets("a004e634-29c7-48b6-becc-87fe16be7538")
                .Build();

            string xrmUriStr = configuration["D365_URL"] ?? "http://acme.crm.dynamics.com";
            string usernameStr = configuration["D365_USER"] ?? "admin@acme.onmicrosoft.com";
            string passwordStr = configuration["D365_PWD"] ?? "Password@12345";

            Uri xrmUri = new Uri(xrmUriStr);
            var username = usernameStr.ToSecureString();
            var password = passwordStr.ToSecureString();

            XrmTestBrowser = new Browser(new BrowserOptions()
            {
                BrowserType = BrowserType.Chrome,
                Headless = false,
                PrivateMode = true,
                EnableRecording = true,
                UserAgentValue = "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko",
                Height = 1080,
                Width = 1920
            });

            

            XrmTestBrowser.LoginPage.Login(xrmUri, username, password);

            // login via ADFS.
            var d = XrmTestBrowser.Driver;
            d.FindElement(By.Id("userNameInput")).SendKeys(username.ToUnsecureString());

            d.FindElement(By.Id("passwordInput")).SendKeys(password.ToUnsecureString());
            d.ClickWhenAvailable(By.Id("submitButton"), new TimeSpan(0, 0, 2));

            //Wait for CRM Page to load
            d.WaitUntilVisible(By.XPath(Elements.Xpath[Reference.Login.CrmMainPage])
                , new TimeSpan(0, 0, 60),
                e =>
                {
                    d.WaitForPageToLoad();
                    d.SwitchTo().Frame(0);
                    d.WaitForPageToLoad();
                });

        }

        public void CustomLoginAction(LoginRedirectEventArgs args)
        {
            //Login Page details go here.  You will need to find out the id of the password field on the form as well as the submit button. 
            //You will also need to add a reference to the Selenium Webdriver to use the base driver. 

            //Example
            //--------------------------------------------------------------------------------------
            var d = args.Driver;
            d.FindElement(By.Id("userNameInput")).SendKeys(args.Username.ToUnsecureString());

            d.FindElement(By.Id("passwordInput")).SendKeys(args.Password.ToUnsecureString());
            d.ClickWhenAvailable(By.Id("submitButton"), new TimeSpan(0, 0, 2));

            //Insert any additional code as required for the SSO scenario

            //Wait for CRM Page to load
            d.WaitUntilVisible(By.XPath(Elements.Xpath[Reference.Login.CrmMainPage])
                , new TimeSpan(0, 0, 60),
                e =>
                {
                    d.WaitForPageToLoad();
                    d.SwitchTo().Frame(0);
                    d.WaitForPageToLoad();
                });
            //--------------------------------------------------------------------------------------
        }

        protected void SetOptionSet(string id, string value)
        {
            OptionSet optionset = new OptionSet() { Name = id, Value = value };

            try
            {                
                XrmTestBrowser.Entity.SetValue(optionset);
            }
            catch (ElementClickInterceptedException)
            {
                // temporary fix for Dynamics form layout problems. 
                //JavaScriptClick($"{id}_i");
                //XrmTestBrowser.ThinkTime(100);

                
                XrmTestBrowser.Driver.ExecuteScript($"var selectObj = document.getElementById('{id}_i');"
                    + "for (var i=0; i<selectObj.options.length; i++){"
                    + $"if (selectObj.options[i].text == '{value}')"
                    + "{ selectObj.options[i].selected = true; }} selectObj.click(); "                    
                    );
                try
                {
                    XrmTestBrowser.Entity.SetValue(optionset);
                }
                catch (ElementClickInterceptedException)
                { }               
            }

            XrmTestBrowser.ThinkTime(500);

        }

        // Avoid Selenium nags about control intercepts
        protected void JavaScriptClick (string id)
        {            
            XrmTestBrowser.Driver.ExecuteScript($"document.getElementById(\"{id}\").click();");
        }


        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            XrmTestBrowser.Driver.Dispose();
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