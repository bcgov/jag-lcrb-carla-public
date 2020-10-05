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
            // check that the legal name field is populated
            NgWebElement uiLegalName = ngDriver.FindElement(By.CssSelector("div:nth-of-type(2) > div:nth-of-type(1) > div > div > div:nth-of-type(1) > app-field:nth-of-type(1) > section > div > section > input"));
            string fieldValue = uiLegalName.GetAttribute("value");
            Assert.True(fieldValue != null);

            NgWebElement uiBusinessNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='businessNumber']"));
            Assert.True(uiBusinessNumber.GetAttribute("value") == "123456789");

            NgWebElement uiPhysicalAddressStreet = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressStreet']"));
            Assert.True(uiPhysicalAddressStreet.GetAttribute("value") == "645 Tyee Road");

            NgWebElement uiPhysicalAddressStreet2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressStreet2']"));
            Assert.True(uiPhysicalAddressStreet2.GetAttribute("value") == "West");

            NgWebElement uiPhysicalAddressCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressCity']"));
            Assert.True(uiPhysicalAddressCity.GetAttribute("value") == "Victoria");

            NgWebElement uiPhysicalAddressPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressPostalCode']"));
            Assert.True(uiPhysicalAddressPostalCode.GetAttribute("value") == "V9A6X5");

            NgWebElement uiMailingAddressStreet = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressStreet']"));
            Assert.True(uiMailingAddressStreet.GetAttribute("value") == "#22");

            NgWebElement uiMailingAddressStreet2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressStreet2']"));
            Assert.True(uiMailingAddressStreet2.GetAttribute("value") == "700 Bellevue Way NE");

            NgWebElement uiMailingAddressCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressCity']"));
            Assert.True(uiMailingAddressCity.GetAttribute("value") == "Bellevue");

            NgWebElement uiMailingAddressProvince = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressProvince']"));
            Assert.True(uiMailingAddressProvince.GetAttribute("value") == "WA");

            NgWebElement uiMailingAddressPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressPostalCode']"));
            Assert.True(uiMailingAddressPostalCode.GetAttribute("value") == "98004");

            NgWebElement uiMailingAddressCountry = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressCountry']"));
            Assert.True(uiMailingAddressCountry.GetAttribute("value") == "United States");

            NgWebElement uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            Assert.True(uiContactPhone.GetAttribute("value") == "(250) 181-1818");

            NgWebElement uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactEmail']"));
            Assert.True(uiContactEmail.GetAttribute("value") == "test@automation.com");

            if ((bizType == "n indigenous nation account profile") || (bizType == " local government account profile"))
            {
                NgWebElement uiWebsiteUrl = ngDriver.FindElement(By.CssSelector("input[formcontrolname='websiteUrl']"));
                Assert.True(uiWebsiteUrl.GetAttribute("value") == "https://www.liquorpolicy.org");
            }

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
