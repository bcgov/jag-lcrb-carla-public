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
        [And(@"I review the account profile for a(.*)")]
        public void ReviewAccountProfile(string businessType)
        {
            /*
            Page Title: Please Review the Account Profile
            */

            string bizNumber = "123456789";
            string incorporationNumber = "BC1234567";

            string physStreetAddress1 = "645 Tyee Road";
            string physStreetAddress2 = "West";
            string physCity = "Victoria";
            string physPostalCode = "V9A 6X5";

            string mailStreet1 = "#22";
            string mailStreet2 = "700 Bellevue Way NE";
            string mailCity = "Bellevue";
            string mailProvince = "WA";
            string mailPostalCode = "98004";
            string mailCountry = "United States";

            string bizPhoneNumber = "2501811818";
            string bizEmail = "test@automation.com";
            string corpTitle = "CEO";
            string corpContactPhone = "7781811818";
            string corpContactEmail = "automated@test.com";

            // enter the business number
            NgWebElement uiBizNumber = ngDriver.FindElement(By.CssSelector("input[formControlName='businessNumber']"));
            uiBizNumber.SendKeys(bizNumber);

            // enter the private/public corporation or society incorporation number
            if (businessType == " private corporation" || businessType == " society" || businessType == " public corporation")
            {
                // enter incorporation number
                NgWebElement uiCorpNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='bcIncorporationNumber']"));
                uiCorpNumber.SendKeys(incorporationNumber);

                // select date of incorporation (= today)
                NgWebElement uiCalendar1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='dateOfIncorporationInBC']"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].click();", uiCalendar1);

                NgWebElement uiCalendar2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
                IJavaScriptExecutor executor2 = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor2.ExecuteScript("arguments[0].click();", uiCalendar2);
            }

            // enter the physical street address 1
            NgWebElement uiPhysStreetAddress1 = ngDriver.FindElement(By.CssSelector("input[formControlName='physicalAddressStreet']"));
            uiPhysStreetAddress1.SendKeys(physStreetAddress1);

            // enter the physical street address 2
            NgWebElement uiPhysStreetAddress2 = ngDriver.FindElement(By.CssSelector("input[formControlName='physicalAddressStreet2']"));
            uiPhysStreetAddress2.SendKeys(physStreetAddress2);

            // enter the physical city
            NgWebElement uiPhysCity = ngDriver.FindElement(By.CssSelector("input[formControlName='physicalAddressCity']"));
            uiPhysCity.SendKeys(physCity);

            // enter the physical postal code
            NgWebElement uiPhysPostalCode = ngDriver.FindElement(By.CssSelector("input[formControlName='physicalAddressPostalCode']"));
            uiPhysPostalCode.SendKeys(physPostalCode);

            // enter the mailing street address 1
            NgWebElement uiMailingStreetAddress1 = ngDriver.FindElement(By.CssSelector("input[formControlName='mailingAddressStreet']"));
            uiMailingStreetAddress1.Clear();
            uiMailingStreetAddress1.SendKeys(mailStreet1);

            // enter the mailing street address 2
            NgWebElement uiMailingStreetAddress2 = ngDriver.FindElement(By.CssSelector("input[formControlName='mailingAddressStreet2']"));
            uiMailingStreetAddress2.Clear();
            uiMailingStreetAddress2.SendKeys(mailStreet2);

            // enter the mailing city
            NgWebElement uiMailingCity = ngDriver.FindElement(By.CssSelector("input[formControlName='mailingAddressCity']"));
            uiMailingCity.Clear();
            uiMailingCity.SendKeys(mailCity);

            // enter the mailing province
            NgWebElement uiMailingProvince = ngDriver.FindElement(By.CssSelector("input[formControlName='mailingAddressProvince']"));
            uiMailingProvince.Clear();
            uiMailingProvince.SendKeys(mailProvince);

            // enter the mailing postal code
            NgWebElement uiMailingPostalCode = ngDriver.FindElement(By.CssSelector("input[formControlName='mailingAddressPostalCode']"));
            uiMailingPostalCode.Clear();
            uiMailingPostalCode.SendKeys(mailPostalCode);

            // enter the mailing country
            NgWebElement uiMailingCountry = ngDriver.FindElement(By.CssSelector("input[formControlName='mailingAddressCountry']"));
            uiMailingCountry.Clear();
            uiMailingCountry.SendKeys(mailCountry);

            // enter the business phone number
            NgWebElement uiBizPhoneNumber = ngDriver.FindElement(By.CssSelector("input[formControlName='contactPhone']"));
            uiBizPhoneNumber.SendKeys(bizPhoneNumber);

            // enter the business email
            NgWebElement uiBizEmail = ngDriver.FindElement(By.CssSelector("input[formControlName='contactEmail']"));
            uiBizEmail.SendKeys(bizEmail);

            if ((businessType == "n indigenous nation") || (businessType == " local government"))
            {
                string liquorPolicyLink = "https://www.liquorpolicy.org";

                // enter the liquor policy information link
                NgWebElement uiLiquorPolicyLink = ngDriver.FindElement(By.CssSelector("input[formcontrolname='websiteUrl']"));
                uiLiquorPolicyLink.SendKeys(liquorPolicyLink);
            }

            // enter the contact title
            NgWebElement uiCorpTitle = ngDriver.FindElement(By.CssSelector("input[formControlName='jobTitle']"));
            uiCorpTitle.SendKeys(corpTitle);

            // enter the contact phone number
            NgWebElement uiCorpContactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName='telephone1']"));
            uiCorpContactPhone.SendKeys(corpContactPhone);

            // enter the contact email
            NgWebElement uiCorpContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailaddress1']"));
            uiCorpContactEmail.SendKeys(corpContactEmail);

            if ((businessType == "n indigenous nation") || (businessType == " local government"))
            {
                // select 'Yes' for connection to a federal producer
                NgWebElement uiINConnectionFederalProducer = ngDriver.FindElement(By.CssSelector("input[formcontrolname='iNConnectionToFederalProducer']"));
                uiINConnectionFederalProducer.Click();

                // enter the name of the federal producer and details of the connection 
                string INnameAndDetails = "Name and details of federal producer (automated test) for IN/local government.";
                NgWebElement uiINDetailsFederalProducer = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='iNConnectionToFederalProducerDetails']"));
                uiINDetailsFederalProducer.SendKeys(INnameAndDetails);
            }

            if ((businessType == " private corporation") || (businessType == " sole proprietorship") || (businessType == " university"))
            {
                // select 'Yes' for corporation connection to federal producer 
                NgWebElement uiCorpConnectionFederalProducer = ngDriver.FindElement(By.CssSelector("input[formcontrolname='corpConnectionFederalProducer']"));
                uiCorpConnectionFederalProducer.Click();

                // enter the name of the federal producer and details of the connection 
                string nameAndDetails = "The name of the federal producer and details of the connection.";
                NgWebElement uiDetailsFederalProducer2 = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='corpConnectionFederalProducerDetails']"));
                uiDetailsFederalProducer2.SendKeys(nameAndDetails);

                // select 'Yes' for federal producer connection to corporation
                NgWebElement uiCorpConnectionFederalProducer2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='federalProducerConnectionToCorp']"));
                uiCorpConnectionFederalProducer2.Click();

                // enter the name of the federal producer and details of the connection 
                string nameAndDetails2 = "Name and details of federal producer connection to corporation.";
                NgWebElement uiDetailsFederalProducer3 = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='federalProducerConnectionToCorpDetails']"));
                uiDetailsFederalProducer3.SendKeys(nameAndDetails2);
            }

            if ((businessType == " partnership"))
            {
                // select 'Yes' for partnership connection to federal producer 
                NgWebElement uiPartnerConnectionFederalProducer = ngDriver.FindElement(By.CssSelector("input[formcontrolname='partnersConnectionFederalProducer']"));
                uiPartnerConnectionFederalProducer.Click();

                // enter the name of the federal producer and details of the connection 
                string nameAndDetails = "The name of the federal producer and details of the connection (partnership).";
                NgWebElement uiDetailsFederalProducer2 = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='partnersConnectionFederalProducerDetails']"));
                uiDetailsFederalProducer2.SendKeys(nameAndDetails);

                // select 'Yes' for federal producer connection to corporation
                NgWebElement uiCorpConnectionFederalProducer2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='federalProducerConnectionToCorp']"));
                uiCorpConnectionFederalProducer2.Click();

                // enter the name of the federal producer and details of the connection 
                string nameAndDetails2 = "Name and details of federal producer connection to corporation.";
                NgWebElement uiDetailsFederalProducer3 = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='federalProducerConnectionToCorpDetails']"));
                uiDetailsFederalProducer3.SendKeys(nameAndDetails2);
            }

            if (businessType == " public corporation")
            {
                // select 'Yes' for corporation connection to federal producer 
                NgWebElement uiCorpConnectionFederalProducer = ngDriver.FindElement(By.CssSelector("input[formcontrolname='corpConnectionFederalProducer']"));
                uiCorpConnectionFederalProducer.Click();

                // enter the name of the federal producer and details of the connection 
                string nameAndDetails = "The name of the federal producer and details of the connection.";
                NgWebElement uiDetailsFederalProducer2 = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='corpConnectionFederalProducerDetails']"));
                uiDetailsFederalProducer2.SendKeys(nameAndDetails);

                // select 'Yes' for shareholder connection
                NgWebElement uiShareholderConnectionConnectionToCorp = ngDriver.FindElement(By.CssSelector("input[formcontrolname='share20PlusConnectionProducer']"));
                uiShareholderConnectionConnectionToCorp.Click();

                string shareholderDetails = "Details of shareholder relationship.";
                NgWebElement uiShareholderDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='share20PlusConnectionProducerDetails']"));
                uiShareholderDetails.SendKeys(shareholderDetails);

                // select 'Yes' for family connection
                NgWebElement uiFamilyConnectionConnectionToCorp = ngDriver.FindElement(By.CssSelector("input[formcontrolname='share20PlusFamilyConnectionProducer']"));
                uiFamilyConnectionConnectionToCorp.Click();

                // enter details of family connection
                string familyRelationship = "Details of family relationship (automated test).";
                NgWebElement uiFamilyConnectionDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='share20PlusFamilyConnectionProducerDetail']"));
                uiFamilyConnectionDetails.SendKeys(familyRelationship);
            }

            if (businessType == " society")
            {
                // select 'Yes' for society connection to federal producer 
                NgWebElement uiSocietyConnectionFederalProducer = ngDriver.FindElement(By.CssSelector("input[formcontrolname='societyConnectionFederalProducer']"));
                uiSocietyConnectionFederalProducer.Click();

                // enter details of society connection
                string societyDetails = "Details of society/federal producer relationship.";
                NgWebElement uiSocietyConnectionDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='societyConnectionFederalProducerDetails']"));
                uiSocietyConnectionDetails.SendKeys(societyDetails);
            }

            // click on the liquor financial interest radio button
            NgWebElement uiLiquorFinInterestRadio = ngDriver.FindElement(By.XPath("//app-connection-to-producers/div[3]/section[1]/input[1]"));
            uiLiquorFinInterestRadio.Click();

            // enter the details of the financial interest
            string finDetails = "Details of the financial interest (automated test).";
            NgWebElement uiLiquorFinInterestTextArea = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname = 'liquorFinancialInterestDetails']"));
            uiLiquorFinInterestTextArea.SendKeys(finDetails);

            // click on Continue to Organization Review button
            NgWebElement uiContinueAppButton = ngDriver.FindElement(By.Id("continueToApp"));
            uiContinueAppButton.Click();
        }
    }
}
