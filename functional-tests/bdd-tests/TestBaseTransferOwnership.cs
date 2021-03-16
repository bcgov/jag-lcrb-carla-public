using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a transfer of ownership for (.*)")]
        public void RequestOwnershipTransfer(string licenceType)
        {
            /* 
            Page Title: Licences & Authorizations
            */

            var transferOwnership = "Transfer Licence";

            // click on the Transfer Ownership link
            var uiTransferOwnership = ngDriver.FindElement(By.LinkText(transferOwnership));
            uiTransferOwnership.Click();

            // check that licence number field is populated
            var uiLicenseNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='licenseNumber']"));
            var fieldValueLicenseNumber = uiLicenseNumber.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueLicenseNumber));

            if (licenceType != "an agent")
            {
                // check that establishment name field is populated
                var uiEstablishmentName =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentName']"));
                var fieldValueEstablishmentName = uiEstablishmentName.GetProperty("value");
                Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentName));
            }

            // check that establishment address street field is populated
            var uiEstablishmentAddressStreet =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            var fieldValueEstablishmentAddressStreet = uiEstablishmentAddressStreet.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentAddressStreet));

            // check that establishment address city field is populated
            var uiEstablishmentAddressCity =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            var fieldValueEstablishmentAddressCity = uiEstablishmentAddressCity.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentAddressCity));

            // check that establishment province field is populated
            var uiEstablishmentProvince =
                ngDriver.FindElement(By.CssSelector("input.form-control[value='British Columbia']"));
            var fieldValueEstablishmentProvince = uiEstablishmentProvince.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentProvince));

            // check that establishment postal code field is populated
            var uiEstablishmentAddressPostalCode =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressPostalCode']"));
            var fieldValueEstablishmentAddressPostalCode = uiEstablishmentAddressPostalCode.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentAddressPostalCode));

            // check that establishment country field is populated
            var uiEstablishmentAddressCountry =
                ngDriver.FindElement(By.CssSelector("input.form-control[value='Canada']"));
            var fieldValueEstablishmentCountry = uiEstablishmentAddressCountry.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentCountry));

            if (licenceType != "an agent")
            {
                // check that establishment PID is populated
                var uiEstablishmentParcelId =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentParcelId']"));
                var fieldValueEstablishmentParcelId = uiEstablishmentParcelId.GetProperty("value");
                Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentParcelId));
            }

            var licensee = "GunderCorp";

            // search for the proposed licensee
            var uiProposedLicensee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiProposedLicensee.SendKeys(licensee);

            var uiThirdPartyOperatorOption = ngDriver.FindElement(By.CssSelector("mat-option[role='option'] span"));
            uiThirdPartyOperatorOption.Click();

            // click on consent to licence transfer checkbox
            var uiConsentToTransfer =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='transferConsent']"));
            uiConsentToTransfer.Click();

            // click on authorize signature checkbox
            var uiAuthorizeSignature =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizeSignature.Click();

            // click on signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            // click on submit transfer button
            var uiSubmitTransferButton =
                ngDriver.FindElement(By.CssSelector("app-application-ownership-transfer button.btn-primary"));
            uiSubmitTransferButton.Click();

            ClickLicencesTab();
        }
    }
}