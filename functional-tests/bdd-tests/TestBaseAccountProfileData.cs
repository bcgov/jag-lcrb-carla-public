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
            // check legal name field is populated
            NgWebElement uiLegalName = ngDriver.FindElement(By.CssSelector("div:nth-of-type(2) > div:nth-of-type(1) > div > div > div:nth-of-type(1) > app-field:nth-of-type(1) > section > div > section > input"));
            string fieldValueLegalName = uiLegalName.GetAttribute("value");
            Assert.True(fieldValueLegalName != null);

            // check business number is correct
            NgWebElement uiBusinessNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='businessNumber']"));
            Assert.True(uiBusinessNumber.GetAttribute("value") == "123456789");

            // check business type has been selected correctly
            if (bizType != "n indigenous nation account profile")
            {
                NgWebElement uiBusinessType = ngDriver.FindElement(By.CssSelector("select.form-control"));

                if (bizType == " private corporation")
                {
                    Assert.True(uiBusinessType.GetAttribute("value") == "Private Corporation");
                }

                if (bizType == " public corporation")
                {
                    Assert.True(uiBusinessType.GetAttribute("value") == "Public Corporation");
                }

                if (bizType == " sole proprietorship")
                {
                    Assert.True(uiBusinessType.GetAttribute("value") == "Sole Proprietor");
                }

                if (bizType == " partnership")
                {
                    Assert.True(uiBusinessType.GetAttribute("value") == "Partnership");
                }

                if (bizType == " society")
                {
                    Assert.True(uiBusinessType.GetAttribute("value") == "Society");
                }

                if (bizType == " university")
                {
                    Assert.True(uiBusinessType.GetAttribute("value") == "University");
                }

                if (bizType == " local government")
                {
                    Assert.True(uiBusinessType.GetAttribute("value") == "Local Government");
                }
            }

            // check incorporation number and date of incorporation are correct
            if (bizType == " private corporation" || bizType == " society" || bizType == " public corporation")
            {
                NgWebElement uiIncorporationNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='bcIncorporationNumber']"));
                Assert.True(uiIncorporationNumber.GetAttribute("value") == "BC1234567");

                NgWebElement uiIncorporationDate = ngDriver.FindElement(By.CssSelector("input[formcontrolname='bcIncorporationNumber']"));
                string fieldValueIncorporationDate = uiIncorporationDate.GetAttribute("value");
                Assert.True(fieldValueIncorporationDate != null);
            }

            // check street address 1 is correct
            NgWebElement uiPhysicalAddressStreet = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressStreet']"));
            Assert.True(uiPhysicalAddressStreet.GetAttribute("value") == "645 Tyee Road");

            // check street address 2 is correct
            NgWebElement uiPhysicalAddressStreet2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressStreet2']"));
            Assert.True(uiPhysicalAddressStreet2.GetAttribute("value") == "West");

            // check physical address city is correct
            NgWebElement uiPhysicalAddressCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressCity']"));
            Assert.True(uiPhysicalAddressCity.GetAttribute("value") == "Victoria");

            // check physical address province is correct
            NgWebElement uiPhysicalAddressProvince = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressProvince']"));
            Assert.True(uiPhysicalAddressProvince.GetAttribute("value") == "British Columbia");

            // check physical address postal code is correct
            NgWebElement uiPhysicalAddressPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressPostalCode']"));
            Assert.True(uiPhysicalAddressPostalCode.GetAttribute("value") == "V9A6X5");

            // check physical address country is correct
            NgWebElement uiPhysicalAddressCountry = ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressCountry']"));
            Assert.True(uiPhysicalAddressCountry.GetAttribute("value") == "Canada");

            // check mailing address street 1 is correct
            NgWebElement uiMailingAddressStreet = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressStreet']"));
            Assert.True(uiMailingAddressStreet.GetAttribute("value") == "#22");

            // check mailing address street 2 is correct
            NgWebElement uiMailingAddressStreet2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressStreet2']"));
            Assert.True(uiMailingAddressStreet2.GetAttribute("value") == "700 Bellevue Way NE");

            // check mailing address city is correct
            NgWebElement uiMailingAddressCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressCity']"));
            Assert.True(uiMailingAddressCity.GetAttribute("value") == "Bellevue");

            // check mailing address province/state is correct
            NgWebElement uiMailingAddressProvince = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressProvince']"));
            Assert.True(uiMailingAddressProvince.GetAttribute("value") == "WA");
 
            // check mailing address postal/zip is correct
            NgWebElement uiMailingAddressPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressPostalCode']"));
            Assert.True(uiMailingAddressPostalCode.GetAttribute("value") == "98004");

            // check mailing address country is correct
            NgWebElement uiMailingAddressCountry = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressCountry']"));
            Assert.True(uiMailingAddressCountry.GetAttribute("value") == "United States");

            // check contact phone is correct
            NgWebElement uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            Assert.True(uiContactPhone.GetAttribute("value") == "(250) 181-1818");

            // check contact email is correct
            NgWebElement uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactEmail']"));
            Assert.True(uiContactEmail.GetAttribute("value") == "test@automation.com");

            // check liquor policy information link is correct
            if ((bizType == "n indigenous nation account profile") || (bizType == " local government account profile"))
            {
                NgWebElement uiWebsiteUrl = ngDriver.FindElement(By.CssSelector("input[formcontrolname='websiteUrl']"));
                Assert.True(uiWebsiteUrl.GetAttribute("value") == "https://www.liquorpolicy.org");
            }

            // check authorized person first name is populated
            NgWebElement uiFirstName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstname']"));
            string fieldValueFirstName = uiFirstName.GetAttribute("value");
            Assert.True(fieldValueFirstName != null);

            // check authorized person last name is populated
            NgWebElement uiLastName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastname']"));
            string fieldValueLastName = uiLastName.GetAttribute("value");
            Assert.True(fieldValueLastName != null);

            // check authorized person title/position is correct
            NgWebElement uiJobTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='jobTitle']"));
            Assert.True(uiJobTitle.GetAttribute("value") == "CEO");

            // check authorized person phone number is correct
            NgWebElement uiTelephone1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='telephone1']"));
            Assert.True(uiTelephone1.GetAttribute("value") == "(778) 181-1818");

            // check authorized person email address is correct
            NgWebElement uiEmailAddress1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailaddress1']"));
            Assert.True(uiEmailAddress1.GetAttribute("value") == "automated@test.com");

            // check indigenous nation connection details are correct
            if ((bizType == "n indigenous nation account profile") || (bizType == " local government account profile"))
            {
                NgWebElement uiINConnectionToFederalProducerDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='iNConnectionToFederalProducerDetails']"));
                Assert.True(uiINConnectionToFederalProducerDetails.GetAttribute("value") == "Name and details of federal producer (automated test) for IN/local government.");
            }

            // check partnership connection details are correct
            if (bizType == " partnership")
            {
                NgWebElement uiDetailsFederalProducer2 = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='partnersConnectionFederalProducerDetails']"));
                Assert.True(uiDetailsFederalProducer2.GetAttribute("value") == "The name of the federal producer and details of the connection (partnership).");
            }

            // check federal producer connection to corporation details are correct
            if ((bizType == " partnership") || (bizType == " private corporation") || (bizType == " sole proprietorship") || (bizType == " university"))
            {
                NgWebElement uiDetailsFederalProducer3 = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='federalProducerConnectionToCorpDetails']"));
                Assert.True(uiDetailsFederalProducer3.GetAttribute("value") == "Name and details of federal producer connection to corporation.");
            }

            // check corporation connection to federal producer details are correct
            if ((bizType == " private corporation") || (bizType == " public corporation") || (bizType == " sole proprietorship") || (bizType == " university"))
            {
                NgWebElement uiDetailsFederalProducer2 = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='corpConnectionFederalProducerDetails']"));
                Assert.True(uiDetailsFederalProducer2.GetAttribute("value") == "The name of the federal producer and details of the connection.");
            }

            // check public corporation shareholder and family relationship details are correct
            if (bizType == " public corporation")
            {
                NgWebElement uiShareholderDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='share20PlusConnectionProducerDetails']"));
                Assert.True(uiShareholderDetails.GetAttribute("value") == "Details of shareholder relationship.");

                NgWebElement uiFamilyConnectionDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='share20PlusFamilyConnectionProducerDetail']"));
                Assert.True(uiFamilyConnectionDetails.GetAttribute("value") == "Details of family relationship (automated test).");
            }

            // check society federal producer details are correct
            if (bizType == " society")
            {
                NgWebElement uiSocietyConnectionDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='societyConnectionFederalProducerDetails']"));
                Assert.True(uiSocietyConnectionDetails.GetAttribute("value") == "Details of society/federal producer relationship.");
            }

            // check financial interest details are correct
            NgWebElement uiLiquorFinancialInterestDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='liquorFinancialInterestDetails']"));
            Assert.True(uiLiquorFinancialInterestDetails.GetAttribute("value") == "Details of the financial interest (automated test).");
        }
    }
}
