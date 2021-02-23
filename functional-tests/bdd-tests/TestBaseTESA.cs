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
        [And(@"I complete the TESA application for a (.*)")]
        public void CompleteTESAApplication(string applicationType)
        {
            /* 
            Page Title: Temporary Expanded Service Areas Application
            */

            // create test data
            string areaDescription = "Sample area description";
            string occupantLoad = "180";

            // upload floorplan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[3]");

            if (applicationType == "Manufacturer licence")
            {

                // click on service area button
                NgWebElement uiServiceAreas = ngDriver.FindElement(By.CssSelector("[formcontrolname= 'serviceAreas'] button"));
                uiServiceAreas.Click();

                // enter area description
                NgWebElement uiAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
                uiAreaDescription.SendKeys(areaDescription);

                // enter occupant load
                NgWebElement uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
                uiOccupantLoad.SendKeys(occupantLoad);
            }

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}
