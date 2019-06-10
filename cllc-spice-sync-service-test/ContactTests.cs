using Microsoft.Dynamics365.UIAutomation.Api;
using System;
using System.Collections.Generic;
using Xunit;

namespace CllcSpiceSyncServiceTest
{
    public class ContactTests : TestBase
    {
        [Fact]
        public void CreateContact()
        {
            XrmTestBrowser.ThinkTime(1000);
            // 3. Go to Sales/Accounts using the Sitemap
            XrmTestBrowser.Navigation.OpenSubArea("Licensing", "Contacts");

            XrmTestBrowser.ThinkTime(500);
            // 4. Change the active view
            XrmTestBrowser.Grid.SwitchView("Active Contacts");

            XrmTestBrowser.ThinkTime(500);
            //5. Click on the "New" button
            XrmTestBrowser.CommandBar.ClickCommand("New");

            XrmTestBrowser.ThinkTime(2000);

            var fields = new List<Field>
            {
                new Field() {Id = "firstname", Value = "Test"},
                new Field() {Id = "lastname", Value = "Contact"}
            };
            //6. Set the attribute values in the form
            XrmTestBrowser.Entity.SetValue(new CompositeControl() { Id = "fullname", Fields = fields });
            XrmTestBrowser.Entity.SetValue("emailaddress1", "test@contoso.com");
            XrmTestBrowser.Entity.SetValue("mobilephone", "555-555-5555");
            XrmTestBrowser.Entity.SetValue("birthdate", DateTime.Parse("11/1/1980"));


            //7. Save the new record
            XrmTestBrowser.CommandBar.ClickCommand("Save");            
        }
    }
}