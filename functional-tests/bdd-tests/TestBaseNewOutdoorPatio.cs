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
        [And(@"I request a new outdoor patio application")]
        public void NewOutdoorPatioApplication()
        {
            /* 
            Page Title: Please Review Your Account Profile
            */

            // click on Continue to Application button
            ContinueToApplicationButton();

            /* 
            Page Title: New Outdoor Patio
            */

            // create test data
            string patioCompDescription = "Sample patio composition description.";
            string patioLocationDescription = "Sample patio location description.";
            string patioAccessDescription = "Sample patio access description.";
            string patioLiquorCarriedDescription = "Sample liquor carried description.";
            string patioAccessControlDescription = "Sample patio access control description.";
            string areaDescription = "Sample area description";
            string occupantLoad = "180";

            // enter patio composition description
            NgWebElement uiPatioCompDescription = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioCompDescription.SendKeys(patioCompDescription);

            // enter patio location description
            NgWebElement uiPatioLocationDescription = ngDriver.FindElement(By.CssSelector("textarea#patioLocationDescription"));
            uiPatioLocationDescription.SendKeys(patioLocationDescription);

            // enter patio access description
            NgWebElement uiPatioAccessDescription = ngDriver.FindElement(By.CssSelector("textarea#patioAccessDescription"));
            uiPatioAccessDescription.SendKeys(patioAccessDescription);

            // click on Is Liquor Carried checkbox
            NgWebElement uiPatioIsLiquorCarried = ngDriver.FindElement(By.CssSelector("input#patioIsLiquorCarried[type='checkbox']"));
            uiPatioIsLiquorCarried.Click();

            // enter Is Liquor Carried description
            NgWebElement uiPatioLiquorCarriedDescription = ngDriver.FindElement(By.CssSelector("textarea#patioLiquorCarriedDescription"));
            uiPatioLiquorCarriedDescription.SendKeys(patioLiquorCarriedDescription);

            // enter patio access control description
            NgWebElement uiPatioAccessControlDescription = ngDriver.FindElement(By.CssSelector("textarea#patioAccessControlDescription"));
            uiPatioAccessControlDescription.SendKeys(patioAccessControlDescription);

            // select fixed patio service bar
            NgWebElement uiFixedPatioServiceBar = ngDriver.FindElement(By.CssSelector("#patioServiceBar button#mat-button-toggle-25-button[type='button']"));
            uiFixedPatioServiceBar.Click();

            // select portable liquor service bar
            NgWebElement uiPortableServiceBar = ngDriver.FindElement(By.CssSelector("#patioServiceBar button#mat-button-toggle-26-button[type='button']"));
            uiPortableServiceBar.Click();

            // select internal liquor service bar
            NgWebElement uiInternalServiceBar = ngDriver.FindElement(By.CssSelector("#patioServiceBar button#mat-button-toggle-27-button[type='button']"));
            uiInternalServiceBar.Click();

            // upload floor plan
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

            // upload exterior photos
            FileUpload("exterior_photos.pdf", "(//input[@type='file'])[5]");

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();
        }
    }
}
