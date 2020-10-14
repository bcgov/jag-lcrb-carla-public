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
        [And(@"I request a personnel name change for a (.*)")]
        public void RequestPersonnelNameChange(string businessType)
        {
            if (businessType != "indigenous nation")
            {
                /* 
                Page Title: Welcome to Liquor and Cannabis Licensing
                */

                // click on the review organization information button
                ClickReviewOrganizationInformation();

                /* 
                Page Title: [client name] Legal Entity Structure
                */

                // click on the Edit button for Leader (partnership, sole proprietorship, private corporation, or society)
                if (businessType == "partnership" || businessType == "sole proprietorship" || businessType == "private corporation" || businessType == "society")
                {
                    NgWebElement uiEditInfoButtonShared = ngDriver.FindElement(By.CssSelector(".fas.fa-pencil-alt span"));
                    uiEditInfoButtonShared.Click();
                }

                // click on the Edit button for Leader (public corporation)
                if (businessType == "public corporation")
                {
                    NgWebElement uiEditInfoButton = ngDriver.FindElement(By.CssSelector("td:nth-child(7) .ng-star-inserted"));
                    uiEditInfoButton.Click();
                }

                // enter a new name for the director
                string newDirectorFirstName = "UpdatedFirstName";
                string newDirectorLastName = "UpdatedLastName";

                NgWebElement uiNewDirectorFirstName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
                uiNewDirectorFirstName.Clear();
                uiNewDirectorFirstName.SendKeys(newDirectorFirstName);

                NgWebElement uiNewDirectorLasttName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastNameNew']"));
                uiNewDirectorLasttName.Clear();
                uiNewDirectorLasttName.SendKeys(newDirectorLastName);

                // click on the Confirm button
                NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector(".fa-save span"));
                uiConfirmButton.Click();

                // upload a marriage certificate document
                if (businessType == "public corporation" || businessType == "partnership")
                {
                    FileUpload("marriage_certificate.pdf","(//input[@type='file'])[12]");
                }

                if (businessType == "private corporation")
                {
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[18]");
                }

                if ((businessType == "society") || (businessType == "sole proprietorship"))
                {
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[9]");
                }

                // click on Submit Organization Information button
                NgWebElement uiSubmitOrgStructure = ngDriver.FindElement(By.CssSelector("button.btn-primary[name='submit-application']"));
                uiSubmitOrgStructure.Click();

                MakePayment();
            }
        }


        [And(@"I confirm the correct personnel name change fee for a (.*)")]
        public void PersonnelNameChangeFee(string applicationType)
        {
            /* 
            Page Title: Payment Approved
            */

            if (applicationType == "Cannabis licence")
            {
                // check Cannabis name change fee
                // Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$500.00')]")).Displayed);
            }

            if (applicationType == "Catering licence")
            {
                // check Catering name change fee
                // Assert.True(ngDriver.WrappedDriver.FindElement(By.XPath("//body[contains(.,'$220.00')]")).Displayed);
            }
        }


        [And(@"I change a personnel email address for a (.*)")]
        public void RequestPersonnelEmailChange(string businessType)
        {
            if (businessType != "indigenous nation")
            {
                /* 
                Page Title: Welcome to Liquor and Cannabis Licensing
                */

                // click on the review organization information button
                ClickReviewOrganizationInformation();

                /* 
                Page Title: [client name] Legal Entity Structure
                */

                // click on the Edit button for Leader (partnership, sole proprietorship, public corporation, or society)
                if (businessType == "partnership" || businessType == "sole proprietorship" || businessType == "public corporation" || businessType == "society")
                {
                    NgWebElement uiEditInfoButtonPartner = ngDriver.FindElement(By.CssSelector(".fas.fa-pencil-alt span"));
                    uiEditInfoButtonPartner.Click();
                }

                // click on the Edit button for Leader (private corporation)
                if (businessType == "private corporation")
                {
                    NgWebElement uiEditInfoButton = ngDriver.FindElement(By.CssSelector("td:nth-child(7) .ng-star-inserted"));
                    uiEditInfoButton.Click();
                }

                // enter a new email for the associate
                string newEmail = "newemail@test.com";

                NgWebElement uiNewEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
                uiNewEmail.Clear();
                uiNewEmail.SendKeys(newEmail);

                // click on the Confirm button
                NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector(".fa-save span"));
                uiConfirmButton.Click();

                // click on Confirm Organization Information is Complete button
                NgWebElement uiOrgInfoButton2 = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
                uiOrgInfoButton2.Click();

                /* 
                Page Title: Welcome to Liquor and Cannabis Licensing
                */

                // check that dashboard is displayed (i.e. no payment has been required)
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Welcome to Liquor and Cannabis Licensing')]")).Displayed);
            }
        }
    }
}
