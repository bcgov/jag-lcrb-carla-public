using Microsoft.Dynamics365.UIAutomation.Api;
using Microsoft.Dynamics365.UIAutomation.Browser;


using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Xunit;
using Microsoft.Extensions.Configuration;


namespace CllcSpiceSyncServiceTest
{
    public class ContactTests
    {

        private IConfigurationRoot attrs;

        public ContactTests()
        {
            attrs = new ConfigurationBuilder()                
                .AddEnvironmentVariables()
                .AddUserSecrets("dc6f3b78-5234-4b46-96e3-75849fde4479")
                .Build();
            

        }

        [Fact]
        public void CreateContact()
        {
            // start by getting secrets.
            


            // 1. Create instance of the browser
            using (var xrmBrowser = new Browser(new BrowserOptions() {
                BrowserType = BrowserType.Chrome,
                Headless = false,
                PrivateMode = true,
                EnableRecording = true

            }))
            {
                var urlStr = attrs["D365_URL"] ?? "http://acme.crm.dynamics.com";
                var userNameStr = attrs["D365_USER"] ?? "admin@acme.onmicrosoft.com";
                var pwdStr = attrs["D365_PWD"] ?? "Password@12345";
                Console.WriteLine($"Login to {urlStr}...");

                var url = new Uri(urlStr);
                var userName = userNameStr.ToSecureString();
                var pwd = pwdStr.ToSecureString();

                // 2. Log-in to Dynamics 365
                xrmBrowser.LoginPage.Login(url, userName, pwd);
                xrmBrowser.ThinkTime(200);
                if (xrmBrowser.Driver.IsVisible(By.XPath(Elements.Xpath[Reference.Login.StaySignedIn])))
                {
                    xrmBrowser.Driver.ClickWhenAvailable(By.XPath(Elements.Xpath[Reference.Login.StaySignedIn]));
                    xrmBrowser.Driver.FindElement(By.XPath(Elements.Xpath[Reference.Login.StaySignedIn])).Submit();
                }

                xrmBrowser.ThinkTime(1000);
                // 3. Go to Sales/Accounts using the Sitemap
                xrmBrowser.Navigation.OpenSubArea("Licensing", "Contacts");

                xrmBrowser.ThinkTime(500);
                // 4. Change the active view
                xrmBrowser.Grid.SwitchView("Active Contacts");

                xrmBrowser.ThinkTime(500);
                //5. Click on the "New" button
                xrmBrowser.CommandBar.ClickCommand("New");

                xrmBrowser.ThinkTime(2000);

                var fields = new List<Field>
                {
                    new Field() {Id = "firstname", Value = "Test"},
                    new Field() {Id = "lastname", Value = "Contact"}
                };
                //6. Set the attribute values in the form
                xrmBrowser.Entity.SetValue(new CompositeControl() { Id = "fullname", Fields = fields });
                xrmBrowser.Entity.SetValue("emailaddress1", "test@contoso.com");
                xrmBrowser.Entity.SetValue("mobilephone", "555-555-5555");
                xrmBrowser.Entity.SetValue("birthdate", DateTime.Parse("11/1/1980"));
                

                //7. Save the new record
                xrmBrowser.CommandBar.ClickCommand("Save");
            }
        }
    }
}