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
        [And(@"I request structural alterations to an approved lounge or special events area")]
        public void StructuralAlterations()
        {
            /* 
            Page Title: Licences
            */

            string structuralAlterations = "Structural Alterations to an Approved Lounge or Special Events Area";

            // click on the Structural Alterations Application link
            NgWebElement uiStructuralAlterations = ngDriver.FindElement(By.LinkText(structuralAlterations));
            uiStructuralAlterations.Click();

            ContinueToApplicationButton();

            // create test data
            string outdoorAreaDescription = "Sample outdoor area description";
            string outdoorAreaCapacity = "10";
            string capacityAreaOccupants = "20";

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the floor plan
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadFloorplan.SendKeys(floorplanPath);

            // add outside area
            NgWebElement uiOutdoorArea = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] button"));
            uiOutdoorArea.Click();

            // enter the outdooor area description
            NgWebElement uiOutdoorAreaDescription = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] input[formcontrolname='areaLocation']"));
            uiOutdoorAreaDescription.SendKeys(outdoorAreaDescription);

            // enter the outdoor area occupant load
            NgWebElement uiOutdoorAreaOccupantLoad = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] input[formcontrolname='capacity']"));
            uiOutdoorAreaOccupantLoad.SendKeys(outdoorAreaCapacity);

            // enter capacity area occupant load
            NgWebElement uiCapacityAreaOccupantLoad = ngDriver.FindElement(By.CssSelector("[formgroupname='capacityArea'] input[formcontrolname='capacity']"));
            uiCapacityAreaOccupantLoad.SendKeys(capacityAreaOccupants);

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
