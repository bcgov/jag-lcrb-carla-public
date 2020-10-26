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
        [And(@"I request an on-site store endorsement")]
        public void OnSiteStoreEndorsement()
        {
            /* 
            Page Title: Licences & Authorizations
            */

            string onSiteStoreEndorsement = "On-Site Store Endorsement Application";

            // click on the On-Site Store Endorsement Application link
            NgWebElement uiOnSiteStoreEndorsement = ngDriver.FindElement(By.LinkText(onSiteStoreEndorsement));
            uiOnSiteStoreEndorsement.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Manufacturer On-Site Store Endorsement Application
            */

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isPermittedInZoning']"));
            uiZoningCheckbox.Click();

            // upload the floor plan
            FileUpload("floor_plan.pdf","(//input[@type='file'])[2]");

            // upload the site plan
            FileUpload("site_plan.pdf","(//input[@type='file'])[5]");

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
