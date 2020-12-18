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
        [And(@"I submit a Food Primary structural change application")]
        public void FPStructuralChange()
        {
            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Food Primary Structural Change Application
            */

            // create test data
            string description1 = "Test automation outline of the proposed change.";
            string description2 = "Test automation patio location.";
            string areaDescription = "Test automation area description.";
            string occupantLoad = "180";

            // enter the description of the change
            NgWebElement uiDescriptionOfChange = ngDriver.FindElement(By.CssSelector("textarea#description1"));
            uiDescriptionOfChange.SendKeys(description1);

            // enter the location of the patio
            NgWebElement uiPatioLocation = ngDriver.FindElement(By.CssSelector("textarea#Description2"));
            uiPatioLocation.SendKeys(description2);

            // upload a floor plan document
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[2]");

            // click on service area button
            NgWebElement uiServiceAreas = ngDriver.FindElement(By.CssSelector("[formcontrolname= 'serviceAreas'] button"));
            uiServiceAreas.Click();

            // enter area description
            NgWebElement uiAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
            uiAreaDescription.SendKeys(areaDescription);

            // enter occupant load
            NgWebElement uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiOccupantLoad.SendKeys(occupantLoad);

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

            /* 
            Page Title: Payment Approved
            */

            ClickLicencesTab();
        }
    }
}
