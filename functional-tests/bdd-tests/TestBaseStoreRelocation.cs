using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a store relocation for (.*)")]
        public void RequestStoreRelocation(string applicationType)
        {
            /* 
            Page Title: Licences & Authorizations
            */

            var requestRelocationLink = "Request Relocation";

            // click on the request location link
            var uiRequestRelocation = ngDriver.FindElement(By.LinkText(requestRelocationLink));
            uiRequestRelocation.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Submit a Licence Relocation Application
            */

            if (applicationType == "Cannabis")
            {
                var proposedAddress = "Automated Test Street";
                var proposedCity = "Victoria";
                var proposedPostalCode = "V9A 6X5";
                var pid = "012345678";

                // enter the proposed street address
                var uiProposedAddress =
                    ngDriver.FindElement(
                        By.CssSelector(".ngtest-new-address input[formcontrolname='establishmentAddressStreet']"));
                uiProposedAddress.SendKeys(proposedAddress);

                // enter the proposed city
                var uiProposedCity =
                    ngDriver.FindElement(
                        By.CssSelector(".ngtest-new-address input[formcontrolname='establishmentAddressCity']"));
                uiProposedCity.SendKeys(proposedCity);

                // enter the postal code
                var uiProposedPostalCode =
                    ngDriver.FindElement(
                        By.CssSelector(".ngtest-new-address input[formcontrolname='establishmentAddressPostalCode']"));
                uiProposedPostalCode.SendKeys(proposedPostalCode);

                // enter the PID
                var uiProposedPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
                uiProposedPID.SendKeys(pid);
            }

            // upload a supporting document
            FileUpload("checklist.pdf", "(//input[@type='file'])[2]");

            // select the authorized to submit checkbox
            var uiAuthToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthToSubmit.Click();

            // select the signature agreement checkbox
            var uiSigAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSigAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            // pay for the relocation application
            MakePayment();
        }

        [And(@"I confirm the relocation request is displayed on the dashboard")]
        public void RequestedRelocationOnDashboard()
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // confirm that relocation request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,' Relocation Application Under Review ')]"))
                .Displayed);
        }
    }
}