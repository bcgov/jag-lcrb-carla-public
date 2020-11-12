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
        [And(@"I request a third party operator")]
        public void RequestThirdPartyOperator()
        {
            // navigate back to Licenses tab
            ngDriver.Navigate().GoToUrl($"{baseUri}licences");

            /* 
            Page Title: Licences & Authorizations
            Subtitle:   Catering Licences
            */

            string addOrChangeThirdParty = "Add or Change a Third Party Operator";

            // click on the Add or Change a Third Party Operator Link
            NgWebElement uiAddOrChangeThirdPartyOp = ngDriver.FindElement(By.LinkText(addOrChangeThirdParty));
            uiAddOrChangeThirdPartyOp.Click();

            /* 
            Page Title: Add or Change a Third Party Operator
            */

            // check that licence number field is populated
            NgWebElement uiLicenseNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='licenseNumber']"));
            string fieldValueLicenseNumber = uiLicenseNumber.GetAttribute("value");
            Assert.True(fieldValueLicenseNumber != null);

            // check that establishment name field is populated
            NgWebElement uiEstablishmentName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentName']"));
            string fieldValueEstablishmentName = uiEstablishmentName.GetAttribute("value");
            Assert.True(fieldValueEstablishmentName != null);

            // check that establishment address street field is populated
            NgWebElement uiEstablishmentAddressStreet = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            string fieldValueEstablishmentAddressStreet = uiEstablishmentAddressStreet.GetAttribute("value");
            Assert.True(fieldValueEstablishmentAddressStreet != null);

            // check that establishment address city field is populated
            NgWebElement uiEstablishmentAddressCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            string fieldValueEstablishmentAddressCity = uiEstablishmentAddressCity.GetAttribute("value");
            Assert.True(fieldValueEstablishmentAddressCity != null);

            // check that establishment province field is populated
            NgWebElement uiEstablishmentProvince = ngDriver.FindElement(By.CssSelector("input[formcontrolname='TODO']"));
            string fieldValueEstablishmentProvince = uiEstablishmentProvince.GetAttribute("value");
            Assert.True(fieldValueEstablishmentProvince != null);

            // check that establishment postal code field is populated
            NgWebElement uiEstablishmentAddressPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressPostalCode']"));
            string fieldValueEstablishmentAddressPostalCode = uiEstablishmentAddressPostalCode.GetAttribute("value");
            Assert.True(fieldValueEstablishmentAddressPostalCode != null);

            // check that establishment country field is populated
            NgWebElement uiEstablishmentAddressCountry = ngDriver.FindElement(By.CssSelector("input[formcontrolname='TODO']"));
            string fieldValueEstablishmentCountry = uiEstablishmentAddressCountry.GetAttribute("value");
            Assert.True(fieldValueEstablishmentCountry != null);

            // check that establishment PID is populated
            NgWebElement uiEstablishmentParcelId = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentParcelId']"));
            string fieldValueEstablishmentParcelId = uiEstablishmentParcelId.GetAttribute("value");
            Assert.True(fieldValueEstablishmentParcelId != null);

            // create test data
            string thirdparty = "GunderCorp TestBusiness";

            // search for the proposed licensee
            NgWebElement uiThirdPartyOperator = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiThirdPartyOperator.SendKeys(thirdparty);

            var uiThirdPartyOperatorOption = ngDriver.FindElements(By.CssSelector("mat-option[role='option'] span"));
            if (uiThirdPartyOperatorOption.Count > 0)
            {
                uiThirdPartyOperatorOption[0].Click();
            }
            else
            {
                throw new Exception($"Unable to find {thirdparty}");
            }
            
            // click on authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            // click on submit button
            ClickOnSubmitButton();

            // navigate back to Licenses tab
            ngDriver.Navigate().GoToUrl($"{baseUri}licences");

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // confirm that the application has been initiated
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Third Party Operator Application Initiated')]")).Displayed);
        }
    }
}
