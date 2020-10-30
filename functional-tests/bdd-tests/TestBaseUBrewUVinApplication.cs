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
        [And(@"I complete the UBrew / UVin application for (.*)")]
        public void UBrewUVinApplication()
        {
            string establishmentName = "Point Ellis Greenhouse";
            string streetAddress = "645 Tyee Road";
            string city = "Victoria";
            string postalCode = "V9A6X5";
            string PID = "999999999";
            string storeEmail = "auto@test.com";
            string storePhone = "2222222222";
            string contactTitle = "Brewmaster";

            // enter the establishment name
            NgWebElement uiProposedLicensee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiProposedLicensee.SendKeys(establishmentName);

            // enter the street address
            NgWebElement uiProposedLicensee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiProposedLicensee.SendKeys(streetAddress);

            // enter the city
            NgWebElement uiProposedLicensee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiProposedLicensee.SendKeys(city);

            // enter the postal code
            NgWebElement uiProposedLicensee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiProposedLicensee.SendKeys(postalCode);

            // enter the PID
            NgWebElement uiProposedLicensee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiProposedLicensee.SendKeys(PID);

            // enter the store email
            NgWebElement uiProposedLicensee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiProposedLicensee.SendKeys(storeEmail);

            // enter the store phone
            NgWebElement uiProposedLicensee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiProposedLicensee.SendKeys(storePhone);

            // upload the signage document

            // select

            // select

            // select

            // enter the contact title
            NgWebElement uiProposedLicensee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiProposedLicensee.SendKeys(contactTitle);

            // click on authorize signature checkbox
            NgWebElement uiAuthorizeSignature = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit']"));
            uiAuthorizeSignature.Click();

            // click on signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();


            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'test inspection')]")).Displayed);

        }
    }
}
