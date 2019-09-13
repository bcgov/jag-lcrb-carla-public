using Microsoft.Dynamics365.UIAutomation.Api;
using System;
using System.Collections.Generic;
using Xunit;
using RandomDataGenerator;
using RandomDataGenerator.Randomizers;
using RandomDataGenerator.FieldOptions;
using OpenQA.Selenium;

namespace FunctionalTest
{
    public class WorkerScreeningTests : TestBase
    {
        [Fact]
        public void CreateWorkerScreening()
        {
            string email = RandomizerFactory.GetRandomizer(new FieldOptionsEmailAddress()).Generate() + ".xom";

            string firstName = RandomizerFactory.GetRandomizer(new FieldOptionsFirstName()).Generate();

            string lastName = RandomizerFactory.GetRandomizer(new FieldOptionsLastName()).Generate();

            

            // first create a contact.

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
                new Field() {Id = "firstname", Value = firstName},
                new Field() {Id = "lastname", Value = lastName}
            };
            //6. Set the attribute values in the form
            XrmTestBrowser.Entity.SetValue(new CompositeControl() { Id = "fullname", Fields = fields });
            XrmTestBrowser.Entity.SetValue("emailaddress1", email);
            XrmTestBrowser.Entity.SetValue("mobilephone", "555-555-5555");
            XrmTestBrowser.Entity.SetValue("birthdate", DateTime.Parse("1/1/1970"));


            //7. Save the new record
            XrmTestBrowser.CommandBar.ClickCommand("Save");

            // now create an email

            XrmTestBrowser.ThinkTime(1000);
            // 3. Go to Sales/Accounts using the Sitemap
            XrmTestBrowser.Navigation.OpenSubArea("Compliance & Enforcement", "Worker Verifications");

            XrmTestBrowser.ThinkTime(500);
            // 4. Change the active view
            XrmTestBrowser.Grid.SwitchView("Active Worker Verifications");

            XrmTestBrowser.ThinkTime(500);
            //5. Click on the "New" button
            XrmTestBrowser.CommandBar.ClickCommand("New");

            XrmTestBrowser.ThinkTime(2000);

            XrmTestBrowser.Entity.SetValue("adoxio_ismanual", true);

            XrmTestBrowser.Entity.SetValue("adoxio_email", email);

            // set the contact

            XrmTestBrowser.ThinkTime(500);
            IWebElement userOrTeamDiv = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"adoxio_contactid_lookupValue\"]"));
            userOrTeamDiv.Click();

            // Change the text field.
            IWebElement userOrTeamText = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"adoxio_contactid_ledit\"]"));
            userOrTeamText.SendKeys(configuration["APPLICATION_ASSIGNEE"]);

            // click the search button.
            IWebElement searchButton = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"adoxio_contactid_lookupSearch\"]"));

            searchButton.Click();

            // find the first item and click on it.
            IWebElement firstItem = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"item0\"]"));
            firstItem.Click();

            XrmTestBrowser.ThinkTime(1000);

            // click the OK button.
            try
            {
                XrmTestBrowser.Driver.FindElement(By.XPath("//button[@id=\"ok_id\"]")).Click();
            }
            catch (StaleElementReferenceException)
            { // ignore the stale element, we have moved on. 
            }

            // switch back to the main frame.

            XrmTestBrowser.Driver.SwitchTo().ParentFrame();

            XrmTestBrowser.ThinkTime(500);

            // save the new record
            XrmTestBrowser.CommandBar.ClickCommand("Save");            
        }
    }
}