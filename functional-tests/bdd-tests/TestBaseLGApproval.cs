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
        [And(@"I specify that the zoning allows the endorsement")]
        public void ZoningAllowsEndorsement()
        {
            // create test data
            string appsForReview = "Applications for Review";
            string reviewApp = "Review Application";

            // click on Applications for Review link
            ClickOnLink(appsForReview);

            // click on Review Application link
            ClickOnLink(reviewApp);

            // select 'Allows' for zoning confirmation
            NgWebElement uiAllowsZoning = ngDriver.FindElement(By.CssSelector("[formcontrolname='lgZoning'] mat-radio-button#mat-radio-2"));
            uiAllowsZoning.Click();
        }


        [And(@"I specify my contact details")]
        public void SpecifyContactDetails()
        {
            // create test data
            string nameOfOfficial = "Official Name";
            string title = "Title";
            string phone = "1811811818";
            string email = "test@automation.com";
            string zoningComments = "Sample zoning comments.";

            // enter the name of the official
            NgWebElement uiOfficialName = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'lGNameOfOfficial']"));
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

            // enter the zoning comments
            NgWebElement uiZoningComments = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='lGDecisionComments']"));
            uiZoningComments.SendKeys(zoningComments);

            // click on the Submit button
            ClickOnSubmitButton();
        }


        [And(@"I review the local government response for (.*)")]
        public void ReviewLocalGovernment(string responseType)
        {
            if (responseType == "a picnic area endorsement")
            {

                // create test data
                string completeApplication = "Complete Application";

                // click on Complete Application link
                ClickOnLink(completeApplication);

                ContinueToApplicationButton();

                Assert.True(ngDriver.FindElement(By.XPath($"//body[contains(.,'Manufacturer Picnic Area Endorsement Application ')]")).Displayed);

                ClickOnSubmitButton();
            }
        }


        [And(@"the dashboard status is updated as (.*)")]
        public void DashboardStatus(string status)
        {
            if (status == "Application Under Review")
            {
                //System.Threading.Thread.Sleep(4000);

                Assert.True(ngDriver.FindElement(By.XPath($"//body[contains(.,' Application Under Review ')]")).Displayed);

                Assert.True(ngDriver.FindElement(By.XPath($"//body[contains(.,'Add Supporting Documents')]")).Displayed);
            }
        }
    }
}