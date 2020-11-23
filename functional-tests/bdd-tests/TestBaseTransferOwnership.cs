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

            // check that licence number field is populated
            NgWebElement uiLicenseNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='licenseNumber']"));
            string fieldValueLicenseNumber = uiLicenseNumber.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueLicenseNumber));

            // check that establishment name field is populated
            NgWebElement uiEstablishmentName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentName']"));
            string fieldValueEstablishmentName = uiEstablishmentName.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentName));

            // check that establishment address street field is populated
            NgWebElement uiEstablishmentAddressStreet = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            string fieldValueEstablishmentAddressStreet = uiEstablishmentAddressStreet.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentAddressStreet));

            // check that establishment address city field is populated
            NgWebElement uiEstablishmentAddressCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            string fieldValueEstablishmentAddressCity = uiEstablishmentAddressCity.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentAddressCity));

            // check that establishment province field is populated
            NgWebElement uiEstablishmentProvince = ngDriver.FindElement(By.CssSelector("input.form-control[value='British Columbia']"));
            string fieldValueEstablishmentProvince = uiEstablishmentProvince.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentProvince));

            // check that establishment postal code field is populated
            NgWebElement uiEstablishmentAddressPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressPostalCode']"));
            string fieldValueEstablishmentAddressPostalCode = uiEstablishmentAddressPostalCode.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentAddressPostalCode));

            // check that establishment country field is populated
            NgWebElement uiEstablishmentAddressCountry = ngDriver.FindElement(By.CssSelector("input.form-control[value='Canada']"));
            string fieldValueEstablishmentCountry = uiEstablishmentAddressCountry.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentCountry));

            // check that establishment PID is populated
            NgWebElement uiEstablishmentParcelId = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentParcelId']"));
            string fieldValueEstablishmentParcelId = uiEstablishmentParcelId.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentParcelId));

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

            ngDriver.Navigate().Refresh();

            // check for transfer initiated status 
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Transfer Requested')]")).Displayed);
        }
    }
}
