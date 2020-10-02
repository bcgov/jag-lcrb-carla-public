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
        [And(@"I request a store relocation for (.*)")]
        public void RequestStoreRelocation(string applicationType)
        {
            /* 
            Page Title: Licences
            */

            string requestRelocationLink = "Request Relocation";

            // click on the request location link
            NgWebElement uiRequestRelocation = ngDriver.FindElement(By.LinkText(requestRelocationLink));
            uiRequestRelocation.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Submit a Licence Relocation Application
            */

            if (applicationType == "Cannabis")
            {

                string proposedAddress = "Automated Test Street";
                string proposedCity = "Victoria";
                string proposedPostalCode = "V9A 6X5";
                string pid = "012345678";

                // enter the proposed street address
                NgWebElement uiProposedAddress = ngDriver.FindElement(By.CssSelector(".ngtest-new-address input[formcontrolname='establishmentAddressStreet']"));
                uiProposedAddress.SendKeys(proposedAddress);

                // enter the proposed city
                NgWebElement uiProposedCity = ngDriver.FindElement(By.CssSelector(".ngtest-new-address input[formcontrolname='establishmentAddressCity']"));
                uiProposedCity.SendKeys(proposedCity);

                // enter the postal code
                NgWebElement uiProposedPostalCode = ngDriver.FindElement(By.CssSelector(".ngtest-new-address input[formcontrolname='establishmentAddressPostalCode']"));
                uiProposedPostalCode.SendKeys(proposedPostalCode);

                // enter the PID
                NgWebElement uiProposedPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
                uiProposedPID.SendKeys(pid);
            }

            // upload a supporting document
            FileUpload("checklist.pdf","(//input[@type='file'])[2]");

            // select the authorized to submit checkbox
            NgWebElement uiAuthToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSigAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSigAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            // pay for the relocation application
            MakePayment();

            if (applicationType == "Cannabis")
            {
                /* 	
                Page Title: Payment Approved	
                */

                // confirm correct payment amount
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$220.00')]")).Displayed);

                // return to the Licences tab
                ClickLicencesTab();
            }
        }

        [And(@"I confirm the relocation request is displayed on the dashboard")]
        public void RequestedRelocationOnDashboard()
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // confirm that relocation request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Relocation Request')]")).Displayed);
        }
    }
}
