using System;
using OpenQA.Selenium;
using Protractor;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I review the account profile for a(.+)")]
        public void ReviewAccountProfile(string businessType)
        {
            /*
            Page Title: Please Review the Account Profile
            */

            // used for OneStop testing
            // string bizNumber = "977517895";
            // used for release testing
            var bizNumber = "111111111";
            var incorporationNumber = "BC1234567";

            var physStreetAddress1 = "645 Tyee Road";
            var physStreetAddress2 = "West";
            var physCity = "Victoria";
            var physPostalCode = "V9A 6X5";

            var mailStreet1 = "#22";
            var mailStreet2 = "700 Bellevue Way NE";
            var mailCity = "Bellevue";
            var mailPostalCode = "T2E 8A2";

            var bizPhoneNumber = "2501811818";
            var bizEmail = "test@automation.com";
            var corpGiven = "Automated";
            var corpSurname = "Testing";
            var corpTitle = "CEO";
            var corpContactPhone = "7781811818";
            var corpContactEmail = "automated@test.com";

            // enter the business number
            NgWebElement uiBizNumber = null;
            for (var i = 0; i < 50; i++)
                try
                {
                    var numbers = ngDriver.FindElements(By.CssSelector("input[formControlName='businessNumber']"));
                    if (numbers.Count > 0)
                    {
                        uiBizNumber = numbers[0];
                        break;
                    }
                }
                catch (Exception)
                {
                }

            uiBizNumber.SendKeys(bizNumber);

            // enter the private/public corporation or society incorporation number
            if (businessType == " private corporation" || businessType == " society" ||
                businessType == " public corporation")
            {
                // enter incorporation number
                var uiCorpNumber =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='bcIncorporationNumber']"));
                uiCorpNumber.SendKeys(incorporationNumber);

                // select date of incorporation (= today)
                var uiCalendar1 =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='dateOfIncorporationInBC']"));
                JavaScriptClick(uiCalendar1);

                var uiCalendar2 =
                    ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
                JavaScriptClick(uiCalendar2);
            }

            // enter the physical street address 1
            var uiPhysStreetAddress1 =
                ngDriver.FindElement(By.CssSelector("input[formControlName='physicalAddressStreet']"));
            uiPhysStreetAddress1.SendKeys(physStreetAddress1);

            // enter the physical street address 2
            var uiPhysStreetAddress2 =
                ngDriver.FindElement(By.CssSelector("input[formControlName='physicalAddressStreet2']"));
            uiPhysStreetAddress2.SendKeys(physStreetAddress2);

            // enter the physical city
            var uiPhysCity = ngDriver.FindElement(By.CssSelector("input[formControlName='physicalAddressCity']"));
            uiPhysCity.SendKeys(physCity);

            // select non default province
            var uiNonDefaultProvince =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='physicalAddressProvince'] option[value='Alberta']"));
            uiNonDefaultProvince.Click();

            // select default province
            var uiDefaultProvince =
                ngDriver.FindElement(
                    By.CssSelector(
                        "select[formcontrolname='physicalAddressProvince'] option[value='British Columbia']"));
            uiDefaultProvince.Click();

            // enter the physical postal code
            var uiPhysPostalCode =
                ngDriver.FindElement(By.CssSelector("input[formControlName='physicalAddressPostalCode']"));
            uiPhysPostalCode.SendKeys(physPostalCode);

            // enter the mailing street address 1
            var uiMailingStreetAddress1 =
                ngDriver.FindElement(By.CssSelector("input[formControlName='mailingAddressStreet']"));
            uiMailingStreetAddress1.Clear();
            uiMailingStreetAddress1.SendKeys(mailStreet1);

            // enter the mailing street address 2
            var uiMailingStreetAddress2 =
                ngDriver.FindElement(By.CssSelector("input[formControlName='mailingAddressStreet2']"));
            uiMailingStreetAddress2.Clear();
            uiMailingStreetAddress2.SendKeys(mailStreet2);

            // enter the mailing city
            var uiMailingCity = ngDriver.FindElement(By.CssSelector("input[formControlName='mailingAddressCity']"));
            uiMailingCity.Clear();
            uiMailingCity.SendKeys(mailCity);

            // select non default province
            var uiNonDefaultProvince2 =
                ngDriver.FindElement(
                    By.CssSelector("select[formcontrolname='mailingAddressProvince'] option[value='Alberta']"));
            uiNonDefaultProvince2.Click();

            // enter the mailing postal code
            var uiMailingPostalCode =
                ngDriver.FindElement(By.CssSelector("input[formControlName='mailingAddressPostalCode']"));
            uiMailingPostalCode.Clear();
            uiMailingPostalCode.SendKeys(mailPostalCode);

            // enter the business phone number
            var uiBizPhoneNumber = ngDriver.FindElement(By.CssSelector("input[formControlName='contactPhone']"));
            uiBizPhoneNumber.SendKeys(bizPhoneNumber);

            // enter the business email
            var uiBizEmail = ngDriver.FindElement(By.CssSelector("input[formControlName='contactEmail']"));
            uiBizEmail.SendKeys(bizEmail);

            if (businessType == "n indigenous nation" || businessType == " local government")
            {
                var liquorPolicyLink = "https://www.liquorpolicy.org";

                // enter the liquor policy information link
                var uiLiquorPolicyLink = ngDriver.FindElement(By.CssSelector("input[formcontrolname='websiteUrl']"));
                uiLiquorPolicyLink.SendKeys(liquorPolicyLink);
            }

            // enter the contact given name
            var uiFirstName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstname']"));
            uiFirstName.SendKeys(corpGiven);

            // enter the contact surname
            var uiLastName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastname']"));
            uiLastName.SendKeys(corpSurname);

            // enter the contact title
            var uiCorpTitle = ngDriver.FindElement(By.CssSelector("input[formControlName='jobTitle']"));
            uiCorpTitle.SendKeys(corpTitle);

            // enter the contact phone number
            var uiCorpContactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName='telephone1']"));
            uiCorpContactPhone.SendKeys(corpContactPhone);

            // enter the contact email
            var uiCorpContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailaddress1']"));
            uiCorpContactEmail.SendKeys(corpContactEmail);

            if (businessType == "n indigenous nation" || businessType == " local government")
            {
                // select 'Yes' for connection to a federal producer
                var uiINConnectionFederalProducer =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='iNConnectionToFederalProducer']"));
                uiINConnectionFederalProducer.Click();

                // enter the name of the federal producer and details of the connection 
                var INnameAndDetails = "Name and details of federal producer (automated test) for IN/local government.";
                var uiINDetailsFederalProducer =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='iNConnectionToFederalProducerDetails']"));
                uiINDetailsFederalProducer.SendKeys(INnameAndDetails);
            }

            if (businessType == " private corporation" || businessType == " sole proprietorship" ||
                businessType == " university")
            {
                // select 'Yes' for corporation connection to federal producer 
                var uiCorpConnectionFederalProducer =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='corpConnectionFederalProducer']"));
                uiCorpConnectionFederalProducer.Click();

                // enter the name of the federal producer and details of the connection 
                var nameAndDetails = "The name of the federal producer and details of the connection.";
                var uiDetailsFederalProducer2 =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='corpConnectionFederalProducerDetails']"));
                uiDetailsFederalProducer2.SendKeys(nameAndDetails);

                // select 'Yes' for federal producer connection to corporation
                var uiCorpConnectionFederalProducer2 =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='federalProducerConnectionToCorp']"));
                uiCorpConnectionFederalProducer2.Click();

                // enter the name of the federal producer and details of the connection 
                var nameAndDetails2 = "Name and details of federal producer connection to corporation.";
                var uiDetailsFederalProducer3 =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='federalProducerConnectionToCorpDetails']"));
                uiDetailsFederalProducer3.SendKeys(nameAndDetails2);
            }

            if (businessType == " partnership")
            {
                // select 'Yes' for partnership connection to federal producer 
                var uiPartnerConnectionFederalProducer =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='partnersConnectionFederalProducer']"));
                uiPartnerConnectionFederalProducer.Click();

                // enter the name of the federal producer and details of the connection 
                var nameAndDetails = "The name of the federal producer and details of the connection (partnership).";
                var uiDetailsFederalProducer2 =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='partnersConnectionFederalProducerDetails']"));
                uiDetailsFederalProducer2.SendKeys(nameAndDetails);

                // select 'Yes' for federal producer connection to corporation
                var uiCorpConnectionFederalProducer2 =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='federalProducerConnectionToCorp']"));
                uiCorpConnectionFederalProducer2.Click();

                // enter the name of the federal producer and details of the connection 
                var nameAndDetails2 = "Name and details of federal producer connection to corporation.";
                var uiDetailsFederalProducer3 =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='federalProducerConnectionToCorpDetails']"));
                uiDetailsFederalProducer3.SendKeys(nameAndDetails2);
            }

            if (businessType == " public corporation")
            {
                // select 'Yes' for 'Does the corporation have any association, connection or financial interest in a federally licensed producer of cannabis?'
                var uiCorpConnectionFederalProducer =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='corpConnectionFederalProducer']"));
                uiCorpConnectionFederalProducer.Click();

                // enter the name of the federal producer and details of the connection 
                var nameAndDetails = "The name of the federal producer and details of the connection.";
                var uiDetailsFederalProducer2 =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='corpConnectionFederalProducerDetails']"));
                uiDetailsFederalProducer2.SendKeys(nameAndDetails);

                // select 'Yes' for 'Does a federally licensed producer of cannabis have any association, connection or financial interest in the corporation?'
                var uiShareholderConnectionConnectionToCorp =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='federalProducerConnectionToCorp']"));
                uiShareholderConnectionConnectionToCorp.Click();

                var shareholderDetails = "Details of shareholder relationship.";
                var uiShareholderDetails =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='federalProducerConnectionToCorpDetails']"));
                uiShareholderDetails.SendKeys(shareholderDetails);

                // select 'Yes' for 'Do you or any of your shareholders have any amount of ownership interest in another B.C. liquor licence, or any association with a third party operator for another liquor licence, or have an immediate family member (spouse, parent, sibling or child) with any amount of ownership interest in another liquor licence?'
                var uiFamilyConnectionConnectionToCorp =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='liquorFinancialInterest']"));
                uiFamilyConnectionConnectionToCorp.Click();

                // enter details of family connection
                var familyRelationship = "Details of family relationship (automated test).";
                var uiFamilyConnectionDetails =
                    ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='liquorFinancialInterestDetails']"));
                uiFamilyConnectionDetails.SendKeys(familyRelationship);
            }

            if (businessType == " society")
            {
                // select 'Yes' for society connection to federal producer 
                var uiSocietyConnectionFederalProducer =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='societyConnectionFederalProducer']"));
                uiSocietyConnectionFederalProducer.Click();

                // enter details of society connection
                var societyDetails = "Details of society/federal producer relationship.";
                var uiSocietyConnectionDetails =
                    ngDriver.FindElement(
                        By.CssSelector("textarea[formcontrolname='societyConnectionFederalProducerDetails']"));
                uiSocietyConnectionDetails.SendKeys(societyDetails);
            }

            // click on the liquor financial interest radio button
            var uiLiquorFinInterestRadio =
                ngDriver.FindElement(By.XPath("//app-connection-to-producers/div[2]/section/input[1]"));
            uiLiquorFinInterestRadio.Click();

            // enter the details of the financial interest
            var finDetails = "Details of the financial interest (automated test).";
            var uiLiquorFinInterestTextArea =
                ngDriver.FindElement(By.CssSelector("textarea[formcontrolname = 'liquorFinancialInterestDetails']"));
            uiLiquorFinInterestTextArea.SendKeys(finDetails);

            // click on Continue to Organization Review button
            var uiContinueAppButton = ngDriver.FindElement(By.Id("continueToApp"));
            uiContinueAppButton.Click();
        }
    }
}