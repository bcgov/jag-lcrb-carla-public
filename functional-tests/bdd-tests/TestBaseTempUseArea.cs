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
        [And(@"I submit a temporary use area endorsement application")]
        public void TempUseAreaApplication()
        {
            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Temporary Use Area Endorsement at a Ski Hill or Golf Course Application
            */

            // create test data
            string patioCompDescription = "Patio comp description.";
            string patioAccessControlDescription = "Patio access control description.";
            string patioLiquorCarriedDescription = "Patio liquor carried description.";
            string removeIntoxicatedPatrons = "Removal of intoxicated patrons description.";
            string respectForNeighbours = "Respect for neighbours description.";
            string areaDescription = "Area description.";
            string occupantLoad = "180";

            // enter patio comp description
            NgWebElement uiPatioCompDescription = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioCompDescription.SendKeys(patioCompDescription);

            // enter patio access control description
            NgWebElement uiPatioAccessControlDescription = ngDriver.FindElement(By.CssSelector("textarea#patioAccessControlDescription"));
            uiPatioAccessControlDescription.SendKeys(patioAccessControlDescription);

            // enter patio liquor carried description
            NgWebElement uiPatioLiquorCarriedDescription = ngDriver.FindElement(By.CssSelector("textarea#patioLiquorCarriedDescription"));
            uiPatioLiquorCarriedDescription.SendKeys(patioLiquorCarriedDescription);

            // enter removal of intoxicated patrons description
            NgWebElement uiRemoveIntoxicatedPatrons = ngDriver.FindElement(By.CssSelector("textarea#description1"));
            uiRemoveIntoxicatedPatrons.SendKeys(removeIntoxicatedPatrons);

            // enter respect for neighbours
            NgWebElement uiRespectForNeighbours = ngDriver.FindElement(By.CssSelector("textarea#Description2"));
            uiRespectForNeighbours.SendKeys(respectForNeighbours);

            // click Fixed option
            NgWebElement uiFixedOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-25-button"));
            uiFixedOption.Click();

            // click Portable option
            NgWebElement uiPortableOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-26-button"));
            uiPortableOption.Click();

            // click Interior option
            NgWebElement uiInteriorOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-27-button"));
            uiInteriorOption.Click();

            // select the outside areas button
            NgWebElement uiOutsideAreas = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] button"));
            uiOutsideAreas.Click();

            // enter the area description
            NgWebElement uiAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
            uiAreaDescription.SendKeys(areaDescription);

            // enter the occupant load
            NgWebElement uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiOccupantLoad.SendKeys(occupantLoad);

            // upload the signage document
            FileUpload("signage.pdf", "(//input[@type='file'])[2]");

            // upload the exterior photos
            FileUpload("exterior_photos.pdf", "(//input[@type='file'])[5]");

            // upload the supporting documents
            FileUpload("associates.pdf", "(//input[@type='file'])[8]");

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();
        }
    }
}
