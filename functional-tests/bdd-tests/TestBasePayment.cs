using System;
using System.Threading;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I enter the payment information")]
        public void MakePayment()
        {
            var testCC = configuration["test_cc"];
            var testCVD = configuration["test_ccv"];
            var tempWait = ngDriver.Manage().Timeouts().ImplicitWait;

            ngDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);

            ngDriver.WrappedDriver.FindElement(By.Name("trnCardNumber")).SendKeys(testCC);
            ngDriver.WrappedDriver.FindElement(By.Name("trnCardCvd")).SendKeys(testCVD);

            var currentUrl = ngDriver.WrappedDriver.Url;

            ngDriver.WrappedDriver.FindElement(By.Name("submitButton")).Click();

            ngDriver.Manage().Timeouts().ImplicitWait = tempWait;

            // wait for the page transition to occur
            for (var i = 0; i < 10; i++)
                if (ngDriver.WrappedDriver.Url != currentUrl)
                    break;
                else
                    // wait a second
                    Thread.Sleep(1000);
            // now ensure that angular is loaded.
            for (var i = 0; i < 10; i++)
                try
                {
                    ngDriver.WaitForAngular();
                    break;
                }
                catch (Exception)
                {
                    // ignore exception but sleep for a second.
                    Thread.Sleep(1000);
                }
        }


        [And(@"I pay the licensing fee")]
        public void PayLicenceFee()
        {
            /* 
            Page Title: Licences & Authorizations
            */

            // create test data
            var firstYearLicenceFee = "Pay First Year Fee";
            var returnToDashboard = "Return to Dashboard";

            // click on the pay first year licence fee link
            var uiFirstYearLicenceFees = ngDriver.FindElements(By.LinkText(firstYearLicenceFee));
            if (uiFirstYearLicenceFees.Count > 0)
                uiFirstYearLicenceFees[0].Click();
            else
                throw new Exception("Unable to find Pay First Year Fee link");

            // pay the licence fee
            MakePayment();

            /* 
            Page Title: Payment Approved
            */

            IgnoreSynchronizationFalse();

            // click on the return to dashboard link
            var uiReturnToDashboard = ngDriver.FindElement(By.LinkText(returnToDashboard));
            JavaScriptClick(uiReturnToDashboard);
        }


        [And(@"I confirm the payment receipt for a(.*)")]
        public void ConfirmPaymentReceipt(string applicationType)
        {
            Thread.Sleep(3000);

            /* 
            Page Title: Payment Approved
            */

            if (applicationType == " Cannabis Retail Store application")
                // confirm that payment receipt is for $7,500.00
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$7,500.00')]")).Displayed);

            if (applicationType == " Catering application")
                // confirm that payment receipt is for $475.00
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$475.00')]")).Displayed);

            if (applicationType == " Manufacturer Licence application")
                // confirm that payment receipt is for $550.00
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$550.00')]")).Displayed);

            if (applicationType == " UBrew / UVin application")
                // confirm that payment receipt is for $550.00
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$550.00')]")).Displayed);

            if (applicationType == " Food Primary application")
                // confirm that payment receipt is for $950.00
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$950.00')]")).Displayed);

            if (applicationType == "n Agent Licence")
                // confirm that payment receipt is for $220.00
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$220.00')]")).Displayed);
        }


        [Then(@"I confirm the payment receipt for a (.*)")]
        public void ThenConfirmPaymentReceipt(string applicationType)
        {
            ConfirmPaymentReceipt(applicationType);
        }
    }
}