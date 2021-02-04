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

            switch (appType)
            {
                case "society":
                    // click on Change of Directors or Officers
                    NgWebElement uiChangeOfDirectorsOrOfficers = ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiChangeOfDirectorsOrOfficers.Click();
                    // click on Name Change, Licensee -- Society
                    NgWebElement uiNameChangeLicenseeSociety = ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                    uiNameChangeLicenseeSociety.Click();
                    // click on Name Change, Person
                    NgWebElement uiNameChangePerson = ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                    uiNameChangePerson.Click();
                    // click on Addition of Receiver or Executor
                    NgWebElement uiAdditionOfReceiverOrExecutor = ngDriver.FindElement(By.CssSelector("#mat-checkbox-6.mat-checkbox"));
                    uiAdditionOfReceiverOrExecutor.Click();
                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[3]");
                    break;
                case "private corporation":
                    // click on Internal Transfer of Shares
                    NgWebElement uiInternalTransferOfShares = ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiInternalTransferOfShares.Click();
                    // click on External Transfer of Shares
                    NgWebElement uiExternalTransferOfShares = ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                    uiExternalTransferOfShares.Click();
                    // click on Change of Directors or Officers
                    NgWebElement uiChangeOfDirectorsOrOfficers2 = ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                    uiChangeOfDirectorsOrOfficers2.Click();
                    // click on Name Change, Licensee -- Corporation
                    NgWebElement uiNameChangeLicenseePrivateCorporation = ngDriver.FindElement(By.CssSelector("#mat-checkbox-6.mat-checkbox"));
                    uiNameChangeLicenseePrivateCorporation.Click();
                    // click on Name Change, Person
                    NgWebElement uiNameChangePerson2 = ngDriver.FindElement(By.CssSelector("#mat-checkbox-7.mat-checkbox"));
                    uiNameChangePerson2.Click();
                    // click on Addition of Receiver or Executor
                    NgWebElement uiAdditionOfReceiverOrExecutor2 = ngDriver.FindElement(By.CssSelector("#mat-checkbox-8.mat-checkbox"));
                    uiAdditionOfReceiverOrExecutor2.Click();
                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[3]");
                    break;
                case "partnership":
                    // click on Name Change, Licensee -- Partnership
                    NgWebElement uiNameChangeLicenseePartnership = ngDriver.FindElement(By.CssSelector("#mat-checkbox-3.mat-checkbox"));
                    uiNameChangeLicenseePartnership.Click();
                    // click on Name Change, Person
                    NgWebElement uiNameChangePerson3 = ngDriver.FindElement(By.CssSelector("#mat-checkbox-4.mat-checkbox"));
                    uiNameChangePerson3.Click();
                    // click on Addition of Receiver or Executor
                    NgWebElement uiAdditionOfReceiverOrExecutor3 = ngDriver.FindElement(By.CssSelector("#mat-checkbox-5.mat-checkbox"));
                    uiAdditionOfReceiverOrExecutor3.Click();
                    // enter person first name
                    // enter person last name
                    // enter person new first name
                    // enter person new last name
                    // upload copy of marriage certificate

                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[3]");
                    break;
                case "sole proprietorship":
                    // upload Personal History Summary document
                    FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[3]");
                    break;
            }

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$7,500.234200')]")).Displayed);

        }
    }
}
