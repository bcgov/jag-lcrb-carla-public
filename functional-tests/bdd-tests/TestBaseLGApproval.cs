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
        [And(@"I specify my contact details as the approving authority for (.*)")]
        public void SpecifyContactDetails(string applicationType)
        {
            /* 
            Page Title: Provide Confirmation of Zoning
            */

            // create test data
            string nameOfOfficial = "Official Name";
            string title = "Title";
            string phone = "1811811818";
            string email = "test@automation.com";

            // enter the name of the official
            NgWebElement uiOfficialName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lGNameOfOfficial']"));
            uiOfficialName.SendKeys(nameOfOfficial);

            // enter the official's title
            NgWebElement uiOfficialTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lGTitlePosition']"));
            uiOfficialTitle.SendKeys(title);

            // enter the official's phone number
            NgWebElement uiOfficialPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lGContactPhone']"));
            uiOfficialPhone.SendKeys(phone);

            // enter the official's email
            NgWebElement uiOfficialEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lGContactEmail']"));
            uiOfficialEmail.SendKeys(email);

            // upload the supporting reports
            if (applicationType == "liquor primary")
            {
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[14]");

                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[17]");
            }
            else if (applicationType == "live theatre")
            {
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[5]");
            }
            else
            {
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[11]");
            }
        }


        [And(@"I review the local government response for (.*)")]
        public void ReviewLocalGovernment(string responseType)
        {
            if (responseType == "a picnic area endorsement")
            {
                ContinueToApplicationButton();

                /* 
                Page Title: Manufacturer Picnic Area Endorsement Application (Sent to LG/IN)
                */

                Assert.True(ngDriver.FindElement(By.XPath($"//body[contains(.,'Manufacturer Picnic Area Endorsement Application ')]")).Displayed);

                ClickOnSubmitButton();
            }
        }
    }
}