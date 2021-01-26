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
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the Permanent Change to a Licensee application for a (.*)")]
        public void PermanentChangeLicensee(string appType)
        {
            /* 
            Page Title: Permanent Change to a Licensee
            */

            if (appType != "Catering application for a partnership")
            {

                // click on Internal Transfer of Shares
                NgWebElement uiInternalTransferOfShares = ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                uiInternalTransferOfShares.Click();

                // click on External Transfer of Shares
                NgWebElement uiExternalTransferOfShares = ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                uiExternalTransferOfShares.Click();

                // click on Change of Directors or Officers
                NgWebElement uiChangeOfDirectorsOrOfficers = ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                uiChangeOfDirectorsOrOfficers.Click();

                // click on Name Change, Licensee -- Corporation
                NgWebElement uiNameChangeLicenseeCorporation = ngDriver.FindElement(By.CssSelector("#mat-checkbox-6.mat-checkbox"));
                uiNameChangeLicenseeCorporation.Click();

                // click on Name Change, Person
                NgWebElement uiNameChangePerson = ngDriver.FindElement(By.CssSelector("#mat-checkbox-7.mat-checkbox"));
                uiNameChangePerson.Click();

                // click on Addition of Receiver or Executor
                NgWebElement uiAdditionOfReceiverOrExecutor = ngDriver.FindElement(By.CssSelector("#mat-checkbox-8.mat-checkbox"));
                uiAdditionOfReceiverOrExecutor.Click();
            }

            if (appType == "Catering application for a private corporation")
            {
                // upload Central Securities Register forms
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                // upload supporting business documentation
                FileUpload("business_plan.pdf", "(//input[@type='file'])[6]");

                // upload shareholders holding less than 10% interest
                FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[9]");

                // upload Cannabis Associate Security Screening Forms
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[12]");

                // upload Financial Integrity Documents
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[15]");
            }

            if (appType == "Catering application for a partnership")
            {
                // upload Personal History Summary ocuments
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[15]");
            }

            if (appType == "Catering application for a sole proprietorship")
            { }

            if (appType == "Catering application for a society")
            { }

            if (appType == "CRS application for a private corporation")
            {
                // upload Cannabis Associate Security Screening Forms
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[]");

                // upload Financial Integrity Documents
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[]");
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
