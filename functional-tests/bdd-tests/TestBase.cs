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

namespace bdd_tests
{
    public abstract class TestBase : Feature, IDisposable
    {       
        // Protractor driver
        protected NgWebDriver ngDriver;

        protected IConfigurationRoot configuration;

        protected string baseUri;

        protected string businessTypeShared;

        protected string applicationTypeShared;

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

        public void CarlaHome()
        { 
            ngDriver.Navigate().GoToUrl($"{baseUri}");
            ngDriver.WaitForAngular();
        }


        [And(@"I click on Home page")]
        public void ClickOnHomePage()
        {
            CarlaHome();
        }

        private void DoLogin(string businessType)
        {
            ngDriver.Navigate().GoToUrl($"{baseUri}dashboard");

            /* 
            Page Title: Terms of Use
            */

            // select the acceptance checkbox
            NgWebElement termsOfUseCheckbox = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            termsOfUseCheckbox.Click();

            // click on the Continue button
            NgWebElement continueButton = ngDriver.FindElement(By.XPath("//section[3]/button"));
            continueButton.Click();

            /* 
            Page Title: Please confirm the business or organization name associated to the Business BCeID.
            */

            // click on the Yes button
            NgWebElement confirmationButton = ngDriver.FindElement(By.XPath("//button"));
            confirmationButton.Click();

            /* 
            Page Title: Please confirm the organization type associated with the Business BCeID:
            */

            // if this is a private corporation, click the radio button
            if (businessType == "private corporation")
            {
                NgWebElement privateCorporationRadio = ngDriver.FindElement(By.XPath("(//input[@value='PrivateCorporation'])"));
                privateCorporationRadio.Click();
            }

            // if this is a public corporation, click the radio button
            if (businessType == "public corporation")
            {
                NgWebElement publicCorporationRadio = ngDriver.FindElement(By.XPath("(//input[@value='PublicCorporation'])"));
                publicCorporationRadio.Click();
            }

            // if this is a sole proprietorship, click the radio button
            if (businessType == "sole proprietorship")
            {
                NgWebElement soleProprietorshipRadio = ngDriver.FindElement(By.XPath("(//input[@value='SoleProprietor'])"));
                soleProprietorshipRadio.Click();
            }

            // if this is a partnership, click the radio button
            if (businessType == "partnership")
            {
                NgWebElement partnershipRadio = ngDriver.FindElement(By.XPath("(//input[@value='Partnership'])"));
                partnershipRadio.Click();
            }

            // if this is a society, click the radio button
            if (businessType == "society")
            {
                NgWebElement societyRadio = ngDriver.FindElement(By.XPath("(//input[@value='Society'])"));
                societyRadio.Click();
            }

            // if this is a university, click the radio button
            if (businessType == "university")
            {
                NgWebElement indigenousNationRadio = ngDriver.FindElement(By.XPath("(//input[@value='University'])"));
                indigenousNationRadio.Click();
            }

            // if this is an indigenous nation, click the radio button
            if (businessType == "indigenous nation")
            {
                NgWebElement indigenousNationRadio = ngDriver.FindElement(By.XPath("(//input[@value='IndigenousNation'])"));
                indigenousNationRadio.Click();
            }

            // click on the Next button
            NgWebElement nextButton = ngDriver.FindElement(By.XPath("//button[contains(.,'Next')]"));
            nextButton.Click();

            /* 
            Page Title: Please confirm the name associated with the Business BCeID login provided.
            */

            // click on the Yes button
            NgWebElement confirmNameButton = ngDriver.FindElement(By.XPath("//button"));
            confirmNameButton.Click();
            ngDriver.WaitForAngular();
        }

        public void CarlaLogin(string businessType)
        {
            businessTypeShared = businessType;

            Random random = new Random();
            
            // load the dashboard page
            string test_start = "login/token/AT" + DateTime.Now.Ticks.ToString() + random.Next(0, 999).ToString();
            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");
            ngDriver.IgnoreSynchronization = false;

            DoLogin(businessType);
        }

        public void CarlaLoginNoCheck()
        {
            // load the dashboard page
            string test_start = configuration["test_start"];

            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");

            ngDriver.WaitForAngular();
        }

        public void CarlaLoginWithUser(string businessType)
        {
            businessTypeShared = businessType;

            // load the dashboard page
            string test_start = configuration["test_start"];
            ngDriver.IgnoreSynchronization = true;
            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");
            ngDriver.IgnoreSynchronization = false;

            DoLogin(businessType);
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

        public void CarlaDeleteCurrentAccount()
        {
            ngDriver.IgnoreSynchronization = true;

            // using wrapped driver as this call is not angular
            ngDriver.Navigate().GoToUrl($"{baseUri}api/accounts/delete/current");

            ngDriver.IgnoreSynchronization = false;

            ngDriver.Navigate().GoToUrl($"{baseUri}logout");
        }


        [And(@"I complete the Cannabis Retail Store application")]
        public void CompleteCannabisApplication()
        {
            /* 
            Page Title: Submit the Cannabis Retail Store Application
            */

            // create application info
            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A6X5";
            string estPID = "012345678";
            string estEmail = "test@test.com";
            string estPhone = "2505555555";
            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";
            string conEmail = "contact@email.com";

            System.Threading.Thread.Sleep(9000);

            // enter the establishment name
            NgWebElement estabName = ngDriver.FindElement(By.Id("establishmentName"));
            estabName.SendKeys(estName);

            // enter the establishment address
            NgWebElement estabAddress = ngDriver.FindElement(By.Id("establishmentAddressStreet"));
            estabAddress.SendKeys(estAddress);

            // enter the establishment city
            NgWebElement estabCity = ngDriver.FindElement(By.Id("establishmentAddressCity"));
            estabCity.SendKeys(estCity);

            // enter the establishment postal code
            NgWebElement estabPostal = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            estabPostal.SendKeys(estPostal);

            // enter the PID
            NgWebElement estabPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            estabPID.SendKeys(estPID);

            if (businessTypeShared == "indigenous nation")
            {
                string indigenousNation = "Ashcroft Indian Band";

                // enter the IN into the dropdown
                NgWebElement uiSelectNation = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-2']/app-application/div[2]/div/div[2]/div[2]/section/div/app-field[2]/section/div[1]/section/select"));
                uiSelectNation.SendKeys(indigenousNation);
            }

            // enter the store email
            NgWebElement estabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            estabEmail.SendKeys(estEmail);

            // enter the store phone number
            NgWebElement estabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            estabPhone.SendKeys(estPhone);

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a store signage document
            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSignage.SendKeys(signagePath);

            // upload a valid interest document
            string validInterestPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "valid_interest.pdf");
            NgWebElement uploadValidInterest = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uploadValidInterest.SendKeys(validInterestPath);

            // upload a floor plan document
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uploadFloorplan.SendKeys(floorplanPath);

            // upload a site plan document
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
            uploadSitePlan.SendKeys(sitePlanPath);

            // upload a financial integrity form
            string finIntegrityPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "fin_integrity.pdf");
            NgWebElement uploadFinIntegrity = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uploadFinIntegrity.SendKeys(finIntegrityPath);

            // enter the first name of the application contact
            NgWebElement contactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            contactGiven.SendKeys(conGiven);

            // enter the last name of the application contact
            NgWebElement contactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            contactSurname.SendKeys(conSurname);

            // enter the role of the application contact
            NgWebElement contactRole = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonRole]"));
            contactRole.SendKeys(conRole);

