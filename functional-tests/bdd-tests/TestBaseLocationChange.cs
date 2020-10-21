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
        [And(@"I request a location change")]
        public void LocationChange()
        {
            /* 
            Page Title: Licences & Authorizations
            */

            string locationChange = "Location Change Application";

            // click on the Location Change Application link
            NgWebElement uiLocationChange = ngDriver.FindElement(By.LinkText(locationChange));
            uiLocationChange.Click();

            ContinueToApplicationButton();

            // create test data
            string additionalPIDs = "012345678, 343434344";
            string proposedChanges = "Details of proposed changes.";

            // enter additional PIDs
            NgWebElement uiAdditionalPIDs = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='pidList']"));
            uiAdditionalPIDs.SendKeys(additionalPIDs);

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1"));
            uiZoningCheckbox.Click();

            // enter the proposed changes
            NgWebElement uiProposedChanges = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='description1']"));
            uiProposedChanges.SendKeys(proposedChanges);

            // upload the signage document
            FileUpload("signage.pdf","(//input[@type='file'])[2]");

            // upload the floor plan
            FileUpload("floor_plan.pdf","(//input[@type='file'])[5]");

            // upload the site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[8]");

            // upload the exterior photos
            FileUpload("exterior_photos.jpg","(//input[@type='file'])[11]");

            // upload the ownership details
            FileUpload("ownership_details.pdf","(//input[@type='file'])[15]");

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            MakePayment();


        }
    }
}
