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
            Page Title: Licences
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

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the signage document
            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadSignage.SendKeys(signagePath);

            // upload the floor plan
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadFloorplan.SendKeys(floorplanPath);

            // upload the site plan
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uiUploadSitePlan.SendKeys(sitePlanPath);

            // upload the exterior photos
            string exteriorPhotosPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "exterior_photos.jpg");
            NgWebElement uiUploadExteriorPhotos = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
            uiUploadExteriorPhotos.SendKeys(exteriorPhotosPath);

            // upload the ownership details
            string ownershipDetailsPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "ownership_details.pdf");
            NgWebElement uiownershipDetails = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uiownershipDetails.SendKeys(ownershipDetailsPath);

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
