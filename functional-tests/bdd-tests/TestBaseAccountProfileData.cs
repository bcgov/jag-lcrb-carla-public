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
        [And(@"the correct data is displayed for a(.*)")]
        public void AccountProfileData(string bizType)
        {
            // check that previously entered account profile data has been saved

            NgWebElement uiBusinessNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='businessNumber']"));
            Assert.True(uiBusinessNumber.GetAttribute("value") == "012345678");

            NgWebElement uiPhysicalAddressStreet = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressStreet']"));
            Assert.True(uiPhysicalAddressStreet.GetAttribute("value") == "645 Tyee Road");

            NgWebElement uiPhysicalAddressStreet2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressStreet2']"));
            Assert.True(uiPhysicalAddressStreet2.GetAttribute("value") == "West of Victoria");

            NgWebElement uiPhysicalAddressCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressCity']"));
            Assert.True(uiPhysicalAddressCity.GetAttribute("value") == "Victoria");

            NgWebElement uiPhysicalAddressPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressPostalCode']"));
            Assert.True(uiPhysicalAddressPostalCode.GetAttribute("value") == "V9A6X5");

            NgWebElement uiMailingAddressStreet = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressStreet']"));
            Assert.True(uiMailingAddressStreet.GetAttribute("value") == "P.O. Box 123");

            NgWebElement uiMailingAddressStreet2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressStreet2']"));
            Assert.True(uiMailingAddressStreet2.GetAttribute("value") == "303 Prideaux St.");

            NgWebElement uiMailingAddressCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressCity']"));
            Assert.True(uiMailingAddressCity.GetAttribute("value") == "Victoria");

            NgWebElement uiMailingAddressProvince = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressProvince']"));
            Assert.True(uiMailingAddressProvince.GetAttribute("value") == "B.C.");

            NgWebElement uiMailingAddressPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressPostalCode']"));
            Assert.True(uiMailingAddressPostalCode.GetAttribute("value") == "V9A6X5");

            NgWebElement uiMailingAddressCountry = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressCountry']"));
            Assert.True(uiMailingAddressCountry.GetAttribute("value") == "Canada");

            NgWebElement uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            Assert.True(uiContactPhone.GetAttribute("value") == "(250) 181-1818");

            NgWebElement uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactEmail']"));
            Assert.True(uiContactEmail.GetAttribute("value") == "test@automation.com");

            NgWebElement uiWebsiteUrl = ngDriver.FindElement(By.CssSelector("input[formcontrolname='websiteUrl']"));
            Assert.True(uiWebsiteUrl.GetAttribute("value") == "https://www.liquorpolicy.org");

            NgWebElement uiJobTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='jobTitle']"));
            Assert.True(uiJobTitle.GetAttribute("value") == "CEO");

            NgWebElement uiTelephone1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='telephone1']"));
            Assert.True(uiTelephone1.GetAttribute("value") == "(778) 181-1818");

            NgWebElement uiEmailAddress1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailaddress1']"));
            Assert.True(uiEmailAddress1.GetAttribute("value") == "automated@test.com");

            NgWebElement uiLiquorFinancialInterestDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='liquorFinancialInterestDetails']"));
            Assert.True(uiLiquorFinancialInterestDetails.GetAttribute("value") == "Details of the financial interest (automated test).");

            if (bizType == "n indigenous nation account profile")
            {
                NgWebElement uiINConnectionToFederalProducerDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='iNConnectionToFederalProducerDetails']"));
                Assert.True(uiINConnectionToFederalProducerDetails.GetAttribute("value") == "Name and details of federal producer (automated test) for IN.");
            }
        }
    }
}
