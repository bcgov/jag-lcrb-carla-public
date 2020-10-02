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

            // upload the signage document
            FileUpload("signage.pdf","(//input[@type='file'])[2]");

            // upload the floor plan
            FileUpload("floor_plan.pdf","(//input[@type='file'])[5]");

            // upload the site plan
            FileUpload("site_plan.pdf","(//input[@type='file'])[8]");

            // upload the exterior photos
            FileUpload("exterior_photos.jpg","(//input[@type='file'])[11]");

            // select 'Yes' for proposed LRS site located within a grocery store
            NgWebElement uiProposedSiteInGrocery = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-1 button#mat-button-toggle-1-button"));
            uiProposedSiteInGrocery.Click();

            // upload grocery declaration document
            FileUpload("grocery_declaration.pdf","(//input[@type='file'])[15]");

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
            FileUpload("valid_interest.pdf","(//input[@type='file'])[18]");

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on the submit for LG/IN review button
            ClickOnSubmitButton();
        }


        [And(@"an LRS licence has been created")]
        public void LRSApproval()
        {
            // TODO
            // To be completed when workflow is ready
        }
    }
}
