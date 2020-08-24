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
        [And(@"I request a structural change")]
        public void RequestStructuralChange()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            string structuralChange = "Request a Structural Change";

            // click on the request structural change link
            NgWebElement uiStructuralChange = ngDriver.FindElement(By.LinkText(structuralChange));
            uiStructuralChange.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Submit the Cannabis Retail Store Structural Change Application
            */

            // create test data
            string description = "Test automation outline of the proposed change.";

            // enter the description of the change
            NgWebElement uiDescriptionOfChange = ngDriver.FindElement(By.Id("description1"));
            uiDescriptionOfChange.SendKeys(description);

            // select not visible from outside checkbox
            NgWebElement uiVisibleFromOutside = ngDriver.FindElement(By.CssSelector(".mat-checkbox-inner-container"));
            uiVisibleFromOutside.Click();

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a floor plan document
            string floorPlan = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiFloorPlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiFloorPlan.SendKeys(floorPlan);

            // select 'no' for changes to entries
            NgWebElement uiChangeToEntries = ngDriver.FindElement(By.Id("mat-button-toggle-2-button"));
            uiChangeToEntries.Click();

            // select authorizedToSubmit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // select signatureAgreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            // pay for the structural change application
            MakePayment();

            System.Threading.Thread.Sleep(4000);

            /* 
            Page Title: Payment Approved
            */

            // confirm correct payment amount
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$440.00')]")).Displayed);

            ClickLicencesTab();
        }


        [And(@"I confirm the structural change request is displayed on the dashboard")]
        public void RequestedStructuralChangeOnDashboard()
        {
            ClickOnDashboard();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // confirm that a structural change request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Structural Change')]")).Displayed);
        }
    }
}