            // enter the phone number of the application contact
            NgWebElement contactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonPhone]"));
            contactPhone.SendKeys(conPhone);

            // enter the email of the application contact
            NgWebElement contactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            contactEmail.SendKeys(conEmail);

            // click on the authorized to submit checkbox
            NgWebElement authorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            authorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement signatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            signatureAgree.Click();

            // retrieve the current URL to get the application ID (needed downstream)
            string URL = ngDriver.Url;

            // retrieve the application ID
            string[] parsedURL = URL.Split('/');

            applicationID = parsedURL[5];

            ClickOnSubmitButton();
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
                // confirm that payment receipt is for $7,500.00
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$475.00')]")).Displayed);
            }
        }

        [And(@"I return to the dashboard")]
        public void AndReturnToDashboard()
        {
            ReturnToDashboard();
        }


        [Then(@"I return to the dashboard")]
        public void ThenReturnToDashboard()
        {
            ReturnToDashboard();
        }

        public void ReturnToDashboard()
        {
            // click on Return to Dashboard link
            string retDash = "Return to Dashboard";
            NgWebElement returnDash = ngDriver.FindElement(By.LinkText(retDash));
            returnDash.Click();
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
            string estPostal = "V9A6X5";
            string estPID = "012345678";
            string estPhone = "2505555555";
            string estEmail = "test@automation.com";
            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";
            string conEmail = "test2@automation.com";

            System.Threading.Thread.Sleep(9000);

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

            // select 'Yes'
            // Do you or any of your shareholders currently hold, have held, or have previously applied for a British Columbia liquor licence ?
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
            NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSignage.SendKeys(signagePath);

            // enter the first name of the application contact
            NgWebElement uiContactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            uiContactGiven.SendKeys(conGiven);

            // enter the last name of the application contact
            NgWebElement uiContactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            uiContactSurname.SendKeys(conSurname);

            // enter the role of the application contact
            NgWebElement uiContactRole = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
            uiContactRole.SendKeys(conRole);

            // enter the phone number of the application contact
            NgWebElement uiContactPhone = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
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

        public void CheckFeatureFlag(string flag)
        {
            ngDriver.IgnoreSynchronization = true;

            // navigate to the feature flags page
            ngDriver.WrappedDriver.Navigate().GoToUrl($"{baseUri}api/features");

            // confirm that the CRS-Renewal flag is enabled during this test
            Assert.True(ngDriver.FindElement(By.XPath($"//body[contains(.,'{flag}')]")).Displayed);

            ngDriver.IgnoreSynchronization = false;
        }

        public void CheckFeatureFlagsCannabis()
        {
            CheckFeatureFlag("CRS-Renewal");
        }

        public void CheckFeatureFlagsCOVIDTempExtension()
        {
            CheckFeatureFlag("CovidApplication");          
        }

        public void CheckFeatureFlagsLiquor()
        {
            CheckFeatureFlag("LiquorOne");        
        }

        public void CheckFeatureFlagsMaps()
        {
            CheckFeatureFlag("Maps");
        }


        [And(@"I request a personnel name change")]
        public void RequestPersonnelNameChange()
        {
            if (businessTypeShared != "indigenous nation")
            {
                // click on Dashboard link
                string dash = "Dashboard";
                NgWebElement returnDash = ngDriver.FindElement(By.LinkText(dash));
                returnDash.Click();

                // click on the review organization information button
                NgWebElement orgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,'REVIEW ORGANIZATION INFORMATION')]"));
                orgInfoButton.Click();

                // click on the Edit button for Key Personnel
                NgWebElement uiEditInfoButton = ngDriver.FindElement(By.XPath("//i/span"));
                uiEditInfoButton.Click();

                // enter a new name for the director
                string newDirectorFirstName = "UpdatedFirstName";
                string newDirectorLastName = "UpdatedLastName";

                NgWebElement uiNewDirectorFirstName = ngDriver.FindElement(By.XPath("//input[@type='text']"));
                uiNewDirectorFirstName.Clear();
                uiNewDirectorFirstName.SendKeys(newDirectorFirstName);

                NgWebElement uiNewDirectorLasttName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
                uiNewDirectorLasttName.Clear();
                uiNewDirectorLasttName.SendKeys(newDirectorLastName);

                // click on the Confirm button
                NgWebElement uiConfirmButton = ngDriver.FindElement(By.XPath("//i/span"));
                uiConfirmButton.Click();

                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload a marriage certificate document
                string marriageCertificate = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "marriage_certificate.pdf");

                if (businessTypeShared == "public corporation")
                {
                    NgWebElement uploadMarriageCert1 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
                    uploadMarriageCert1.SendKeys(marriageCertificate);
                }
                else
                {
                    NgWebElement uploadMarriageCert2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[12]"));
                    uploadMarriageCert2.SendKeys(marriageCertificate);
                }

                // workaround for current bug (LCSD-3214)
                if (businessTypeShared == "partnership")
                {
                    string shares = "100";
                    NgWebElement uiPartnerShares = ngDriver.FindElement(By.XPath("//app-org-structure/div[3]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
                    uiPartnerShares.Clear();
                    uiPartnerShares.SendKeys(shares);
                }

                // click on submit org info button
                NgWebElement orgInfoButton2 = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT ORGANIZATION INFORMATION')]"));
                orgInfoButton2.Click();

                MakePayment();

                // check payment fee
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$500.00')]")).Displayed);

                System.Threading.Thread.Sleep(7000);

                // click on Dashboard link
                NgWebElement returnDash2 = ngDriver.FindElement(By.LinkText(dash));
                returnDash2.Click();

                // click on the review organzation information button
                NgWebElement orgInfoButton3 = ngDriver.FindElement(By.XPath("//button[contains(.,'REVIEW ORGANIZATION INFORMATION')]"));
                orgInfoButton3.Click();

                System.Threading.Thread.Sleep(7000);

                // check that the director name has been updated
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'UpdatedFirstName')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'UpdatedLastName')]")).Displayed);
            }
        }


        [And(@"I request a store relocation")]
        public void RequestStoreRelocation()
        {
            /* 
            Page Title: Licences
            Subtitle: Cannabis Retail Store Licences
            */

            string requestRelocationLink = "Request Relocation";

            // click on the request location link
            NgWebElement uiRequestRelocation = ngDriver.FindElement(By.LinkText(requestRelocationLink));
            uiRequestRelocation.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            // click on the Continue to Application button
            NgWebElement continueButton = ngDriver.FindElement(By.XPath("//button[contains(.,'CONTINUE TO APPLICATION')]"));
            continueButton.Click();

            /* 
            Page Title: Submit a Licence Relocation Application
            */

            string proposedAddress = "Automated Test Street";
            string proposedCity = "Automated City";
            string proposedPostalCode = "A1A 1A1";
            string pid = "012345678";

            // enter the proposed street address
            NgWebElement uiProposedAddress = ngDriver.FindElement(By.XPath("(//input[@id='establishmentAddressStreet'])[2]"));
            uiProposedAddress.SendKeys(proposedAddress);

            // enter the proposed city
            NgWebElement uiProposedCity = ngDriver.FindElement(By.XPath("(//input[@id='establishmentAddressCity'])[2]"));
            uiProposedCity.SendKeys(proposedCity);

            // enter the postal code
            NgWebElement uiProposedPostalCode = ngDriver.FindElement(By.XPath("(//input[@id='establishmentAddressPostalCode'])[3]"));
            uiProposedPostalCode.SendKeys(proposedPostalCode);

            // enter the PID
            NgWebElement uiProposedPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            uiProposedPID.SendKeys(pid);

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a supporting document
            string supportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "checklist.pdf");
            NgWebElement uploadSupportingDoc = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSupportingDoc.SendKeys(supportingDocument);

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

            // confirm correct payment amount
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$220.00')]")).Displayed);

            // return to the Licences tab
            string licencesLink = "Licences";

            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }


        [And(@"I change a personnel email address")]
        public void RequestPersonnelEmailChange()
        {
            if (businessTypeShared != "indigenous nation")
            {
                // click on Dashboard link
                string dash = "Dashboard";
                NgWebElement returnDash = ngDriver.FindElement(By.LinkText(dash));
                returnDash.Click();

                // click on the review organization information button
                NgWebElement orgInfoButton3 = ngDriver.FindElement(By.XPath("//button[contains(.,'REVIEW ORGANIZATION INFORMATION')]"));
                orgInfoButton3.Click();

                // click on the Edit button for Key Personnel
                NgWebElement uiEditInfoButton = ngDriver.FindElement(By.XPath("//i/span"));
                uiEditInfoButton.Click();

                // enter a new email for the director
                string newDirectorEmail = "newemail@test.com";

                NgWebElement uiNewDirectorEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
                uiNewDirectorEmail.Clear();
                uiNewDirectorEmail.SendKeys(newDirectorEmail);

                // click on the Confirm button
                NgWebElement uiConfirmButton = ngDriver.FindElement(By.XPath("//i/span"));
                uiConfirmButton.Click();

                // click on confirm org info button
                NgWebElement orgInfoButton2 = ngDriver.FindElement(By.XPath("//button[contains(.,' CONFIRM ORGANIZATION INFORMATION IS COMPLETE')]"));
                orgInfoButton2.Click();

                // check that dashboard is displayed (i.e. no payment has been required)
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Welcome to Liquor and Cannabis Licensing')]")).Displayed);
            }
        }


        [And(@"I pay the licensing fee for (.*)")]
        public void PayLicenceFee(string feeType)
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            if (feeType == "Cannabis")
            {
                string licenceFee = "Pay Licence Fee and Plan Store Opening";

                // click on the pay licence fee link
                NgWebElement uiLicenceFee = ngDriver.FindElement(By.LinkText(licenceFee));
                uiLicenceFee.Click();

                /* 
                Page Title: Plan Your Store Opening
                */

                string reasonDay = "Automated test: Reason for opening date.";

                // select the opening date
                NgWebElement uiCalendar1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
                uiCalendar1.Click();

                NgWebElement uiCalendar2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
                uiCalendar2.Click();

                // enter the reason for the opening date
                NgWebElement uiReasonDate = ngDriver.FindElement(By.XPath("//textarea"));
                uiReasonDate.SendKeys(reasonDay);

                NgWebElement paymentButton = ngDriver.FindElement(By.XPath("//button[contains(.,' PAY LICENCE FEE AND RECEIVE LICENCE')]"));
                paymentButton.Click();
            }

            if (feeType == "Catering")
            {
                string firstYearLicenceFee = "Pay First Year Licensing Fee";

                // click on the pay first year licence fee link
                NgWebElement uiFirstYearLicenceFee = ngDriver.FindElement(By.LinkText(firstYearLicenceFee));
                uiFirstYearLicenceFee.Click();
            }

            // pay the licence fee
            MakePayment();

            System.Threading.Thread.Sleep(7000);

            if (feeType == "Cannabis")
            {
                // confirm correct payment amount for CRS
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$1,500.00')]")).Displayed);
            }

            if (feeType == "Catering")
            {
                // confirm correct payment amount for Catering
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$450.00')]")).Displayed);
            }

            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
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
            string physicalAddCity = "Automated test city";
            string physicalAddPostalCode = "V8V4Y3";

            // enter event contact name
            NgWebElement uiEventContactName = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiEventContactName.SendKeys(eventContactName);

            // enter event contact phone
            NgWebElement uiEventContactPhone = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiEventContactPhone.SendKeys(eventContactPhone);

            // select event type
            NgWebElement uiEventType = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[6]/div[1]/app-field/section/div/section/select/option[2]"));
            uiEventType.Click();

            // enter event description
            NgWebElement uiEventDescription = ngDriver.FindElement(By.XPath("//textarea"));
            uiEventDescription.SendKeys(eventDescription);

            // enter event client or host name
            NgWebElement uiEventClientOrHostName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiEventClientOrHostName.SendKeys(eventClientOrHostName);

            // enter maximum attendance
            NgWebElement uiMaxAttendance = ngDriver.FindElement(By.XPath("//input[@type='number']"));
            uiMaxAttendance.SendKeys(maximumAttendance);

            // enter maximum staff attendance
            NgWebElement uiMaxStaffAttendance = ngDriver.FindElement(By.XPath("(//input[@type='number'])[2]"));
            uiMaxStaffAttendance.SendKeys(maximumStaffAttendance);

            // select whether minors are attending - yes
            NgWebElement uiMinorsAttending = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[6]/div[6]/app-field/section/div[1]/section/select/option[1]"));
            uiMinorsAttending.Click();

            // select type of food service provided - Full Service Meal
            NgWebElement uiFoodServiceProvided = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[6]/div[7]/app-field/section/div[1]/section/select/option[3]"));
            uiFoodServiceProvided.Click();

            // select type of entertainment provided - Live Entertainment
            NgWebElement uiEntertainmentProvided = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[6]/div[8]/app-field/section/div[1]/section/select/option[4]"));
            uiEntertainmentProvided.Click();

            // enter venue name description
            NgWebElement uiVenueNameDescription = ngDriver.FindElement(By.XPath("//div[7]/div/app-field/section/div/section/textarea"));
            uiVenueNameDescription.SendKeys(venueNameDescription);

            // select venue location - both (indoors/outdoors)
            NgWebElement uiVenueLocation = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[7]/div[2]/app-field/section/div[1]/section/select/option[3]"));
            uiVenueLocation.Click();

            // enter venue additional info
            NgWebElement uiVenueAdditionalInfo = ngDriver.FindElement(By.XPath("//div[3]/app-field/section/div/section/textarea"));
            uiVenueAdditionalInfo.SendKeys(venueAdditionalInfo);

            // enter physical address - street address 1
            NgWebElement uiPhysicalAddStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            uiPhysicalAddStreetAddress1.SendKeys(physicalAddStreetAddress1);

            // enter physical address - street address 2 
            NgWebElement uiPhysicalAddStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiPhysicalAddStreetAddress2.SendKeys(physicalAddStreetAddress2);

            // enter physical address - city
            NgWebElement uiPhysicalAddCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiPhysicalAddCity.SendKeys(physicalAddCity);

            // enter physical address - postal code
            NgWebElement uiPhysicalAddPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiPhysicalAddPostalCode.SendKeys(physicalAddPostalCode);

            // select start date
            NgWebElement uiVenueStartDate1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            uiVenueStartDate1.Click();

            NgWebElement uiVenueStartDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            uiVenueStartDate2.Click();

            // select end date
            NgWebElement uiVenueEndDate1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiVenueEndDate1.Click();

            NgWebElement uiVenueEndDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            uiVenueEndDate2.Click();

            // select event and liquor service times are different on specific dates checkbox
            NgWebElement uiEventLiquorServiceTimesDifferent = ngDriver.FindElement(By.Id("mat-checkbox-1"));
            uiEventLiquorServiceTimesDifferent.Click();

            // select terms and conditions checkbox
            NgWebElement uiTermsAndConditions = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-event-form/div/form/div[9]/div/mat-checkbox"));
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


        [And(@"I plan the store opening")]
        public void PlanStoreOpening()
        { 
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            string storePlanningLink = "Store Opening Inspection Checklist";

            // click on the store planning link
            NgWebElement uiStorePlanning = ngDriver.FindElement(By.LinkText(storePlanningLink));
            uiStorePlanning.Click();

            /* 
            Page Title: Plan Your Store Opening
            */

            System.Threading.Thread.Sleep(7000);

            // select checkboxes to confirm store opening details
            NgWebElement check1 = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            check1.Click();

            NgWebElement check2 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[2]"));
            check2.Click();

            NgWebElement check3 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[3]"));
            check3.Click();

            NgWebElement check4 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[4]"));
            check4.Click();

            NgWebElement check5 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[5]"));
            check5.Click();

            NgWebElement check6 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[6]"));
            check6.Click();

            NgWebElement check7 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[7]"));
            check7.Click();

            NgWebElement check8 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[8]"));
            check8.Click();

            NgWebElement check9 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[9]"));
            check9.Click();

            NgWebElement check10 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[10]"));
            check10.Click();

            NgWebElement check11 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[11]"));
            check11.Click();

            NgWebElement check12 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[12]"));
            check12.Click();

            NgWebElement check13 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[13]"));
            check13.Click();

            NgWebElement check14 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[14]"));
            check14.Click();

            //click on the Save button
            NgWebElement saveButton = ngDriver.FindElement(By.XPath("//button[contains(.,'SAVE')]"));
            saveButton.Click();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            System.Threading.Thread.Sleep(7000);

            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }


        [And(@"I click on the Start Application button for (.*)")]
        public void ClickStartApplication(string applicationType)
        {
            if (applicationType == "Catering")
            {
                // click on the Catering Start Application button
                NgWebElement startApp_button = ngDriver.FindElement(By.Id("startCatering"));
                startApp_button.Click();
            }

            if (applicationType == "a Cannabis Retail Store")
            {
                NgWebElement startApp_button = ngDriver.FindElement(By.CssSelector("button[id='startCRS']"));
                startApp_button.Click();
            }

            applicationTypeShared = applicationType;
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

            // click on continue to application button
            NgWebElement continueToApplicationButton = ngDriver.FindElement(By.XPath("//button[contains(.,'CONTINUE TO APPLICATION')]"));
            continueToApplicationButton.Click();

            /* 
            Page Title: Submit the Cannabis Retail Store Structural Change Application
            */

            // create test data
            string description = "Test automation outline of the proposed change.";

            // enter the description of the change
            NgWebElement descriptionOfChange = ngDriver.FindElement(By.Id("description1"));
            descriptionOfChange.SendKeys(description);

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a floor plan document
            string floorPlan = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiFloorPlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiFloorPlan.SendKeys(floorPlan);

            // select 'no' for changes to entries
            NgWebElement changeToEntries = ngDriver.FindElement(By.Id("mat-button-toggle-2-button"));
            changeToEntries.Click();

            // select authorizedToSubmit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // select signatureAgreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton2();

            System.Threading.Thread.Sleep(3000);

            // pay for the structural change application
            MakePayment();

            System.Threading.Thread.Sleep(7000);

            // confirm correct payment amount
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$440.00')]")).Displayed);

            // return to the Licences tab
            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
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

            // return to the Licences tab
            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }

        public void RequestedApplicationsOnDashboard()
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            System.Threading.Thread.Sleep(7000);

            string dashboard = "Dashboard";

            // click on the Dashboard link
            NgWebElement uiDashboard = ngDriver.FindElement(By.LinkText(dashboard));
            uiDashboard.Click();

            // confirm that relocation request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Relocation Request')]")).Displayed);

            // confirm that a name or branding change request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name or Branding Change')]")).Displayed);

            // confirm that a structural change request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Structural Change')]")).Displayed);
        }


        [And(@"I request a valid store name or branding change for (.*)")]
        public void RequestNameBrandingChange(string changeType)
        {
            string nameBrandingLink = "Request Store Name or Branding Change";

            // click on the Request Store Name or Branding Change link
            NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText(nameBrandingLink));
            uiRequestChange.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            // click on the Continue to Application button
            NgWebElement continueButton = ngDriver.FindElement(By.XPath("//button[contains(.,'CONTINUE TO APPLICATION')]"));
            continueButton.Click();

            /*
            Page Title: Submit a Name or Branding Change Application
            */

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a supporting document
            string supportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSupportingDoc = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSupportingDoc.SendKeys(supportingDocument);

            if (changeType == "Cannabis")
            {
                // click on the store exterior change button	
                NgWebElement uiStoreExterior = ngDriver.FindElement(By.Id("mat-button-toggle-2-button"));
                uiStoreExterior.Click();
            }

            // click on the authorized to submit checkbox
            NgWebElement uiAuthSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSigAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSigAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton2();

            // pay for the branding change application
            MakePayment();

            System.Threading.Thread.Sleep(7000);

            // confirm correct payment amount	
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$220.00')]")).Displayed);

            // return to the Licences tab
            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }

        public void CateringRelocationRequest()
        {
            /* 
            Page Title: Licences
            Subtitle: Catering Licences
            */

            string requestRelocationLink = "Request Relocation";

            // click on the request location link
            NgWebElement uiRequestRelocation = ngDriver.FindElement(By.LinkText(requestRelocationLink));
            uiRequestRelocation.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            // click on the Continue to Application button
            NgWebElement continueButton = ngDriver.FindElement(By.XPath("//button[contains(.,'CONTINUE TO APPLICATION')]"));
            continueButton.Click();

            /* 
            Page Title: Submit a Licence Relocation Application
            */

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a supporting document
            string supportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "checklist.pdf");
            NgWebElement uploadSupportingDoc = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSupportingDoc.SendKeys(supportingDocument);

            // select the authorized to submit checkbox
            NgWebElement uiAuthToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSigAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSigAgreement.Click();

            // click on the Submit & Pay button
            ClickOnSubmitButton2();

            // pay for the relocation application
            MakePayment();

            System.Threading.Thread.Sleep(7000);

            // confirm correct payment amount
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$330.00')]")).Displayed);
        }

        public void Dispose()
        {
            ngDriver.Quit();

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        public void SharedCalendarDate()
        {
            // click on the previous button
            NgWebElement openCalendarPrevious = ngDriver.FindElement(By.CssSelector(".mat-calendar .mat-calendar-previous-button"));
            openCalendarPrevious.Click();

            // click on the first day
            NgWebElement openCalendarYear = ngDriver.FindElement(By.CssSelector(".mat-calendar-content .mat-calendar-body-cell-content:first-child"));
            openCalendarYear.Click();
        }


        [And(@"I request a third party operator")]
        public void RequestThirdPartyOperator()
        {
            // return to the Licences tab
            string licencesLink2 = "Licences";

            NgWebElement uiLicences2 = ngDriver.FindElement(By.LinkText(licencesLink2));
            uiLicences2.Click();

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
            NgWebElement thirdPartyOperator = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            thirdPartyOperator.SendKeys(thirdparty);

            NgWebElement thirdPartyOperatorOption = ngDriver.FindElement(By.XPath("//mat-option[@id='mat-option-0']/span"));
            thirdPartyOperatorOption.Click();

            // click on authorized to submit checkbox
            NgWebElement authorizedToSubmit = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            authorizedToSubmit.Click();

            // click on signature agreement checkbox
            NgWebElement signatureAgreement = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[2]"));
            signatureAgreement.Click();

            // click on submit button
            ClickOnSubmitButton2();

            // return to the Licences tab
            string licencesLink = "Licences";

            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();

            // confirm that the application has been initiated
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Third Party Operator Application Initiated')]")).Displayed);
        }


        [And(@"I request a transfer of ownership")]
        public void RequestOwnershipTransfer()
        {
            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            string transferOwnership = "Transfer Ownership";

            // click on the Transfer Ownership link
            NgWebElement uiTransferOwnership = ngDriver.FindElement(By.LinkText(transferOwnership));
            uiTransferOwnership.Click();

            /* 
            Page Title: Transfer Your Catering Licence
            */

            string licensee = "GunderCorp TestBusiness";

            // search for the proposed licensee
            NgWebElement proposedLicensee = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            proposedLicensee.SendKeys(licensee);

            NgWebElement thirdPartyOperatorOption = ngDriver.FindElement(By.XPath("//*[@id='mat-option-1']/span"));
            thirdPartyOperatorOption.Click();

            // click on consent to licence transfer checkbox
            NgWebElement consentToTransfer = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            consentToTransfer.Click();

            // click on authorize signature checkbox
            NgWebElement authorizeSignature = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[2]"));
            authorizeSignature.Click();

            // click on signature agreement checkbox
            NgWebElement signatureAgreement = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[3]"));
            signatureAgreement.Click();

            // click on submit transfer button
            NgWebElement submitTransferButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT TRANSFER')]"));
            submitTransferButton.Click();

            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();

            // check for transfer initiated status 
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'TRANSFER INITIATED')]")).Displayed);
        }

        public void ApplicationsVisibleOnDashboard()
        {
            /* 
            Page Title: Licences
            Subtitle:   Catering Licences
            */

            string dashboard = "Dashboard";

            // click on the Dashboard link
            NgWebElement uiDashboard = ngDriver.FindElement(By.LinkText(dashboard));
            uiDashboard.Click();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // confirm that relocation request is displayed
            //Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Relocation Request')]")).Displayed);

            // confirm that a name or branding change request is displayed - TODO
            //Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name or Branding Change')]")).Displayed);

            // confirm that a third party operator request is displayed
            //Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Third-Party Operator')]")).Displayed);
        }


        [And(@"the application is approved")]
        public void ApplicationIsApproved()
        {
            ngDriver.IgnoreSynchronization = true;

            // navigate to api/applications/<Application ID>/process
            ngDriver.Navigate().GoToUrl($"{baseUri}api/applications/{applicationID}/process");

            // wait for the autoamted approval process to run
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'OK')]")).Displayed);

            ngDriver.IgnoreSynchronization = false;

            // navigate back to dashboard
            ngDriver.Navigate().GoToUrl($"{baseUri}/dashboard");
        }


        [And(@"I complete the eligibility disclosure")]
        public void CompleteEligibilityDisclosure()
        {
            ngDriver.WaitForAngular();

            /* 
            Page Title: Cannabis Retail Store Licence Eligibility Disclosure
            */

            // select response: On or after March 1, 2020, did you or any of your associates own, operate, provide financial support to, or receive income from an unlicensed cannabis retail store or retailer?           
            // select Yes radio button 
            NgWebElement yesRadio1 = ngDriver.FindElement(By.Id("mat-radio-2"));
            yesRadio1.Click();

            // complete field: Please indicate the name and location of the retailer or store 
            string nameAndLocation = "Automated test name and location of retailer";

            NgWebElement uiNameAndLocation = ngDriver.FindElement(By.CssSelector("input[formControlName=\"nameLocationUnlicencedRetailer\"]"));
            uiNameAndLocation.SendKeys(nameAndLocation);

            // select response: Does the retailer or store continue to operate?
            // select Yes for Question 2 using radio button
            NgWebElement yesRadio2 = ngDriver.FindElement(By.Id("mat-radio-5"));
            yesRadio2.Click();

            // select response: On or after March 1, 2020, were you or any of your associates involved with the distribution or supply of cannabis to a licensed or unlicensed cannabis retail store or retailer?
            // select Yes using radio button
            NgWebElement yesRadio3 = ngDriver.FindElement(By.Id("mat-radio-8"));
            yesRadio3.Click();

            // complete field: Please indicate the details of your involvement
            string involvementDetails = "Automated test - details of the involvement";

            NgWebElement matCheckbox = ngDriver.FindElement(By.XPath("//textarea"));
            matCheckbox.SendKeys(involvementDetails);

            // complete field: Please indicate the name and location of the retailer or store           
            string nameAndLocation2 = "Automated test name and location of retailer (2)";
            NgWebElement uiNameAndLocation2 = ngDriver.FindElement(By.CssSelector("input[formControlName=\"nameLocationRetailer\"]"));

            uiNameAndLocation2.SendKeys(nameAndLocation2);

            // select response: Do you continue to be involved?
            // select Yes for Question 2 using radio button
            NgWebElement yesRadio4 = ngDriver.FindElement(By.Id("mat-radio-11"));
            yesRadio4.Click();

            // select certification checkbox
            NgWebElement uiCheckbox = ngDriver.FindElement(By.Id("mat-checkbox-1"));
            uiCheckbox.Click();

            // enter the electronic signature
            string electricSignature = "Automated Test";

            NgWebElement sigCheckbox = ngDriver.FindElement(By.Id("eligibilitySignature"));
            sigCheckbox.SendKeys(electricSignature);

            // click on the Submit button
            ClickOnSubmitButton();
        }


        [And(@"I review the account profile")]
        public void ReviewAccountProfile()
        {
            /*
            Page Title: Please Review the Account Profile
            */

            string bizNumber = "012345678";
            string incorporationNumber = "BC1234567";

            string physStreetAddress1 = "645 Tyee Road";
            string physStreetAddress2 = "West of Victoria";
            string physCity = "Victoria";
            string physPostalCode = "V8V4Y3";

            string mailStreet1 = "P.O. Box 123";
            string mailStreet2 = "303 Prideaux St.";
            string mailCity = "Nanaimo";
            string mailProvince = "B.C.";
            string mailPostalCode = "V9R2N3";
            string mailCountry = "Canada";

            string bizPhoneNumber = "2501811818";
            string bizEmail = "test@automation.com";
            string corpGiven = "CorpGiven";
            string corpSurname = "CorpSurname";
            string corpTitle = "CEO";
            string corpContactPhone = "7781811818";
            string corpContactEmail = "automated@test.com";

            // enter the business number
            NgWebElement uiBizNumber = ngDriver.FindElement(By.CssSelector("input[formControlName=\"businessNumber\"]"));
            uiBizNumber.SendKeys(bizNumber);

            // enter the private corporation or society incorporation number
            if (businessTypeShared == "private corporation" || businessTypeShared == "society")
            {
                NgWebElement uiCorpNumber = ngDriver.FindElement(By.Id("bcIncorporationNumber"));
                uiCorpNumber.SendKeys(incorporationNumber);

                NgWebElement uiCalendar1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
                uiCalendar1.Click();

                NgWebElement uiCalendar2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
                uiCalendar2.Click();
            }

            // enter the physical street address 1
            NgWebElement uiPhysStreetAddress1 = ngDriver.FindElement(By.CssSelector("input[formControlName=\"physicalAddressStreet\"]"));
            uiPhysStreetAddress1.SendKeys(physStreetAddress1);

            // enter the physical street address 2
            NgWebElement uiPhysStreetAddress2 = ngDriver.FindElement(By.CssSelector("input[formControlName=\"physicalAddressStreet2\"]"));
            uiPhysStreetAddress2.SendKeys(physStreetAddress2);

            // enter the physical city
            NgWebElement uiPhysCity = ngDriver.FindElement(By.CssSelector("input[formControlName=\"physicalAddressCity\"]"));
            uiPhysCity.SendKeys(physCity);

            // enter the physical postal code
            NgWebElement uiPhysPostalCode = ngDriver.FindElement(By.CssSelector("input[formControlName=\"physicalAddressPostalCode\"]"));
            uiPhysPostalCode.SendKeys(physPostalCode);

            // select and deselect same mailing address to confirm checkbox is available
            NgWebElement uiSameAsMailingAddress = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiSameAsMailingAddress.Click();
            uiSameAsMailingAddress.Click();

            // enter the mailing street address 1
            NgWebElement uiMailingStreetAddress1 = ngDriver.FindElement(By.CssSelector("input[formControlName=\"mailingAddressStreet\"]"));
            uiMailingStreetAddress1.Clear();
            uiMailingStreetAddress1.SendKeys(mailStreet1);

            // enter the mailing street address 2
            NgWebElement uiMailingStreetAddress2 = ngDriver.FindElement(By.CssSelector("input[formControlName=\"mailingAddressStreet2\"]"));
            uiMailingStreetAddress2.Clear();
            uiMailingStreetAddress2.SendKeys(mailStreet2);

            // enter the mailing city
            NgWebElement uiMailingCity = ngDriver.FindElement(By.CssSelector("input[formControlName=\"mailingAddressCity\"]"));
            uiMailingCity.Clear();
            uiMailingCity.SendKeys(mailCity);

            // enter the mailing province
            NgWebElement uiMailingProvince = ngDriver.FindElement(By.CssSelector("input[formControlName=\"mailingAddressProvince\"]"));
            uiMailingProvince.Clear();
            uiMailingProvince.SendKeys(mailProvince);

            // enter the mailing postal code
            NgWebElement uiMailingPostalCode = ngDriver.FindElement(By.CssSelector("input[formControlName=\"mailingAddressPostalCode\"]"));
            uiMailingPostalCode.Clear();
            uiMailingPostalCode.SendKeys(mailPostalCode);

            // enter the mailing country
            NgWebElement uiMailingCountry = ngDriver.FindElement(By.CssSelector("input[formControlName=\"mailingAddressCountry\"]"));
            uiMailingCountry.Clear();
            uiMailingCountry.SendKeys(mailCountry);

            // enter the business phone number
            NgWebElement uiBizPhoneNumber = ngDriver.FindElement(By.CssSelector("input[formControlName=\"contactPhone\"]"));
            uiBizPhoneNumber.SendKeys(bizPhoneNumber);

            // enter the business email
            NgWebElement uiBizEmail = ngDriver.FindElement(By.CssSelector("input[formControlName=\"contactEmail\"]"));
            uiBizEmail.SendKeys(bizEmail);

            // (re)enter the first name of contact           
            NgWebElement uiCorpGiven = ngDriver.FindElement(By.CssSelector("input[formControlName=\"firstname\"]"));
            uiCorpGiven.SendKeys(corpGiven);

            // (re)enter the last name of contact           
            NgWebElement uiCorpSurname = ngDriver.FindElement(By.CssSelector("input[formControlName=\"lastname\"]"));
            uiCorpSurname.SendKeys(corpSurname);

            // enter the contact title
            NgWebElement uiCorpTitle = ngDriver.FindElement(By.CssSelector("input[formControlName=\"jobTitle\"]"));
            uiCorpTitle.SendKeys(corpTitle);

            // enter the contact phone number
            NgWebElement uiCorpContactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName=\"telephone1\"]"));
            uiCorpContactPhone.SendKeys(corpContactPhone);

            ngDriver.WaitForAngular();

            // enter the contact email
            NgWebElement uiCorpContactEmail = ngDriver.FindElement(By.CssSelector("div[formGroupName=\"primarycontact\"] input[formControlName=\"emailaddress1\"]"));
            uiCorpContactEmail.SendKeys(corpContactEmail);

            // select 'No' for connection to a federal producer - switched off to test text areas
            NgWebElement corpConnectionFederalProducer = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[2]"));
            corpConnectionFederalProducer.Click();

            // select 'Yes' for connection to a federal producer
            NgWebElement corpConnectionFederalProducer2 = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[1]"));
            corpConnectionFederalProducer2.Click();

            // enter the name of the federal producer and details of the connection 
            string nameAndDetails = "Name and details of federal producer (automated test).";

            NgWebElement uiDetailsFederalProducer = ngDriver.FindElement(By.XPath("//textarea"));
            uiDetailsFederalProducer.SendKeys(nameAndDetails);

            // select 'No' for federal producer's connection to business - switched off to test text areas
            if ((businessTypeShared != "indigenous nation") && (businessTypeShared != "society"))
            {
                NgWebElement federalProducerConnectionToCorp = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[4]"));
                federalProducerConnectionToCorp.Click();
            }

            // select 'Yes' for federal producer's connection to business
            if ((businessTypeShared != "indigenous nation") && (businessTypeShared != "society"))
            {
                NgWebElement federalProducerConnectionToCorp = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[3]"));
                federalProducerConnectionToCorp.Click();

                // enter the name of the federal producer and details of the connection 
                string nameAndDetails2 = "Name and details of federal producer (automated test) (2).";

                NgWebElement uiDetailsFederalProducer2 = ngDriver.FindElement(By.XPath("(//textarea[@id=''])[2]"));
                uiDetailsFederalProducer2.SendKeys(nameAndDetails2);
            }

            if (businessTypeShared == "public corporation")
            {
                string familyRelationship = "Details of family relationship (automated test).";

                // select 'Yes' for family connection
                NgWebElement familyConnectionConnectionToCorp = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[5]"));
                familyConnectionConnectionToCorp.Click();

                // enter details of family connection
                NgWebElement familyConnectionDetails = ngDriver.FindElement(By.XPath("(//textarea[@id=''])[3]"));
                familyConnectionDetails.SendKeys(familyRelationship);
            }

            // click on Continue to Organization Review button
            NgWebElement continueAppButton = ngDriver.FindElement(By.Id("continueToApp"));
            continueAppButton.Click();
        }


        [And(@"the account is deleted")]
        public void DeleteMyAccount()
        {
            this.CarlaDeleteCurrentAccount();
        }


        [Then(@"I see the login page")]
        public void SeeLogin()
        {
            Assert.True(ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }


        [And(@"I review the security screening requirements")]
        public void ReviewSecurityScreeningRequirements()
        {
            /* 
            Page Title: Security Screening Requirements
            */
            
            // confirm that private corporation personnel are present
            if (businessTypeShared == "private corporation")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnel1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'PrivateCorp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnel2')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'BizShareholderPrivateCorp')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'IndividualShareholder2')]")).Displayed);
            }

            // confirm that sole proprietor personnel are present
            if (businessTypeShared == "sole proprietorship")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Leader')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'SoleProprietor')]")).Displayed);
            }

            // confirm that society personnel are present
            if (businessTypeShared == "society")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Director')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Society')]")).Displayed);
            }

            // confirm that public corporation personnel are present
            if (businessTypeShared == "public corporation")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'KeyPersonnel1')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Public Corp')]")).Displayed);
            }

            // confirm that partnership personnel are present
            if (businessTypeShared == "partnership")
            {
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Individual')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Partner')]")).Displayed);

                // switched off - pending LCSD-3126
                //Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Partner2')]")).Displayed);
            }
        }


        [And(@"I enter the payment information")]
        public void EnterPaymentInfo()
        {
            MakePayment();
        }

        
        [And(@"I am logged in to the dashboard as a (.*)")]
        public void ViewDashboard(string businessType)
        {
            CarlaLogin(businessType);
        }


        [And(@"I review the organization structure")]
        public void ReviewOrganizationStructure()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            if (businessTypeShared == "private corporation")
            {
                // find the upload test files in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload a notice of articles document
                string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
                NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
                uploadSignage.SendKeys(noticeOfArticles);

                // upload a central securities register document
                string centralSecuritiesRegister = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
                NgWebElement uploadCentralSecReg = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
                uploadCentralSecReg.SendKeys(centralSecuritiesRegister);

                // upload a special rights and restrictions document
                string specialRightsRestrictions = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
                NgWebElement uploadSpecialRightsRes = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
                uploadSpecialRightsRes.SendKeys(specialRightsRestrictions);

                /********** Key Personnel **********/

                // create the key personnel data
                string keyPersonnelFirstName = "KeyPersonnel1";
                string keyPersonnelLastName = "PrivateCorp";
                string keyPersonnelTitle = "CTO";
                string keyPersonnelEmail = "keypersonnel1@privatecorp.com";

                // open key personnel form  
                if (applicationTypeShared == "Catering")
                {
                    // open key personnel form   
                    NgWebElement openKeyPersonnelFormCat = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[4]/section/app-associate-list/div/button"));
                    openKeyPersonnelFormCat.Click();
                }
                else
                {
                    NgWebElement openKeyPersonnelForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[4]/section/app-associate-list/div/button"));
                    openKeyPersonnelForm.Click();
                }

                // enter key personnel first name
                NgWebElement uiKeyPersonFirst = ngDriver.FindElement(By.CssSelector("input[formControlName=\"firstNameNew\"]"));
                uiKeyPersonFirst.SendKeys(keyPersonnelFirstName);

                // enter key personnel last name
                NgWebElement uiKeyPersonLast = ngDriver.FindElement(By.CssSelector("input[formControlName=\"lastNameNew\"]"));
                uiKeyPersonLast.SendKeys(keyPersonnelLastName);

                // select key personnel role
                if (applicationTypeShared == "Catering")
                {
                    NgWebElement uiKeyPersonRoleCat = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[3]"));
                    uiKeyPersonRoleCat.Click();
                }
                else
                {
                    NgWebElement uiKeyPersonRole = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
                    uiKeyPersonRole.Click();
                }

                // enter key personnel title
                NgWebElement uiKeyPersonTitle = ngDriver.FindElement(By.CssSelector("input[formControlName=\"titleNew\"]"));
                uiKeyPersonTitle.SendKeys(keyPersonnelTitle);

                // enter key personnel email
                NgWebElement uiKeyPersonEmail = ngDriver.FindElement(By.CssSelector("input[formControlName=\"emailNew\"]"));
                uiKeyPersonEmail.SendKeys(keyPersonnelEmail);

                // enter key personnel DOB
                NgWebElement openKeyPersonnelDOB = ngDriver.FindElement(By.CssSelector("input[formControlName=\"dateofBirthNew\"]"));
                openKeyPersonnelDOB.Click();

                // select the date
                SharedCalendarDate();

                /********** Individual Shareholder **********/

                // create the shareholder data
                string shareholderFirstName = "IndividualShareholder1";
                string shareholderLastName = "PrivateCorp";
                string shareholderVotingShares = "500";
                string shareholderEmail = "individualshareholder1@privatecorp.com";

                // open shareholder form    
                NgWebElement uiOpenShare = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section/app-associate-list/div/button"));
                uiOpenShare.Click();

                // enter shareholder first name
                NgWebElement uiShareFirst = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName=\"firstNameNew\"]"));
                uiShareFirst.SendKeys(shareholderFirstName);

                // enter shareholder last name
                NgWebElement uiShareLast = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='lastNameNew']"));
                uiShareLast.SendKeys(shareholderLastName);

                // enter number of voting shares
                NgWebElement uiShareVotes = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='numberofSharesNew']"));
                uiShareVotes.SendKeys(shareholderVotingShares);

                // enter shareholder email
                NgWebElement uiShareEmail = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='emailNew']"));
                uiShareEmail.SendKeys(shareholderEmail);

                // enter shareholder DOB
                NgWebElement uiCalendarS1 = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='dateofBirthNew']"));
                uiCalendarS1.Click();

                // select the date
                SharedCalendarDate();

                /********** Business Shareholder **********/

                // create the business shareholder data
                string businessName = "Business Shareholder 1";
                string businessVotingShares = "50";
                string businessEmail = "business@shareholder1.com";

                // open business shareholder form    
                NgWebElement uiOpenShareBiz = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/button"));
                uiOpenShareBiz.Click();

                // enter business name
                NgWebElement uiShareFirstBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='businessNameNew']"));
                uiShareFirstBiz.SendKeys(businessName);

                // enter business voting shares
                NgWebElement uiShareVotesBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='numberofSharesNew']"));
                uiShareVotesBiz.SendKeys(businessVotingShares);

                // select the business type
                NgWebElement uiShareBizType = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/select/option[2]"));
                uiShareBizType.Click();

                // enter business shareholder email
                NgWebElement uiShareEmailBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='emailNew']"));
                uiShareEmailBiz.SendKeys(businessEmail);

                // select the business shareholder confirm button
                NgWebElement uiShareBizConfirmButton = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[5]/i/span"));
                uiShareBizConfirmButton.Click();

                // upload a notice of articles document for business shareholder
                string noticeOfArticlesBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
                NgWebElement uploadNoticeofArticlesBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[12]"));
                uploadNoticeofArticlesBiz.SendKeys(noticeOfArticlesBiz);

                // upload a central securities register document for business shareholder
                string centralSecuritiesRegisterBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
                NgWebElement uploadCentralSecRegBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
                uploadCentralSecRegBiz.SendKeys(centralSecuritiesRegisterBiz);

                // upload a special rights and restrictions document for business shareholder
                string specialRightsRestrictionsBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
                NgWebElement uploadSpecialRightsResBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[18]"));
                uploadSpecialRightsResBiz.SendKeys(specialRightsRestrictionsBiz);

                /********** Business Shareholder - Key Personnel **********/

                // create business shareholder key personnel data
                string keyPersonnelFirstNameBiz = "KeyPersonnel2";
                string keyPersonnelLastNameBiz = "BizShareholderPrivateCorp";
                string keyPersonnelTitleBiz = "Event Planner";
                string keyPersonnelEmailBiz = "keypersonnel2bizshareholder@privatecorp.com";

                // open business shareholder > key personnel form
                NgWebElement openKeyPersonnelFormBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/button"));
                openKeyPersonnelFormBiz.Click();

                // enter business shareholder > key personnel first name
                NgWebElement uiKeyPersonFirstBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
                uiKeyPersonFirstBiz.SendKeys(keyPersonnelFirstNameBiz);

                // enter business shareholder > key personnel last name
                NgWebElement uiKeyPersonLastBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='lastNameNew']"));
                uiKeyPersonLastBiz.SendKeys(keyPersonnelLastNameBiz);

                // select business shareholder > key personnel role
                NgWebElement uiKeyPersonRoleBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
                uiKeyPersonRoleBiz.Click();

                // enter business shareholder > key personnel title
                NgWebElement uiKeyPersonTitleBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='titleNew']"));
                uiKeyPersonTitleBiz.SendKeys(keyPersonnelTitleBiz);

                // enter business shareholder > key personnel email 
                NgWebElement uiKeyPersonEmailBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='emailNew']"));
                uiKeyPersonEmailBiz.SendKeys(keyPersonnelEmailBiz);

                // enter business shareholder > key personnel DOB
                NgWebElement uiKeyPersonnelDOB1Biz1 = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='dateofBirthNew']"));
                uiKeyPersonnelDOB1Biz1.Click();

                // select the date
                SharedCalendarDate();

                /********** Business Shareholder - Individual Shareholder **********/

                // create the business shareholder > individual shareholder data
                string shareholderFirstNameBiz = "IndividualShareholder2";
                string shareholderLastNameBiz = "BizShareholderPrivateCorp";
                string shareholderVotingSharesBiz = "500";
                string shareholderEmailBiz = "individualshareholder2bizshareholder@privatecorp.com";

                // open business shareholder > individual shareholder form
                NgWebElement uiOpenIndyShareBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/button"));
                uiOpenIndyShareBiz.Click();

                // enter business shareholder > individual shareholder first name
                NgWebElement uiIndyShareFirstBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
                uiIndyShareFirstBiz.SendKeys(shareholderFirstNameBiz);

                // enter business shareholder > individual shareholder last name
                NgWebElement uiIndyShareLastBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
                uiIndyShareLastBiz.SendKeys(shareholderLastNameBiz);

                // enter business shareholder > individual number of voting shares
                NgWebElement uiIndyShareVotesBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
                uiIndyShareVotesBiz.SendKeys(shareholderVotingSharesBiz);

                // enter business shareholder > individual shareholder email
                NgWebElement uiIndyShareEmailBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
                uiIndyShareEmailBiz.SendKeys(shareholderEmailBiz);

                // enter business shareholder > individual shareholder DOB
                NgWebElement uiCalendarIndyS1Biz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
                uiCalendarIndyS1Biz.Click();

                // select the date
                SharedCalendarDate();
            }

            if (businessTypeShared == "sole proprietorship")
            {
                // open the leader row   
                NgWebElement openLeaderForm = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[2]/app-associate-list/div/button"));
                openLeaderForm.Click();

                // create the leader info
                string firstName = "Leader";
                string lastName = "SoleProprietor";
                string email = "leader@soleproprietor.com";

                // enter the leader first name
                NgWebElement uiFirstName = ngDriver.FindElement(By.XPath("//input[@type='text']"));
                uiFirstName.SendKeys(firstName);

                // enter the leader last name
                NgWebElement uiLastName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
                uiLastName.SendKeys(lastName);

                // enter the leader email
                NgWebElement uiEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
                uiEmail.SendKeys(email);

                // select the leader DOB
                NgWebElement openLeaderDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
                openLeaderDOB.Click();

                // select the date
                SharedCalendarDate();
            }

            if (businessTypeShared == "society")
            {
                // create society data
                string membershipFee = "2500";
                string membershipNumber = "200";

                // enter Annual Membership Fee
                NgWebElement uiMemberFee = ngDriver.FindElement(By.XPath("//input[@type='text']"));
                uiMemberFee.SendKeys(membershipFee);

                // enter Number of Members
                NgWebElement uiMemberNumber = ngDriver.FindElement(By.XPath("(//input[@type='number'])"));
                uiMemberNumber.SendKeys(membershipNumber);

                // open the director row 
                NgWebElement openKeyPersonnelForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[3]/section/app-associate-list/div/button"));
                openKeyPersonnelForm.Click();

                // create the director info
                string firstName = "Director";
                string lastName = "Society";
                string title = "Chair";
                string email = "director@society.com";

                // enter the director first name
                NgWebElement uiFirstName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
                uiFirstName.SendKeys(firstName);

                // enter the director last name
                NgWebElement uiLastName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
                uiLastName.SendKeys(lastName);

                // select the director position
                NgWebElement uiPosition = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
                uiPosition.Click();

                // enter the director title
                NgWebElement uiTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
                uiTitle.SendKeys(title);

                // enter the director email
                NgWebElement uiEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
                uiEmail.SendKeys(email);

                // select the director DOB
                NgWebElement openKeyPersonnelDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
                openKeyPersonnelDOB.Click();

                // select the date
                SharedCalendarDate();
            }

            if (businessTypeShared == "public corporation")
            {
                // create the key personnel data
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
                NgWebElement uploadNOA = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
                uploadNOA.SendKeys(NOAPath);

                // open key personnel form
                NgWebElement openKeyPersonnelForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[3]/section/app-associate-list/div/button"));
                openKeyPersonnelForm.Click();

                // enter key personnel first name
                NgWebElement openKeyPersonnelFirst = ngDriver.FindElement(By.XPath("//input[@type='text']"));
                openKeyPersonnelFirst.SendKeys(keyPersonnelFirst);

                // enter key personnel last name
                NgWebElement openKeyPersonnelLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
                openKeyPersonnelLast.SendKeys(keyPersonnelLast);

                // select key personnel role
                NgWebElement openKeyPersonnelRole = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
                openKeyPersonnelRole.Click();

                // enter key personnel title
                NgWebElement openKeyPersonnelTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
                openKeyPersonnelTitle.SendKeys(keyPersonnelTitle);

                // enter key personnel email
                NgWebElement openKeyPersonnelEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
                openKeyPersonnelEmail.SendKeys(keyPersonnelEmail);

                // select key person DOB
                NgWebElement openKeyPersonDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
                openKeyPersonDOB.Click();

                // select the date
                SharedCalendarDate();
            }

            if (businessTypeShared == "partnership")
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
                NgWebElement uploadPartnershipAgreement = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
                uploadPartnershipAgreement.SendKeys(partnershipPath);

                // open partner row
                NgWebElement uiPartnerRow = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[3]/section/app-associate-list/div/button"));
                uiPartnerRow.Click();

                // enter partner first name
                NgWebElement uiPartnerFirst = ngDriver.FindElement(By.XPath("//input[@type='text']"));
                uiPartnerFirst.SendKeys(partnerFirstName);

                // enter partner last name
                NgWebElement uiPartnerLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
                uiPartnerLast.SendKeys(partnerLastName);

                // enter partner percentage
                NgWebElement uiPartnerPercentage = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
                uiPartnerPercentage.SendKeys(partnerPercentage);

                // enter partner email
                NgWebElement uiPartnerEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
                uiPartnerEmail.SendKeys(partnerEmail);

                // enter partner DOB
                NgWebElement openPartnerDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
                openPartnerDOB.Click();

                // select the date
                SharedCalendarDate();

                // open business partner row
                NgWebElement openPartnerRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[3]/section[2]/app-associate-list/div/button"));
                openPartnerRow.Click();

                // create business partner info
                string bizPartnerName = "Business Partner";
                string bizPartnerPercentage = "50";
                string bizPartnerEmail = "business@partner.com";

                // enter the business partner name
                NgWebElement uiBizPartnerName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
                uiBizPartnerName.SendKeys(bizPartnerName);

                // enter the business partner percentage
                NgWebElement uiBizPartnerPercentage = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
                uiBizPartnerPercentage.SendKeys(bizPartnerPercentage);

                // select the business type using dropdown
                NgWebElement uiShareBizType = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[3]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/select/option[4]"));
                uiShareBizType.Click();

                // enter the business partner email
                NgWebElement uiBizPartnerEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
                uiBizPartnerEmail.SendKeys(bizPartnerEmail);

                // click on the business shareholder confirm button
                NgWebElement uiConfirmButton = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[3]/section[2]/app-associate-list/div/table/tr/td[5]/i[1]"));
                uiConfirmButton.Click();

                // upload a second partnership agreement
                string partnershipPath2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "partnership_agreement.pdf");
                NgWebElement uploadPartnership2Agreement = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
                uploadPartnership2Agreement.SendKeys(partnershipPath2);

                // open individual partner 2 row
                NgWebElement openPartner2Row = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section[1]/app-associate-list/div/button"));
                openPartner2Row.Click();

                // create individual partner 2 info
                string partner2FirstName = "Individual";
                string partner2LastName = "Partner2";
                string partner2Percentage = "502";
                string partner2Email = "individual@partner2.com";

                // enter individual partner2 first name
                NgWebElement uiPartner2First = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
                uiPartner2First.SendKeys(partner2FirstName);

                // enter individual partner2 last name
                NgWebElement uiPartner2Last = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
                uiPartner2Last.SendKeys(partner2LastName);

                // enter individual partner2 percentage
                NgWebElement uiPartner2Percentage = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
                uiPartner2Percentage.SendKeys(partner2Percentage);

                // enter individual partner2 email
                NgWebElement uiPartner2Email = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
                uiPartner2Email.SendKeys(partner2Email);

                // enter individual partner2 DOB
                NgWebElement openPartner2DOB = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
                openPartner2DOB.Click();

                // select the date
                SharedCalendarDate();

                // click on individual partner2 confirm button
                NgWebElement uiConfirmButton2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section[1]/app-associate-list/div/table/tr/td[6]/i[1]"));
                uiConfirmButton2.Click();
            }

            if (businessTypeShared == "indigenous nation")
            {
                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload the associates document
                string associatesPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "associates.pdf");
                NgWebElement uploadAssociates = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
                uploadAssociates.SendKeys(associatesPath);
            }
        }


        [And(@"I click on the Complete Organization Information button")]
        public void CompleteOrgInfo()
        {
            // click on the complete organzation information button
            NgWebElement orgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,'COMPLETE ORGANIZATION INFORMATION')]"));
            orgInfoButton.Click();
        }


        [And(@"I click on the Pay for Application button")]
        public void ClickOnPayButton()
        {
            NgWebElement payButton = ngDriver.FindElement(By.XPath("//button[contains(.,'Pay for Application')]"));
            payButton.Click();
        }


        [And(@"I submit the organization structure")]
        public void SubmitOrgStructure()
        {
            // click on the Submit Org Info button
            NgWebElement submitOrgInfoButton = ngDriver.FindElement(By.XPath("//button[text()=' SUBMIT ORGANIZATION INFORMATION']"));
            submitOrgInfoButton.Click();
        }


        [And(@"I click on the Submit button")]
        public void ClickOnSubmitButton()
        {
            NgWebElement submitButton = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT')]"));
            submitButton.Click();
        }

        public void ClickOnSubmitButton2()
        {
            NgWebElement submitButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT')]"));
            submitButton.Click();
        }


        [And(@"I click on the licence download link")]
        public void ClickLicenceDownloadLink()
        {
            string downloadLink = "Download Licence";

            // click on the Licences link
            NgWebElement uiDownloadLicence = ngDriver.FindElement(By.LinkText(downloadLink));
            uiDownloadLicence.Click();
        }


        [And(@"I show the store as open on the map")]
        public void ShowStoreOpenOnMap()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            string showOpenOnMap = "Show Store as Open on Map";

            // click on the Show Store as Open on Map link
            NgWebElement uiShowOpenOnMap = ngDriver.FindElement(By.LinkText(showOpenOnMap));
            uiShowOpenOnMap.Click();

            /* 
            Page Title: Apply for a cannabis licence
            */

            System.Threading.Thread.Sleep(7000);

            string dashboard = "Dashboard";

            // click on the Dashboard link
            NgWebElement uiDashboard = ngDriver.FindElement(By.LinkText(dashboard));
            uiDashboard.Click(); 
        }


        [And(@"I click on the Licences tab for (.*)")]
        public void ClickOnLicencesTab(string applicationType)
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            applicationTypeShared = applicationType;

            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }
    }
}
