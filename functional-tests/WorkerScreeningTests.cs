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

            string lastName = RandomizerFactory.GetRandomizer(new FieldOptionsLastName()).Generate() + "-WS-Test";

            

            // first create a contact.

            
            // 1. Go to Sales/Accounts using the Sitemap
            XrmTestBrowser.Navigation.OpenSubArea("Licensing", "Contacts");


            XrmTestBrowser.ThinkTime(100);

            //2. Click on the "New" button
            XrmTestBrowser.CommandBar.ClickCommand("New");

            XrmTestBrowser.ThinkTime(500);

            var fields = new List<Field>
            {
                new Field() {Id = "firstname", Value = firstName},
                new Field() {Id = "lastname", Value = lastName}
            };

            //3. Set the attribute values in the form
            XrmTestBrowser.Entity.SetValue(new CompositeControl() { Id = "fullname", Fields = fields });
            XrmTestBrowser.Entity.SetValue("emailaddress1", email);
            XrmTestBrowser.Entity.SetValue("mobilephone", "555-555-5555");
            XrmTestBrowser.Entity.SetValue("birthdate", DateTime.Parse("1/1/1970"));


            //4. Save the new record
            XrmTestBrowser.CommandBar.ClickCommand("Save");

            XrmTestBrowser.ThinkTime(100);

            // now create a worker verification

            // 3. Go to Sales/Accounts using the Sitemap
            XrmTestBrowser.Navigation.OpenSubArea("Compliance & Enforcement", "Worker Verifications");

            
            //5. Click on the "New" button
            XrmTestBrowser.CommandBar.ClickCommand("New");

            XrmTestBrowser.ThinkTime(2000);

            XrmTestBrowser.Entity.SetValue("adoxio_email", email);


            SetOptionSet("adoxio_ismanual", "Yes");
            SetOptionSet("adoxio_consentvalidated", "Yes");

            // Click on the contact field.
            XrmTestBrowser.ThinkTime(500);
            IWebElement contactDiv = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"adoxio_contactid\"]"));
            contactDiv.Click();

             // set the contact
             // Change the text field.
             IWebElement contactIdText = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"adoxio_contactid_ledit\"]"));
            contactIdText.SendKeys(firstName + " " + lastName);

            XrmTestBrowser.ThinkTime(500);
            IWebElement userOrTeamDiv = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"adoxio_contactid_i\"]"));
            userOrTeamDiv.Click();

            

            // click the search button.
            //IWebElement searchButton = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"adoxio_contactid_lookupSearch\"]"));

            //searchButton.Click();

            // find the first item and click on it.
            IWebElement firstItem = XrmTestBrowser.Driver.FindElement(By.XPath("//*[@id=\"item0\"]"));
            firstItem.Click();

            XrmTestBrowser.ThinkTime(1000);
           

            // switch back to the main frame.

            XrmTestBrowser.Driver.SwitchTo().ParentFrame();

            XrmTestBrowser.ThinkTime(500);

            // save the new record
            XrmTestBrowser.CommandBar.ClickCommand("Save");            
        }
    }
}