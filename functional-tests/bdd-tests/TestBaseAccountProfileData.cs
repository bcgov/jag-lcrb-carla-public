using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"the correct data is displayed for a(.*)")]
        public void AccountProfileData(string bizType)
        {
            /* 
            Page Title: Account Profile
            */

            // check legal name field is populated
            var uiLegalName = ngDriver.FindElement(By.CssSelector(
                "div:nth-of-type(2) > div:nth-of-type(1) > div > div > div:nth-of-type(1) > app-field:nth-of-type(1) > section > div > section > input"));
            var fieldValueLegalName = uiLegalName.GetAttribute("value");
            Assert.True(fieldValueLegalName != null);

            // check business number is correct
            var uiBusinessNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='businessNumber']"));
            Assert.True(uiBusinessNumber.GetAttribute("value") == "111111111");

            // check business type has been selected correctly
            if (bizType != "n indigenous nation account profile")
            {
                var uiBusinessType = ngDriver.FindElement(By.CssSelector("select.form-control"));

                if (bizType == " private corporation")
                    Assert.True(uiBusinessType.GetAttribute("value") == "Private Corporation");

                if (bizType == " public corporation")
                    Assert.True(uiBusinessType.GetAttribute("value") == "Public Corporation");

                if (bizType == " sole proprietorship")
                    Assert.True(uiBusinessType.GetAttribute("value") == "Sole Proprietor");

                if (bizType == " partnership") Assert.True(uiBusinessType.GetAttribute("value") == "Partnership");

                if (bizType == " society") Assert.True(uiBusinessType.GetAttribute("value") == "Society");

                if (bizType == " university") Assert.True(uiBusinessType.GetAttribute("value") == "University");

                if (bizType == " local government")
                    Assert.True(uiBusinessType.GetAttribute("value") == "Local Government");
            }

            // check incorporation number and date of incorporation are correct
            if (bizType == " private corporation" || bizType == " society" || bizType == " public corporation")
            {
                var uiIncorporationNumber =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='bcIncorporationNumber']"));
                Assert.True(uiIncorporationNumber.GetAttribute("value") == "BC1234567");

                var uiIncorporationDate =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='bcIncorporationNumber']"));
                var fieldValueIncorporationDate = uiIncorporationDate.GetAttribute("value");
                Assert.True(fieldValueIncorporationDate != null);
            }

            // check street address 1 is correct
            var uiPhysicalAddressStreet =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressStreet']"));
            Assert.True(uiPhysicalAddressStreet.GetAttribute("value") == "645 Tyee Road");

            // check street address 2 is correct
            var uiPhysicalAddressStreet2 =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressStreet2']"));
            Assert.True(uiPhysicalAddressStreet2.GetAttribute("value") == "West");

            // check physical address city is correct
            var uiPhysicalAddressCity =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressCity']"));
            Assert.True(uiPhysicalAddressCity.GetAttribute("value") == "Victoria");

            // check physical address province is correct
            var uiPhysicalAddressProvince =
                ngDriver.FindElement(By.CssSelector("select[formcontrolname='physicalAddressProvince']"));
            Assert.True(uiPhysicalAddressProvince.GetAttribute("value") == "British Columbia");

            // check physical address postal code is correct
            var uiPhysicalAddressPostalCode =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressPostalCode']"));
            Assert.True(uiPhysicalAddressPostalCode.GetAttribute("value") == "V9A6X5");

            // check physical address country is correct
            var uiPhysicalAddressCountry =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='physicalAddressCountry']"));
            Assert.True(uiPhysicalAddressCountry.GetAttribute("value") == "Canada");

            // check mailing address street 1 is correct
            var uiMailingAddressStreet =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressStreet']"));
            Assert.True(uiMailingAddressStreet.GetAttribute("value") == "#22");

            // check mailing address street 2 is correct
            var uiMailingAddressStreet2 =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressStreet2']"));
            Assert.True(uiMailingAddressStreet2.GetAttribute("value") == "700 Bellevue Way NE");

            // check mailing address city is correct
            var uiMailingAddressCity =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressCity']"));
            Assert.True(uiMailingAddressCity.GetAttribute("value") == "Bellevue");

            // check mailing address province/state is correct
            var uiMailingAddressProvince =
                ngDriver.FindElement(By.CssSelector("select[formcontrolname='mailingAddressProvince']"));
            Assert.True(uiMailingAddressProvince.GetAttribute("value") == "Alberta");

            // check mailing address postal/zip is correct
            var uiMailingAddressPostalCode =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressPostalCode']"));
            Assert.True(uiMailingAddressPostalCode.GetAttribute("value") == "T2E8A2");

            // check mailing address country is correct
            var uiMailingAddressCountry =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='mailingAddressCountry']"));
            Assert.True(uiMailingAddressCountry.GetAttribute("value") == "Canada");

            // check contact phone is correct
            var uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            Assert.True(uiContactPhone.GetAttribute("value") == "(250) 181-1818");

            // check contact email is correct
            var uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactEmail']"));
            Assert.True(uiContactEmail.GetAttribute("value") == "test@automation.com");

            // check liquor policy information link is correct
            if (bizType == "n indigenous nation account profile" || bizType == " local government account profile")
            {
                var uiWebsiteUrl = ngDriver.FindElement(By.CssSelector("input[formcontrolname='websiteUrl']"));
                Assert.True(uiWebsiteUrl.GetAttribute("value") == "https://www.liquorpolicy.org");
            }

            // check authorized person first name is populated
            var uiFirstName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstname']"));
            var fieldValueFirstName = uiFirstName.GetAttribute("value");
            Assert.True(fieldValueFirstName != null);

            // check authorized person last name is populated
            var uiLastName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastname']"));
            var fieldValueLastName = uiLastName.GetAttribute("value");
            Assert.True(fieldValueLastName != null);

            // check authorized person title/position is correct
            var uiJobTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='jobTitle']"));
            Assert.True(uiJobTitle.GetAttribute("value") == "CEO");

            // check authorized person phone number is correct
            var uiTelephone1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='telephone1']"));
            Assert.True(uiTelephone1.GetAttribute("value") == "(778) 181-1818");

            // check authorized person email address is correct
            var uiEmailAddress1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailaddress1']"));
            Assert.True(uiEmailAddress1.GetAttribute("value") == "automated@test.com");

            // check indigenous nation connection details are correct
            if (bizType == "n indigenous nation account profile" || bizType == " local government account profile")
            {
                var uiINConnectionToFederalProducerDetails =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='iNConnectionToFederalProducerDetails']"));
                Assert.True(uiINConnectionToFederalProducerDetails.GetAttribute("value") ==
                            "Name and details of federal producer (automated test) for IN/local government.");
            }

            // check partnership connection details are correct
            if (bizType == " partnership")
            {
                var uiDetailsFederalProducer2 =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='partnersConnectionFederalProducerDetails']"));
                Assert.True(uiDetailsFederalProducer2.GetAttribute("value") ==
                            "The name of the federal producer and details of the connection (partnership).");
            }

            // check federal producer connection to corporation details are correct
            if (bizType == " partnership" || bizType == " private corporation" || bizType == " sole proprietorship" ||
                bizType == " university")
            {
                var uiDetailsFederalProducer3 =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='federalProducerConnectionToCorpDetails']"));
                Assert.True(uiDetailsFederalProducer3.GetAttribute("value") ==
                            "Name and details of federal producer connection to corporation.");
            }

            // check corporation connection to federal producer details are correct
            if (bizType == " private corporation" || bizType == " public corporation" ||
                bizType == " sole proprietorship" || bizType == " university")
            {
                var uiDetailsFederalProducer2 =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='corpConnectionFederalProducerDetails']"));
                Assert.True(uiDetailsFederalProducer2.GetAttribute("value") ==
                            "The name of the federal producer and details of the connection.");
            }

            // check public corporation shareholder and family relationship details are correct
            if (bizType == " public corporation")
            {
                var uiShareholderDetails =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='share20PlusConnectionProducerDetails']"));
                Assert.True(uiShareholderDetails.GetAttribute("value") == "Details of shareholder relationship.");

                var uiFamilyConnectionDetails =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='share20PlusFamilyConnectionProducerDetail']"));
                Assert.True(uiFamilyConnectionDetails.GetAttribute("value") ==
                            "Details of family relationship (automated test).");
            }

            // check society federal producer details are correct
            if (bizType == " society")
            {
                var uiSocietyConnectionDetails =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='societyConnectionFederalProducerDetails']"));
                Assert.True(uiSocietyConnectionDetails.GetAttribute("value") ==
                            "Details of society/federal producer relationship.");
            }

            // check financial interest details are correct
            var uiLiquorFinancialInterestDetails =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='liquorFinancialInterestDetails']"));
            Assert.True(uiLiquorFinancialInterestDetails.GetAttribute("value") ==
                        "Details of the financial interest (automated test).");
        }
    }
}