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
        [And(@"I request a transfer of ownership")]
        public void RequestOwnershipTransfer()
        {
            /* 
            Page Title: Licences & Authorizations
            */

            string transferOwnership = "Transfer Licence";

            // click on the Transfer Ownership link
            NgWebElement uiTransferOwnership = ngDriver.FindElement(By.LinkText(transferOwnership));
            uiTransferOwnership.Click();

            string licensee = "GunderCorp";

            // search for the proposed licensee
            NgWebElement uiProposedLicensee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiProposedLicensee.SendKeys(licensee);

            NgWebElement uiThirdPartyOperatorOption = ngDriver.FindElement(By.CssSelector("mat-option[role='option'] span"));
            uiThirdPartyOperatorOption.Click();

            // click on consent to licence transfer checkbox
            NgWebElement uiConsentToTransfer = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'transferConsent']"));
            uiConsentToTransfer.Click();

            // click on authorize signature checkbox
            NgWebElement uiAuthorizeSignature = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit']"));
            uiAuthorizeSignature.Click();

            // click on signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on submit transfer button
            NgWebElement uiSubmitTransferButton = ngDriver.FindElement(By.CssSelector("app-application-ownership-transfer button.btn-primary"));
            uiSubmitTransferButton.Click();

            ClickLicencesTab();

            /* 
            Page Title: Licences & Authorizations
            */

            // check for transfer initiated status 
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Transfer Requested')]")).Displayed);
        }
    }
}
