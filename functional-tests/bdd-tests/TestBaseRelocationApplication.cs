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
        [And(@"I request a relocation application")]
        public void RelocationApplication()
        {
            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Application to Request Relocation
            */

            // create test data
            string areaDescription = "Sample area description";
            string occupantLoad = "180";

            // click zoning checkbox
            NgWebElement uiIsPermittedInZoning = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isPermittedInZoning']"));
            uiIsPermittedInZoning.Click();

            // select 'yes' for ALR location
            NgWebElement uiIsALR = ngDriver.FindElement(By.CssSelector("[formcontrolname='isAlr'] mat-radio-button#mat-radio-2"));
            uiIsALR.Click();

            // upload a floor plan
            FileUpload("floor_plan.pdf","(//input[@type='file'])[5]");

            // click on service area button
            NgWebElement uiServiceAreas = ngDriver.FindElement(By.CssSelector("[formcontrolname= 'serviceAreas'] button"));
            uiServiceAreas.Click();

            // enter area description
            NgWebElement uiAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
            uiAreaDescription.SendKeys(areaDescription);

            // enter occupant load
            NgWebElement uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiOccupantLoad.SendKeys(occupantLoad);

            // select the owner checkbox
            NgWebElement uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            NgWebElement uiFutureValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='willhaveValidInterest']"));
            uiFutureValidInterest.Click();

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
        }
    }
}
