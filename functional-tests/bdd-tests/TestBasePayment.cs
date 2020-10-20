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
        public void MakePayment()
        {
            string testCC = configuration["test_cc"];
            string testCVD = configuration["test_ccv"];
            var tempWait = ngDriver.Manage().Timeouts().ImplicitWait;

            ngDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);

            ngDriver.WrappedDriver.FindElement(By.Name("trnCardNumber")).SendKeys(testCC);

            ngDriver.WrappedDriver.FindElement(By.Name("trnCardCvd")).SendKeys(testCVD);

            ngDriver.WrappedDriver.FindElement(By.Name("submitButton")).Click();

            System.Threading.Thread.Sleep(2000);

            ngDriver.Manage().Timeouts().ImplicitWait = tempWait;            
        }


        [And(@"I enter the payment information")]
        public void EnterPaymentInfo()
        {
            MakePayment();
        }


        [And(@"I pay the licensing fee for (.*)")]
        public void PayLicenceFee(string feeType)
        {
            /* 
            Page Title: Licences
            */

            // create test data
            string firstYearLicenceFee = "Pay First Year Fee";
            string returnToDashboard = "Return to Dashboard";

            // click on the pay first year licence fee link
            NgWebElement uiFirstYearLicenceFee = ngDriver.FindElement(By.LinkText(firstYearLicenceFee));
            uiFirstYearLicenceFee.Click();

            // pay the licence fee
            MakePayment();

            /* 
            Page Title: Payment Approved
            */

            // click on the return to dashboard link
            NgWebElement uiReturnToDashboard = ngDriver.FindElement(By.LinkText(returnToDashboard));
            uiReturnToDashboard.Click();

            ClickLicencesTab();
        }


        [And(@"I confirm the payment receipt for a (.*)")]
        public void ConfirmPaymentReceipt(string applicationType)
        {
            System.Threading.Thread.Sleep(3000);
            
            /* 
            Page Title: Payment Approved
            */

            if (applicationType == "Cannabis Retail Store application")
            {
                // confirm that payment receipt is for $7,500.00
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$7,500.00')]")).Displayed);
            }

            if (applicationType == "Catering application")
            {
                // confirm that payment receipt is for $475.00
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$475.00')]")).Displayed);
            }

            if (applicationType == "Manufacturer Licence application")
            {
                // confirm that payment receipt is for $550.00
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$550.00')]")).Displayed);
            }
        }


        [Then(@"I confirm the payment receipt for a (.*)")]
        public void ThenConfirmPaymentReceipt(string applicationType)
        {
            ConfirmPaymentReceipt(applicationType);
        }

    }
}
