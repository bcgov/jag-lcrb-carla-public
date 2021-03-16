using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

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
            var location1 = "LCRB1";
            var street1 = "645 Tyee Road";
            var city1 = "Victoria";
            var postal1 = "V9A6X5";

            var location2 = "LCRB2";
            var street2 = "645 Tyee St";
            var city2 = "Duncan";
            var postal2 = "V9L1W4";

            var location3 = "LCRB3";
            var street3 = "645 Tyee Road";
            var city3 = "Umpqua";
            var postal3 = "97486";

            var location4 = "LCRB4";
            var street4 = "645 Champion Drive";
            var city4 = "Vancouver";
            var postal4 = "V5H3Z7";

            var location5 = "LCRB5";
            var street5 = "645 Chief St";
            var city5 = "Port Hardy";
            var postal5 = "V0N 2P0";

            // click on Add Additional Storage button
            var uiOffsiteStorageLocations =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='offsiteStorageLocations'] button[type='button']"));
            uiOffsiteStorageLocations.Click();

            // enter location 1
            var uiLocation1 =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='offsiteStorageLocations'] input[formcontrolname='name']"));
            uiLocation1.SendKeys(location1);

            // enter street 1
            var uiStreet1 =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='offsiteStorageLocations'] input[formcontrolname='street1']"));
            uiStreet1.SendKeys(street1);

            // enter city 1
            var uiCity1 =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='offsiteStorageLocations'] input[formcontrolname='city']"));
            uiCity1.SendKeys(city1);

            // enter postal code 1
            var uiPostalCode1 =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='offsiteStorageLocations'] input[formcontrolname='postalCode']"));
            uiPostalCode1.SendKeys(postal1);

            // open second row
            var uiSecondRow =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='offsiteStorageLocations'] button.btn-secondary"));
            uiSecondRow.Click();

            // enter location 2
            var uiLocation2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            uiLocation2.SendKeys(location2);

            // enter street 2
            var uiStreet2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiStreet2.SendKeys(street2);

            // enter city 2
            var uiCity2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiCity2.SendKeys(city2);

            // enter postal code 2
            var uiPostalCode2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiPostalCode2.SendKeys(postal2);

            // open third row
            var uiThirdRow =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='offsiteStorageLocations'] button.btn-secondary"));
            uiThirdRow.Click();

            // enter location 3
            var uiLocation3 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            uiLocation3.SendKeys(location3);

            // enter street 3
            var uiStreet3 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiStreet3.SendKeys(street3);

            // enter city 3
            var uiCity3 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[11]"));
            uiCity3.SendKeys(city3);

            // enter postal code 3
            var uiPostalCode3 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
            uiPostalCode3.SendKeys(postal3);

            // open fourth row
            var uiFourthRow =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='offsiteStorageLocations'] button.btn-secondary"));
            uiFourthRow.Click();

            // enter location 4
            var uiLocation4 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
            uiLocation4.SendKeys(location4);

            // enter street 4
            var uiStreet4 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[14]"));
            uiStreet4.SendKeys(street4);

            // enter city 4
            var uiCity4 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[15]"));
            uiCity4.SendKeys(city4);

            // enter postal code 4
            var uiPostalCode4 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[16]"));
            uiPostalCode4.SendKeys(postal4);

            // open fifth row
            var uiFifthRow =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='offsiteStorageLocations'] button.btn-secondary"));
            uiFifthRow.Click();

            // enter location 5
            var uiLocation5 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[17]"));
            uiLocation5.SendKeys(location5);

            // enter street 5
            var uiStreet5 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[18]"));
            uiStreet5.SendKeys(street5);

            // enter city 5
            var uiCity5 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[19]"));
            uiCity5.SendKeys(city5);

            // enter postal code 5
            var uiPostalCode5 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[20]"));
            uiPostalCode5.SendKeys(postal5);

            // click on the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            uiSignatureAgreement.Click();
        }


        [And(@"I remove a row from the offsite storage application")]
        public void RemoveOffsiteStorageRows()
        {
            // click on the trashbin icon for the second row
            var uiTrash =
                ngDriver.FindElement(By.CssSelector(".offsite-storage .ng-star-inserted:nth-child(3) .fa-trash"));
            uiTrash.Click();
        }


        [And(@"I add and delete more rows to the offsite storage application")]
        public void AddDeleteMoreOffsiteStorageRows()
        {
            // create test data
            var locationNew = "LCRB6";
            var streetNew = "123 New St";
            var cityNew = "Golden";
            var postalNew = "V0A 1H3";

            // add one more row
            var uiNewRow =
                ngDriver.FindElement(
                    By.CssSelector("[formcontrolname='offsiteStorageLocations'] button.btn-secondary"));
            uiNewRow.Click();

            // enter 6th location
            var uiLocationNew = ngDriver.FindElement(By.XPath("(//input[@type='text'])[17]"));
            uiLocationNew.SendKeys(locationNew);

            // enter 6th street
            var uiStreetNew = ngDriver.FindElement(By.XPath("(//input[@type='text'])[18]"));
            uiStreetNew.SendKeys(streetNew);

            // enter 6th city
            var uiCityNew = ngDriver.FindElement(By.XPath("(//input[@type='text'])[19]"));
            uiCityNew.SendKeys(cityNew);

            // enter 6th postal code
            var uiPostalCodeNew = ngDriver.FindElement(By.XPath("(//input[@type='text'])[20]"));
            uiPostalCodeNew.SendKeys(postalNew);

            // delete two pre-existing rows
            var uiTrash =
                ngDriver.FindElement(By.CssSelector(".offsite-storage .ng-star-inserted:nth-child(4) .fa-trash"));
            uiTrash.Click();

            var uiTrash2 =
                ngDriver.FindElement(By.CssSelector(".offsite-storage .ng-star-inserted:nth-child(3) .fa-trash"));
            uiTrash2.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            uiSignatureAgreement.Click();
        }


        [And(@"the updated offsite storage application is correct for (.*)")]
        public void CorrectOffsiteStorageRecords(string scenario)
        {
            if (scenario == "deletion")
                // confirm that second row is no longer displayed
                Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'LCRB2'))]")).Displayed);

            if (scenario == "deletion and addition")
            {
                // confirm that newly deleted rows are displayed as 'Removed'
                Assert.True(ngDriver
                    .FindElement(By.XPath("//app-offsite-table/table/tr[2]/td[1]/span[contains(.,'Removed')]"))
                    .Displayed);
                Assert.True(ngDriver
                    .FindElement(By.XPath("//app-offsite-table/table/tr[3]/td[1]/span[contains(.,'Removed')]"))
                    .Displayed);
            }
        }
    }
}