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
        [And(@"I complete the offsite storage application")]
        public void CompleteOffsiteStorage()
        {
            /* 
            Page Title: Manage Off-Site Storage
            */

            // create test data
            string street1 = "645 Tyee Road";
            string city1 = "Victoria";
            string postal1 = "V9A6X5";

            string street2 = "645 Tyee St";
            string city2 = "Duncan";
            string postal2 = "V9L1W4";

            string street3 = "645 Tyee Road";
            string city3 = "Umpqua";
            string postal3 = "97486";

            // click on Add Additional Storage button
            NgWebElement uiOffsiteStorageLocations = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] button[type='button']"));
            uiOffsiteStorageLocations.Click();

            // enter street 1
            NgWebElement uiStreet1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] input[formcontrolname='street1']"));
            uiStreet1.SendKeys(street1);

            // enter city 1
            NgWebElement uiCity1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] input[formcontrolname='city']"));
            uiCity1.SendKeys(city1);

            // enter postal code 1
            NgWebElement uiPostalCode1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] input[formcontrolname='postalCode']"));
            uiPostalCode1.SendKeys(postal1);

            /*
            // enter street 2
            NgWebElement uiStreet2 = ngDriver.FindElement(By.CssSelector(""));
            uiStreet2.SendKeys(street2);

            // enter city 2
            NgWebElement uiCity2 = ngDriver.FindElement(By.CssSelector(""));
            uiCity2.SendKeys(city2);

            // enter postal code 2
            NgWebElement uiPostalCode2 = ngDriver.FindElement(By.CssSelector(""));
            uiPostalCode2.SendKeys(postal2);

            // enter street 3
            NgWebElement uiStreet3 = ngDriver.FindElement(By.CssSelector(""));
            uiStreet3.SendKeys(street3);

            // enter city 3
            NgWebElement uiCity3 = ngDriver.FindElement(By.CssSelector(""));
            uiCity3.SendKeys(city3);

            // enter postal code 3
            NgWebElement uiPostalCode3 = ngDriver.FindElement(By.CssSelector(""));
            uiPostalCode3.SendKeys(postal3);
            */

            System.Threading.Thread.Sleep(2000);

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-2[formcontrolname='agreement']"));
            uiSignatureAgreement.Click();
        }
    }
}