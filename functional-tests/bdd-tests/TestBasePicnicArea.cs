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
        [And(@"I request a picnic area endorsement")]
        public void PicnicAreaEndorsement()
        {
            /* 
            Page Title: Licences
            */

            string picnicAreaEndorsement = "Picnic Area Endorsement Application";

            // click on the Picnic Area Endorsement Application link
            NgWebElement uiPicnicAreaEndorsement = ngDriver.FindElement(By.LinkText(picnicAreaEndorsement));
            uiPicnicAreaEndorsement.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Manufacturer Picnic Area Endorsement Application
            */

            // create test data
            string proposedChange = "Description of proposed change(s) such as moving, adding or changing approved picnic area(s)";
            string otherBizDetails = "Description of other business details";
            string patioCompositionDescription = "Description of patio composition";
            string capacity = "100";

            // enter the description of the proposed change
            NgWebElement uiProposedChange = ngDriver.FindElement(By.CssSelector("textarea#description1"));
            uiProposedChange.SendKeys(proposedChange);

            // enter the other business details
            NgWebElement uiOtherBizDetails = ngDriver.FindElement(By.CssSelector("textarea#otherBusinessesDetails"));
            uiOtherBizDetails.SendKeys(otherBizDetails);

            // enter the patio composition description
            NgWebElement uiPatioCompDesc = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioCompDesc.SendKeys(patioCompositionDescription);

            // select 'Grass' for patio location
            NgWebElement uiGrass = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-43-button"));
            uiGrass.Click();

            // select 'Earth' for patio location
            NgWebElement uiEarth = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-44-button"));
            uiEarth.Click();

            // select 'Gravel' for patio location
            NgWebElement uiGravel = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-45-button"));
            uiGravel.Click();

            // select 'Finished Flooring' for patio location
            NgWebElement uiFinishedFlooring = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-46-button"));
            uiFinishedFlooring.Click();

            // select 'Cement Sidewalk' for patio location
            NgWebElement uiCementSidewalk = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-47-button"));
            uiCementSidewalk.Click();

            // select 'Other' for patio location
            NgWebElement uiOther = ngDriver.FindElement(By.CssSelector("button#mat-button-toggle-48-button"));
            uiOther.Click();
            
            // enter the capacity
            NgWebElement uiCapacity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiCapacity.Clear();
            uiCapacity.SendKeys(capacity);

            // upload the site plan
            FileUpload("site_plan.pdf","(//input[@type='file'])[2]");

            // upload the exterior photos
            FileUpload("exterior_photos.jpg","(//input[@type='file'])[5]");

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgree.Click();

            ClickOnSubmitButton();

            //System.Threading.Thread.Sleep(3000);
        }
    }
}
