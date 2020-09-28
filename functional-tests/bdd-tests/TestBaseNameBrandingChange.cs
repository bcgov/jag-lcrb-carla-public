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
        /*
        [And(@"I confirm the name or branding change is displayed on the dashboard")]
        public void RequestedNameChangeOnDashboard()
        {
            ClickOnDashboard();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            

            // confirm that a name or branding change request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name or Branding Change')]")).Displayed);
        }
        */


        [And(@"I request a valid store name or branding change for (.*)")]
        public void RequestNameBrandingChange(string changeType)
        {
            /* 
            Page Title: Licences
            */

            string nameBrandingLinkCannabis = "Request Store Name or Branding Change";
            string nameBrandingLinkCateringMfg = "Establishment Name Change Application";

            if ((changeType == "Catering") || (changeType == "Manufacturing"))
            {
                // click on the Establishment Name Change Application link
                NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText(nameBrandingLinkCateringMfg));
                uiRequestChange.Click();
            }

            if (changeType == "Cannabis")
            {
                // click on the Request Store Name or Branding Change link
                NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText(nameBrandingLinkCannabis));
                uiRequestChange.Click();
            }

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /*
            Page Title: Submit a Name or Branding Change Application
            */

            // click on the authorized to submit checkbox
            NgWebElement uiAuthSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSigAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSigAgreement.Click();

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a supporting document
            string supportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSupportingDoc = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadSupportingDoc.SendKeys(supportingDocument);

            if (changeType == "Cannabis")
            {
                // click on the store exterior change button	
                NgWebElement uiStoreExterior = ngDriver.FindElement(By.Id("mat-button-toggle-2-button"));
                uiStoreExterior.Click();
            }

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            // pay for the branding change application
            MakePayment();

            /* 
            Page Title: Payment Approved
            */

            // confirm correct payment amount	
            // Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$220.00')]")).Displayed);
        }
    }
}
