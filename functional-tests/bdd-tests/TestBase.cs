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
    public abstract class TestBase : Feature, IDisposable
    {
        // Protractor driver
        protected NgWebDriver ngDriver;

        protected IConfigurationRoot configuration;

        protected string baseUri;

        protected string applicationID;


        protected TestBase()
        {
            string path = Directory.GetCurrentDirectory();

            configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets("a004e634-29c7-48b6-becc-87fe16be7538")
                .Build();

            //bool runlocal = true;
            ChromeOptions options = new ChromeOptions();
            // run headless when in CI
            if (!string.IsNullOrEmpty(configuration["OPENSHIFT_BUILD_COMMIT"]) || !string.IsNullOrEmpty(configuration["Build.BuildNumber"]))
            {
                Console.Out.WriteLine("Enabling Headless Mode");
                options.AddArguments("headless", "no-sandbox", "disable-web-security", "no-zygote", "disable-gpu", "disable-dev-shm-usage", "disable-infobars", "start-maximized", "hide-scrollbars", "window-size=1920,1080");
                if (!string.IsNullOrEmpty(configuration["CHROME_BINARY_LOCATION"]))
                {
                    options.BinaryLocation = configuration["CHROME_BINARY_LOCATION"];
                }
            }
            else
            {
                options.AddArguments("start-maximized");
            }

            var driver = new ChromeDriver(path, options);

            double timeout = 90.0;

            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);

            ngDriver = new NgWebDriver(driver);

            ngDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeout);
            ngDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeout);
            ngDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeout);

            baseUri = configuration["baseUri"] ?? "https://dev.justice.gov.bc.ca/cannabislicensing";
        }


        [And(@"I click on Home page")]
        public void ClickOnHomePage()
        {
            ngDriver.Navigate().GoToUrl($"{baseUri}");
            ngDriver.WaitForAngular();
        }


        private void DoLogin(string businessType)
        {
            ngDriver.Navigate().GoToUrl($"{baseUri}dashboard");

            /* 
            Page Title: Terms of Use
            */

            // select the acceptance checkbox
            NgWebElement uiTermsOfUseCheckbox = ngDriver.FindElement(By.CssSelector("input.terms-cb[type='checkbox']"));
            uiTermsOfUseCheckbox.Click();

            // click on the Continue button
            NgWebElement uiContinueButton = ngDriver.FindElement(By.CssSelector("button.termsAccept"));
            uiContinueButton.Click();

            /* 
            Page Title: Please confirm the business or organization name associated to the Business BCeID.
            */

            // click on the Yes button
            NgWebElement uiConfirmationButton = ngDriver.FindElement(By.CssSelector("button.confirmYes"));
            uiConfirmationButton.Click();

            /* 
            Page Title: Please confirm the organization type associated with the Business BCeID:
            */

            // if this is a private corporation, click the radio button
            if (businessType == " private corporation")
            {
                NgWebElement uiPrivateCorporationRadio = ngDriver.FindElement(By.CssSelector("input[value='PrivateCorporation'][type = 'radio']"));
                uiPrivateCorporationRadio.Click();
            }

            // if this is a public corporation, click the radio button
            if (businessType == " public corporation")
            {
                NgWebElement uiPublicCorporationRadio = ngDriver.FindElement(By.CssSelector("[value='PublicCorporation'][type='radio']"));
                uiPublicCorporationRadio.Click();
            }

            // if this is a sole proprietorship, click the radio button
            if (businessType == " sole proprietorship")
            {
                NgWebElement uiSoleProprietorshipRadio = ngDriver.FindElement(By.CssSelector("[value='SoleProprietorship'][type='radio']"));
                uiSoleProprietorshipRadio.Click();
            }

            // if this is a partnership, click the radio button
            if (businessType == " partnership")
            {
                NgWebElement uiPartnershipRadio = ngDriver.FindElement(By.CssSelector("[value='Partnership'][type='radio']"));
                uiPartnershipRadio.Click();
            }

            // if this is a society, click the radio button
            if (businessType == " society")
            {
                NgWebElement uiSocietyRadio = ngDriver.FindElement(By.CssSelector("[type='radio'][value='Society']"));
                uiSocietyRadio.Click();
            }

            // if this is a university, click the radio button
            if (businessType == " university")
            {
                NgWebElement uiUniversityRadio = ngDriver.FindElement(By.CssSelector("[type='radio'][value='University']"));
                uiUniversityRadio.Click();
            }

            // if this is an indigenous nation, click the radio button
            if (businessType == "n indigenous nation")
            {
                NgWebElement uiIndigenousNationRadio = ngDriver.FindElement(By.CssSelector("[value='IndigenousNation'][type='radio']"));
                uiIndigenousNationRadio.Click();
            }

            // if this is a local government, click the radio button
            if (businessType == " local government")
            {
                NgWebElement uiLocalGovernmentRadio = ngDriver.FindElement(By.CssSelector("[value='LocalGovernment'][type='radio']"));
                uiLocalGovernmentRadio.Click();
            }

            // click on the Next button
            NgWebElement uiNextButton = ngDriver.FindElement(By.CssSelector(".btn-primary"));
            uiNextButton.Click();

            /* 
            Page Title: Please confirm the name associated with the Business BCeID login provided.
            */

            // click on the Yes button
            NgWebElement uiConfirmNameButton = ngDriver.FindElement(By.CssSelector("app-bceid-confirmation .btn-primary"));
            uiConfirmNameButton.Click();
        }


        public void CarlaLogin(string businessType)
        {
            Random random = new Random();

            // load the dashboard page
            string test_start = "login/token/AT" + DateTime.Now.Ticks.ToString() + random.Next(0, 999).ToString();
            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");
            ngDriver.IgnoreSynchronization = false;

            DoLogin(businessType);
        }


        public void CarlaLoginNoCheck(string businessType)
        {
            // load the dashboard page
            string test_start = configuration["test_start"];
            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");
            ngDriver.IgnoreSynchronization = false;

            DoLogin(businessType);
        }


        public void CarlaLoginWithUser(string businessType)
        {
            // load the dashboard page
            string test_start = configuration["test_start"];
            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");
            ngDriver.IgnoreSynchronization = false;

            DoLogin(businessType);
        }


        [And(@"I am logged in to the dashboard as a(.*)")]
        public void ViewDashboard(string businessType)
        {
            CarlaLoginNoCheck(businessType);
        }


        [Then(@"I see the login page")]
        public void SeeLogin()
        {
            Assert.True(ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }


        public void Dispose()
        {
            ngDriver.Quit();

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }


        [And(@"the account is deleted")]
        public void DeleteMyAccount()
        {
            this.CarlaDeleteCurrentAccount();
        }


        public void NavigateToFeatures()
        {
            ngDriver.IgnoreSynchronization = true;
            // navigate to the feature flags page
            ngDriver.WrappedDriver.Navigate().GoToUrl($"{baseUri}api/features");
        }


        public void IgnoreSynchronizationFalse()
        {
            ngDriver.IgnoreSynchronization = false;
        }


        public void CheckFeatureFlag(string flag)
        {
            // confirm that the correct flag is enabled during this test
            Assert.True(ngDriver.FindElement(By.XPath($"//body[contains(.,'{flag}')]")).Displayed);
        }


        public void CheckFeatureFlagsCOVIDTempExtension()
        {
            CheckFeatureFlag("CovidApplication");
        }


        public void CheckFeatureFlagsLiquorOne()
        {
            CheckFeatureFlag("LiquorOne");
        }


        public void CheckFeatureFlagsLiquorTwo()
        {
            CheckFeatureFlag("LiquorTwo");
        }


        public void CheckFeatureFlagsMaps()
        {
            CheckFeatureFlag("Maps");
        }


        public void CheckFeatureFlagsLGIN()
        {
            CheckFeatureFlag("LGApprovals");
        }


        public void CheckFeatureFlagsIN()
        {
            CheckFeatureFlag("IndigenousNation");
        }


        public void CheckFeatureFlagsLicenseeChanges()
        {
            CheckFeatureFlag("LicenseeChanges");
        }


        public void CheckFeatureFlagsSecurityScreening()
        {
            CheckFeatureFlag("SecurityScreening");
        }


        public void MakePayment()
        {
            string testCC = configuration["test_cc"];
            string testCVD = configuration["test_ccv"];

            //browser sync - don't wait for Angular
            ngDriver.IgnoreSynchronization = true;

            /* 
            Page Title: Internet Payments Program (Bambora)
            */

            ngDriver.FindElement(By.Name("trnCardNumber")).SendKeys(testCC);

            ngDriver.FindElement(By.Name("trnCardCvd")).SendKeys(testCVD);

            ngDriver.FindElement(By.Name("submitButton")).Click();

            System.Threading.Thread.Sleep(4000);

            //turn back on when returning to Angular
            ngDriver.IgnoreSynchronization = false;
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

            // find the licence ID - TODO
            NgWebElement uiLicenceID = ngDriver.FindElement(By.XPath("//a/span"));

            string firstYearLicenceFee = "Pay First Year Licensing Fee";

            // click on the pay first year licence fee link
            NgWebElement uiFirstYearLicenceFee = ngDriver.FindElement(By.LinkText(firstYearLicenceFee));
            uiFirstYearLicenceFee.Click();

            // pay the licence fee
            MakePayment();

            System.Threading.Thread.Sleep(4000);

            /* 
            Page Title: Payment Approved
            */

            if (feeType == "Cannabis")
            {
                // confirm correct payment amount for CRS
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$1,500.00')]")).Displayed);
            }

            ClickLicencesTab();
        }


        [And(@"I confirm the payment receipt for a (.*)")]
        public void ConfirmPaymentReceipt(string applicationType)
        {
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


        public void CarlaDeleteCurrentAccount()
        {
            ngDriver.IgnoreSynchronization = true;

            // using wrapped driver as this call is not angular
            ngDriver.Navigate().GoToUrl($"{baseUri}api/accounts/delete/current");

            ngDriver.IgnoreSynchronization = false;

            ngDriver.Navigate().GoToUrl($"{baseUri}logout");
        }


        [Then(@"I return to the dashboard")]
        public void ThenReturnToDashboard()
        {
            ReturnToDashboard();
        }


        [And(@"I return to the dashboard")]
        public void ReturnToDashboard()
        {
            string retDash = "Return to Dashboard";

            // click on Return to Dashboard link
            NgWebElement uiReturnDash = ngDriver.FindElement(By.LinkText(retDash));
            uiReturnDash.Click();
        }


        [And(@"I click on the link for (.*)")]
        public void ClickOnLink(string specificLink)
        {
            NgWebElement uiRequestedLink = ngDriver.FindElement(By.LinkText(specificLink));
            uiRequestedLink.Click();
        }


        [And(@"the correct terms and conditions are displayed for (.*)")]
        public void CorrectTermsAndConditionsDisplayed(string licenceType)
        {
            if (licenceType == "Catering")
            {
                // check that the correct text is displayed for Catering
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'For sale and service of liquor at another person')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s event where food service is catered by the licensee, unless otherwise permitted.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'The terms and conditions to which this licence is subject include the terms and conditions contained in the licensee Terms and Conditions Handbook, which is available on the Liquor and Cannabis Regulation Branch website. The Terms and Conditions Handbook is amended from time to time.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Licensee may only serve liquor at a catered event for which LCRB has issued a catering authorization.')]")).Displayed);
            }

            if (licenceType == "CRS")
            {
                // check that the correct text is displayed for CRS
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'This licence is subject to the terms and conditions specified in the restriction or approval letter(s) and those contained in the Cannabis Retail Store Handbook, which may be amended from time to time.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Packaged cannabis may only be sold within the service area outlined in blue on the LCRB approved floor plan, unless otherwise endorsed or approved by the LCRB.')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'The establishment may be open anytime between the hours of 9 a.m. and 11 p.m., subject to further restriction by the local government or Indigenous nation.')]")).Displayed);
            }

        }


        [And(@"I click on the Licences tab")]
        public void ClickLicencesTab()
        {
            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }


        [And(@"the expiry date is changed to today")]
        public void ExpiryDateToday()
        {
            ngDriver.IgnoreSynchronization = true;

            // navigate to api/Licenses/<Application ID>/setexpiry
            ngDriver.Navigate().GoToUrl($"{baseUri}api/Licenses/{applicationID}/setexpiry");

            // wait for the automated expiry process to run
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'OK')]")).Displayed);

            ngDriver.IgnoreSynchronization = false;

            // navigate back to Licenses tab
            ngDriver.Navigate().GoToUrl($"{baseUri}licences");
        }


        [And(@"I renew the licence")]
        public void RenewLicence()
        {
            string renewLicenceLink = "Renew Licence";

            // click on the Renew Licence link
            NgWebElement uiRenewLicence = ngDriver.FindElement(By.LinkText(renewLicenceLink));
            uiRenewLicence.Click();

            // select 'No'
            // 1. Have you or any partner, shareholder, director, or officer of this licensee been arrested for, charged with, or convicted of a criminal offence within the past 12 months that you have not reported to the LCRB?
            NgWebElement uiCriminalOffence = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalCriminalOffenceCheck'] button#mat-button-toggle-16-button"));
            uiCriminalOffence.Click();

            // select 'No'
            // 2. Has there been an unreported sale of the business associated with the licence within the past 12 months? 
            NgWebElement uiUnreportedSale = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalUnreportedSaleOfBusiness'] button#mat-button-toggle-18-button"));
            uiUnreportedSale.Click();

            // select 'No'
            // 3. Our records show that this establishment is licensed as a Private Corporation. Has this changed? 
            NgWebElement uiBusinessType = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalBusinessType'] button#mat-button-toggle-20-button"));
            uiBusinessType.Click();

            // select 'No'
            // 4. Have you, any partner, shareholder, director, officer, or an immediate family member of any of the aforementioned associates acquired a new interest or expanded an existing interest - financial or otherwise - in a federal producer of cannabis within the past 12 months? 
            NgWebElement uiTiedHouse = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalTiedhouse'] button#mat-button-toggle-22-button"));
            uiTiedHouse.Click();

            // select 'No'
            // 5. Has a federal produce of cannabis acquired a new interest or expanded an existing interest - financial or otherwise - in the licensee Private Corporation within the past 12 months? 
            NgWebElement uiTiedHouseFederalInterest = ngDriver.FindElement(By.CssSelector("[formcontrolname='tiedhouseFederalInterest'] button#mat-button-toggle-24-button"));
            uiTiedHouseFederalInterest.Click();

            // select 'No'
            // 6. Have you made any unreported changes to your organizational leadership within the past 12 months?
            NgWebElement uiOrgLeadership = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalOrgLeadership'] button#mat-button-toggle-26-button"));
            uiOrgLeadership.Click();

            // select 'No'
            // 7. Have you made any unreported changes to your key personnel within the past 12 months?
            NgWebElement uiKeyPersonnel = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalkeypersonnel'] button#mat-button-toggle-28-button"));
            uiKeyPersonnel.Click();

            // select 'No'
            // 8. Have you made any unreported changes to your share structure within the past 12 months? 
            NgWebElement uiShareholderStructure = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalShareholders'] button#mat-button-toggle-30-button"));
            uiShareholderStructure.Click();

            // select 'No'
            // 9. Do you have an outstanding payable fine under the Offence Act or outstanding payable monetary penalty under the Cannabis Control and Licensing Act that has not yet been paid?
            NgWebElement uiOutstandingFine = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalOutstandingFines'] button#mat-button-toggle-32-button"));
            uiOutstandingFine.Click();

            // select 'No'
            // 10. Have you made an unreported change to your store’s name in the past 12 months?
            NgWebElement uiBrandingChange = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalBranding'] button#mat-button-toggle-34-button"));
            uiBrandingChange.Click();

            // select 'No'
            // 11. Have you updated the store’s signage or branding in the past 12 months?
            NgWebElement uiSignageChange = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalSignage'] button#mat-button-toggle-36-button"));
            uiSignageChange.Click();

            // select 'No'
            // 12. Have you made an unreported change of location of your establishment within the past 12 months? (This includes any changes to the Parcel Identification Number where your establishment is located, even if the physical location has not changed).
            NgWebElement uiEstablishmentAddress = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalEstablishmentAddress'] button#mat-button-toggle-38-button"));
            uiEstablishmentAddress.Click();

            // select 'No'
            // 13. Have you sold the property or transferred the lease associated with this cannabis retail store licence within the past 12 months?
            NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalValidInterest'] button#mat-button-toggle-40-button"));
            uiValidInterest.Click();

            // select 'No'
            // 14. Are you aware of any local government or Indigenous nation zoning changes with respect to your establishment location?
            NgWebElement uiZoning = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalZoning'] button#mat-button-toggle-42-button"));
            uiZoning.Click();

            // select 'No'
            // 15. Have you made any unreported changes to the store’s floor plan within the past 12 months? 
            NgWebElement uiFloorPlan = ngDriver.FindElement(By.CssSelector("[formcontrolname='renewalFloorPlan'] button#mat-button-toggle-44-button"));
            uiFloorPlan.Click();

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgree.Click();

            ClickOnSubmitButton();

            MakePayment();

            // confirm renewal amount
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$1500.00')]")).Displayed);
        }


        [And(@"I click on the branding change link for (.*)")]
        public void ClickOnBrandingChangeLink(string changeType)
        {
            /* 
            Page Title: Licences
            */

            string nameBrandingLinkCannabis = "Request Store Name or Branding Change";
            string nameBrandingLinkCateringMfg = "Establishment Name Change Application";

            if ((changeType == "Catering") || (changeType == "Manufacturing"))
            {
                // click on the Establishment Name Change Application link
                NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText(nameBrandingLinkCateringMfg));
                uiRequestChange.Click();
            }

            if (changeType == "Cannabis")
            {
                // click on the Request Store Name or Branding Change link
                NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText(nameBrandingLinkCannabis));
                uiRequestChange.Click();
            }
        }


        [And(@"I click on the button for (.*)")]
        public void ClickOnButton(string specificButton)
        {
            if (specificButton == "CRS terms and conditions")
            {
                // click on the Terms and Conditions button
                NgWebElement uiTermsAndConditions = ngDriver.FindElement(By.CssSelector("mat-expansion-panel mat-expansion-panel-header[role='button']"));
                uiTermsAndConditions.Click();
            }

            if (specificButton == "Catering terms and conditions")
            {
                // click on the Terms and Conditions button
                NgWebElement uiTermsAndConditions = ngDriver.FindElement(By.CssSelector("mat-expansion-panel mat-expansion-panel-header[role='button']"));
                uiTermsAndConditions.Click();
            }

            if (specificButton == "Confirm Organization Information is Complete")
            {
                // click on the Confirm Organization Information is Complete button
                NgWebElement uiCompleteButton = ngDriver.FindElement(By.CssSelector("app-application-licensee-changes button.btn-primary"));
                uiCompleteButton.Click();
            }

            if (specificButton == "Pay for Application")
            {
                // click on the Pay for Application button
                NgWebElement uiPayButton = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
                uiPayButton.Click();
            }

            if (specificButton == "Pay for Application for an indigenous nation")
            {
                // click on the Pay for Application button
                NgWebElement uiPayButton = ngDriver.FindElement(By.CssSelector(".justify-content-between button.btn-primary"));
                uiPayButton.Click();
            }

            if (specificButton == "Submit Organization Information")
            {
                // click on the Submit Org Info button
                NgWebElement uiSubmitOrgInfoButton = ngDriver.FindElement(By.CssSelector("app-application-licensee-changes button.btn-primary"));
                uiSubmitOrgInfoButton.Click();
            }
        }


        [And(@"I click on the Submit button")]
        public void ClickOnSubmitButton()
        {
            NgWebElement uiSubmitButton = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
            uiSubmitButton.Click();
        }


        [And(@"I click on the Dashboard link")]
        public void ClickOnDashboard()
        {
            string dashboard = "Dashboard";

            // click on the Dashboard link
            NgWebElement uiDashboard = ngDriver.FindElement(By.LinkText(dashboard));
            uiDashboard.Click();
        }


        [And(@"I click on the Save for Later button")]
        public void ClickSaveForLater()
        {
            // click on the Save For Later button
            NgWebElement uiSaveForLaterButton = ngDriver.FindElement(By.CssSelector("button.btn-secondary span"));
            uiSaveForLaterButton.Click();
        }


        public void SharedCalendarDate()
        {
            // click on the previous button
            NgWebElement uiOpenCalendarPrevious = ngDriver.FindElement(By.CssSelector(".mat-calendar .mat-calendar-previous-button"));
            uiOpenCalendarPrevious.Click();

            // click on the first day
            NgWebElement uiOpenCalendarYear = ngDriver.FindElement(By.CssSelector(".mat-calendar-content .mat-calendar-body-cell-content:first-child"));
            uiOpenCalendarYear.Click();
        }


        [And(@"I click on the Continue to Application button")]
        public void ContinueToApplicationButton()
        {
            // click on the Continue to Application button
            NgWebElement uiContinueButton = ngDriver.FindElement(By.CssSelector("button#continueToApp"));
            uiContinueButton.Click();
        }


        [And(@"I click on the Review Organization Information button")]
        public void ClickReviewOrganizationInformation()
        {
            // click on the review organization information button
            NgWebElement uiOrgInfoButton = ngDriver.FindElement(By.CssSelector("button.btn-primary[routerlink='/org-structure']"));
            uiOrgInfoButton.Click();
        }


        [And(@"I click on the Start Application button for (.*)")]
        public void ClickStartApplication(string applicationType)
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            if (applicationType == "Catering")
            {
                // click on the Catering Start Application button
                NgWebElement uiStartAppButton = ngDriver.FindElement(By.Id("startCatering"));
                uiStartAppButton.Click();
            }

            if (applicationType == "a Cannabis Retail Store")
            {
                // click on the Cannabis Start Application button
                NgWebElement uiStartAppButton = ngDriver.FindElement(By.CssSelector("button[id='startCRS']"));
                uiStartAppButton.Click();
            }

            if (applicationType == "a Rural Agency Store")
            {
                // click on the Rural Store Start Application button
                NgWebElement uiStartAppButton = ngDriver.FindElement(By.CssSelector("button[id='startRAS']"));
                uiStartAppButton.Click();
            }

            if (applicationType == "a Manufacturer Licence")
            {
                // click on the Manufacturer Licence Start Application button
                NgWebElement uiStartAppButton = ngDriver.FindElement(By.CssSelector("button[id='startMfg']"));
                uiStartAppButton.Click();
            }
        }


        [And(@"I complete the Rural Agency Store application")]
        public void CompleteRuralAgencyStoreApplication()
        {
            /* 
            Page Title: Rural Agency Store Information Submission
            */

            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A 6X5";
            string estPID = "012345678";
            string estEmail = "test@test.com";
            string estPhone = "2505555555";
            string certNumber = "012";

            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";
            string conEmail = "contact@email.com";

            // enter the establishment name
            NgWebElement uiEstabName = ngDriver.FindElement(By.Id("establishmentName"));
            uiEstabName.SendKeys(estName);

            // enter the establishment address
            NgWebElement uiEstabAddress = ngDriver.FindElement(By.Id("establishmentAddressStreet"));
            uiEstabAddress.SendKeys(estAddress);

            // enter the establishment city
            NgWebElement uiEstabCity = ngDriver.FindElement(By.Id("establishmentAddressCity"));
            uiEstabCity.SendKeys(estCity);

            // enter the establishment postal code
            NgWebElement uiEstabPostal = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            uiEstabPostal.SendKeys(estPostal);

            // enter the PID
            NgWebElement uiEstabPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            uiEstabPID.SendKeys(estPID);

            // enter the store email
            NgWebElement uiEstabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            uiEstabEmail.SendKeys(estEmail);

            // enter the store phone number
            NgWebElement uiEstabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            uiEstabPhone.SendKeys(estPhone);

            // enter the Rural Agency Store Certificate Number
            NgWebElement uiRuralStoreCertNumber = ngDriver.FindElement(By.Id("certNumber"));
            uiRuralStoreCertNumber.SendKeys(certNumber);

            // select the owner checkbox
            NgWebElement uiOwnerCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwnerCheckbox.Click();

            // select the owner's valid interest checkbox
            NgWebElement uiValidInterestCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterestCheckbox.Click();

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='willhaveValidInterest']"));
            uiZoningCheckbox.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a valid interest document
            string validInterestPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "valid_interest.pdf");
            NgWebElement uiUploadValidInterest = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
            uiUploadValidInterest.SendKeys(validInterestPath);

            // upload a floor plan document
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadFloorplan.SendKeys(floorplanPath);

            // enter the first name of the application contact
            NgWebElement uiContactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            uiContactGiven.SendKeys(conGiven);

            // enter the last name of the application contact
            NgWebElement uiContactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            uiContactSurname.SendKeys(conSurname);

            // enter the role of the application contact
            NgWebElement uiContactRole = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonRole]"));
            uiContactRole.SendKeys(conRole);

            // enter the phone number of the application contact
            NgWebElement uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonPhone]"));
            uiContactPhone.SendKeys(conPhone);

            // enter the email of the application contact
            NgWebElement uiContactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            uiContactEmail.SendKeys(conEmail);

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgree.Click();

            ClickOnSubmitButton();
        }


        [And(@"I complete the Cannabis Retail Store application for a(.*)")]
        public void CompleteCannabisApplication(string businessType)
        {
            /* 
            Page Title: Submit the Cannabis Retail Store Application
            */

            // create application info
            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A 6X5";
            string estPID = "012345678";
            string estEmail = "test@test.com";
            string estPhone = "2505555555";
            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";
            string conEmail = "contact@email.com";
            string indigenousNation = "Cowichan Tribes";
            string policeJurisdiction = "RCMP Shawnigan Lake";

            System.Threading.Thread.Sleep(3000);

            // enter the establishment name
            NgWebElement uiEstabName = ngDriver.FindElement(By.Id("establishmentName"));
            uiEstabName.SendKeys(estName);

            // enter the establishment address
            NgWebElement uiEstabAddress = ngDriver.FindElement(By.Id("establishmentAddressStreet"));
            uiEstabAddress.SendKeys(estAddress);

            // enter the establishment city
            NgWebElement uiEstabCity = ngDriver.FindElement(By.Id("establishmentAddressCity"));
            uiEstabCity.SendKeys(estCity);

            // enter the establishment postal code
            NgWebElement uiEstabPostal = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            uiEstabPostal.SendKeys(estPostal);

            // enter the PID
            NgWebElement uiEstabPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            uiEstabPID.SendKeys(estPID);

            if (businessType == "n indigenous nation")
            {
                // select the IN 
                NgWebElement uiSelectNation = ngDriver.FindElement(By.CssSelector("[formcontrolname='indigenousNationId'] option[value='236686fc-d9d3-e811-90f0-005056832371']"));
                uiSelectNation.Click();
            }

            // select 'yes' for ALR inclusion
            NgWebElement uiALRInclusion = ngDriver.FindElement(By.CssSelector("[formcontrolname='isAlr'] mat-radio-button#mat-radio-14"));
            uiALRInclusion.Click();

            // search for and select the indigenous nation
            NgWebElement uiIndigenousNation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='indigenousNation']"));
            uiIndigenousNation.SendKeys(indigenousNation);

            NgWebElement uiIndigenousNation2 = ngDriver.FindElement(By.CssSelector("#mat-option-0 span"));
            uiIndigenousNation2.Click();

            // search for and select the  police jurisdiction
            NgWebElement uiPoliceJurisdiction = ngDriver.FindElement(By.CssSelector("input[formcontrolname='policeJurisdiction']"));
            uiPoliceJurisdiction.SendKeys(policeJurisdiction);

            NgWebElement uiPoliceJurisdiction2 = ngDriver.FindElement(By.CssSelector("#mat-option-2 span"));
            uiPoliceJurisdiction2.Click();

            // enter the store email
            NgWebElement uiEstabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            uiEstabEmail.SendKeys(estEmail);

            // enter the store phone number
            NgWebElement uiEstabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            uiEstabPhone.SendKeys(estPhone);

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a store signage document
            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadSignage.SendKeys(signagePath);

            // upload a valid interest document
            string validInterestPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "valid_interest.pdf");
            NgWebElement uiUploadValidInterest = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uiUploadValidInterest.SendKeys(validInterestPath);

            // select not visible from outside checkbox
            NgWebElement uiVisibleFromOutside = ngDriver.FindElement(By.CssSelector(".mat-checkbox-inner-container"));
            uiVisibleFromOutside.Click();

            // upload a floor plan document
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uiUploadFloorplan.SendKeys(floorplanPath);

            // upload a site plan document
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
            uiUploadSitePlan.SendKeys(sitePlanPath);

            // upload a proof of zoning form
            string zoningPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "fin_integrity.pdf");
            NgWebElement uiUploadZoning = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uiUploadZoning.SendKeys(zoningPath);

            // upload a financial integrity form
            string finIntegrityPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "fin_integrity.pdf");
            NgWebElement uiUploadFinIntegrity = ngDriver.FindElement(By.XPath("(//input[@type='file'])[18]"));
            uiUploadFinIntegrity.SendKeys(finIntegrityPath);

            // enter the first name of the application contact
            NgWebElement uiContactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            uiContactGiven.SendKeys(conGiven);

            // enter the last name of the application contact
            NgWebElement uiContactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            uiContactSurname.SendKeys(conSurname);

            // enter the role of the application contact
            NgWebElement uiContactRole = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonRole]"));
            uiContactRole.SendKeys(conRole);

            // enter the phone number of the application contact
            NgWebElement uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonPhone]"));
            uiContactPhone.SendKeys(conPhone);

            // enter the email of the application contact
            NgWebElement uiContactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            uiContactEmail.SendKeys(conEmail);

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgree.Click();

            // retrieve the current URL to get the application ID (needed downstream)
            string URL = ngDriver.Url;

            // retrieve the application ID
            string[] parsedURL = URL.Split('/');

            applicationID = parsedURL[5];

            // click on the submit button
            NgWebElement uiSubmitButton = ngDriver.FindElement(By.CssSelector(".application-wrapper button.btn-primary"));
            uiSubmitButton.Click();
        }


        [And(@"I complete the Manufacturer application for a (.*)")]
        public void CompleteManufacturerApplication(string manufacturerType)
        {
            /* 
            Page Title: Manufacturer (Winery, Brewery, Distillery or Co-Packer) Licence Application
            */

            // create test data
            string estName = "Manufacturer's Establishment";
            string streetLocation = "123 Innovation Street";
            string city = "Victoria";
            string postal = "V9A 6X5";
            string PID = "111111111";
            string additionalPIDs = "999999999, 000000000, 181818181";
            string storeEmail = "store@email.com";
            string storePhone = "250-012-3456";
            string contactTitle = "Sommelier";
            string contactPhone = "778-181-1818";
            string contactEmail = "contact@email.com";
            string indigenousNation = "Cowichan Tribes";
            string policeJurisdiction = "RCMP Shawnigan Lake";

            // enter the establishment name
            NgWebElement uiEstabName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentName']"));
            uiEstabName.SendKeys(estName);

            // enter the establishment street address
            NgWebElement uiEstabStreetAddress = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            uiEstabStreetAddress.SendKeys(streetLocation);

            // enter the establishment city
            NgWebElement uiEstabCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            uiEstabCity.SendKeys(city);

            // enter the establishment postal code
            NgWebElement uiEstabPostal = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressPostalCode']"));
            uiEstabPostal.SendKeys(postal);

            // enter the PID
            NgWebElement uiEstabPID = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentParcelId']"));
            uiEstabPID.SendKeys(PID);

            // enter additional PIDs
            NgWebElement uiAdditionalEstabPID = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='pidList']"));
            uiAdditionalEstabPID.SendKeys(additionalPIDs);

            // select the proof of zoning checkbox
            NgWebElement uiProofOfZoning = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1"));
            uiProofOfZoning.Click();

            // select 'yes' for ALR inclusion
            NgWebElement uiALRInclusion = ngDriver.FindElement(By.CssSelector("[formcontrolname='isAlr'] mat-radio-button#mat-radio-2"));
            uiALRInclusion.Click();

            // search for and select the indigenous nation
            NgWebElement uiIndigenousNation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='indigenousNation']"));
            uiIndigenousNation.SendKeys(indigenousNation);

            NgWebElement uiIndigenousNation2 = ngDriver.FindElement(By.CssSelector("#mat-option-0 span"));
            uiIndigenousNation2.Click();

            // search for and select the police jurisdiction
            NgWebElement uiPoliceJurisdiction = ngDriver.FindElement(By.CssSelector("input[formcontrolname='policeJurisdiction']"));
            uiPoliceJurisdiction.SendKeys(policeJurisdiction);

            NgWebElement uiPoliceJurisdiction2 = ngDriver.FindElement(By.CssSelector("#mat-option-2 span"));
            uiPoliceJurisdiction2.Click();

            // enter the store email
            NgWebElement uiEstabEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentEmail']"));
            uiEstabEmail.SendKeys(storeEmail);

            // enter the store phone number
            NgWebElement uiEstabPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentPhone']"));
            uiEstabPhone.SendKeys(storePhone);

            if (manufacturerType == "winery")
            {
                // select winery radio button
                NgWebElement uiWinery = ngDriver.FindElement(By.CssSelector("mat-radio-button#mat-radio-5"));
                uiWinery.Click();
            }

            if (manufacturerType == "distillery")
            {
                // select distillery radio button
                NgWebElement uiDistillery = ngDriver.FindElement(By.CssSelector("mat-radio-button#mat-radio-6"));
                uiDistillery.Click();
            }

            if (manufacturerType == "brewery")
            {
                // select brewery radio button
                NgWebElement uiBrewery = ngDriver.FindElement(By.CssSelector("mat-radio-button#mat-radio-7"));
                uiBrewery.Click();
            }

            if (manufacturerType == "co-packer")
            {
                // select co-packer radio button
                NgWebElement uiCoPacker = ngDriver.FindElement(By.CssSelector("mat-radio-button#mat-radio-8"));
                uiCoPacker.Click();
            }

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the business plan
            string businessPlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "production_sales_forecast.pdf");
            NgWebElement uiUploadBusinessPlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadBusinessPlan.SendKeys(businessPlanPath);

            // upload the production sales forecast
            string productionSalesPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "production_sales_forecast.pdf");
            NgWebElement uiUploadProductionSales = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadProductionSales.SendKeys(productionSalesPath);

            if (manufacturerType == "winery")
            {
                // create winery test data
                string grapesAcres = "100";
                string fruitAcres = "5";
                string honeyBeehives = "7";

                // enter the grapes acreage 
                NgWebElement uiGrapes = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mfgAcresOfGrapes']"));
                uiGrapes.SendKeys(grapesAcres);

                // enter the fruit acreage
                NgWebElement uiFruit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mfgAcresOfFruit']"));
                uiFruit.SendKeys(fruitAcres);
                
                // enter the number of beehives
                NgWebElement uiHoney = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mfgAcresOfHoney']"));
                uiHoney.SendKeys(honeyBeehives);

                // select the blending checkbox
                NgWebElement uiBlending = ngDriver.FindElement(By.CssSelector("#mat-checkbox-10 .mat-checkbox-inner-container"));
                uiBlending.Click();
                
                // select the crushing checkbox
                NgWebElement uiCrushing = ngDriver.FindElement(By.CssSelector("#mat-checkbox-11 .mat-checkbox-inner-container"));
                uiCrushing.Click();

                // select the filtering checkbox
                NgWebElement uiFiltering = ngDriver.FindElement(By.CssSelector("#mat-checkbox-12 .mat-checkbox-inner-container"));
                uiFiltering.Click();

                // select the aging, for at least 3 months checkbox
                NgWebElement uiAging = ngDriver.FindElement(By.CssSelector("#mat-checkbox-13 .mat-checkbox-inner-container"));
                uiAging.Click();

                // select the secondary fermentation or carbonation checkbox
                NgWebElement uiSecondaryFermentation = ngDriver.FindElement(By.CssSelector("#mat-checkbox-14 .mat-checkbox-inner-container"));
                uiSecondaryFermentation.Click();

                // select the packaging checkbox
                NgWebElement uiPackaging = ngDriver.FindElement(By.CssSelector("#mat-checkbox-15 .mat-checkbox-inner-container"));
                uiPackaging.Click();
            }

            // select 'yes' for neutral grain spirits            
            NgWebElement uiNeutralGrains = ngDriver.FindElement(By.CssSelector("[formcontrolname='mfgUsesNeutralGrainSpirits'] mat-radio-button[value='Yes']"));
            uiNeutralGrains.Click();

            if (manufacturerType == "brewery")
            {
                // select 'Yes' for the brewery operating with brew pub on site
                NgWebElement uiPubOnSite = ngDriver.FindElement(By.CssSelector("[formcontrolname='mfgBrewPubOnSite'] mat-radio-button[value='Yes']"));
                uiPubOnSite.Click();

                // select 'Yes' for piping from brewery
                NgWebElement uiPiping = ngDriver.FindElement(By.CssSelector("[formcontrolname='mfgPipedInProduct'] mat-radio-button[value='Yes']"));
                uiPiping.Click();

                // upload brew sheets sample
                string brewSheetsPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "brew_sheets.pdf");
                NgWebElement uiUploadBrewSheets = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
                uiUploadBrewSheets.SendKeys(brewSheetsPath);

                // upload the business insurance
                string insurancePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "business_insurance.pdf");
                NgWebElement uiUploadInsurance = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
                uiUploadInsurance.SendKeys(insurancePath);
            }

            if (manufacturerType == "brewery")
            {
                // upload the store signage
                string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[14]"));
                uiUploadSignage.SendKeys(signagePath);

                // upload the floor plan 
                string floorPlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
                NgWebElement uiUploadFloorPlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[17]"));
                uiUploadFloorPlan.SendKeys(floorPlanPath);

                // upload the site plan
                string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
                NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[20]"));
                uiUploadSitePlan.SendKeys(sitePlanPath);
            }
            else
            {
                // upload the store signage
                string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
                uiUploadSignage.SendKeys(signagePath);

                // upload the floor plan 
                string floorPlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
                NgWebElement uiUploadFloorPlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
                uiUploadFloorPlan.SendKeys(floorPlanPath);

                // upload the site plan 
                string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
                NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[14]"));
                uiUploadSitePlan.SendKeys(sitePlanPath);
            }

            // select the owner checkbox
            NgWebElement uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            NgWebElement uiFutureValidInterest = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-4[formcontrolname='willhaveValidInterest']"));
            uiFutureValidInterest.Click();

            // enter the contact title
            NgWebElement uiContactTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonRole']"));
            uiContactTitle.SendKeys(contactTitle);

            // enter the contact phone number
            NgWebElement uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonPhone']"));
            uiContactPhone.SendKeys(contactPhone);

            // enter the contact email address
            NgWebElement uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonEmail']"));
            uiContactEmail.SendKeys(contactEmail);

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // retrieve the current URL to get the application ID (needed downstream)
            string URL = ngDriver.Url;

            // retrieve the application ID
            string[] parsedURL = URL.Split('/');

            applicationID = parsedURL[5];

            // click on the Submit & Pay button
            ClickOnSubmitButton();
        }


        [And(@"I complete the Catering application")]
        public void CompleteCateringApplication()
        {
            /* 
            Page Title: Catering Licence Application
            */

            // create application info
            string prevAppDetails = "Here are the previous application details (automated test).";
            string liqConnectionDetails = "Here are the liquor industry connection details (automated test).";
            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A 6X5";
            string estPID = "012345678";
            string estPhone = "2505555555";
            string estEmail = "test@automation.com";
            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";
            string conEmail = "test2@automation.com";

            // enter the establishment name
            NgWebElement uiEstabName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentName']"));
            uiEstabName.SendKeys(estName);

            // enter the establishment address
            NgWebElement uiEstabAddress = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            uiEstabAddress.SendKeys(estAddress);

            // enter the establishment city
            NgWebElement uiEstabCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            uiEstabCity.SendKeys(estCity);

            // enter the establishment postal code
            NgWebElement uiEstabPostal = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            uiEstabPostal.SendKeys(estPostal);

            // enter the PID
            NgWebElement uiEstabPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            uiEstabPID.SendKeys(estPID);

            // select 'Yes'
            // Is your establishment located on the Agricultural Land Reserve (ALR)?
            NgWebElement uiYesALRZoning = ngDriver.FindElement(By.CssSelector("[formcontrolname='isAlr'] mat-radio-button#mat-radio-2"));
            uiYesALRZoning.Click();

            // enter the store email
            NgWebElement uiEstabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            uiEstabEmail.SendKeys(estEmail);

            // enter the store phone number
            NgWebElement uiEstabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            uiEstabPhone.SendKeys(estPhone);

            // select 'Yes'
            // Do you or any of your shareholders currently hold, have held, or have previously applied for a British Columbia liquor licence?
            NgWebElement uiPreviousLicenceYes = ngDriver.FindElement(By.Id("mat-button-toggle-1-button"));
            uiPreviousLicenceYes.Click();

            // enter the previous application details
            NgWebElement uiPreviousApplicationDetails = ngDriver.FindElement(By.Id("previousApplicationDetails"));
            uiPreviousApplicationDetails.SendKeys(prevAppDetails);

            // select 'Yes'
            // Do you hold a Rural Agency Store Appointment?
            NgWebElement uiRuralAgencyStore = ngDriver.FindElement(By.Id("mat-button-toggle-4-button"));
            uiRuralAgencyStore.Click();

            // select 'Yes'
            // Do you, or any of your shareholders, have any connection, financial or otherwise, direct or indirect, with a distillery, brewery or winery?
            NgWebElement uiOtherBusinessYes = ngDriver.FindElement(By.Id("mat-button-toggle-7-button"));
            uiOtherBusinessYes.Click();

            // enter the connection details
            NgWebElement uiLiqIndConnection = ngDriver.FindElement(By.Id("liquorIndustryConnectionsDetails"));
            uiLiqIndConnection.SendKeys(liqConnectionDetails);

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a store signage document
            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadSignage.SendKeys(signagePath);

            // enter the first name of the application contact
            NgWebElement uiContactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            uiContactGiven.SendKeys(conGiven);

            // enter the last name of the application contact
            NgWebElement uiContactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            uiContactSurname.SendKeys(conSurname);

            // enter the role of the application contact
            NgWebElement uiContactRole = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonRole']"));
            uiContactRole.SendKeys(conRole);

            // enter the phone number of the application contact
            NgWebElement uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonPhone']"));
            uiContactPhone.SendKeys(conPhone);

            // enter the email of the application contact
            NgWebElement uiContactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            uiContactEmail.SendKeys(conEmail);

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();

            // retrieve the current URL to get the application ID (needed downstream)
            string URL = ngDriver.Url;

            // retrieve the application ID
            string[] parsedURL = URL.Split('/');

            string[] tempFix = parsedURL[5].Split(';');

            applicationID = tempFix[0];
        }


        [And(@"I request a personnel name change for a (.*)")]
        public void RequestPersonnelNameChange(string businessType)
        {
            if (businessType != "indigenous nation")
            {
                ClickOnDashboard();

                /* 
                Page Title: Welcome to Liquor and Cannabis Licensing
                */

                // click on the review organization information button
                ClickReviewOrganizationInformation();

                /* 
                Page Title: [client name] Detailed Organization Information
                */

                // click on the Edit button for Key Personnel (partnership, sole proprietorship, private corporation, or society)
                if (businessType == "partnership" || businessType == "sole proprietorship" || businessType == "private corporation" || businessType == "society")
                {
                    NgWebElement uiEditInfoButtonShared = ngDriver.FindElement(By.CssSelector(".fas.fa-pencil-alt span"));
                    uiEditInfoButtonShared.Click();
                }

                // click on the Edit button for Key Personnel (public corporation)
                if (businessType == "public corporation")
                {
                    NgWebElement uiEditInfoButton = ngDriver.FindElement(By.CssSelector("td:nth-child(7) .ng-star-inserted"));
                    uiEditInfoButton.Click();
                }

                // enter a new name for the director
                string newDirectorFirstName = "UpdatedFirstName";
                string newDirectorLastName = "UpdatedLastName";

                NgWebElement uiNewDirectorFirstName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
                uiNewDirectorFirstName.Clear();
                uiNewDirectorFirstName.SendKeys(newDirectorFirstName);

                NgWebElement uiNewDirectorLasttName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastNameNew']"));
                uiNewDirectorLasttName.Clear();
                uiNewDirectorLasttName.SendKeys(newDirectorLastName);

                // click on the Confirm button
                NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector(".fa-save span"));
                uiConfirmButton.Click();

                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload a marriage certificate document
                string marriageCertificate = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "marriage_certificate.pdf");

                if (businessType == "public corporation" || businessType == "partnership")
                {
                    NgWebElement uiUploadMarriageCert = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
                    uiUploadMarriageCert.SendKeys(marriageCertificate);
                }

                if (businessType == "private corporation")
                {
                    NgWebElement uiUploadMarriageCert = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
                    uiUploadMarriageCert.SendKeys(marriageCertificate);
                }

                if (businessType == "society")
                {
                    NgWebElement uiUploadMarriageCert = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
                    uiUploadMarriageCert.SendKeys(marriageCertificate);
                }

                if (businessType == "sole proprietorship")
                {
                    NgWebElement uiUploadMarriageCert = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
                    uiUploadMarriageCert.SendKeys(marriageCertificate);
                }

                // click on Submit Organization Information button
                NgWebElement uiSubmitOrgStructure = ngDriver.FindElement(By.CssSelector("button.btn-primary[name='submit-application']"));
                uiSubmitOrgStructure.Click();

                MakePayment();

                System.Threading.Thread.Sleep(3000);
            }
        }


        [And(@"I confirm the correct personnel name change fee for a (.*)")]
        public void PersonnelNameChangeFee(string applicationType)
        {
            /* 
            Page Title: Payment Approved
            */

            if (applicationType == "Cannabis licence")
            {
                // check Cannabis name change fee
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$500.00')]")).Displayed);
            }

            if (applicationType == "Catering licence")
            {
                // check Catering name change fee
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$220.00')]")).Displayed);
            }
        }


        [And(@"I confirm that the director name has been updated")]
        public void DirectorNameUpdated()
        {
            // click on Dashboard link
            ClickOnDashboard();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // click on the review organization information button
            ClickReviewOrganizationInformation();

            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // check that the director name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'UpdatedFirstName')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'UpdatedLastName')]")).Displayed);
        }


        [And(@"I request a store relocation for (.*)")]
        public void RequestStoreRelocation(string applicationType)
        {
            /* 
            Page Title: Licences
            */

            string requestRelocationLink = "Request Relocation";

            // click on the request location link
            NgWebElement uiRequestRelocation = ngDriver.FindElement(By.LinkText(requestRelocationLink));
            uiRequestRelocation.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Submit a Licence Relocation Application
            */

            if (applicationType == "Cannabis")
            {

                string proposedAddress = "Automated Test Street";
                string proposedCity = "Victoria";
                string proposedPostalCode = "V9A 6X5";
                string pid = "012345678";

                // enter the proposed street address
                NgWebElement uiProposedAddress = ngDriver.FindElement(By.CssSelector(".ngtest-new-address input[formcontrolname='establishmentAddressStreet']"));
                uiProposedAddress.SendKeys(proposedAddress);

                // enter the proposed city
                NgWebElement uiProposedCity = ngDriver.FindElement(By.CssSelector(".ngtest-new-address input[formcontrolname='establishmentAddressCity']"));
                uiProposedCity.SendKeys(proposedCity);

                // enter the postal code
                NgWebElement uiProposedPostalCode = ngDriver.FindElement(By.CssSelector(".ngtest-new-address input[formcontrolname='establishmentAddressPostalCode']"));
                uiProposedPostalCode.SendKeys(proposedPostalCode);

                // enter the PID
                NgWebElement uiProposedPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
                uiProposedPID.SendKeys(pid);
            }

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a supporting document
            string supportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "checklist.pdf");
            NgWebElement uiUploadSupportingDoc = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadSupportingDoc.SendKeys(supportingDocument);

            // select the authorized to submit checkbox
            NgWebElement uiAuthToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSigAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSigAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            // pay for the relocation application
            MakePayment();

            System.Threading.Thread.Sleep(4000);

            if (applicationType == "Cannabis")
            {
                /* 	
                Page Title: Payment Approved	
                */

                // confirm correct payment amount
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$220.00')]")).Displayed);

                // return to the Licences tab
                ClickLicencesTab();
            }

            if (applicationType == "Catering")
            {
                /* 	
                Page Title: Payment Approved	
                */

                // confirm correct payment amount	
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$330.00')]")).Displayed);
            }
        }


        [And(@"I change a personnel email address for a (.*)")]
        public void RequestPersonnelEmailChange(string businessType)
        {
            if (businessType != "indigenous nation")
            {
                // click on Dashboard link
                ClickOnDashboard();

                /* 
                Page Title: Welcome to Liquor and Cannabis Licensing
                */

                // click on the review organization information button
                ClickReviewOrganizationInformation();

                /* 
                Page Title: [client name] Detailed Organization Information
                */

                // click on the Edit button for Key Personnel (partnership, sole proprietorship, public corporation, or society)
                if (businessType == "partnership" || businessType == "sole proprietorship" || businessType == "public corporation" || businessType == "society")
                {
                    NgWebElement uiEditInfoButtonPartner = ngDriver.FindElement(By.CssSelector(".fas.fa-pencil-alt span"));
                    uiEditInfoButtonPartner.Click();
                }

                // click on the Edit button for Key Personnel (private corporation)
                if (businessType == "private corporation")
                {
                    NgWebElement uiEditInfoButton = ngDriver.FindElement(By.CssSelector("td:nth-child(7) .ng-star-inserted"));
                    uiEditInfoButton.Click();
                }

                // enter a new email for the associate
                string newEmail = "newemail@test.com";

                NgWebElement uiNewEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
                uiNewEmail.Clear();
                uiNewEmail.SendKeys(newEmail);

                // click on the Confirm button
                NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector(".fa-save span"));
                uiConfirmButton.Click();

                // click on Confirm Organization Information is Complete button
                NgWebElement uiOrgInfoButton2 = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
                uiOrgInfoButton2.Click();

                /* 
                Page Title: Welcome to Liquor and Cannabis Licensing
                */

                // check that dashboard is displayed (i.e. no payment has been required)
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Welcome to Liquor and Cannabis Licensing')]")).Displayed);
            }
        }


        [And(@"I request an event authorization")]
        public void RequestEventAuthorization()
        {
            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            string requestEventAuthorization = "Request Event Authorization";

            // click on the request event authorization link
            NgWebElement uiRequestEventAuthorization = ngDriver.FindElement(By.LinkText(requestEventAuthorization));
            uiRequestEventAuthorization.Click();

            /* 
            Page Title: Catered Event Authorization Request
            */

            // create event authorization data
            string eventContactName = "AutoTestEventContactName";
            string eventContactPhone = "2500000000";

            string eventDescription = "Automated test event description added here.";
            string eventClientOrHostName = "Automated test event";
            string maximumAttendance = "100";
            string maximumStaffAttendance = "25";

            string venueNameDescription = "Automated test venue name or description";
            string venueAdditionalInfo = "Automated test additional venue information added here.";
            string physicalAddStreetAddress1 = "Automated test street address 1";
            string physicalAddStreetAddress2 = "Automated test street address 2";
            string physicalAddCity = "Victoria";
            string physicalAddPostalCode = "V9A 6X5";

            // enter event contact name
            NgWebElement uiEventContactName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactName']"));
            uiEventContactName.SendKeys(eventContactName);

            // enter event contact phone
            NgWebElement uiEventContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPhone']"));
            uiEventContactPhone.SendKeys(eventContactPhone);

            // select event type
            NgWebElement uiEventType = ngDriver.FindElement(By.CssSelector("[formcontrolname='eventType'] option[value='2: 845280002']"));
            uiEventType.Click();

            // enter event description
            NgWebElement uiEventDescription = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='eventTypeDescription']"));
            uiEventDescription.SendKeys(eventDescription);

            // enter event client or host name
            NgWebElement uiEventClientOrHostName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='clientHostname']"));
            uiEventClientOrHostName.SendKeys(eventClientOrHostName);

            // enter maximum attendance
            NgWebElement uiMaxAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxAttendance']"));
            uiMaxAttendance.SendKeys(maximumAttendance);

            // enter maximum staff attendance
            NgWebElement uiMaxStaffAttendance = ngDriver.FindElement(By.CssSelector("input[formcontrolname='maxStaffAttendance']"));
            uiMaxStaffAttendance.SendKeys(maximumStaffAttendance);

            // select whether minors are attending - yes
            NgWebElement uiMinorsAttending = ngDriver.FindElement(By.CssSelector("[formcontrolname='minorsAttending'] option[value='true']"));
            uiMinorsAttending.Click();

            // select type of food service provided
            NgWebElement uiFoodServiceProvided = ngDriver.FindElement(By.CssSelector("[formcontrolname='foodService'] option[value='0: 845280000']"));
            uiFoodServiceProvided.Click();

            // select type of entertainment provided
            NgWebElement uiEntertainmentProvided = ngDriver.FindElement(By.CssSelector("[formcontrolname='entertainment'] option[value='1: 845280001']"));
            uiEntertainmentProvided.Click();

            // enter venue name description
            NgWebElement uiVenueNameDescription = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='venueDescription']"));
            uiVenueNameDescription.SendKeys(venueNameDescription);

            // select venue location
            NgWebElement uiVenueLocation = ngDriver.FindElement(By.CssSelector("[formcontrolname='specificLocation'] option[value='2: 845280002']"));
            uiVenueLocation.Click();

            // enter venue additional info
            NgWebElement uiVenueAdditionalInfo = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='additionalLocationInformation']"));
            uiVenueAdditionalInfo.SendKeys(venueAdditionalInfo);

            // enter physical address - street address 1
            NgWebElement uiPhysicalAddStreetAddress1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street1']"));
            uiPhysicalAddStreetAddress1.SendKeys(physicalAddStreetAddress1);

            // enter physical address - street address 2 
            NgWebElement uiPhysicalAddStreetAddress2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='street2']"));
            uiPhysicalAddStreetAddress2.SendKeys(physicalAddStreetAddress2);

            // enter physical address - city
            NgWebElement uiPhysicalAddCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='city']"));
            uiPhysicalAddCity.SendKeys(physicalAddCity);

            // enter physical address - postal code
            NgWebElement uiPhysicalAddPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='postalCode']"));
            uiPhysicalAddPostalCode.SendKeys(physicalAddPostalCode);

            // select start date
            NgWebElement uiVenueStartDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='startDate']"));
            uiVenueStartDate1.Click();

            NgWebElement uiVenueStartDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            uiVenueStartDate2.Click();

            // select end date
            NgWebElement uiVenueEndDate1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='endDate']"));
            uiVenueEndDate1.Click();

            NgWebElement uiVenueEndDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            uiVenueEndDate2.Click();

            // select event and liquor service times are different on specific dates checkbox
            NgWebElement uiEventLiquorServiceTimesDifferent = ngDriver.FindElement(By.Id("mat-checkbox-1"));
            uiEventLiquorServiceTimesDifferent.Click();

            // select terms and conditions checkbox
            NgWebElement uiTermsAndConditions = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            uiTermsAndConditions.Click();

            // click on the submit button
            ClickOnSubmitButton();

            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            // click on the Event History bar - TODO
            // NgWebElement expandEventHistory = ngDriver.FindElement(By.Id("mat-expansion-panel-header-1"));
            // expandEventHistory.Click();

            // confirm that the Event Status = In Review and the Client or Host Name is present - TODO
            // Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,eventContactName)]")).Displayed);
        }


        [And(@"I request a structural change")]
        public void RequestStructuralChange()
        {
           /* 
           Page Title: Licences
           Subtitle:   Cannabis Retail Store Licences
           */

            string structuralChange = "Request a Structural Change";

            // click on the request structural change link
            NgWebElement uiStructuralChange = ngDriver.FindElement(By.LinkText(structuralChange));
            uiStructuralChange.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Submit the Cannabis Retail Store Structural Change Application
            */

            // create test data
            string description = "Test automation outline of the proposed change.";

            // enter the description of the change
            NgWebElement uiDescriptionOfChange = ngDriver.FindElement(By.Id("description1"));
            uiDescriptionOfChange.SendKeys(description);

            // select not visible from outside checkbox
            NgWebElement uiVisibleFromOutside = ngDriver.FindElement(By.CssSelector(".mat-checkbox-inner-container"));
            uiVisibleFromOutside.Click();

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a floor plan document
            string floorPlan = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiFloorPlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiFloorPlan.SendKeys(floorPlan);

            // select 'no' for changes to entries
            NgWebElement uiChangeToEntries = ngDriver.FindElement(By.Id("mat-button-toggle-2-button"));
            uiChangeToEntries.Click();

            // select authorizedToSubmit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // select signatureAgreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            // pay for the structural change application
            MakePayment();

            System.Threading.Thread.Sleep(4000);

            /* 
            Page Title: Payment Approved
            */

            // confirm correct payment amount
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$440.00')]")).Displayed);

            ClickLicencesTab();
        }


        [And(@"I review the federal reports")]
        public void ReviewFederalReports()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            string reviewReports = "Review Federal Reports";

            // click on the Review Federal Reports link
            NgWebElement uiReviewFedReports = ngDriver.FindElement(By.LinkText(reviewReports));
            uiReviewFedReports.Click();

            /* 
            Page Title: Federal Reporting
            */

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Federal Reporting')]")).Displayed);

            ClickLicencesTab();
        }


        [And(@"I confirm the relocation request is displayed on the dashboard")]
        public void RequestedRelocationOnDashboard()
        {
            ClickOnDashboard();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // confirm that relocation request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Relocation Request')]")).Displayed);
        }


        [And(@"I confirm the name or branding change is displayed on the dashboard")]
        public void RequestedNameChangeOnDashboard()
        {
            ClickOnDashboard();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // confirm that a name or branding change request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name or Branding Change')]")).Displayed);
        }


        [And(@"I confirm the structural change request is displayed on the dashboard")]
        public void RequestedStructuralChangeOnDashboard()
        {
            ClickOnDashboard();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // confirm that a structural change request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Structural Change')]")).Displayed);
        }


        [And(@"I request a valid store name or branding change for (.*)")]
        public void RequestNameBrandingChange(string changeType)
        {
            /* 
            Page Title: Licences
            */

            string nameBrandingLinkCannabis = "Request Store Name or Branding Change";
            string nameBrandingLinkCateringMfg = "Establishment Name Change Application";

            if ((changeType == "Catering") || (changeType == "Manufacturing"))
            {
                // click on the Establishment Name Change Application link
                NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText(nameBrandingLinkCateringMfg));
                uiRequestChange.Click();
            }

            if (changeType == "Cannabis")
            {
                // click on the Request Store Name or Branding Change link
                NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText(nameBrandingLinkCannabis));
                uiRequestChange.Click();
            }

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /*
            Page Title: Submit a Name or Branding Change Application
            */

            // click on the authorized to submit checkbox
            NgWebElement uiAuthSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSigAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSigAgreement.Click();

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a supporting document
            string supportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSupportingDoc = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadSupportingDoc.SendKeys(supportingDocument);

            if (changeType == "Cannabis")
            {
                // click on the store exterior change button	
                NgWebElement uiStoreExterior = ngDriver.FindElement(By.Id("mat-button-toggle-2-button"));
                uiStoreExterior.Click();
            }

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            // pay for the branding change application
            MakePayment();

            /* 
            Page Title: Payment Approved
            */

            // confirm correct payment amount	
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$220.00')]")).Displayed);

            // return to the Licences tab
            ClickLicencesTab();
        }


        [And(@"I request a third party operator")]
        public void RequestThirdPartyOperator()
        {
            // return to the Licences tab
            ClickLicencesTab();

            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            string addOrChangeThirdParty = "Add or Change a Third Party Operator";

            // click on the Add or Change a Third Party Operator Link
            NgWebElement uiAddOrChangeThirdPartyOp = ngDriver.FindElement(By.LinkText(addOrChangeThirdParty));
            uiAddOrChangeThirdPartyOp.Click();

            /* 
            Page Title: Add or Change a Third Party Operator
            */

            string thirdparty = "GunderCorp TestBusiness";

            // search for the proposed licensee
            NgWebElement uiThirdPartyOperator = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiThirdPartyOperator.SendKeys(thirdparty);

            NgWebElement uiThirdPartyOperatorOption = ngDriver.FindElement(By.CssSelector("mat-option[role='option'] span"));
            uiThirdPartyOperatorOption.Click();

            // click on authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            // click on submit button
            ClickOnSubmitButton();

            // return to the Licences tab
            ClickLicencesTab();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // confirm that the application has been initiated
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Third Party Operator Application Initiated')]")).Displayed);
        }
        

        [And(@"I request a transfer of ownership")]
        public void RequestOwnershipTransfer()
        {
            /* 
            Page Title: Licences
            */

            string transferOwnership = "Transfer Licence";

            // click on the Transfer Ownership link
            NgWebElement uiTransferOwnership = ngDriver.FindElement(By.LinkText(transferOwnership));
            uiTransferOwnership.Click();

            string licensee = "GunderCorp TestBusiness";

            // search for the proposed licensee
            NgWebElement uiProposedLicensee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiProposedLicensee.SendKeys(licensee);

            NgWebElement uiThirdPartyOperatorOption = ngDriver.FindElement(By.CssSelector("mat-option[role='option'] span"));
            uiThirdPartyOperatorOption.Click();

            // click on consent to licence transfer checkbox
            NgWebElement uiConsentToTransfer = ngDriver.FindElement(By.CssSelector("input[formcontrolname = 'transferConsent']"));
            uiConsentToTransfer.Click();

            // click on authorize signature checkbox
            NgWebElement uiAuthorizeSignature = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit']"));
            uiAuthorizeSignature.Click();

            // click on signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on submit transfer button
            NgWebElement uiSubmitTransferButton = ngDriver.FindElement(By.CssSelector("app-application-ownership-transfer button.btn-primary"));
            uiSubmitTransferButton.Click();

            ClickLicencesTab();

            /* 
            Page Title: Licences
            */

            // check for transfer initiated status 
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Transfer Requested')]")).Displayed);
        }


        [And(@"the application is approved")]
        public void ApplicationIsApproved()
        {
            ngDriver.IgnoreSynchronization = true;

            // navigate to api/applications/<Application ID>/process
            ngDriver.Navigate().GoToUrl($"{baseUri}api/applications/{applicationID}/process");

            // wait for the automated approval process to run
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'OK')]")).Displayed);

            ngDriver.IgnoreSynchronization = false;

            // navigate back to Licenses tab
            ngDriver.Navigate().GoToUrl($"{baseUri}licences");
        }


        [And(@"I complete the eligibility disclosure")]
        public void CompleteEligibilityDisclosure()
        {
            /* 
            Page Title: Cannabis Retail Store Licence Eligibility Disclosure
            */

            // select response: On or after March 1, 2020, did you or any of your associates own, operate, provide financial support to, or receive income from an unlicensed cannabis retail store or retailer?           
            // select Yes radio button 
            NgWebElement uiYesRadio1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isConnectedToUnlicencedStore'] mat-radio-button"));
            uiYesRadio1.Click();

            // complete field: Please indicate the name and location of the retailer or store 
            string nameAndLocation = "Automated test name and location of retailer";

            NgWebElement uiNameAndLocation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='nameLocationUnlicencedRetailer']"));
            uiNameAndLocation.SendKeys(nameAndLocation);

            // select response: Does the retailer or store continue to operate?
            // select Yes for Question 2 using radio button
            NgWebElement uiYesRadio2 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isRetailerStillOperating'] mat-radio-button"));
            uiYesRadio2.Click();

            // select response: On or after March 1, 2020, were you or any of your associates involved with the distribution or supply of cannabis to a licensed or unlicensed cannabis retail store or retailer?
            // select Yes using radio button
            NgWebElement uiYesRadio3 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isInvolvedIllegalDistribution'] mat-radio-button"));
            uiYesRadio3.Click();

            // complete field: Please indicate the details of your involvement
            string involvementDetails = "Automated test - details of the involvement";

            NgWebElement uiInvolvementDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='illegalDistributionInvolvementDetails']"));
            uiInvolvementDetails.SendKeys(involvementDetails);

            // complete field: Please indicate the name and location of the retailer or store           
            string nameAndLocation2 = "Automated test name and location of retailer (2)";

            NgWebElement uiNameAndLocation2 = ngDriver.FindElement(By.CssSelector("input[formControlName='nameLocationRetailer']"));
            uiNameAndLocation2.SendKeys(nameAndLocation2);

            // select response: Do you continue to be involved?
            // select Yes for Question 2 using radio button
            NgWebElement uiYesRadio4 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isInvolvementContinuing'] mat-radio-button"));
            uiYesRadio4.Click();

            // select certification checkbox
            NgWebElement uiCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isEligibilityCertified']"));
            uiCheckbox.Click();

            // enter the electronic signature
            string electricSignature = "Automated Test";

            NgWebElement uiSigCheckbox = ngDriver.FindElement(By.CssSelector("input[formcontrolname='eligibilitySignature']"));
            uiSigCheckbox.SendKeys(electricSignature);

            // click on the Submit button
            NgWebElement uiEligibilitySubmit = ngDriver.FindElement(By.CssSelector("app-eligibility-form button.btn-primary"));
            uiEligibilitySubmit.Click();
        }


        [And(@"I review the account profile for a(.*)")]
        public void ReviewAccountProfile(string businessType)
        {
            /*
            Page Title: Please Review the Account Profile
            */

            string bizNumber = "012345678";
            string incorporationNumber = "BC1234567";

            string physStreetAddress1 = "645 Tyee Road";
            string physStreetAddress2 = "West of Victoria";
            string physCity = "Victoria";
            string physPostalCode = "V9A 6X5";

            string mailStreet1 = "P.O. Box 123";
            string mailStreet2 = "303 Prideaux St.";
            string mailCity = "Victoria";
            string mailProvince = "B.C.";
            string mailPostalCode = "V9A 6X5";
            string mailCountry = "Canada";

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
                NgWebElement uiCorpNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='bcIncorporationNumber']"));
                uiCorpNumber.SendKeys(incorporationNumber);

                NgWebElement uiCalendar1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='dateOfIncorporationInBC']"));
                uiCalendar1.Click();

                NgWebElement uiCalendar2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
                uiCalendar2.Click();
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

            if ((businessType == "n indigenous nation"))
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

            if ((businessType == "n indigenous nation"))
            {
                // select 'Yes' for connection to a federal producer
                NgWebElement uiINConnectionFederalProducer = ngDriver.FindElement(By.CssSelector("input[formcontrolname='iNConnectionToFederalProducer']"));
                uiINConnectionFederalProducer.Click();

                // enter the name of the federal producer and details of the connection 
                string INnameAndDetails = "Name and details of federal producer (automated test) for IN.";
                NgWebElement uiINDetailsFederalProducer = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='iNConnectionToFederalProducerDetails']"));
                uiINDetailsFederalProducer.SendKeys(INnameAndDetails);
            }

            if ((businessType == " private corporation") || (businessType == " sole proprietorship"))
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


        [And(@"I review the security screening requirements for a multilevel business")]
        public void ReviewSecurityScreeningRequirementsMulti()
        {
            /* 
            Page Title: Security Screening Requirements
            */

            // confirm that private corporation personnel are present
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder0')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader0')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholderBiz2First')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderBiz2First')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholderBiz3First')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderBiz3First')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholderBiz4First')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderBiz4First')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholderBiz5First')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderBiz5First')]")).Displayed);
        }


        [And(@"I review the security screening requirements for a(.*)")]
        public void ReviewSecurityScreeningRequirements(string businessType)
        {
            /* 
            Page Title: Security Screening Requirements
            */

            // confirm that private corporation personnel are present
            if (businessType == " private corporation")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader0')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'PrivateCorp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder0')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'BizShareholderPrivateCorp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1')]")).Displayed);
            }

            // confirm that sole proprietor personnel are present
            if (businessType == " sole proprietorship")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'SoleProprietor')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader2')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader3')]")).Displayed);
            }

            // confirm that society personnel are present
            if (businessType == " society")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Director')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Society')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Director2')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Director3')]")).Displayed);
            }

            // confirm that public corporation personnel are present
            if (businessType == " public corporation")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnel1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Public Corp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnel2')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnel3')]")).Displayed);
            }

            // confirm that partnership personnel are present
            if (businessType == " partnership")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Individual')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Partner')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Partner2')]")).Displayed);
            }
        }


        [And(@"I review the organization structure for a(.*)")]
        public void ReviewOrganizationStructure(string businessType)
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            if (businessType == " private corporation")
            {
                // find the upload test files in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload a notice of articles document
                string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
                NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
                uiUploadSignage.SendKeys(noticeOfArticles);

                // upload a central securities register document
                string centralSecuritiesRegister = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
                NgWebElement uiUploadCentralSecReg = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
                uiUploadCentralSecReg.SendKeys(centralSecuritiesRegister);

                // upload a special rights and restrictions document
                string specialRightsRestrictions = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
                NgWebElement uiUploadSpecialRightsRes = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
                uiUploadSpecialRightsRes.SendKeys(specialRightsRestrictions);

                // upload an additional supporting document
                string additionalSupportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "associates.pdf");
                NgWebElement uiUploadAdditionalSupportingDocument = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
                uiUploadAdditionalSupportingDocument.SendKeys(additionalSupportingDocument);

                /********** Leader #0 **********/

                // create the leader data
                string leaderFirstName = "Leader0";
                string leaderLastName = "PrivateCorp";
                string leaderTitle = "CTO";
                string leaderEmail = "leader0@privatecorp.com";

                // open leader #0 form  
                NgWebElement uiOpenLeaderForm = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leadership'][changetypesuffix='Leadership'] button"));
                uiOpenLeaderForm.Click();
                
                // enter leader #0 first name
                NgWebElement uiLeaderFirst = ngDriver.FindElement(By.CssSelector("input[formControlName='firstNameNew']"));
                uiLeaderFirst.SendKeys(leaderFirstName);

                // enter leader #0 last name
                NgWebElement uiLeaderLast = ngDriver.FindElement(By.CssSelector("input[formControlName='lastNameNew']"));
                uiLeaderLast.SendKeys(leaderLastName);

                // select leader #0 role
                NgWebElement uiLeaderRole = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leadership'][changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
                uiLeaderRole.Click();

                // enter leader #0 title
                NgWebElement uiLeaderTitle = ngDriver.FindElement(By.CssSelector("input[formControlName='titleNew']"));
                uiLeaderTitle.SendKeys(leaderTitle);

                // enter leader #0 email
                NgWebElement uiLeaderEmail = ngDriver.FindElement(By.CssSelector("input[formControlName='emailNew']"));
                uiLeaderEmail.SendKeys(leaderEmail);

                // enter leader #0 DOB
                NgWebElement uiOpenLeaderDOB = ngDriver.FindElement(By.CssSelector("input[formControlName='dateofBirthNew']"));
                uiOpenLeaderDOB.Click();

                // select the date
                SharedCalendarDate();

                // click on the Confirm button
                NgWebElement uiConfirmButtonLeader = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] .fa-save span"));
                uiConfirmButtonLeader.Click();

                /********** Individual Shareholder #0 **********/

                // create the shareholder data
                string shareholderFirstName = "IndividualShareholder0";
                string shareholderLastName = "PrivateCorp";
                string shareholderVotingShares = "500";
                string shareholderEmail = "individualshareholder0@privatecorp.com";

                // open shareholder #0 form
                NgWebElement uiOpenShare = ngDriver.FindElement(By.CssSelector("[changetypesuffix='IndividualShareholder'][addlabel='Add Individual Shareholder'] button"));
                uiOpenShare.Click();

                // enter shareholder #0 first name
                NgWebElement uiShareFirst = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName=\"firstNameNew\"]"));
                uiShareFirst.SendKeys(shareholderFirstName);

                // enter shareholder #0 last name
                NgWebElement uiShareLast = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='lastNameNew']"));
                uiShareLast.SendKeys(shareholderLastName);

                // enter shareholder #0 number of voting shares
                NgWebElement uiShareVotes = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='numberofSharesNew']"));
                uiShareVotes.SendKeys(shareholderVotingShares);

                // enter shareholder #0 email
                NgWebElement uiShareEmail = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='emailNew']"));
                uiShareEmail.SendKeys(shareholderEmail);

                // enter shareholder #0 DOB
                NgWebElement uiCalendarS1 = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='dateofBirthNew']"));
                uiCalendarS1.Click();

                // select the date
                SharedCalendarDate();

                // click on the Confirm button
                NgWebElement uiConfirmButtonIndyShareholder = ngDriver.FindElement(By.CssSelector("[changetypesuffix='IndividualShareholder'] .fa-save span"));
                uiConfirmButtonIndyShareholder.Click();

                /********** Business Shareholder #1 **********/

                // create the business shareholder data
                string businessName = "Business Shareholder 1";
                string businessVotingShares = "50";
                string businessEmail = "business@shareholder1.com";

                // open business shareholder #1 form    
                NgWebElement uiOpenShareBiz = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] button"));
                uiOpenShareBiz.Click();

                // enter business shareholder #1 name
                NgWebElement uiShareFirstBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='businessNameNew']"));
                uiShareFirstBiz.SendKeys(businessName);

                // enter business shareholder #1 voting shares
                NgWebElement uiShareVotesBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='numberofSharesNew']"));
                uiShareVotesBiz.SendKeys(businessVotingShares);

                // select the business shareholder #1 type
                NgWebElement uiShareBizType = ngDriver.FindElement(By.CssSelector("[formcontrolname='businessType'] option[value='PrivateCorporation']"));
                uiShareBizType.Click();

                // enter business shareholder #1 email
                NgWebElement uiShareEmailBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='emailNew']"));
                uiShareEmailBiz.SendKeys(businessEmail);

                // select the business shareholder #1 confirm button
                NgWebElement uiShareBizConfirmButton = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] .fa-save.ng-star-inserted span"));
                uiShareBizConfirmButton.Click();

                // upload a notice of articles document for business shareholder #1 
                string noticeOfArticlesBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
                NgWebElement uiUploadNoticeofArticlesBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
                uiUploadNoticeofArticlesBiz.SendKeys(noticeOfArticlesBiz);

                // upload a central securities register document for business shareholder #1 
                string centralSecuritiesRegisterBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
                NgWebElement uiUploadCentralSecRegBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[18]"));
                uiUploadCentralSecRegBiz.SendKeys(centralSecuritiesRegisterBiz);

                // upload a special rights and restrictions document for business shareholder #1 
                string specialRightsRestrictionsBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
                NgWebElement uiUploadSpecialRightsResBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[21]"));
                uiUploadSpecialRightsResBiz.SendKeys(specialRightsRestrictionsBiz);

                /********** Business Shareholder #1 - Leader #1 **********/

                // create business shareholder #1 leader data
                string leaderFirstNameBiz = "Leader1";
                string leaderLastNameBiz = "BizShareholderPrivateCorp";
                string leaderTitleBiz = "Event Planner";
                string leaderEmailBiz = "leader1bizshareholder@privatecorp.com";

                // open business shareholder #1 > leader form #1
                NgWebElement uiOpenLeaderFormBiz = ngDriver.FindElement(By.CssSelector("#cdk-accordion-child-0 .padded-section:nth-child(1) .btn-secondary"));
                uiOpenLeaderFormBiz.Click();

                // enter business shareholder #1 > leader #1 first name
                NgWebElement uiLeaderFirstBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
                uiLeaderFirstBiz.SendKeys(leaderFirstNameBiz);

                // enter business shareholder #1 > leader #1 last name
                NgWebElement uiLeaderLastBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='lastNameNew']"));
                uiLeaderLastBiz.SendKeys(leaderLastNameBiz);

                // select business shareholder #1 > leader #1 role
                NgWebElement uiLeaderRoleBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [addlabel='Add Leadership'][changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
                uiLeaderRoleBiz.Click();

                // enter business shareholder #1 > leader #1 title
                NgWebElement uiLeaderTitleBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='titleNew']"));
                uiLeaderTitleBiz.SendKeys(leaderTitleBiz);

                // enter business shareholder #1 > leader #1 email 
                NgWebElement uiLeaderEmailBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='emailNew']"));
                uiLeaderEmailBiz.SendKeys(leaderEmailBiz);

                // enter business shareholder #1 > leader #1 DOB
                NgWebElement uiLeaderDOB1Biz1 = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='dateofBirthNew']"));
                uiLeaderDOB1Biz1.Click();

                // select the date
                SharedCalendarDate();

                // click on the Confirm button
                NgWebElement uiConfirmButtonBusLeader = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] .fa-save span"));
                uiConfirmButtonBusLeader.Click();

                /********** Business Shareholder #1 - Individual Shareholder #1 **********/

                // create the business shareholder #1 > individual shareholder data
                string shareholderFirstNameBiz = "IndividualShareholder1";
                string shareholderLastNameBiz = "BizShareholderPrivateCorp";
                string shareholderVotingSharesBiz = "500";
                string shareholderEmailBiz = "individualshareholder1bizshareholder@privatecorp.com";

                // open business shareholder #1 > individual shareholder #1 form
                NgWebElement uiOpenIndyShareBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [addlabel='Add Individual Shareholder'] button"));
                uiOpenIndyShareBiz.Click();

                // enter business shareholder #1 > individual shareholder #1 first name
                NgWebElement uiIndyShareFirstBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
                uiIndyShareFirstBiz.SendKeys(shareholderFirstNameBiz);

                // enter business shareholder #1 > individual shareholder #1 last name
                NgWebElement uiIndyShareLastBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
                uiIndyShareLastBiz.SendKeys(shareholderLastNameBiz);

                // enter business shareholder #1 > individual shareholder #1 number of voting shares
                NgWebElement uiIndyShareVotesBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
                uiIndyShareVotesBiz.SendKeys(shareholderVotingSharesBiz);

                // enter business shareholder #1 > individual shareholder #1 email
                NgWebElement uiIndyShareEmailBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
                uiIndyShareEmailBiz.SendKeys(shareholderEmailBiz);

                // enter business shareholder #1 > individual shareholder #1 DOB
                NgWebElement uiCalendarIndyS1Biz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
                uiCalendarIndyS1Biz.Click();

                // select the date
                SharedCalendarDate();

                // click on the Confirm button
                NgWebElement uiConfirmButtonBusIndyShareholder = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] .fa-save span"));
                uiConfirmButtonBusIndyShareholder.Click();
            }

            if (businessType == " sole proprietorship")
            {
                // find the upload test files in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload an additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
                NgWebElement uiUploadadditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
                uiUploadadditionalSupporting.SendKeys(additionalSupportingPath);

                /********** Sole Proprietor > Leader #1 **********/

                // open the leader row
                NgWebElement uiOpenLeaderForm = ngDriver.FindElement(By.CssSelector("button.btn.btn-secondary"));
                uiOpenLeaderForm.Click();

                // create the leader info
                string firstName = "Leader";
                string lastName = "SoleProprietor";
                string email = "leader@soleproprietor.com";

                // enter the leader first name
                NgWebElement uiFirstName = ngDriver.FindElement(By.CssSelector("[formControlName='firstNameNew']"));
                uiFirstName.SendKeys(firstName);

                // enter the leader last name
                NgWebElement uiLastName = ngDriver.FindElement(By.CssSelector("[formControlName='lastNameNew']"));
                uiLastName.SendKeys(lastName);

                // enter the leader email
                NgWebElement uiEmail = ngDriver.FindElement(By.CssSelector("[formControlName='emailNew']"));
                uiEmail.SendKeys(email);

                // select the leader DOB
                NgWebElement uiOpenLeaderDOB = ngDriver.FindElement(By.CssSelector("[formcontrolname='dateofBirthNew']"));
                uiOpenLeaderDOB.Click();

                // select the date
                SharedCalendarDate();

                /********** Sole Proprietor > Leader #2 **********/

                // open the leader #2 row
                NgWebElement uiOpenLeaderForm2 = ngDriver.FindElement(By.CssSelector("button.btn.btn-secondary"));
                uiOpenLeaderForm2.Click();

                // create the leader #2 info
                string firstName2 = "Leader2";
                string email2 = "leader2@soleproprietor.com";

                // enter the leader #2 first name
                NgWebElement uiFirstName2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiFirstName2.SendKeys(firstName2);

                // enter the leader #2 last name
                NgWebElement uiLastName2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiLastName2.SendKeys(lastName);

                // enter the leader #2 email
                NgWebElement uiEmail2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(3) input[formControlName='emailNew']"));
                uiEmail2.SendKeys(email2);

                // select the leader #2 DOB
                NgWebElement uiOpenLeaderDOB2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(4) input[formControlName='dateofBirthNew']"));
                uiOpenLeaderDOB2.Click();

                // select the date
                SharedCalendarDate();

                /********** Sole Proprietor > Leader #3 **********/

                // open the leader #3 row
                NgWebElement uiOpenLeaderForm3 = ngDriver.FindElement(By.CssSelector("button.btn.btn-secondary"));
                uiOpenLeaderForm3.Click();

                // create the leader #2 info
                string firstName3 = "Leader3";
                string email3 = "leader3@soleproprietor.com";

                // enter the leader #2 first name
                NgWebElement uiFirstName3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiFirstName3.SendKeys(firstName3);

                // enter the leader #2 last name
                NgWebElement uiLastName3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiLastName3.SendKeys(lastName);

                // enter the leader #3 email
                NgWebElement uiEmail3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(3) input[formControlName='emailNew']"));
                uiEmail3.SendKeys(email3);

                // select the leader #3 DOB
                NgWebElement uiOpenLeaderDOB3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(4) input[formControlName='dateofBirthNew']"));
                uiOpenLeaderDOB3.Click();

                // select the date
                SharedCalendarDate();
            }

            if (businessType == " society")
            {
                /********** Director #1 **********/

                // create society data
                string membershipFee = "2500";
                string membershipNumber = "200";

                // enter Annual Membership Fee
                NgWebElement uiMemberFee = ngDriver.FindElement(By.CssSelector("[formcontrolname='annualMembershipFee']"));
                uiMemberFee.SendKeys(membershipFee);

                // enter Number of Members
                NgWebElement uiMemberNumber = ngDriver.FindElement(By.CssSelector("[formcontrolname='numberOfMembers']"));
                uiMemberNumber.SendKeys(membershipNumber);

                // find the upload test files in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload an additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
                NgWebElement uiUploadAdditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
                uiUploadAdditionalSupporting.SendKeys(additionalSupportingPath);

                // open the director #1 row 
                NgWebElement uiOpenKeyPersonnelForm = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'][changetypesuffix='Leadership'] button"));
                uiOpenKeyPersonnelForm.Click();

                // create the director #1 info
                string firstName = "Director";
                string lastName = "Society";
                string title = "Chair";
                string email = "director@society.com";

                // enter the director #1 first name
                NgWebElement uiFirstName = ngDriver.FindElement(By.CssSelector("[formcontrolname='firstNameNew']"));
                uiFirstName.SendKeys(firstName);

                // enter the director #1 last name
                NgWebElement uiLastName = ngDriver.FindElement(By.CssSelector("[formcontrolname='lastNameNew']"));
                uiLastName.SendKeys(lastName);

                // select the director #1 position
                NgWebElement uiPosition = ngDriver.FindElement(By.CssSelector("[formcontrolname='isDirectorNew']"));
                uiPosition.Click();

                // enter the director #1 title
                NgWebElement uiTitle = ngDriver.FindElement(By.CssSelector("[formcontrolname='titleNew']"));
                uiTitle.SendKeys(title);

                // enter the director #1 email
                NgWebElement uiEmail = ngDriver.FindElement(By.CssSelector("[formcontrolname='emailNew']"));
                uiEmail.SendKeys(email);

                // select the director #1 DOB
                NgWebElement uiOpenKeyPersonnelDOB = ngDriver.FindElement(By.CssSelector("[formcontrolname='dateofBirthNew']"));
                uiOpenKeyPersonnelDOB.Click();

                // select the date
                SharedCalendarDate();

                /********** Director #2 **********/

                // open the director #2 row 
                NgWebElement uiOpenKeyPersonnelForm2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] button"));
                uiOpenKeyPersonnelForm2.Click();

                // create the director #2 info
                string firstName2 = "Director2";
                string email2 = "director2@society.com";

                // enter the director #2 first name
                NgWebElement uiFirstName2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiFirstName2.SendKeys(firstName2);

                // enter the director #2 last name
                NgWebElement uiLastName2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiLastName2.SendKeys(lastName);

                // select the director #2 position
                NgWebElement uiPosition2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(3) input[formControlName='isDirectorNew']"));
                uiPosition2.Click();

                // enter the director #2 title
                NgWebElement uiTitle2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(4) input[formControlName='titleNew']"));
                uiTitle2.SendKeys(title);

                // enter the director #2 email
                NgWebElement uiEmail2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(5) input[formControlName='emailNew']"));
                uiEmail2.SendKeys(email2);

                // select the director #2 DOB
                NgWebElement uiOpenKeyPersonnelDOB2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(6) input[formControlName='dateofBirthNew']"));
                uiOpenKeyPersonnelDOB2.Click();

                // select the date
                SharedCalendarDate();

                /********** Director #3 **********/

                // open the director #3 row 
                NgWebElement uiOpenKeyPersonnelForm3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] button"));
                uiOpenKeyPersonnelForm3.Click();

                // create the director #3 info
                string firstName3 = "Director3";
                string email3 = "director3@society.com";

                // enter the director #3 first name
                NgWebElement uiFirstName3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiFirstName3.SendKeys(firstName3);

                // enter the director #3 last name
                NgWebElement uiLastName3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiLastName3.SendKeys(lastName);

                // select the director #3 position
                NgWebElement uiPosition3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(3) input[formControlName='isDirectorNew']"));
                uiPosition3.Click();

                // enter the director #3 title
                NgWebElement uiTitle3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(4) input[formControlName='titleNew']"));
                uiTitle3.SendKeys(title);

                // enter the director #3 email
                NgWebElement uiEmail3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(5) input[formControlName='emailNew']"));
                uiEmail3.SendKeys(email3);

                // select the director #2 DOB
                NgWebElement uiOpenKeyPersonnelDOB3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(6) input[formControlName='dateofBirthNew']"));
                uiOpenKeyPersonnelDOB3.Click();

                // select the date
                SharedCalendarDate();
            }

            if (businessType == " public corporation")
            {
                /********** Key Personnel #1 **********/

                // create the key personnel #1 data
                string keyPersonnelFirst = "KeyPersonnel1";
                string keyPersonnelLast = "Public Corp";
                string keyPersonnelTitle = "CEO";
                string keyPersonnelEmail = "keypersonnel1@publiccorp.com";

                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload NOA form
                string NOAPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uiUploadNOA = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
                uiUploadNOA.SendKeys(NOAPath);

                // upload additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uiUploadAdditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
                uiUploadAdditionalSupporting.SendKeys(additionalSupportingPath);

                // open key personnel #1 form
                NgWebElement uiOpenKeyPersonnelForm = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] button"));
                uiOpenKeyPersonnelForm.Click();

                // enter key personnel #1 first name
                NgWebElement uiKeyPersonnelFirst = ngDriver.FindElement(By.CssSelector("[formcontrolname='firstNameNew']"));
                uiKeyPersonnelFirst.SendKeys(keyPersonnelFirst);

                // enter key personnel #1 last name
                NgWebElement uiKeyPersonnelLast = ngDriver.FindElement(By.CssSelector("[formcontrolname='lastNameNew']"));
                uiKeyPersonnelLast.SendKeys(keyPersonnelLast);

                // select key personnel #1 role
                NgWebElement uiKeyPersonnelRole = ngDriver.FindElement(By.CssSelector("[formcontrolname='isDirectorNew']"));
                uiKeyPersonnelRole.Click();

                // enter key personnel #1 title
                NgWebElement uiKeyPersonnelTitle = ngDriver.FindElement(By.CssSelector("[formcontrolname='titleNew']"));
                uiKeyPersonnelTitle.SendKeys(keyPersonnelTitle);

                // enter key personnel #1 email
                NgWebElement uiKeyPersonnelEmail = ngDriver.FindElement(By.CssSelector("[formcontrolname='emailNew']"));
                uiKeyPersonnelEmail.SendKeys(keyPersonnelEmail);

                // select key person #1 DOB
                NgWebElement uiKeyPersonDOB = ngDriver.FindElement(By.CssSelector("[formcontrolname='dateofBirthNew']"));
                uiKeyPersonDOB.Click();

                // select the date
                SharedCalendarDate();

                /********** Key Personnel #2 **********/

                // create the second key personnel #2 data
                string keyPersonnelFirst2 = "KeyPersonnel2";
                string keyPersonnelEmail2 = "keypersonnel2@publiccorp.com";

                // open key personnel #2 form
                NgWebElement uiOpenKeyPersonnelForm2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] button"));
                uiOpenKeyPersonnelForm2.Click();

                // enter key personnel #2 first name
                NgWebElement uiKeyPersonnelFirst2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiKeyPersonnelFirst2.SendKeys(keyPersonnelFirst2);

                // enter key personnel #2 last name
                NgWebElement uiKeyPersonnelLast2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiKeyPersonnelLast2.SendKeys(keyPersonnelLast);

                // select key personnel #2 role
                NgWebElement uiKeyPersonnelRole2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(3) input[formControlName='isDirectorNew']"));
                uiKeyPersonnelRole2.Click();

                // enter key personnel #2 title
                NgWebElement uiKeyPersonnelTitle2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(4) input[formControlName='titleNew']"));
                uiKeyPersonnelTitle2.SendKeys(keyPersonnelTitle);

                // enter key personnel #2 email
                NgWebElement uiKeyPersonnelEmail2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(5) input[formControlName='emailNew']"));
                uiKeyPersonnelEmail2.SendKeys(keyPersonnelEmail2);

                // select key person #2 DOB
                NgWebElement uiKeyPersonDOB2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(6) input[formControlName='dateofBirthNew']"));
                uiKeyPersonDOB2.Click();

                // select the date
                SharedCalendarDate();

                /********** Key Personnel #3 **********/

                // create the second key personnel #3 data
                string keyPersonnelFirst3 = "KeyPersonnel3";
                string keyPersonnelEmail3 = "keypersonnel3@publiccorp.com";

                // open key personnel #3 form
                NgWebElement uiOpenKeyPersonnelForm3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] button"));
                uiOpenKeyPersonnelForm3.Click();

                // enter key personnel #3 first name
                NgWebElement uiKeyPersonnelFirst3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiKeyPersonnelFirst3.SendKeys(keyPersonnelFirst3);

                // enter key personnel #3 last name
                NgWebElement uiKeyPersonnelLast3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiKeyPersonnelLast3.SendKeys(keyPersonnelLast);

                // select key personnel #3 role
                NgWebElement uiKeyPersonnelRole3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(3) input[formControlName='isDirectorNew']"));
                uiKeyPersonnelRole3.Click();

                // enter key personnel #3 title
                NgWebElement uiKeyPersonnelTitle3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(4) input[formControlName='titleNew']"));
                uiKeyPersonnelTitle3.SendKeys(keyPersonnelTitle);

                // enter key personnel #3 email
                NgWebElement uiKeyPersonnelEmail3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(5) input[formControlName='emailNew']"));
                uiKeyPersonnelEmail3.SendKeys(keyPersonnelEmail3);

                // select key person #3 DOB
                NgWebElement uiKeyPersonDOB3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(6) input[formControlName='dateofBirthNew']"));
                uiKeyPersonDOB3.Click();

                // select the date
                SharedCalendarDate();
            }

            if (businessType == " partnership")
            {
                // create individual partner info
                string partnerFirstName = "Individual";
                string partnerLastName = "Partner";
                string partnerPercentage = "50";
                string partnerEmail = "individual@partner.com";

                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload the partnership agreement
                string partnershipPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "partnership_agreement.pdf");
                NgWebElement uiUploadPartnershipAgreement = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
                uiUploadPartnershipAgreement.SendKeys(partnershipPath);

                // upload the additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "partnership_agreement.pdf");
                NgWebElement uiUploadAdditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
                uiUploadAdditionalSupporting.SendKeys(additionalSupportingPath);

                // open partner row
                NgWebElement uiPartnerRow = ngDriver.FindElement(By.CssSelector("[changetypesuffix='IndividualShareholder'] button"));
                uiPartnerRow.Click();

                // enter partner first name
                NgWebElement uiPartnerFirst = ngDriver.FindElement(By.CssSelector("[formcontrolname='firstNameNew']"));
                uiPartnerFirst.SendKeys(partnerFirstName);

                // enter partner last name
                NgWebElement uiPartnerLast = ngDriver.FindElement(By.CssSelector("[formcontrolname='lastNameNew']"));
                uiPartnerLast.SendKeys(partnerLastName);

                // enter partner percentage
                NgWebElement uiPartnerPercentage = ngDriver.FindElement(By.CssSelector("[formcontrolname='interestPercentageNew']"));
                uiPartnerPercentage.SendKeys(partnerPercentage);

                // enter partner email
                NgWebElement uiPartnerEmail = ngDriver.FindElement(By.CssSelector("[formcontrolname='emailNew']"));
                uiPartnerEmail.SendKeys(partnerEmail);

                // enter partner DOB
                NgWebElement uiOpenPartnerDOB = ngDriver.FindElement(By.CssSelector("[formcontrolname='dateofBirthNew']"));
                uiOpenPartnerDOB.Click();

                // select the date
                SharedCalendarDate();

                // open business partner row
                NgWebElement uiOpenPartnerRow = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] button"));
                uiOpenPartnerRow.Click();

                // create business partner info
                string bizPartnerName = "Business Partner";
                string bizPartnerPercentage = "50";
                string bizPartnerEmail = "business@partner.com";

                // enter the business partner name
                NgWebElement uiBizPartnerName = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='businessNameNew']"));
                uiBizPartnerName.SendKeys(bizPartnerName);

                // enter the business partner percentage
                NgWebElement uiBizPartnerPercentage = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='interestPercentageNew']"));
                uiBizPartnerPercentage.SendKeys(bizPartnerPercentage);

                // select the business type using dropdown
                NgWebElement uiShareBizType = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='Partnership']"));
                uiShareBizType.Click();

                // enter the business partner email
                NgWebElement uiBizPartnerEmail = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='emailNew']"));
                uiBizPartnerEmail.SendKeys(bizPartnerEmail);

                // click on the business shareholder confirm button
                NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'][addlabel='Add Business Partner'] .fa-save span"));
                uiConfirmButton.Click();

                // upload a second partnership agreement
                string partnershipPath2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "partnership_agreement.pdf");
                NgWebElement uiUploadPartnership2Agreement = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
                uiUploadPartnership2Agreement.SendKeys(partnershipPath2);

                // open individual partner 2 row
                NgWebElement uiOpenPartner2Row = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
                uiOpenPartner2Row.Click();

                // create individual partner 2 info
                string partner2FirstName = "Individual";
                string partner2LastName = "Partner2";
                string partner2Percentage = "50";
                string partner2Email = "individual@partner2.com";

                // enter individual partner2 first name
                NgWebElement uiPartner2First = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formControlName='firstNameNew']"));
                uiPartner2First.SendKeys(partner2FirstName);

                // enter individual partner2 last name
                NgWebElement uiPartner2Last = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formControlName='lastNameNew']"));
                uiPartner2Last.SendKeys(partner2LastName);

                // enter individual partner2 percentage
                NgWebElement uiPartner2Percentage = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formControlName='interestPercentageNew']"));
                uiPartner2Percentage.SendKeys(partner2Percentage);

                // enter individual partner2 email
                NgWebElement uiPartner2Email = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formControlName='emailNew']"));
                uiPartner2Email.SendKeys(partner2Email);

                // enter individual partner2 DOB
                NgWebElement uiOpenPartner2DOB = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formControlName='dateofBirthNew']"));
                uiOpenPartner2DOB.Click();

                // select the date
                SharedCalendarDate();

                // click on individual partner2 confirm button
                NgWebElement uiConfirmButton2 = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Partner'][changetypesuffix='BusinessShareholder'] .fa-save span"));
                uiConfirmButton2.Click();
            }

            if (businessType == "n indigenous nation")
            {
                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload the associates document
                string associatesPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "associates.pdf");
                NgWebElement uiUploadAssociates = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
                uiUploadAssociates.SendKeys(associatesPath);

                // upload the additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "associates.pdf");
                NgWebElement uiUploadAdditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
                uiUploadAdditionalSupporting.SendKeys(additionalSupportingPath);
            }
        }


        [And(@"I click on the Complete Organization Information button")]
        public void CompleteOrgInfo()
        {
            // click on the complete organization information button
            NgWebElement uiOrgInfoButton = ngDriver.FindElement(By.CssSelector("button.btn-primary[routerlink='/org-structure']"));
            uiOrgInfoButton.Click();
        }


        [And(@"I show the store as open on the map")]
        public void ShowStoreOpenOnMap()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            // click on the Show Store as Open on Map checkbox
            NgWebElement uiMapCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox"));
            uiMapCheckbox.Click();
        }


        [And(@"I do not complete the application correctly")]
        public void CompleteApplicationIncorrectly()
        {
            ClickOnSubmitButton();

            System.Threading.Thread.Sleep(5000);
        }


        [And(@"the expected validation errors are thrown for a(.*)")]
        public void ValidationErrorMessages(string applicationType)
        {
            if (applicationType == " licensee representative")
            {
                // check  missing representative name error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Representative Name is a required field')]")).Displayed);

                // check missing telephone error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Telephone is a required field')]")).Displayed);

                // check missing email error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'E-mail Address is a required field')]")).Displayed);

                // check missing signature agreement error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);

                // check missing scope of authority error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one scope of authority must be selected')]")).Displayed);

                // check missing declaration checkbox error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Declaration Checkbox')]")).Displayed);
            }
            else
            {
                // check missing authorized to submit error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that you are authorized to submit the application.')]")).Displayed);

                // check missing signature agreement error is thrown
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please affirm that all of the information provided for this application is true and complete.')]")).Displayed);

                if ((applicationType == " Manufacturing application") || (applicationType == " Cannabis application") || (applicationType == " Catering application") || (applicationType == " Rural Store application") || (applicationType == "n indigenous nation Cannabis application"))
                {
                    // check missing street address error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the street address')]")).Displayed);

                    // check missing city error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);

                    // check missing postal code error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);

                    // check missing PID error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the Parcel Identifier (format: 9 digits)')]")).Displayed);

                    // check missing business contact error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the business contact')]")).Displayed);

                    // check missing business contact phone number error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s 10-digit phone number')]")).Displayed);

                    // check missing business contact email error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'s email address')]")).Displayed);

                    // check missing establishment name error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Establishment Name is required')]")).Displayed);
                }

                if ((applicationType == " Manufacturing application") || (applicationType == " Cannabis application") || (applicationType == "n indigenous nation Cannabis application"))
                { 
                    // check missing police jurisdiction error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'policeJurisdiction is not valid')]")).Displayed);

                    // check missing indigenous nation error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'indigenousNation is not valid')]")).Displayed);
                }

                if ((applicationType == " Manufacturing application") || (applicationType == " Cannabis application") || (applicationType == " Catering application") || (applicationType == " location change application") || (applicationType == "n indigenous nation Cannabis application"))
                { 
                    // check missing signage document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                }

                if ((applicationType == " Manufacturing application") || (applicationType == " Cannabis application") || (applicationType == " facility structural change application") || (applicationType == " location change application") || (applicationType == "n indigenous nation Cannabis application"))
                {
                    // check missing site plan document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one site plan document is required.')]")).Displayed);

                    // check missing floor plan document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                }

                if ((applicationType == " Cannabis application") || (applicationType == "n indigenous nation Cannabis application"))
                {
                    // check missing product not visible from outside error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please confirm that product will not be visible from the outside')]")).Displayed);

                    // check missing zoning document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one zoning document is required.')]")).Displayed);

                    // check missing Financial Integrity document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Financial Integrity form is required.')]")).Displayed);

                    // check missing supporting document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one supporting document is required.')]")).Displayed);
                }

                if (applicationType == "n event authorization")
                {
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'TO BE COMPLETED')]")).Displayed);

                    // waiting for bug fix: LCSD-3663
                }

                if (applicationType == " transfer of ownership")
                {
                    // check missing value error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please select a value')]")).Displayed);

                    // check missing transfer consent error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please consent to the transfer')]")).Displayed);
                }

                if ((applicationType == " Catering transfer of ownership") || (applicationType == " CRS transfer of ownership"))
                {
                     // check missing proposed transferee error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please select the proposed transferee')]")).Displayed);

                    // check missing transfer consent error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please consent to the transfer')]")).Displayed);
                }

                if (applicationType == " CRS Branding Change application")
                {
                    // check missing proposed change error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'proposedChange is not valid')]")).Displayed);

                    // check missing signage document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                }

                if ((applicationType == " Branding Change application") || (applicationType == " location change application"))
                {
                    // check missing signage document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one signage document is required.')]")).Displayed);
                }

                if ((applicationType == " store relocation application") || (applicationType == " Catering store relocation application"))
                {
                    // check missing street address error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the street address')]")).Displayed);

                    // check missing city error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the city')]")).Displayed);

                    // check missing postal code error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter the postal code')]")).Displayed);
                }

                if (applicationType == " structural change application")
                {
                    // check missing product not visible from outside error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please confirm that product will not be visible from the outside')]")).Displayed);
                }

                if ((applicationType == " structural change application") || (applicationType == " Rural Store application"))
                {
                    // check missing description error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter a description')]")).Displayed);

                    // check missing floor plan document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one floor plan document is required.')]")).Displayed);
                }

                if (applicationType == " Catering third party application")
                {
                    // check missing value error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please select a value')]")).Displayed);
                }

                if (applicationType == " location change application")
                {
                    // check missing description error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please enter a description')]")).Displayed);

                    // check missing supporting document error is thrown
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'At least one supporting document is required.')]")).Displayed);
                }

                if (applicationType == "n indigenous nation Cannabis application")
                {
                    // check missing IN error is shown
                    // waiting for bug fix: LCSD-3671
                }

                if (applicationType == " licence renewal application")
                {
                    // check for missing error messages
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 1')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 2')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 3')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 4')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 5')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 6')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 7')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 8')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 9')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 10')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 11')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 12')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 13')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 14')]")).Displayed);
                    Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Please answer question 15')]")).Displayed);
                }
            }
        }


        [And(@"I enter the same individual as a director and a shareholder")]
        public void SameDirectorShareholder()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document
            string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
            uiUploadSignage.SendKeys(noticeOfArticles);

            // upload a central securities register document
            string centralSecuritiesRegister = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadCentralSecReg = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uiUploadCentralSecReg.SendKeys(centralSecuritiesRegister);

            // upload a special rights and restrictions document
            string specialRightsRestrictions = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSpecialRightsRes = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
            uiUploadSpecialRightsRes.SendKeys(specialRightsRestrictions);

            /***** Key Personnel #1 *****/

            // click on the Add Key Personnel button
            NgWebElement uiAddKeyPersonnel = ngDriver.FindElement(By.CssSelector(".padded-section:nth-child(1) .btn-secondary"));
            uiAddKeyPersonnel.Click();

            // create data
            string sameIndividualFirstName = "Same1";
            string sameIndividualLastName = "Individual";
            string sameIndividualEmail = "same@individual.com";
            string sameTitle = "CEO";
            string votingShares = "100";

            string sameIndividualEmail2 = "same@individual2.com";

            string sparePersonnelFirstName = "Spare";
            string sparePersonnelLastName = "KeyPersonnel";
            string sparePersonnelTitle = "CFO";
            string sparePersonnelEmail = "cfo@test.com";

            // enter the key personnel first name 
            NgWebElement uiSameIndividualFirstName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
            uiSameIndividualFirstName.SendKeys(sameIndividualFirstName);

            // enter the key personnel last name 
            NgWebElement uiSameIndividualLastName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastNameNew']"));
            uiSameIndividualLastName.SendKeys(sameIndividualLastName);

            // click the key personnel checkbox
            NgWebElement uiSameRole = ngDriver.FindElement(By.CssSelector("input[formcontrolname='isDirectorNew']"));
            uiSameRole.Click();

            // enter the key personnel title
            NgWebElement uiSameTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='titleNew']"));
            uiSameTitle.SendKeys(sameTitle);

            // enter the key personnel email 
            NgWebElement uiSameIndividualEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
            uiSameIndividualEmail.SendKeys(sameIndividualEmail);

            // select the key personnel DOB
            NgWebElement uiOpenKeyPersonnelDOB = ngDriver.FindElement(By.CssSelector("input[formcontrolname='dateofBirthNew']"));
            uiOpenKeyPersonnelDOB.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector(".fa-save span"));
            uiConfirmButton.Click();

            /***** Key Personnel #2 *****/
            /* This extra person has been added because the calendar selection is unreliable via SharedCalendarDate(). DO NOT REMOVE. */

            // click on the Add Key Personnel button
            NgWebElement uiAddKeyPersonnel2 = ngDriver.FindElement(By.CssSelector(".padded-section:nth-child(1) .btn-secondary"));
            uiAddKeyPersonnel2.Click();

            // enter the key personnel first name 
            NgWebElement uiSameIndividualFirstName2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
            uiSameIndividualFirstName2.SendKeys(sameIndividualFirstName);

            // enter the key personnel last name 
            NgWebElement uiSameIndividualLastName2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastNameNew']"));
            uiSameIndividualLastName2.SendKeys(sameIndividualLastName);

            // click the key personnel checkbox
            NgWebElement uiSameRole2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='isDirectorNew']"));
            uiSameRole2.Click();

            // enter the key personnel title
            NgWebElement uiSameTitle2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='titleNew']"));
            uiSameTitle2.SendKeys(sameTitle);

            // enter the key personnel email 
            NgWebElement uiSameIndividualEmail2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
            uiSameIndividualEmail2.SendKeys(sameIndividualEmail);

            // select the key personnel DOB
            NgWebElement uiOpenKeyPersonnelDOB2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='dateofBirthNew']"));
            uiOpenKeyPersonnelDOB2.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton1a = ngDriver.FindElement(By.CssSelector(".fa-save span"));
            uiConfirmButton1a.Click();

            /***** Key Personnel #3 *****/

            // click on the add key personnel button for spare
            NgWebElement uiAddKeyPersonnel3 = ngDriver.FindElement(By.CssSelector(".ng-touched .btn-secondary"));
            uiAddKeyPersonnel3.Click();

            // enter the spare key personnel first name
            NgWebElement uiSpareFirstName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
            uiSpareFirstName.SendKeys(sparePersonnelFirstName);

            // enter the spare key personnel last name 
            NgWebElement uiSpareLastName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastNameNew']"));
            uiSpareLastName.SendKeys(sparePersonnelLastName);

            // click the spare key personnel checkbox            
            NgWebElement uiSpareRole = ngDriver.FindElement(By.CssSelector("input[formcontrolname='isDirectorNew']"));
            uiSpareRole.Click();

            // enter the spare key personnel title
            NgWebElement uiSpareTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='titleNew']"));
            uiSpareTitle.SendKeys(sparePersonnelTitle);

            // enter the spare key personnel email 
            NgWebElement uiSpareEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
            uiSpareEmail.SendKeys(sparePersonnelEmail);

            // select the spare key personnel DOB
            NgWebElement uiOpenSpareDOB = ngDriver.FindElement(By.CssSelector("input[formcontrolname='dateofBirthNew']"));
            uiOpenSpareDOB.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton2 = ngDriver.FindElement(By.CssSelector(".fa-save span"));
            uiConfirmButton2.Click();

            // delete the first same individual - DO NOT REMOVE
            NgWebElement uiDeleteButton = ngDriver.FindElement(By.XPath("//app-associate-list/div/table/tr[1]/td[7]/i[2]/span"));
            uiDeleteButton.Click();

            /***** Individual Shareholder *****/

            // click on Add Individual Shareholder
            NgWebElement uiAddIndividualShareholder = ngDriver.FindElement(By.CssSelector(".padded-section:nth-child(2) .btn-secondary"));
            uiAddIndividualShareholder.Click();

            // enter the shareholder first name
            NgWebElement uiSameIndividualFirstName3 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
            uiSameIndividualFirstName3.SendKeys(sameIndividualFirstName);

            // enter the shareholder last name
            NgWebElement uiSameIndividualLastName3 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastNameNew']"));
            uiSameIndividualLastName3.SendKeys(sameIndividualLastName);

            // enter the shareholder number of voting shares
            NgWebElement uiSameIndividualVotingShare = ngDriver.FindElement(By.CssSelector("input[formcontrolname='numberofSharesNew']"));
            uiSameIndividualVotingShare.SendKeys(votingShares);

            // enter the shareholder email
            NgWebElement uiSameIndividualEmail3 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
            uiSameIndividualEmail3.SendKeys(sameIndividualEmail2);

            // enter the shareholder DOB
            NgWebElement uiCalendarS1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='dateofBirthNew']"));
            uiCalendarS1.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton3 = ngDriver.FindElement(By.CssSelector(".fa-save span"));
            uiConfirmButton3.Click();
        }


        [And(@"I delete only the director record")]
        public void DeleteDirectorRecord()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // click on the delete button for key personnel > director record    
            NgWebElement uiEditInfoButton = ngDriver.FindElement(By.XPath("//app-associate-list/div/table/tr[1]/td[7]/i[2]/span"));
            uiEditInfoButton.Click();

            // click on the Submit Org Info button
            NgWebElement uiSubmitOrgInfoButton = ngDriver.FindElement(By.CssSelector("app-application-licensee-changes button.btn-primary"));
            uiSubmitOrgInfoButton.Click();
        }


        [And(@"only the shareholder record is displayed")]
        public void ShareholderRecordDisplayed()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // check that the director email is not displayed to confirm deletion
            Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'same@individual.com'))]")).Displayed);

            // check that the shareholder email is displayed to confirm remains
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'same@individual2.com')]")).Displayed);
        }


        [And(@"I modify only the director record")]
        public void ModifyOnlyDirectorRecord()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // create new name for same individual
            string newFirstName = "NewFirstName";
            string newLastName = "NewLastName";

            // click on the edit button for key personnel 
            NgWebElement uiEditInfoButton = ngDriver.FindElement(By.XPath("//i/span"));
            uiEditInfoButton.Click();

            // enter the new first name 
            NgWebElement uiNewFirstName = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiNewFirstName.Clear();
            uiNewFirstName.SendKeys(newFirstName);

            // enter the new last name 
            NgWebElement uiNewLastName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiNewLastName.Clear();
            uiNewLastName.SendKeys(newLastName);

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.XPath("//i/span"));
            uiConfirmButton.Click();

            // click on the Submit Org Info button
            NgWebElement uiSubmitOrgInfoButton = ngDriver.FindElement(By.CssSelector("app-application-licensee-changes button.btn-primary"));
            uiSubmitOrgInfoButton.Click();
        }


        [And(@"the director and shareholder name are identical")]
        public void DirectorShareholderNameIdentical()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // check that the director first name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'NewFirstName')]")).Displayed);

            // check that the director last name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[1]/td[2]/span[contains(.,'NewLastName')]")).Displayed);

            // check that the shareholder first name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/span[contains(.,'NewFirstName')]")).Displayed);

            // check that the shareholder last name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr/td[2]/span[contains(.,'NewLastName')]")).Displayed);
        }


        [And(@"I request a licensee representative")]
        public void RequestLicenseeRepresentative()
        {
            /* 
            Page Title: Licences
            */

            // click on the Licensee Representative link
            string addLicensee = "Add Licensee Representative";
            NgWebElement uiAddLicensee = ngDriver.FindElement(By.LinkText(addLicensee));
            uiAddLicensee.Click();

            // create test data
            string representativeName = "Automated Test";
            string telephone = "2005081818";
            string email = "automated@test.com";

            // enter the representative name
            NgWebElement uiFullName = ngDriver.FindElement(By.CssSelector("input[formControlName='representativeFullName']"));
            uiFullName.SendKeys(representativeName);

            // enter the representative telephone number
            NgWebElement uiPhoneNumber = ngDriver.FindElement(By.CssSelector("input[formControlName='representativePhoneNumber']"));
            uiPhoneNumber.SendKeys(telephone);

            // enter the representative email address
            NgWebElement uiEmail = ngDriver.FindElement(By.CssSelector("input[formControlName='representativeEmail']"));
            uiEmail.SendKeys(email);

            // click on the submit permanent change applications checkbox
            NgWebElement uiCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanSubmitPermanentChangeApplications']"));
            uiCheckbox.Click();

            // click on the sign temporary change applications checkbox
            NgWebElement uiCheckbox1 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanSignTemporaryChangeApplications']"));
            uiCheckbox1.Click();

            // click on the obtain licence info from branch checkbox
            NgWebElement uiCheckbox2 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanObtainLicenceInformation']"));
            uiCheckbox2.Click();

            // click on sign grocery annual proof of sales revenue checkbox
            NgWebElement uiCheckbox3 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanSignGroceryStoreProofOfSale']"));
            uiCheckbox3.Click();

            // click on attend education sessions checkbox
            NgWebElement uiCheckbox4 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanAttendEducationSessions']"));
            uiCheckbox4.Click();

            // click on attend compliance meetings checkbox
            NgWebElement uiCheckbox5 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanAttendComplianceMeetings']"));
            uiCheckbox5.Click();

            // click on represent licensee at enforcement hearings checkbox
            NgWebElement uiCheckbox6 = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='representativeCanRepresentAtHearings']"));
            uiCheckbox6.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.XPath("//app-field/section/div/section/section/input"));
            uiSignatureAgree.Click();

            ClickOnSubmitButton();
        }


        [And(@"the organization structure page is displayed")]
        public void OrgStructureDisplays()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // confirm that the page loads
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Detailed Organization Information')]")).Displayed);
        }

      
        [And(@"I add a business shareholder with the same individual as a director and a shareholder")]
        public void BusinessShareholderSameDirShare()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // create the business shareholder data
            string businessName = "Business Shareholder";
            string businessVotingShares = "50";
            string businessEmail = "business@shareholder.com";

            // open business shareholder form    
            NgWebElement uiOpenShareBiz = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] button"));
            uiOpenShareBiz.Click();

            // enter business shareholder name
            NgWebElement uiShareFirstBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='businessNameNew']"));
            uiShareFirstBiz.SendKeys(businessName);

            // enter business shareholder voting shares
            NgWebElement uiShareVotesBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='numberofSharesNew']"));
            uiShareVotesBiz.SendKeys(businessVotingShares);

            // select the business shareholder type
            NgWebElement uiShareBizType = ngDriver.FindElement(By.CssSelector("[formcontrolname='businessType'] option[value='PrivateCorporation']"));
            uiShareBizType.Click();

            // enter business shareholder email
            NgWebElement uiShareEmailBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='emailNew']"));
            uiShareEmailBiz.SendKeys(businessEmail);

            // select the business shareholder confirm button
            NgWebElement uiShareBizConfirmButton = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] .fa-save.ng-star-inserted span"));
            uiShareBizConfirmButton.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder 
            string noticeOfArticlesBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadNoticeofArticlesBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[12]"));
            uiUploadNoticeofArticlesBiz.SendKeys(noticeOfArticlesBiz);

            // upload a central securities register document for business shareholder 
            string centralSecuritiesRegisterBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uiUploadCentralSecRegBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uiUploadCentralSecRegBiz.SendKeys(centralSecuritiesRegisterBiz);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uiUploadSpecialRightsResBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[18]"));
            uiUploadSpecialRightsResBiz.SendKeys(specialRightsRestrictionsBiz);

            /********** Business Shareholder - Key Personnel #1 **********/

            // create business shareholder #1 key personnel data
            string keyPersonnelFirstNameBiz = "Same2";
            string keyPersonnelLastNameBiz = "Individual2";
            string keyPersonnelTitleBiz = "Event Planner";
            string keyPersonnelEmailBiz = "sameindividual@privatecorp.com";

            // open business shareholder > key personnel form #1
            NgWebElement uiOpenKeyPersonnelFormBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] app-associate-list[addlabel='Add Key Personnel'][changetypesuffix='Leadership'] button"));
            uiOpenKeyPersonnelFormBiz.Click();

            // enter business shareholder > key personnel #1 first name
            NgWebElement uiKeyPersonFirstBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiKeyPersonFirstBiz.SendKeys(keyPersonnelFirstNameBiz);

            // enter business shareholder > key personnel #1 last name
            NgWebElement uiKeyPersonLastBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='lastNameNew']"));
            uiKeyPersonLastBiz.SendKeys(keyPersonnelLastNameBiz);

            // select business shareholder > key personnel #1 role
            NgWebElement uiKeyPersonRoleBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [addlabel='Add Key Personnel'][changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiKeyPersonRoleBiz.Click();

            // enter business shareholder > key personnel #1 title
            NgWebElement uiKeyPersonTitleBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='titleNew']"));
            uiKeyPersonTitleBiz.SendKeys(keyPersonnelTitleBiz);

            // enter business shareholder > key personnel #1 email 
            NgWebElement uiKeyPersonEmailBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='emailNew']"));
            uiKeyPersonEmailBiz.SendKeys(keyPersonnelEmailBiz);

            // enter business shareholder > key personnel #1 DOB
            NgWebElement uiKeyPersonnelDOB1Biz1 = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='dateofBirthNew']"));
            uiKeyPersonnelDOB1Biz1.Click();

            // select the date
            SharedCalendarDate();

            /********** Business Shareholder - Individual Shareholder #1 **********/

            // open business shareholder #1 > individual shareholder #1 form
            NgWebElement uiOpenIndyShareBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [addlabel='Add Individual Shareholder'] button"));
            uiOpenIndyShareBiz.Click();

            // enter business shareholder #1 > individual shareholder #1 first name
            NgWebElement uiIndyShareFirstBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz.SendKeys(keyPersonnelFirstNameBiz);

            // enter business shareholder #1 > individual shareholder #1 last name
            NgWebElement uiIndyShareLastBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz.SendKeys(keyPersonnelLastNameBiz);

            // enter business shareholder #1 > individual shareholder #1 number of voting shares
            string shareholderVotingSharesBiz = "10";
            NgWebElement uiIndyShareVotesBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz.SendKeys(shareholderVotingSharesBiz);

            // enter business shareholder #1 > individual shareholder #1 email
            NgWebElement uiIndyShareEmailBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz.SendKeys(keyPersonnelEmailBiz);

            // enter business shareholder #1 > individual shareholder #1 DOB
            NgWebElement uiCalendarIndyS1Biz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz.Click();

            // select the date
            SharedCalendarDate();
        }


        [And(@"I add a second individual as a director and a shareholder to the business shareholder")]
        public void BusinessShareholderSameDirShare2()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // create business shareholder key personnel #2 data
            string keyPersonnelFirstNameBiz = "Same3";
            string keyPersonnelLastNameBiz = "Individual3";
            string keyPersonnelTitleBiz = "Event Planner";
            string keyPersonnelEmailBiz = "sameindividual2@privatecorp.com";

            // open business shareholder > key personnel #2 form
            NgWebElement uiKeyPersonShareBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix= 'BusinessShareholder'] [changetypesuffix= 'Leadership'] button"));
            uiKeyPersonShareBiz.Click();

            // enter business shareholder > key personnel #2 first name
            NgWebElement uiKeyPersonFirstBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiKeyPersonFirstBiz.SendKeys(keyPersonnelFirstNameBiz);

            // enter business shareholder > key personnel #2 last name
            NgWebElement uiKeyPersonLastBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiKeyPersonLastBiz.SendKeys(keyPersonnelLastNameBiz);

            // select business shareholder > key personnel #2 role
            NgWebElement uiKeyPersonRoleBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiKeyPersonRoleBiz.Click();

            // enter business shareholder > key personnel #2 title
            NgWebElement uiKeyPersonTitleBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiKeyPersonTitleBiz.SendKeys(keyPersonnelTitleBiz);

            // enter business shareholder > key personnel #2 email 
            NgWebElement uiKeyPersonEmailBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiKeyPersonEmailBiz.SendKeys(keyPersonnelEmailBiz);

            // enter business shareholder > key personnel #2 DOB
            NgWebElement uiKeyPersonnelDOB1Biz1 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiKeyPersonnelDOB1Biz1.Click();

            // select the date
            SharedCalendarDate();

            // open business shareholder > individual shareholder #2 form
            NgWebElement uiOpenIndyShareBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
            uiOpenIndyShareBiz.Click();

            // enter business shareholder > individual shareholder #2 first name
            NgWebElement uiIndyShareFirstBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz.SendKeys(keyPersonnelFirstNameBiz);

            // enter business shareholder > individual shareholder #2 last name
            NgWebElement uiIndyShareLastBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz.SendKeys(keyPersonnelLastNameBiz);

            // enter business shareholder > individual shareholder #2 number of voting shares
            string shareholderVotingSharesBiz = "10";
            NgWebElement uiIndyShareVotesBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz.SendKeys(shareholderVotingSharesBiz);

            // enter business shareholder > individual shareholder #2 email
            NgWebElement uiIndyShareEmailBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz.SendKeys(keyPersonnelEmailBiz);

            // enter business shareholder > individual shareholder #2 DOB
            NgWebElement uiCalendarIndyS1Biz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz.Click();

            // select the date
            SharedCalendarDate();
        }

        
        [And(@"the org structure is correct")]
        public void OrgStructureCorrect()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // confirm that first individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same1')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/span[contains(.,'Same1')]")).Displayed);

            // confirm that second individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same2')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same2')]")).Displayed);

            // confirm that third individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[2]/td[1]/span[contains(.,'Same3')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[2]/td[1]/span[contains(.,'Same3')]")).Displayed);
        }


        [And(@"the org structure is correct after payment")]
        public void OrgStructureCorrectPayment()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // confirm that first individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same1')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/span[contains(.,'Same1')]")).Displayed);

            // confirm that second individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same2')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same2')]")).Displayed);

            // confirm that third individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[2]/td[1]/span[contains(.,'Same3')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[2]/td[1]/span[contains(.,'Same3')]")).Displayed);
        }


        [And(@"I remove the latest director and shareholder")]
        public void RemoveLatestDirectorShareholder()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // delete the most recent director
            NgWebElement uiRemoveDirector = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[2]/td[7]/i[2]/span"));
            uiRemoveDirector.Click();

            // delete the most recent shareholder
            NgWebElement uiRemoveShareholder = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[2]/td[6]/i[2]/span"));
            uiRemoveShareholder.Click();
        }


        [And(@"I remove the latest director after saving")]
        public void RemoveLatestDirectorAfterSave()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // delete the most recent director
            NgWebElement uiRemoveDirector = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[2]/td[7]/i[2]/span"));
            uiRemoveDirector.Click();
        }


        [And(@"I remove the latest shareholder after saving")]
        public void RemoveLatestShareholderAfterSave()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // delete the most recent shareholder 
            NgWebElement uiRemoveShareholder = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[2]/td[6]/i[2]/span"));
            uiRemoveShareholder.Click();
        }


        [And(@"the latest director and shareholder is removed")]
        public void LatestDirectorShareholderRemoved()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // confirm that the most recent director and shareholder not present
            Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'Same3'))]")).Displayed);
        }


        [And(@"I remove the business shareholder")]
        public void RemoveBusinessShareholder()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // delete the business shareholder
            NgWebElement uiRemoveBizShareholder = ngDriver.FindElement(By.XPath("//app-org-structure/div/div[5]/section[2]/app-associate-list/div/table/tr[1]/td[5]/i[2]/span"));
            uiRemoveBizShareholder.Click();
        }


        [And(@"the business shareholder is removed")]
        public void BusinessShareholderRemoved()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // confirm that the business shareholder not present	
            Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'business@shareholder.com'))]")).Displayed);
        }

   
        [And(@"the saved org structure is present")]
        public void SaveOrgStructurePresent()
        { 
            // TODO
        }


        [And(@"I add in business shareholders of different business types")]
        public void MixedBusinessShareholders()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            /********** Business Shareholder - Public Corporation **********/

            // create public corporation test data
            string businessNamePublicCorp = "Public Corporation";
            string sharesPublicCorp = "10";
            string emailAddressPublicCorp = "public@corporation.com";

            // click on the Add Business Shareholder button
            NgWebElement uiAddPublicCorporationRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[5]/section[2]/app-associate-list/div/button"));
            uiAddPublicCorporationRow.Click();

            // add the public corporation business name
            NgWebElement uiAddPublicCorporationBizName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiAddPublicCorporationBizName.SendKeys(businessNamePublicCorp);

            // add the public corporation number of shares
            NgWebElement uiAddPublicCorporationShares = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiAddPublicCorporationShares.SendKeys(sharesPublicCorp);

            // select the public corporation organization type
            NgWebElement uiAddOrganizationTypePublicCorp = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='PublicCorporation']"));
            uiAddOrganizationTypePublicCorp.Click();

            // add the public corporation email address
            NgWebElement uiAddEmailAddressPublicCorp = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiAddEmailAddressPublicCorp.SendKeys(emailAddressPublicCorp);

            // click on the public corporation Confirm button
            NgWebElement uiConfirmButtonPublicCorp = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiConfirmButtonPublicCorp.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document
            string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[21]"));
            uiUploadSignage.SendKeys(noticeOfArticles);

            // click on the Add Key Personnel button
            NgWebElement uiAddPublicCorporationKeyPersonnel = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] #cdk-accordion-child-1 [changetypesuffix='Leadership'] button"));
            uiAddPublicCorporationKeyPersonnel.Click();

            // create public corp key personnel data
            string firstName = "KeyPersonnelPubCorp";
            string lastName = "Public Corporation";
            string title = "CTO";
            string email = "keypersonnel@pubcorp.com";

            // enter the key personnel first name 
            NgWebElement uiKeyPersonnelFirstName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiKeyPersonnelFirstName.SendKeys(firstName);

            // enter the key personnel last name 
            NgWebElement uiKeyPersonnelLastName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiKeyPersonnelLastName.SendKeys(lastName);

            // click the key personnel checkbox
            NgWebElement uiKeyPersonnelRole = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isOfficerNew']"));
            uiKeyPersonnelRole.Click();

            // enter the key personnel title
            NgWebElement uiKeyPersonnelTitle = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiKeyPersonnelTitle.SendKeys(title);

            // enter the key personnel email 
            NgWebElement uiKeyPersonnelEmail = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiKeyPersonnelEmail.SendKeys(email);

            // select the key personnel DOB
            NgWebElement uiOpenKeyPersonnelDOB = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiOpenKeyPersonnelDOB.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] .fa-save span"));
            uiConfirmButton.Click();

            /********** Business Shareholder - Sole Proprietorship **********/

            // create sole proprietorship test data
            string businessNameSoleProprietorship = "Sole Proprietorship";
            string sharesSoleProprietorship = "10";
            string emailAddressSoleProprietorship = "sole@proprietorship.com";

            // click on the sole proprietorship Add Business Shareholder button
            NgWebElement uiAddSoleProprietorshipRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[5]/section[2]/app-associate-list/div/button"));
            uiAddSoleProprietorshipRow.Click();

            // add the sole proprietorship business name
            NgWebElement uiAddSoleProprietorshipBizName = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiAddSoleProprietorshipBizName.SendKeys(businessNameSoleProprietorship);

            // add the sole proprietorship number of shares
            NgWebElement uiAddSoleProprietorshipShares = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiAddSoleProprietorshipShares.SendKeys(sharesSoleProprietorship);

            // select the sole proprietorship organization type
            NgWebElement uiAddOrganizationTypeSoleProprietorship = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='SoleProprietorship']"));
            uiAddOrganizationTypeSoleProprietorship.Click();

            // add the sole proprietorship email address
            NgWebElement uiAddEmailAddressSoleProprietorship = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiAddEmailAddressSoleProprietorship.SendKeys(emailAddressSoleProprietorship);

            // click on the sole proprietorship Confirm button
            NgWebElement uiConfirmButtonSoleProprietorship = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiConfirmButtonSoleProprietorship.Click();

            // open the sole proprietorship leader row
            NgWebElement uiAddSoleProprietorshipLeaderRow = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'][addlabel='Add Leader'] button.btn-secondary"));
            uiAddSoleProprietorshipLeaderRow.Click();

            // create leader data
            string firstNameLeader = "LeaderSoleProp";
            string lastNameLeader = "LastName";
            string leaderEmail = "leader@soleprop.com";

            // add the leader first name
            NgWebElement uiAddSoleProprietorshipFirstName = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leader'][changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiAddSoleProprietorshipFirstName.SendKeys(firstNameLeader);

            // add the leader last name
            NgWebElement uiAddSoleProprietorshipLastName = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leader'][changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiAddSoleProprietorshipLastName.SendKeys(lastNameLeader);

            // add the leader email
            NgWebElement uiAddSoleProprietorshipEmail = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leader'][changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiAddSoleProprietorshipEmail.SendKeys(leaderEmail);

            // add the leader DOB
            NgWebElement uiAddSoleProprietorshipDOB = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leader'][changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiAddSoleProprietorshipDOB.Click();

            SharedCalendarDate();

            // click on leader confirm button
            NgWebElement uiConfirmSoleProprietorship = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leader'][changetypesuffix='Leadership'] .fa-save span"));
            uiConfirmSoleProprietorship.Click();

            /********** Business Shareholder - Society **********/

            // create society test data
            string businessNameSociety = "Society";
            string sharesSociety = "10";
            string emailAddressSociety = "society@test.com";

            // click on the society Add Business Shareholder button
            NgWebElement uiAddSocietyRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[5]/section[2]/app-associate-list/div/button"));
            uiAddSocietyRow.Click();

            // add the society business name
            NgWebElement uiAddSocietyBizName = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiAddSocietyBizName.SendKeys(businessNameSociety);

            // add the society number of shares
            NgWebElement uiAddSocietyShares = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiAddSocietyShares.SendKeys(sharesSociety);

            // select the society organization type
            NgWebElement uiAddOrganizationTypeSociety = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='Society']"));
            uiAddOrganizationTypeSociety.Click();

            // add the society email address
            NgWebElement uiAddEmailAddressSociety = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiAddEmailAddressSociety.SendKeys(emailAddressSociety);

            // click on the society Confirm button
            NgWebElement uiConfirmButtonSociety = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiConfirmButtonSociety.Click();

            // create society data
            string membershipFee = "1000";
            string memberCount = "100";
            string directorFirstName = "DirectorSociety";
            string directorLastName = "Society";
            string directorTitle = "CFO";
            string directorEmail = "cfo@society.com";

            // add the society's annual membership fee
            NgWebElement uiAddMembershipFee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='annualMembershipFee']"));
            uiAddMembershipFee.SendKeys(membershipFee);

            // add the society's number of members
            NgWebElement uiAddMembershipCount = ngDriver.FindElement(By.CssSelector("input[formcontrolname='numberOfMembers']"));
            uiAddMembershipCount.SendKeys(memberCount);

            // click on the director/officer row
            NgWebElement uiOpenDirectorRow = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] button"));
            uiOpenDirectorRow.Click();

            // add the director first name
            NgWebElement uiAddDirectorFirst = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='firstNameNew']"));
            uiAddDirectorFirst.SendKeys(directorFirstName);

            // add the director last name
            NgWebElement uiAddDirectorLast = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='lastNameNew']"));
            uiAddDirectorLast.SendKeys(directorLastName);

            // select the director position
            NgWebElement uiAddDirectorPosition = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='isOfficerNew']"));
            uiAddDirectorPosition.Click();

            // add the director title
            NgWebElement uiAddDirectorTitle = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='titleNew']"));
            uiAddDirectorTitle.SendKeys(directorTitle);

            // add the director email
            NgWebElement uiAddDirectorEmail = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='emailNew']"));
            uiAddDirectorEmail.SendKeys(directorEmail);

            // add the director DOB
            NgWebElement uiAddDirectorDOB = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='dateofBirthNew']"));
            uiAddDirectorDOB.Click();

            SharedCalendarDate();

            // click on director confirm button
            NgWebElement uiConfirmDirector = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] .fa-save span"));
            uiConfirmDirector.Click();

            /********** Business Shareholder - Trust **********/

            // create trust test data
            string businessNameTrust = "Trust";
            string sharesTrust = "10";
            string emailAddressTrust = "trust@test.com";

            // click on the trust Add Business Shareholder button
            NgWebElement uiAddTrustRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[5]/section[2]/app-associate-list/div/button"));
            uiAddTrustRow.Click();

            // add the trust business name
            NgWebElement uiAddTrustBizName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiAddTrustBizName.SendKeys(businessNameTrust);

            // add the trust number of shares
            NgWebElement uiAddTrustShares = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiAddTrustShares.SendKeys(sharesTrust);

            // select the trust organization type
            NgWebElement uiAddOrganizationTypeTrust = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='Trust']"));
            uiAddOrganizationTypeTrust.Click();

            // add the trust email address
            NgWebElement uiAddEmailAddressTrust = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiAddEmailAddressTrust.SendKeys(emailAddressTrust);

            // click on the trust Confirm button
            NgWebElement uiConfirmButtonTrust = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiConfirmButtonTrust.Click();

            // click on add trustee button
            NgWebElement uiAddTrustee = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] button"));
            uiAddTrustee.Click();

            // create trustee test data
            string firstNameTrustee = "TrusteeTrust";
            string lastNameTrustee = "Trust";
            string emailAddressTrustee = "trustee@test.com";

            // add trustee first name
            NgWebElement uiAddTrusteeFirst = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] input[formcontrolname='firstNameNew']"));
            uiAddTrusteeFirst.SendKeys(firstNameTrustee);

            // add trustee last name
            NgWebElement uiAddTrusteeLast = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] input[formcontrolname='lastNameNew']"));
            uiAddTrusteeLast.SendKeys(lastNameTrustee);

            // add trustee email
            NgWebElement uiAddTrusteeEmail = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] input[formcontrolname='emailNew']"));
            uiAddTrusteeEmail.SendKeys(emailAddressTrustee);

            // add trustee DOB
            NgWebElement uiAddTrusteeDOB = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] input[formcontrolname='dateofBirthNew']"));
            uiAddTrusteeDOB.Click();

            SharedCalendarDate();

            // click on trustee confirm button
            NgWebElement uiConfirmTrustee = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] .fa-save span"));
            uiConfirmTrustee.Click();

            /********** Business Shareholder - Partnership **********/

            // create partnership test data
            string businessNamePartnership = "Partnership";
            string sharesPartnership = "10";
            string emailAddressPartnership = "partnership@test.com";

            // click on the partnership Add Business Shareholder button
            NgWebElement uiAddPartnershipRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[5]/section[2]/app-associate-list/div/button"));
            uiAddPartnershipRow.Click();

            // add the partnership business name
            NgWebElement uiAddPartnershipBizName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiAddPartnershipBizName.SendKeys(businessNamePartnership);

            // add the partnership number of shares
            NgWebElement uiAddPartnershipShares = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiAddPartnershipShares.SendKeys(sharesPartnership);

            // select the partnership organization type
            NgWebElement uiAddOrganizationTypePartnership = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='Partnership']"));
            uiAddOrganizationTypePartnership.Click();

            // add the partnership email address
            NgWebElement uiAddEmailAddressPartnership = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiAddEmailAddressPartnership.SendKeys(emailAddressPartnership);

            // click on the partnership Confirm button
            NgWebElement uiConfirmButtonPartnership = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiConfirmButtonPartnership.Click();

            // click on the individual partner row
            NgWebElement uiAddPartnerRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-5']/div/section/app-org-structure/div/div[3]/section[1]/app-associate-list/div/button"));
            uiAddPartnerRow.Click();

            // create the individual partner data
            string firstNamePartner = "Individual";
            string lastNamePartner = "Partner";
            string percentage = "50";
            string emailPartner = "individual@partner.com";

            // add the individual partner first name
            NgWebElement uiFirstNamePartner = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiFirstNamePartner.SendKeys(firstNamePartner);

            // add the individual partner last name
            NgWebElement uiLastNamePartner = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiLastNamePartner.SendKeys(lastNamePartner);

            // add the individual partner percentage
            NgWebElement uiPartnerPercentage = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] input[formcontrolname='interestPercentageNew']"));
            uiPartnerPercentage.SendKeys(percentage);

            // add the individual partner email address
            NgWebElement uiEmailPartner = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiEmailPartner.SendKeys(emailPartner);

            // add the individual partner DOB
            NgWebElement uiPartnerDOB = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiPartnerDOB.Click();

            SharedCalendarDate();

            // click on the individual partner confirm button
            NgWebElement uiPartnerConfirm = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] .fa-save span"));
            uiPartnerConfirm.Click();

            // upload partnership agreement
            string partnershipAgreement = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "partnership_agreement.pdf");
            NgWebElement uiUploadPartner = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-5']/div/section/app-org-structure/div/div[2]/section/app-file-uploader/div/ngx-file-drop/div/div/input"));
            uiUploadPartner.SendKeys(partnershipAgreement);
        }


        [And(@"I review the mixed business shareholder types security screening requirements")]
        public void SecurityScreeningsMixedBusinessShareholders()
        {
            /* 
            Page Title: Security Screening Requirements
            */

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnel0')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder0')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnel1')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnelPubCorp')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'LeaderSoleProp')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'DirectorSociety')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'TrusteeTrust')]")).Displayed);
        }


        [And(@"the mixed business shareholder org structure is correct")]
        public void ReviewMixedBusinessShareholdersOrgStructure()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            //TODO
        }


        [And(@"I enter business shareholders of different business types to be saved for later")]
        public void SaveForLaterMixedBusinessShareholders()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            /********** Business Shareholder - Public Corporation **********/

            // create public corporation test data
            string businessNamePublicCorp = "Public Corporation";
            string sharesPublicCorp = "10";
            string emailAddressPublicCorp = "public@corporation.com";

            // click on the Add Business Shareholder button
            NgWebElement uiAddPublicCorporationRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[5]/section[2]/app-associate-list/div/button"));
            uiAddPublicCorporationRow.Click();

            // add the public corporation business name
            NgWebElement uiAddPublicCorporationBizName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiAddPublicCorporationBizName.SendKeys(businessNamePublicCorp);

            // add the public corporation number of shares
            NgWebElement uiAddPublicCorporationShares = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiAddPublicCorporationShares.SendKeys(sharesPublicCorp);

            // select the public corporation organization type
            NgWebElement uiAddOrganizationTypePublicCorp = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='PublicCorporation']"));
            uiAddOrganizationTypePublicCorp.Click();

            // add the public corporation email address
            NgWebElement uiAddEmailAddressPublicCorp = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiAddEmailAddressPublicCorp.SendKeys(emailAddressPublicCorp);

            // click on the public corporation Confirm button
            NgWebElement uiConfirmButtonPublicCorp = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiConfirmButtonPublicCorp.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document
            string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[21]"));
            uiUploadSignage.SendKeys(noticeOfArticles);

            // click on the Add Key Personnel button
            NgWebElement uiAddPublicCorporationKeyPersonnel = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] #cdk-accordion-child-1 [changetypesuffix='Leadership'] button"));
            uiAddPublicCorporationKeyPersonnel.Click();

            // create public corp key personnel data
            string firstName = "KeyPersonnelPubCorp";
            string lastName = "Public Corporation";
            string title = "CTO";
            string email = "keypersonnel@pubcorp.com";

            // enter the key personnel first name 
            NgWebElement uiKeyPersonnelFirstName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiKeyPersonnelFirstName.SendKeys(firstName);

            // enter the key personnel last name 
            NgWebElement uiKeyPersonnelLastName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiKeyPersonnelLastName.SendKeys(lastName);

            // click the key personnel checkbox
            NgWebElement uiKeyPersonnelRole = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isOfficerNew']"));
            uiKeyPersonnelRole.Click();

            // enter the key personnel title
            NgWebElement uiKeyPersonnelTitle = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiKeyPersonnelTitle.SendKeys(title);

            // enter the key personnel email 
            NgWebElement uiKeyPersonnelEmail = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiKeyPersonnelEmail.SendKeys(email);

            // select the key personnel DOB
            NgWebElement uiOpenKeyPersonnelDOB = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiOpenKeyPersonnelDOB.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] .fa-save span"));
            uiConfirmButton.Click();
        }


        [And(@"the saved for later mixed business shareholder org structure is correct")]
        public void SaveForLaterMixedBusinessShareholdersCorrectOrgStructure()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // confirm that expected personnel and businesses are present
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnel0')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder0')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Business Shareholder 1')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnel1')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Public Corporation')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnelPubCorp')]")).Displayed);
        }


        [And(@"I confirm that no duplicates are shown in the org structure")]
        public void CheckOrgStructureDuplicates()
        {
            // check that KeyPersonnel0 only displays once
            var keyPerson0Elements = ngDriver.FindElements(By.XPath("//body[contains(.,'keypersonnel0@privatecorp.com')]")).Count;
            Assert.True(keyPerson0Elements == 1);

            // check that IndividualShareholder0 only displays once
            var indyShareholder0Elements = ngDriver.FindElements(By.XPath("//body[contains(.,'individualshareholder0@privatecorp.com')]")).Count;
            Assert.True(indyShareholder0Elements == 1);

            // check that Business Shareholder 1 only displays once
            var bizShareholder1Elements = ngDriver.FindElements(By.XPath("//body[contains(.,'business@shareholder1.com')]")).Count;
            Assert.True(bizShareholder1Elements == 1);

            // check that KeyPersonnel1 only displays once
            var keyPersonnel1Elements = ngDriver.FindElements(By.XPath("//body[contains(.,'keypersonnel1bizshareholder@privatecorp.com')]")).Count;
            Assert.True(keyPersonnel1Elements == 1);

            // check that IndividualShareholder1 only displays once
            var indyShareholder1Elements = ngDriver.FindElements(By.XPath("//body[contains(.,'individualshareholder1bizshareholder@privatecorp.com')]")).Count;
            Assert.True(indyShareholder1Elements == 1);
        }


        [And(@"I request a picnic area endorsement")]
        public void PicnicAreaEndorsement()
        {
            /* 
            Page Title: Licences
            */

            string picnicAreaEndorsement = "Picnic Area Endorsement Application";

            // click on the Picnic Area Endorsement Application link
            NgWebElement uiPicnicAreaEndorsement = ngDriver.FindElement(By.LinkText(picnicAreaEndorsement));
            uiPicnicAreaEndorsement.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Manufacturer Picnic Area Endorsement Application
            */

            // create test data
            string proposedChange = "Description of proposed change(s) such as moving, adding or changing approved picnic area(s)";
            string capacity = "100";

            // enter the description of the proposed change in the text area
            NgWebElement uiProposedChange = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='description1']"));
            uiProposedChange.SendKeys(proposedChange);

            // enter the capacity
            NgWebElement uiCapacity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiCapacity.SendKeys(capacity);

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the site plan
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadSitePlan.SendKeys(sitePlanPath);

            // upload the exterior photos
            string exteriorPhotosPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "exterior_photos.pdf");
            NgWebElement uiUploadExteriorPhotos = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadExteriorPhotos.SendKeys(exteriorPhotosPath);

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgree.Click();

            ClickOnSubmitButton();

            //MakePayment();

            System.Threading.Thread.Sleep(3000);

            // click on Licences tab
            ClickLicencesTab();

            // confirm that 'Picnic Area Endorsement' is displayed
            //Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Picnic Area Endorsement')]")).Displayed);
        }


        [And(@"I request an on-site store endorsement")]
        public void OnSiteStoreEndorsement()
        {
            /* 
            Page Title: Licences
            */

            string onSiteStoreEndorsement = "On-Site Store Endorsement Application";

            // click on the On-Site Store Endorsement Application link
            NgWebElement uiOnSiteStoreEndorsement = ngDriver.FindElement(By.LinkText(onSiteStoreEndorsement));
            uiOnSiteStoreEndorsement.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Manufacturer On-Site Store Endorsement Application
            */

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1"));
            uiZoningCheckbox.Click();
            
            // select 'Yes' for ALR zoning           
            NgWebElement uiYesALRZoning = ngDriver.FindElement(By.CssSelector("[formcontrolname='isAlr'] mat-radio-button#mat-radio-2"));
            uiYesALRZoning.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the floor plan
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadFloorplan.SendKeys(floorplanPath);

            // upload the site plan
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadSitePlan.SendKeys(sitePlanPath);

            // select the owner checkbox
            NgWebElement uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            NgWebElement uiFutureValidInterest = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-4[formcontrolname='willhaveValidInterest']"));
            uiFutureValidInterest.Click();

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            MakePayment();

            System.Threading.Thread.Sleep(3000);
        }


        [And(@"I request a lounge area endorsement")]
        public void LoungeAreaEndorsement()
        {
            /* 
            Page Title: Licences
            */

            string loungeAreaEndorsement = "Lounge Area Endorsement Application";

            // click on the Lounge Area Endorsement Application link
            NgWebElement uiLoungeAreaEndorsement = ngDriver.FindElement(By.LinkText(loungeAreaEndorsement));
            uiLoungeAreaEndorsement.Click();

            ContinueToApplicationButton();

            /* 
            Page Title: Lounge Area Endorsement Application
            */

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1"));
            uiZoningCheckbox.Click();

            // select 'Yes' for ALR zoning           
            NgWebElement uiYesALRZoning = ngDriver.FindElement(By.CssSelector("[formcontrolname='isAlr'] mat-radio-button#mat-radio-2"));
            uiYesALRZoning.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the floor plan
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadFloorplan.SendKeys(floorplanPath);

            // add a service area
            NgWebElement uiServiceArea = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceAreas'] button"));
            uiServiceArea.Click();

            // creeate test data
            string areaDescription = "Area description";
            string occupantLoad = "100";

            // enter the area description
            NgWebElement uiAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
            uiAreaDescription.SendKeys(areaDescription);

            // enter the occupant load
            NgWebElement uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiOccupantLoad.SendKeys(occupantLoad);

            // upload the site plan
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadSitePlan.SendKeys(sitePlanPath);

            // select the Sunday opening time
            NgWebElement uiSundayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSundayOpen'] option[value='10:00']"));
            uiSundayOpen.Click();

            // select the Sunday closing time
            NgWebElement uiSundayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSundayClose'] option[value='16:00']"));
            uiSundayClose.Click();

            // select the Monday opening time
            NgWebElement uiMondayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursMondayOpen'] option[value='09:00']"));
            uiMondayOpen.Click();

            // select the Monday closing time
            NgWebElement uiMondayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursMondayClose'] option[value='23:00']"));
            uiMondayClose.Click();

            // select the Tuesday opening time
            NgWebElement uiTuesdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursTuesdayOpen'] option[value='09:15']"));
            uiTuesdayOpen.Click();

            // select the Tuesday closing time
            NgWebElement uiTuesdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursTuesdayClose'] option[value='22:45']"));
            uiTuesdayClose.Click();

            // select the Wednesday opening time
            NgWebElement uiWednesdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursWednesdayOpen'] option[value='09:30']"));
            uiWednesdayOpen.Click();

            // select the Wednesday closing time
            NgWebElement uiWednesdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursWednesdayClose'] option[value='12:00']"));
            uiWednesdayClose.Click();

            // select the Thursday opening time
            NgWebElement uiThursdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursThursdayOpen'] option[value='13:00']"));
            uiThursdayOpen.Click();

            // select the Thursday closing time
            NgWebElement uiThursdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursThursdayClose'] option[value='14:00']"));
            uiThursdayClose.Click();

            // select the Friday opening time
            NgWebElement uiFridayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursFridayOpen'] option[value='12:15']"));
            uiFridayOpen.Click();

            // select the Friday closing time
            NgWebElement uiFridayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursFridayClose'] option[value='21:15']"));
            uiFridayClose.Click();

            // select the Saturday opening time
            NgWebElement uiSaturdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSaturdayOpen'] option[value='10:00']"));
            uiSaturdayOpen.Click();

            // select the Saturday closing time
            NgWebElement uiSaturdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSaturdayClose'] option[value='22:00']"));
            uiSaturdayClose.Click();

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Pending external review')]")).Displayed);

            System.Threading.Thread.Sleep(3000);
        }


        [And(@"I request a facility structural change")]
        public void FacilityStructuralChange()
        {
            /* 
            Page Title: Licences
            */

            string facilityStructuralChange = "Facility Structural Change Application";

            // click on the Facility Structural Change Application link
            NgWebElement uiFacilityStructuralChange = ngDriver.FindElement(By.LinkText(facilityStructuralChange));
            uiFacilityStructuralChange.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            ContinueToApplicationButton();

            /* 
            Page Title: Manufacturing Facility Structural Change Application
            */

            // create test data
            string patioPerimeter = "Sample height and composition of the patio perimeter";
            string patioLocation = "Sample location of the patio";
            string patronAccess = "Sample description of how patrons will access the patio.";
            string carryLiquor = "Sample description of how servers have to carry liquor through any unlicensed area to get to the patio.";
            string patioManagement = "Sample description of how staff will manage and control the patio from the interior service area.";

            // enter the patio perimeter info into the text area
            NgWebElement uiPatioPerimeter = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioPerimeter.SendKeys(patioPerimeter);

            // enter the patio location info into the text area
            NgWebElement uiPatioLocation = ngDriver.FindElement(By.CssSelector("textarea#patioLocationDescription"));
            uiPatioLocation.SendKeys(patioLocation);

            // enter the patron access info into the text area
            NgWebElement uiPatronAccess = ngDriver.FindElement(By.CssSelector("textarea#patioAccessDescription"));
            uiPatronAccess.SendKeys(patronAccess);

            // select the carry liquor checkbox
            NgWebElement uiCarryLiquorCheckbox = ngDriver.FindElement(By.CssSelector("input#patioIsLiquorCarried[type='checkbox']"));
            uiCarryLiquorCheckbox.Click();

            // enter the carry liquor description into the text area
            NgWebElement uiCarryLiquorTextArea = ngDriver.FindElement(By.CssSelector("textarea#patioLiquorCarriedDescription"));
            uiCarryLiquorTextArea.SendKeys(carryLiquor);

            // enter the patron management description into the text area
            NgWebElement uiPatioMgmt = ngDriver.FindElement(By.CssSelector("textarea#patioAccessControlDescription"));
            uiPatioMgmt.SendKeys(patioManagement);

            // select 'Grass' for patio location
            NgWebElement uiGrass = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-67-button .mat-button-toggle-label-content"));
            uiGrass.Click();
            
            // select 'Earth' for patio location
            NgWebElement uiEarth = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-68-button .mat-button-toggle-label-content"));
            uiEarth.Click();
            
            // select 'Gravel' for patio location
            NgWebElement uiGravel = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-69-button .mat-button-toggle-label-content"));
            uiGravel.Click();
            
            // select 'Finished Flooring' for patio location
            NgWebElement uiFinishedFlooring = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-70-button .mat-button-toggle-label-content"));
            uiFinishedFlooring.Click();

            // select 'Cement Sidewalk' for patio location
            NgWebElement uiCementSidewalk = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-71-button .mat-button-toggle-label-content"));
            uiCementSidewalk.Click();
            
            // select 'Other' for patio location
            NgWebElement uiOther = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-72-button .mat-button-toggle-label-content"));
            uiOther.Click();
            
            // select 'Fixed Patio' for bar
            NgWebElement uiFixedPatio = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-74-button .mat-button-toggle-label-content"));
            uiFixedPatio.Click();
            
            // select 'Portable' for bar
            NgWebElement uiPortable = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-75-button .mat-button-toggle-label-content"));
            uiPortable.Click();
            
            // select 'Interior' for bar
            NgWebElement uiInterior = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-76-button .mat-button-toggle-label-content"));
            uiInterior.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the floor plan
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadFloorplan.SendKeys(floorplanPath);

            // upload the site plan
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadSitePlan.SendKeys(sitePlanPath);

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            MakePayment();

            System.Threading.Thread.Sleep(3000);
        }


        [And(@"I request a location change")]
        public void LocationChange()
        {
            /* 
            Page Title: Licences
            */

            string locationChange = "Location Change Application";

            // click on the Location Change Application link
            NgWebElement uiLocationChange = ngDriver.FindElement(By.LinkText(locationChange));
            uiLocationChange.Click();

            ContinueToApplicationButton();

            // create test data
            string additionalPIDs = "012345678, 343434344";
            string proposedChanges = "Details of proposed changes.";

            // enter additional PIDs
            NgWebElement uiAdditionalPIDs = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='pidList']"));
            uiAdditionalPIDs.SendKeys(additionalPIDs);

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1"));
            uiZoningCheckbox.Click();

            // select 'Yes' for ALR zoning           
            NgWebElement uiYesALRZoning = ngDriver.FindElement(By.CssSelector("[formcontrolname='isAlr'] mat-radio-button#mat-radio-2"));
            uiYesALRZoning.Click();

            // enter the proposed changes
            NgWebElement uiProposedChanges = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='description1']"));
            uiProposedChanges.SendKeys(proposedChanges);

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the signage document
            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadSignage.SendKeys(signagePath);

            // upload the floor plan
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadFloorplan.SendKeys(floorplanPath);

            // upload the site plan
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uiUploadSitePlan.SendKeys(sitePlanPath);

            // upload the exterior photos
            string exteriorPhotosPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "exterior_photos.pdf");
            NgWebElement uiUploadExteriorPhotos = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
            uiUploadExteriorPhotos.SendKeys(exteriorPhotosPath);

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            MakePayment();

            System.Threading.Thread.Sleep(3000);
        }


        [And(@"I request a special event area endorsement")]
        public void SpecialEventAreaEndorsement()
        {
            /* 
            Page Title: Licences
            */

            string specialEventAreaEndorsement = "Special Event Area Endorsement Application";

            // click on the Special Event Area Endorsement Application link
            NgWebElement uiSpecialEventAreaEndorsement = ngDriver.FindElement(By.LinkText(specialEventAreaEndorsement));
            uiSpecialEventAreaEndorsement.Click();

            ContinueToApplicationButton();

            /* 
            Page Title: Special Event Area Endorsement Application
            */

            // creeate test data
            string patioCompositionDescription = "Patio composition description";
            string patioLocationDescription = "Patio location description";
            string patioAccessDescription = "Patio access description";
            string patioLiquorCarriedDescription = "Patio liquor carried description";
            string patioAccessControlDescription = "Patio access control description";
            string serviceAreaDescription = "Service area description";
            string serviceAreaOccupantLoad = "100";
            string outdoorAreaDescription = "Outdoor area description";
            string outdoorAreaCapacity = "150";
            string contactTitle = "Tester";

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1"));
            uiZoningCheckbox.Click();

            // select 'Yes' for ALR zoning           
            NgWebElement uiYesALRZoning = ngDriver.FindElement(By.CssSelector("[formcontrolname='isAlr'] mat-radio-button#mat-radio-2"));
            uiYesALRZoning.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the letter of intent
            string letterOfIntentPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "letter_of_intent.pdf");
            NgWebElement uiUploadLetterOfIntent = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadLetterOfIntent.SendKeys(letterOfIntentPath);

            // enter the patio composition description
            // Provide height and composition of the patio perimeter or bounding that is designed to control patron entry/exit. (i.e., railing, fencing, planters, hedging, etc.)
            NgWebElement uiPatioCompositionDescription = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioCompositionDescription.SendKeys(patioCompositionDescription);

            // enter the patio location description
            // Describe the location of the patio in relationship to the interior service area.
            NgWebElement uiPatioLocationDescription = ngDriver.FindElement(By.CssSelector("textarea#patioLocationDescription"));
            uiPatioLocationDescription.SendKeys(patioLocationDescription);

            // enter the patio access description
            // Describe how patrons will access the patio (ie. from interior).
            NgWebElement uiPatioAccessDescription = ngDriver.FindElement(By.CssSelector("textarea#patioAccessDescription"));
            uiPatioAccessDescription.SendKeys(patioAccessDescription);

            // select the patio is liquor carried checkbox
            // Servers have to carry liquor through any unlicensed area to get to the patio
            NgWebElement uiPatioLiquorCarried = ngDriver.FindElement(By.CssSelector("input#patioIsLiquorCarried"));
            uiPatioLiquorCarried.Click();

            // enter the patio liquor carried description
            // If checked, please explain:
            NgWebElement uiPatioLiquorCarriedDescription = ngDriver.FindElement(By.CssSelector("textarea#patioLiquorCarriedDescription"));
            uiPatioLiquorCarriedDescription.SendKeys(patioLiquorCarriedDescription);

            // enter the patio access control description
            // Describe how staff will manage and control the patio from the interior service area.
            NgWebElement uiPatioAccessControlDescription = ngDriver.FindElement(By.CssSelector("textarea#patioAccessControlDescription"));
            uiPatioAccessControlDescription.SendKeys(patioAccessControlDescription);

            // select 'Grass' for patio location
            NgWebElement uiGrass = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-67-button .mat-button-toggle-label-content"));
            uiGrass.Click();

            // select 'Earth' for patio location
            NgWebElement uiEarth = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-68-button .mat-button-toggle-label-content"));
            uiEarth.Click();

            // select 'Gravel' for patio location
            NgWebElement uiGravel = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-69-button .mat-button-toggle-label-content"));
            uiGravel.Click();

            // select 'Finished Flooring' for patio location
            NgWebElement uiFinishedFlooring = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-70-button .mat-button-toggle-label-content"));
            uiFinishedFlooring.Click();

            // select 'Cement Sidewalk' for patio location
            NgWebElement uiCementSidewalk = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-71-button .mat-button-toggle-label-content"));
            uiCementSidewalk.Click();

            // select 'Other' for patio location
            NgWebElement uiOther = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-72-button .mat-button-toggle-label-content"));
            uiOther.Click();

            // select 'Fixed Patio' for bar
            NgWebElement uiFixedPatio = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-74-button .mat-button-toggle-label-content"));
            uiFixedPatio.Click();

            // select 'Portable' for bar
            NgWebElement uiPortable = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-75-button .mat-button-toggle-label-content"));
            uiPortable.Click();

            // select 'Interior' for bar
            NgWebElement uiInterior = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-76-button .mat-button-toggle-label-content"));
            uiInterior.Click();

            // upload the floor plan
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadFloorplan.SendKeys(floorplanPath);

            // add a service area
            NgWebElement uiServiceArea = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceAreas'] button"));
            uiServiceArea.Click();

            // enter the service area description
            NgWebElement uiServiceAreaDescription = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceAreas'] input[formcontrolname='areaLocation']"));
            uiServiceAreaDescription.SendKeys(serviceAreaDescription);

            // enter the service area occupant load
            NgWebElement uiServiceAreaOccupantLoad = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceAreas'] input[formcontrolname='capacity']"));
            uiServiceAreaOccupantLoad.SendKeys(serviceAreaOccupantLoad);

            // add outside area
            NgWebElement uiOutdoorArea = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] button"));
            uiOutdoorArea.Click();

            // enter the outdooor area description
            NgWebElement uiOutdoorAreaDescription = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] input[formcontrolname='areaLocation']"));
            uiOutdoorAreaDescription.SendKeys(outdoorAreaDescription);

            // enter the outdoor area occupant load
            NgWebElement uiOutdoorAreaOccupantLoad = ngDriver.FindElement(By.CssSelector("[formcontrolname='outsideAreas'] input[formcontrolname='capacity']"));
            uiOutdoorAreaOccupantLoad.SendKeys(outdoorAreaCapacity);

            // upload the site plan
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uiUploadSitePlan.SendKeys(sitePlanPath);

            // select the Sunday opening time
            NgWebElement uiSundayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSundayOpen'] option[value='10:00']"));
            uiSundayOpen.Click();

            // select the Sunday closing time
            NgWebElement uiSundayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSundayClose'] option[value='16:00']"));
            uiSundayClose.Click();

            // select the Monday opening time
            NgWebElement uiMondayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursMondayOpen'] option[value='09:00']"));
            uiMondayOpen.Click();
            
            // select the Monday closing time
            NgWebElement uiMondayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursMondayClose'] option[value='23:00']"));
            uiMondayClose.Click();
            
            // select the Tuesday opening time
            NgWebElement uiTuesdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursTuesdayOpen'] option[value='09:15']"));
            uiTuesdayOpen.Click();
            
            // select the Tuesday closing time
            NgWebElement uiTuesdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursTuesdayClose'] option[value='22:45']"));
            uiTuesdayClose.Click();
            
            // select the Wednesday opening time
            NgWebElement uiWednesdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursWednesdayOpen'] option[value='09:30']"));
            uiWednesdayOpen.Click();
            
            // select the Wednesday closing time
            NgWebElement uiWednesdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursWednesdayClose'] option[value='12:00']"));
            uiWednesdayClose.Click();
            
            // select the Thursday opening time
            NgWebElement uiThursdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursThursdayOpen'] option[value='13:00']"));
            uiThursdayOpen.Click();
            
            // select the Thursday closing time
            NgWebElement uiThursdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursThursdayClose'] option[value='14:00']"));
            uiThursdayClose.Click();
            
            // select the Friday opening time
            NgWebElement uiFridayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursFridayOpen'] option[value='12:15']"));
            uiFridayOpen.Click();
            
            // select the Friday closing time
            NgWebElement uiFridayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursFridayClose'] option[value='21:15']"));
            uiFridayClose.Click();
            
            // select the Saturday opening time
            NgWebElement uiSaturdayOpen = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSaturdayOpen'] option[value='10:00']"));
            uiSaturdayOpen.Click();
            
            // select the Saturday closing time
            NgWebElement uiSaturdayClose = ngDriver.FindElement(By.CssSelector("select[formcontrolname='serviceHoursSaturdayClose'] option[value='22:00']"));
            uiSaturdayClose.Click();
            
            // enter the contact title
            NgWebElement uiContactTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonRole']"));
            uiContactTitle.SendKeys(contactTitle);

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton();

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Pending external review')]")).Displayed);

            System.Threading.Thread.Sleep(3000);
        }


        [And(@"I add in multiple nested business shareholders")]
        public void AddMultipleBusinessShareholders()
        {
            // add in an additional four nested business shareholders
            BusinessShareholder2();
            BusinessShareholder3();
            BusinessShareholder4();
            BusinessShareholder5();
        }

        public void BusinessShareholder2()
        {
            /********** Business Shareholder #2 **********/

            // create the business shareholder #2 data
            string businessName2 = "Business Shareholder 2";
            string businessVotingShares2 = "100";
            string businessEmail2 = "businessshareholder2@email.com";

            // open business shareholder #2 form
            NgWebElement uiOpenShareBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] button"));
            uiOpenShareBiz2.Click();

            // enter business sharedholder #2 name
            NgWebElement uiShareFirstBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiShareFirstBiz2.SendKeys(businessName2);

            // enter business shareholder #2 voting shares
            NgWebElement uiShareVotesBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiShareVotesBiz2.SendKeys(businessVotingShares2);

            // select business shareholder #2 business type (private corporation) from dropdown
            NgWebElement uiShareBizType2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='PrivateCorporation']"));
            uiShareBizType2.Click();

            // enter business shareholder #2 email
            NgWebElement uiShareEmailBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiShareEmailBiz2.SendKeys(businessEmail2);

            // select the business shareholder #2 confirm button
            NgWebElement uiShareBizConfirmButton2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiShareBizConfirmButton2.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadNoticeofArticlesBiz2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[21]"));
            uiUploadNoticeofArticlesBiz2.SendKeys(noticeOfArticlesBiz2);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uiUploadCentralSecRegBiz2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[24]"));
            uiUploadCentralSecRegBiz2.SendKeys(centralSecuritiesRegisterBiz2);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uiUploadSpecialRightsResBiz2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[27]"));
            uiUploadSpecialRightsResBiz2.SendKeys(specialRightsRestrictionsBiz2);

            /********** Business Shareholder #2 - Leader **********/

            // create business shareholder #2 key personnel data
            string leaderFirstNameBiz2 = "LeaderBiz2First";
            string leaderLastNameBiz2 = "LeaderBiz2Last";
            string leaderTitleBiz2 = "LeaderBiz2Title";
            string leaderEmailBiz2 = "leader@biz2.com";

            // open business shareholder #2 > leader form 
            NgWebElement uiOpenLeaderFormBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] button"));
            uiOpenLeaderFormBiz2.Click();

            // enter business shareholder #2 > leader first name
            NgWebElement uiLeaderFirstBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstBiz2.SendKeys(leaderFirstNameBiz2);

            // enter business shareholder #2 > leader last name
            NgWebElement uiLeaderLastBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiLeaderLastBiz2.SendKeys(leaderLastNameBiz2);

            // select business shareholder #2 > leader role using checkbox
            NgWebElement uiLeaderRoleBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiLeaderRoleBiz2.Click();

            // enter business shareholder #2 > leader title
            NgWebElement uiLeaderTitleBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiLeaderTitleBiz2.SendKeys(leaderTitleBiz2);

            // enter business shareholder #2 > leader email
            NgWebElement uiLeaderEmailBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiLeaderEmailBiz2.SendKeys(leaderEmailBiz2);

            // enter business shareholder #2 > leader DOB
            NgWebElement uiLeaderDOB1Biz12 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiLeaderDOB1Biz12.Click();

            SharedCalendarDate();

            /********** Business Shareholder #2 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz2 = "IndividualShareholderBiz2First";
            string shareholderLastNameBiz2 = "IndividualShareholderBiz2Last";
            string shareholderVotingSharesBiz2 = "1800";
            string shareholderEmailBiz2 = "individualshareholder@biz2.com";

            // open business shareholder #2 > individual shareholder form
            NgWebElement uiOpenIndyShareBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
            uiOpenIndyShareBiz2.Click();

            // enter business shareholder #2 > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz2.SendKeys(shareholderFirstNameBiz2);

            // enter business shareholder #2 > individual shareholder last name
            NgWebElement uiIndyShareLastBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz2.SendKeys(shareholderLastNameBiz2);

            // enter business shareholder #2 > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz2.SendKeys(shareholderVotingSharesBiz2);

            // enter business shareholder > individual shareholder email
            NgWebElement uiIndyShareEmailBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz2.SendKeys(shareholderEmailBiz2);

            // enter business shareholder > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz2.Click();

            SharedCalendarDate();
        }

        public void BusinessShareholder3()
        {
            /********** Business Shareholder #3 **********/

            // create the business shareholder data
            string businessName3 = "Business Shareholder 3";
            string businessVotingShares3 = "3";
            string businessEmail3 = "businessshareholder3@email.com";

            // open business shareholder #3 form
            NgWebElement uiOpenShareBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] button"));
            uiOpenShareBiz3.Click();

            // enter business shareholder #3 business name
            NgWebElement uiShareFirstBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiShareFirstBiz3.SendKeys(businessName3);

            // enter business shareholder #3 voting shares
            NgWebElement uiShareVotesBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiShareVotesBiz3.SendKeys(businessVotingShares3);

            // select the business shareholder #3 business type using dropdown
            NgWebElement uiShareBizType3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='PrivateCorporation']"));
            uiShareBizType3.Click();

            // enter business shareholder #3 email
            NgWebElement uiShareEmailBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiShareEmailBiz3.SendKeys(businessEmail3);

            // select the business shareholder #3 confirm button
            NgWebElement uiShareBizConfirmButton3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiShareBizConfirmButton3.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz3 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadNoticeofArticlesBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[30]"));
            uiUploadNoticeofArticlesBiz3.SendKeys(noticeOfArticlesBiz3);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz3 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uiUploadCentralSecRegBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[33]"));
            uiUploadCentralSecRegBiz3.SendKeys(centralSecuritiesRegisterBiz3);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz3 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uiUploadSpecialRightsResBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[36]"));
            uiUploadSpecialRightsResBiz3.SendKeys(specialRightsRestrictionsBiz3);

            /********** Business Shareholder #3 - Leader **********/

            // create business shareholder leader data
            string leaderFirstNameBiz3 = "LeaderBiz3First";
            string leaderLastNameBiz3 = "LeaderBiz3Last";
            string leaderTitleBiz3 = "LeaderBiz3Title";
            string leaderEmailBiz3 = "leader@biz3.com";

            // open business shareholder #3 > leader form
            NgWebElement uiOpenLeaderFormBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] button"));
            uiOpenLeaderFormBiz3.Click();

            // enter business shareholder #3 > leader first name
            NgWebElement uiLeaderFirstBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstBiz3.SendKeys(leaderFirstNameBiz3);

            // enter business shareholder #3 > leader last name
            NgWebElement uiLeaderLastBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiLeaderLastBiz3.SendKeys(leaderLastNameBiz3);

            // select business shareholder #3 > leader role using checkbox
            NgWebElement uiLeaderRoleBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiLeaderRoleBiz3.Click();

            // enter business shareholder #3 > leader title
            NgWebElement uiLeaderTitleBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiLeaderTitleBiz3.SendKeys(leaderTitleBiz3);

            // enter business shareholder #3 > leader email
            NgWebElement uiLeaderEmailBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiLeaderEmailBiz3.SendKeys(leaderEmailBiz3);

            // enter business shareholder #3 > leader DOB
            NgWebElement uiLeaderDOB1Biz13 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiLeaderDOB1Biz13.Click();

            SharedCalendarDate();

            /********** Business Shareholder #3 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz3 = "IndividualShareholderBiz3First";
            string shareholderLastNameBiz3 = "IndividualShareholderBiz3Last";
            string shareholderVotingSharesBiz3 = "1000";
            string shareholderEmailBiz3 = "individualshareholder@biz3.com";

            // open business shareholder #3 > individual shareholder form
            NgWebElement uiOpenIndyShareBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
            uiOpenIndyShareBiz3.Click();

            // enter business shareholder #3 > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz3.SendKeys(shareholderFirstNameBiz3);

            // enter business shareholder #3 > individual shareholder last name
            NgWebElement uiIndyShareLastBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz3.SendKeys(shareholderLastNameBiz3);

            // enter business shareholder #3 > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz3.SendKeys(shareholderVotingSharesBiz3);

            // enter business shareholder #3 > individual shareholder email
            NgWebElement uiIndyShareEmailBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz3.SendKeys(shareholderEmailBiz3);

            // enter business shareholder #3 > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz3.Click();

            SharedCalendarDate();
        }

        public void BusinessShareholder4()
        {
            /********** Business Shareholder #4 **********/

            // create the business shareholder data
            string businessName4 = "Business Shareholder 4";
            string businessVotingShares4 = "2";
            string businessEmail4 = "businessshareholder4@email.com";

            // open business shareholder #4 form
            NgWebElement uiOpenShareBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] button"));
            uiOpenShareBiz4.Click();

            // enter business shareholder #4 business name
            NgWebElement uiShareFirstBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiShareFirstBiz4.SendKeys(businessName4);

            // enter business shareholder #4 voting shares
            NgWebElement uiShareVotesBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiShareVotesBiz4.SendKeys(businessVotingShares4);

            // select the business shareholder #4 business type using dropdown
            NgWebElement uiShareBizType4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='PrivateCorporation']"));
            uiShareBizType4.Click();

            // enter business shareholder #4 email
            NgWebElement uiShareEmailBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiShareEmailBiz4.SendKeys(businessEmail4);

            // select the business shareholder #4 confirm button
            NgWebElement uiShareBizConfirmButton4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiShareBizConfirmButton4.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz4 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadNoticeofArticlesBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[39]"));
            uiUploadNoticeofArticlesBiz4.SendKeys(noticeOfArticlesBiz4);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz4 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uiUploadCentralSecRegBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[42]"));
            uiUploadCentralSecRegBiz4.SendKeys(centralSecuritiesRegisterBiz4);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz4 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uiUploadSpecialRightsResBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[45]"));
            uiUploadSpecialRightsResBiz4.SendKeys(specialRightsRestrictionsBiz4);

            /********** Business Shareholder #4 - Leader **********/

            // create business shareholder leader data
            string leaderFirstNameBiz4 = "LeaderBiz4First";
            string leaderLastNameBiz4 = "LeaderBiz4Last";
            string leaderTitleBiz4 = "LeaderBiz4Title";
            string leaderEmailBiz4 = "leader@biz4.com";

            // open business shareholder #4 > leader form
            NgWebElement uiOpenLeaderFormBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] button"));
            uiOpenLeaderFormBiz4.Click();

            // enter business shareholder #4 > leader first name
            NgWebElement uiLeaderFirstBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstBiz4.SendKeys(leaderFirstNameBiz4);

            // enter business shareholder #4 > leader last name
            NgWebElement uiLeaderLastBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiLeaderLastBiz4.SendKeys(leaderLastNameBiz4);

            // select business shareholder #4 > leader role using checkbox
            NgWebElement uiLeaderRoleBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiLeaderRoleBiz4.Click();

            // enter business shareholder #4 > leader title
            NgWebElement uiLeaderTitleBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiLeaderTitleBiz4.SendKeys(leaderTitleBiz4);

            // enter business shareholder #4 > leader email
            NgWebElement uiLeaderEmailBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiLeaderEmailBiz4.SendKeys(leaderEmailBiz4);

            // enter business shareholder #4 > leader DOB
            NgWebElement uiLeaderDOB1Biz14 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiLeaderDOB1Biz14.Click();

            SharedCalendarDate();

            /********** Business Shareholder #4 - Individual Shareholder **********/

            // create the business shareholder #4 > individual shareholder data
            string shareholderFirstNameBiz4 = "IndividualShareholderBiz4First";
            string shareholderLastNameBiz4 = "IndividualShareholderBiz4Last";
            string shareholderVotingSharesBiz4 = "1";
            string shareholderEmailBiz4 = "individualshareholder@biz4.com";

            // open business shareholder #4 > individual shareholder form
            NgWebElement uiOpenIndyShareBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
            uiOpenIndyShareBiz4.Click();

            // enter business shareholder #4 > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz4.SendKeys(shareholderFirstNameBiz4);

            // enter business shareholder #4 > individual shareholder last name
            NgWebElement uiIndyShareLastBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz4.SendKeys(shareholderLastNameBiz4);

            // enter business shareholder #4 > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz4.SendKeys(shareholderVotingSharesBiz4);

            // enter business shareholder #4 > individual shareholder email
            NgWebElement uiIndyShareEmailBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz4.SendKeys(shareholderEmailBiz4);

            // enter business shareholder #4 > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz4.Click();

            SharedCalendarDate();
        }

        public void BusinessShareholder5()
        {
            /********** Business Shareholder #5 **********/

            // create the business shareholder data
            string businessName5 = "Business Shareholder 5";
            string businessVotingShares5 = "1";
            string businessEmail5 = "businessshareholder5@email.com";

            // open business shareholder #5 form
            NgWebElement uiOpenShareBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] button"));
            uiOpenShareBiz5.Click();

            // enter business shareholder #5 business name
            NgWebElement uiShareFirstBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiShareFirstBiz5.SendKeys(businessName5);

            // enter business shareholder #5  voting shares
            NgWebElement uiShareVotesBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiShareVotesBiz5.SendKeys(businessVotingShares5);

            // select business shareholder #5 business type using dropdown
            NgWebElement uiShareBizType5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='PrivateCorporation']"));
            uiShareBizType5.Click();

            // enter business shareholder #5 email
            NgWebElement uiShareEmailBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiShareEmailBiz5.SendKeys(businessEmail5);

            // select the business shareholder #5 confirm button
            NgWebElement uiShareBizConfirmButton5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiShareBizConfirmButton5.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz5 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadNoticeofArticlesBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[48]"));
            uiUploadNoticeofArticlesBiz5.SendKeys(noticeOfArticlesBiz5);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz5 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uiUploadCentralSecRegBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[51]"));
            uiUploadCentralSecRegBiz5.SendKeys(centralSecuritiesRegisterBiz5);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz5 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uiUploadSpecialRightsResBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[54]"));
            uiUploadSpecialRightsResBiz5.SendKeys(specialRightsRestrictionsBiz5);

            /********** Business Shareholder #5 - Leader **********/

            // create business shareholder #5 leader data
            string leaderFirstNameBiz5 = "LeaderBiz5First";
            string leaderLastNameBiz5 = "LeaderBiz5Last";
            string leaderTitleBiz5 = "LeaderBiz5Title";
            string leaderEmailBiz5 = "leader@biz5.com";

            // open business shareholder #5 > leader form
            NgWebElement uiOpenLeaderFormBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] button"));
            uiOpenLeaderFormBiz5.Click();

            // enter business shareholder #5 > leader first name
            NgWebElement uiLeaderFirstBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstBiz5.SendKeys(leaderFirstNameBiz5);

            // enter business shareholder #5 > leader last name
            NgWebElement uiLeaderLastBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiLeaderLastBiz5.SendKeys(leaderLastNameBiz5);

            // select business shareholder #5 > leader role using checkbox
            NgWebElement uiLeaderRoleBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiLeaderRoleBiz5.Click();

            // enter business shareholder #5 > leader title
            NgWebElement uiLeaderTitleBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiLeaderTitleBiz5.SendKeys(leaderTitleBiz5);

            // enter business shareholder #5 > leader email
            NgWebElement uiLeaderEmailBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiLeaderEmailBiz5.SendKeys(leaderEmailBiz5);

            // enter business shareholder #5 > leader DOB
            NgWebElement uiLeaderDOB1Biz15 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiLeaderDOB1Biz15.Click();

            SharedCalendarDate();

            /********** Business Shareholder #5 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz5 = "IndividualShareholderBiz5First";
            string shareholderLastNameBiz5 = "IndividualShareholderBiz5Last";
            string shareholderVotingSharesBiz5 = "1";
            string shareholderEmailBiz5 = "individualshareholder@biz5.com";

            // open business shareholder #5 > individual shareholder form
            NgWebElement uiOpenIndyShareBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
            uiOpenIndyShareBiz5.Click();

            // enter business shareholder #5 > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz5.SendKeys(shareholderFirstNameBiz5);

            // enter business shareholder #5 > individual shareholder last name
            NgWebElement uiIndyShareLastBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz5.SendKeys(shareholderLastNameBiz5);

            // enter business shareholder #5 > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz5.SendKeys(shareholderVotingSharesBiz5);

            // enter business shareholder #5 > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz5.Click();

            SharedCalendarDate();

            // enter business shareholder #5 > individual shareholder email
            NgWebElement uiIndyShareEmailBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz5.SendKeys(shareholderEmailBiz5);
        }
    }
}
