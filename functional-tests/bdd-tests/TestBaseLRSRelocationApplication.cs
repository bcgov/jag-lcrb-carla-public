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
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the LRS application")]
        public void CompleteLRSApplication()
        {
            /* 
            Page Title: LRS Relocation Application
            */

            // create test data
            string proposedAddress = "645 Tyee Road";
            string proposedCity = "Victoria";
            string proposedPostalCode = "V9A 6X5";
            string proposedPID = "111111111";

            // enter the proposed address
            NgWebElement uiProposedAddress = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            uiProposedAddress.SendKeys(proposedAddress);

            // enter the proposed city
            NgWebElement uiProposedCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            uiProposedCity.SendKeys(proposedCity);

            // enter the proposed postal code
            NgWebElement uiProposedPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressPostalCode']"));
            uiProposedPostalCode.SendKeys(proposedPostalCode);

            // enter the proposed PID
            NgWebElement uiProposedPID = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentParcelId']"));
            uiProposedPID.SendKeys(proposedPID);

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the signage document
            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadSignage.SendKeys(signagePath);

            // upload the floor plan
            string floorPlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorPlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadFloorPlan.SendKeys(floorPlanPath);

            // upload the site plan
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uiUploadSitePlan.SendKeys(sitePlanPath);

            // upload the exterior photos
            string exteriorPhotosPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "exterior_photos.pdf");
            NgWebElement uiUploadExteriorPhotos = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
            uiUploadExteriorPhotos.SendKeys(exteriorPhotosPath);

            // select 'Yes' for proposed LRS site located within a grocery store
            NgWebElement uiProposedSiteInGrocery = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-1 button#mat-button-toggle-1-button"));
            uiProposedSiteInGrocery.Click();

            // upload grocery declaration document
            string groceryDeclarationPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "grocery_declaration.pdf");
            NgWebElement uiUploadGroceryDeclaration = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uiUploadGroceryDeclaration.SendKeys(groceryDeclarationPath);

            // select the owner checkbox
            NgWebElement uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            NgWebElement uiFutureValidInterest = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-4[formcontrolname='willhaveValidInterest']"));
            uiFutureValidInterest.Click();

            // upload valid interest document
            string validInterestPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "valid_interest.pdf");
            NgWebElement uiUploadValidInterest = ngDriver.FindElement(By.XPath("(//input[@type='file'])[18]"));
            uiUploadValidInterest.SendKeys(validInterestPath);

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on the submit for LG/IN review button
            ClickOnSubmitButton();
        }
    }
}
