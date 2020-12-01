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
        [And(@"I request a facility structural change")]
        public void FacilityStructuralChange()
        {
            /* 
            Page Title: Licences & Authorizations
            */

            string facilityStructuralChange = "Facility Structural Change Application";

            // click on the Facility Structural Change Application link
            NgWebElement uiFacilityStructuralChange = ngDriver.FindElement(By.LinkText(facilityStructuralChange));
            uiFacilityStructuralChange.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Manufacturing Facility Structural Change Application
            */

            // create test data
            string applicationDetails = "Sample application details.";
            string proposedChange = "Sample proposed change";

            // enter the application details into the text area
            NgWebElement uiApplicationDetails = ngDriver.FindElement(By.CssSelector("textarea#otherBusinessesDetails"));
            uiApplicationDetails.SendKeys(applicationDetails);

            // enter the proposed change into the text area
            NgWebElement uiProposedChange = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='description1']"));
            uiProposedChange.SendKeys(proposedChange);

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // upload the floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[2]");

            // upload the site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[5]");

            uiAuthorizedToSubmit.Click();

            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            MakePayment();


        }
    }
}
