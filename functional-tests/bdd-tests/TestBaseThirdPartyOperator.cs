using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a third party operator")]
        public void RequestThirdPartyOperator()
        {
            // navigate back to Licenses tab
            ClickLicencesTab();

            /* 
            Page Title: Licences & Authorizations
            Subtitle:   Catering Licences
            */

            var addOrChangeThirdParty = "Add or Change a Third Party Operator";

            // click on the Add or Change a Third Party Operator Link
            var uiAddOrChangeThirdPartyOp = ngDriver.FindElement(By.LinkText(addOrChangeThirdParty));
            uiAddOrChangeThirdPartyOp.Click();

            /* 
            Page Title: Add or Change a Third Party Operator
            */

            // check that licence number field is populated
            var uiLicenseNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='licenseNumber']"));
            var fieldValueLicenseNumber = uiLicenseNumber.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueLicenseNumber));

            // check that establishment name field is populated
            var uiEstablishmentName =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentName']"));
            var fieldValueEstablishmentName = uiEstablishmentName.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentName));

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

            // check that establishment PID is populated
            var uiEstablishmentParcelId =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentParcelId']"));
            var fieldValueEstablishmentParcelId = uiEstablishmentParcelId.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentParcelId));

            // create test data
            var thirdparty = "GunderCorp";

            // search for the proposed licensee
            var uiThirdPartyOperator =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiThirdPartyOperator.SendKeys(thirdparty);

            var uiThirdPartyOperatorOption = ngDriver.FindElement(By.CssSelector("mat-option[role='option'] span"));
            JavaScriptClick(uiThirdPartyOperatorOption);

            // click on authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            // click on submit button
            ClickOnSubmitButton2();

            // navigate back to Licenses tab
            ClickLicencesTab();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            ngDriver.Navigate().Refresh();

            // confirm that the application has been initiated
            Assert.True(ngDriver
                .FindElement(By.XPath("//body[contains(.,'Third Party Operator Application Initiated')]")).Displayed);
        }


        [And(@"I cancel the third party operator application")]
        public void CancelThirdPartyOperator()
        {
            /* 
            Page Title: Cancel Third Party Operator Application
            */

            // click on agreement checkbox
            var uiTransferConsent =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='transferConsent']"));
            uiTransferConsent.Click();

            // click on Cancel Third Party Application button
            ClickOnSubmitButton2();
        }
    }
}