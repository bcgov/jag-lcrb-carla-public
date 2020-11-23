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
            string location1 = "LCRB1";
            string street1 = "645 Tyee Road";
            string city1 = "Victoria";
            string postal1 = "V9A6X5";

            string location2 = "LCRB2";
            string street2 = "645 Tyee St";
            string city2 = "Duncan";
            string postal2 = "V9L1W4";

            string location3 = "LCRB3";
            string street3 = "645 Tyee Road";
            string city3 = "Umpqua";
            string postal3 = "97486";

            string location4 = "LCRB4";
            string street4 = "645 Champion Drive";
            string city4 = "Vancouver";
            string postal4 = "V5H3Z7";

            string location5 = "LCRB5";
            string street5 = "645 Chief St";
            string city5 = "Port Hardy";
            string postal5 = "V0N 2P0";

            // click on Add Additional Storage button
            NgWebElement uiOffsiteStorageLocations = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] button[type='button']"));
            uiOffsiteStorageLocations.Click();

            // enter location 1
            NgWebElement uiLocation1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] input[formcontrolname='name']"));
            uiLocation1.SendKeys(location1);

            // enter street 1
            NgWebElement uiStreet1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] input[formcontrolname='street1']"));
            uiStreet1.SendKeys(street1);

            // enter city 1
            NgWebElement uiCity1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] input[formcontrolname='city']"));
            uiCity1.SendKeys(city1);

            // enter postal code 1
            NgWebElement uiPostalCode1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] input[formcontrolname='postalCode']"));
            uiPostalCode1.SendKeys(postal1);

            // open second row
            NgWebElement uiSecondRow = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] button.btn-secondary"));
            uiSecondRow.Click();

            // enter location 2
            NgWebElement uiLocation2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            uiLocation2.SendKeys(location2);

            // enter street 2
            NgWebElement uiStreet2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiStreet2.SendKeys(street2);

            // enter city 2
            NgWebElement uiCity2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiCity2.SendKeys(city2);

            // enter postal code 2
            NgWebElement uiPostalCode2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiPostalCode2.SendKeys(postal2);

            // open third row
            NgWebElement uiThirdRow = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] button.btn-secondary"));
            uiThirdRow.Click();

            // enter location 3
            NgWebElement uiLocation3 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiLocation3.SendKeys(location3);

            // enter street 3
            NgWebElement uiStreet3 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiStreet3.SendKeys(street3);

            // enter city 3
            NgWebElement uiCity3 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[11]"));
            uiCity3.SendKeys(city3);

            // enter postal code 3
            NgWebElement uiPostalCode3 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
            uiPostalCode3.SendKeys(postal3);

            // open fourth row
            NgWebElement uiFourthRow = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] button.btn-secondary"));
            uiFourthRow.Click();

            // enter location 4
            NgWebElement uiLocation4 = ngDriver.FindElement(By.XPath(""));
            uiLocation4.SendKeys(location4);

            // enter street 4
            NgWebElement uiStreet4 = ngDriver.FindElement(By.XPath(""));
            uiStreet4.SendKeys(street4);

            // enter city 4
            NgWebElement uiCity4 = ngDriver.FindElement(By.XPath(""));
            uiCity4.SendKeys(city4);

            // enter postal code 4
            NgWebElement uiPostalCode4 = ngDriver.FindElement(By.XPath(""));
            uiPostalCode4.SendKeys(postal4);

            // open fifth row
            NgWebElement uiFifthRow = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] button.btn-secondary"));
            uiFifthRow.Click();

            // enter location 5
            NgWebElement uiLocation5 = ngDriver.FindElement(By.XPath(""));
            uiLocation5.SendKeys(location5);

            // enter street 5
            NgWebElement uiStreet5 = ngDriver.FindElement(By.XPath(""));
            uiStreet5.SendKeys(street5);

            // enter city 5
            NgWebElement uiCity5 = ngDriver.FindElement(By.XPath(""));
            uiCity5.SendKeys(city5);

            // enter postal code 5
            NgWebElement uiPostalCode5 = ngDriver.FindElement(By.XPath(""));
            uiPostalCode5.SendKeys(postal5);

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            uiSignatureAgreement.Click();
        }


        [And(@"I remove a row from the offsite storage application")]
        public void RemoveOffsiteStorageRows()
        {
            // click on the trashbin icon for the second row
            NgWebElement uiTrash = ngDriver.FindElement(By.CssSelector(".offsite-storage .ng-star-inserted:nth-child(3) .fa-trash"));
            uiTrash.Click();
        }


        [And(@"I add and delete more rows to the offsite storage application")]
        public void AddDeleteMoreOffsiteStorageRows()
        {
            // create test data
            string locationNew = "New";
            string streetNew = "123 New St";
            string cityNew = "Golden";
            string postalNew = "V0A 1H3";

            // add one more row
            NgWebElement uiNewRow = ngDriver.FindElement(By.CssSelector("[formcontrolname='offsiteStorageLocations'] button.btn-secondary"));
            uiNewRow.Click();

            // enter new location
            NgWebElement uiLocationNew = ngDriver.FindElement(By.XPath(""));
            uiLocationNew.SendKeys(locationNew);

            // enter new street
            NgWebElement uiStreetNew = ngDriver.FindElement(By.XPath(""));
            uiStreetNew.SendKeys(streetNew);

            // enter new city
            NgWebElement uiCityNew = ngDriver.FindElement(By.XPath(""));
            uiCityNew.SendKeys(cityNew);

            // enter new postal code
            NgWebElement uiPostalCodeNew = ngDriver.FindElement(By.XPath(""));
            uiPostalCodeNew.SendKeys(postalNew);

            // delete two pre-existing rows > status should change to 'Removed'
            NgWebElement uiTrash = ngDriver.FindElement(By.CssSelector(".offsite-storage .ng-star-inserted:nth-child(3) .fa-trash"));
            uiTrash.Click();

            NgWebElement uiTrash2 = ngDriver.FindElement(By.CssSelector(".offsite-storage .ng-star-inserted:nth-child(3) .fa-trash"));
            uiTrash2.Click();
        }


        [And(@"the updated offsite storage application is correct for (.*)")]
        public void CorrectOffsiteStorageRecords(string scenario)
        {
            if (scenario == "deletion")
            {
                // confirm that second row is no longer displayed
                Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'LCRB2'))]")).Displayed);
            }

            if (scenario == "deletion and addition")
            {
                // confirm that second row is no longer displayed
                Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'LCRB2'))]")).Displayed);

                // confirm that all expected rows are displayed

            }
        }
    }
}