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
    public abstract class TestBase : Feature
    {
        protected RemoteWebDriver driver;

        // Protractor driver
        protected NgWebDriver ngDriver;

        //protected FirefoxDriverService driverService;

        protected IConfigurationRoot configuration;

        protected string baseUri;

        protected string businessTypeShared;

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
                options.AddArguments("headless", "no-sandbox", "disable-web-security", "no-zygote", "disable-gpu");
            }
            else
            {
                options.AddArguments("--start-maximized");
            }

            driver = new ChromeDriver(path, options);

            //driver = new FirefoxDriver(FirefoxDriverService);
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(60);

            ngDriver = new NgWebDriver(driver);

            baseUri = configuration["baseUri"] ?? "https://dev.justice.gov.bc.ca/cannabislicensing";
        }

        public void CarlaLoginNoCheck()
        {
            // load the dashboard page
            string test_start = configuration["test_start"];

            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");

            ngDriver.WaitForAngular();
        }

        public void CarlaLogin(string businessType)
        {
            businessTypeShared = businessType;
            
            // load the dashboard page
            string test_start = configuration["test_start"];

            ngDriver.Navigate().GoToUrl($"{baseUri}{test_start}");

            ngDriver.WaitForAngular();

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
                NgWebElement privateCorporationRadio = ngDriver.FindElement(By.Name("InitialBusinessType"));
                privateCorporationRadio.Click();
            }

            // if this is a public corporation, click the radio button
            if (businessType == "public corporation")
            {
                NgWebElement publicCorporationRadio = ngDriver.FindElement(By.XPath("(//input[@name='InitialBusinessType'])[2]"));
                publicCorporationRadio.Click();
            }

            // if this is a sole proprietorship, click the radio button
            if (businessType == "sole proprietorship")
            {
                NgWebElement soleProprietorshipRadio = ngDriver.FindElement(By.XPath("(//input[@name='InitialBusinessType'])[3]"));
                soleProprietorshipRadio.Click();
            }

            // if this is a partnership, click the radio button
            if (businessType == "partnership")
            {
                NgWebElement partnershipRadio = ngDriver.FindElement(By.XPath("(//input[@name='InitialBusinessType'])[4]"));
                partnershipRadio.Click();
            }

            // if this is a society, click the radio button
            if (businessType == "society")
            {
                NgWebElement societyRadio = ngDriver.FindElement(By.XPath("(//input[@name='InitialBusinessType'])[5]"));
                societyRadio.Click();
            }

            // if this is an indigenous nation, click the radio button
            if (businessType == "indigenous nation")
            {
                NgWebElement indigenousNationRadio = ngDriver.FindElement(By.XPath("(//input[@name='InitialBusinessType'])[6]"));
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
        }

        public void MakePayment()
        {
            string testCC = configuration["test_cc"];
            string testCVD = configuration["test_ccv"];

            System.Threading.Thread.Sleep(9000);

            //browser sync - don't wait for Angular
            ngDriver.IgnoreSynchronization = true;

            /* 
            Page Title: Internet Payments Program (Bambora)
            */

            driver.FindElementByName("trnCardNumber").SendKeys(testCC);

            driver.FindElementByName("trnCardCvd").SendKeys(testCVD);

            driver.FindElementByName("submitButton").Click();

            System.Threading.Thread.Sleep(3000);

            //turn back on when returning to Angular
            ngDriver.IgnoreSynchronization = false;
        }
        public void CarlaDeleteCurrentAccount()
        {
            
            string deleteAccountURL = $"{baseUri}api/accounts/delete/current";
            string script = $"return fetch(\"{deleteAccountURL}\", {{method: \"POST\", body: {{}}}})";

            var  deleteResult = ngDriver.ExecuteScript(script);
            var obj = JsonConvert.SerializeObject(deleteResult);
            var json = JsonConvert.DeserializeObject<Dictionary<string,object>>(obj);
            //bool success = (Int64)json["status"] == 404 || (Newtonsoft.Json.Linq.JObject)json["text"] == new Newtonsoft.Json.Linq.JObject("OK");
            //bool success = (Int64)json["status"] != 500;
            //Assert.True(success);

            // note that the above call to delete the account will take a period of time to execute.            
            ngDriver.Navigate().GoToUrl($"{baseUri}logout");
        }

        public void CRSEligibilityDisclosure()
        {
            /* 
            Page Title: Cannabis Retail Store Licence Eligibility Disclosure
            */

            // select response:
            // On or after March 1, 2020, did you or any of your associates own, operate, provide financial support to, or receive income from an unlicensed cannabis retail store or retailer?
            try
            {
                // select No using radio button
                //NgWebElement noRadio1 = ngDriver.FindElement(By.Id("mat-radio-3"));
                //noRadio1.Click();

                // select Yes radio button 
                NgWebElement yesRadio1 = ngDriver.FindElement(By.Id("mat-radio-2"));
                yesRadio1.Click();
            }
            catch (NoSuchElementException)
            {
            }

            // complete field:
            // Please indicate the name and location of the retailer or store
            try
            {
                string nameAndLocation = "Automated test name and location of retailer";

                if (businessTypeShared == "partnership")
                {
                    NgWebElement uiNameAndLocation = ngDriver.FindElement(By.XPath("(//input[@type='text'])[37]"));
                    uiNameAndLocation.SendKeys(nameAndLocation);
                }
                else if (businessTypeShared == "indigenous nation")
                {
                    NgWebElement uiNameAndLocation = ngDriver.FindElement(By.XPath("(//input[@type='text'])[46]"));
                    uiNameAndLocation.SendKeys(nameAndLocation);
                }
                else if (businessTypeShared == "sole proprietorship")
                {
                    NgWebElement uiNameAndLocation = ngDriver.FindElement(By.XPath("(//input[@type='text'])[47]"));
                    uiNameAndLocation.SendKeys(nameAndLocation);
                }
                else
                {
                    NgWebElement uiNameAndLocation = ngDriver.FindElement(By.XPath("(//input[@type='text'])[49]"));
                    uiNameAndLocation.SendKeys(nameAndLocation);
                }       
            }
            catch (NoSuchElementException)
            {
            }

            // select response:
            // Does the retailer or store continue to operate?
            try
            {
                // select Yes for Question 2 using radio button
                NgWebElement noRadio2 = ngDriver.FindElement(By.Id("mat-radio-5"));
                noRadio2.Click();
            }
            catch (NoSuchElementException)
            {
            }

            // select response:
            // On or after March 1, 2020, were you or any of your associates involved with the distribution or supply of cannabis to a licensed or unlicensed cannabis retail store or retailer?
            try
            {
                // select No using radio button
                //NgWebElement noRadio2 = ngDriver.FindElement(By.Id("mat-radio-9"));
                //noRadio2.Click();

                // select Yes using radio button
                NgWebElement yesRadio2 = ngDriver.FindElement(By.Id("mat-radio-8"));
                yesRadio2.Click();
            }
            catch (NoSuchElementException)
            {
            }

            // complete field:
            // Please indicate the details of your involvement

            try
            {
                string involvementDetails = "Automated test - details of the involvement";
                
                NgWebElement matCheckbox = ngDriver.FindElement(By.XPath("//mat-dialog-container[@id='mat-dialog-0']/app-eligibility-form/div/form/div[4]/div/app-field/section/div/section/textarea"));
                matCheckbox.SendKeys(involvementDetails);
            }
            catch (NoSuchElementException)
            {
            }

            // complete field: 
            // Please indicate the name and location of the retailer or store
            try
            {
                string nameAndLocation2 = "Automated test name and location of retailer (2)";

                if (businessTypeShared == "partnership")
                {
                    NgWebElement uiNameAndLocation2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[39]"));
                    uiNameAndLocation2.SendKeys(nameAndLocation2);
                }
                else if (businessTypeShared == "sole proprietorship")
                {
                    NgWebElement uiNameAndLocation2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[49]"));
                    uiNameAndLocation2.SendKeys(nameAndLocation2);
                }
                else
                {
                    NgWebElement uiNameAndLocation2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[51]"));
                    uiNameAndLocation2.SendKeys(nameAndLocation2);
                }
            }
            catch (NoSuchElementException)
            {
            }

            // select response:
            // Do you continue to be involved?
            try
            {
                // select Yes for Question 2 using radio button
                NgWebElement noRadio2 = ngDriver.FindElement(By.Id("mat-radio-11"));
                noRadio2.Click();
            }
            catch (NoSuchElementException)
            {
            }

            // select certification checkbox
            try
            {
                NgWebElement noRadio2 = ngDriver.FindElement(By.Id("mat-checkbox-1"));
                noRadio2.Click();
            }
            catch (NoSuchElementException)
            {
            }

            // enter the electronic signature
            try
            {
                string electricSignature = "Automated Test";

                NgWebElement sigCheckbox = ngDriver.FindElement(By.Id("eligibilitySignature"));
                sigCheckbox.SendKeys(electricSignature);
            }
            catch (NoSuchElementException)
            {
            }

            // click on the Submit button
            try
            {
                NgWebElement submit_button = ngDriver.FindElement(By.XPath("//button[text()='SUBMIT']"));
                submit_button.Click();
            }
            catch (NoSuchElementException)
            {
            }
        }

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
            string mailCountry = "Switzerland";

            string bizPhoneNumber = "2501811818";
            string bizEmail = "test@automation.com";
            string corpGiven = "CorpGiven";
            string corpSurname = "CorpSurname";
            string corpTitle = "CEO";
            string corpContactPhone = "7781811818";
            string corpContactEmail = "automated@test.com";

            // enter the Business Number
            if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiBizNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
                uiBizNumber.SendKeys(bizNumber);
            }
            else
            {
                NgWebElement uiBizNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
                uiBizNumber.SendKeys(bizNumber);
            }

            // enter the private corporation incorporation number
            if (businessTypeShared == "private corporation")
            {
                NgWebElement uiCorpNumber = ngDriver.FindElement(By.Id("bcIncorporationNumber"));
                uiCorpNumber.SendKeys(incorporationNumber);


            }

            // enter the society incorporation number
            if (businessTypeShared == "society")
            {
                NgWebElement uiSocietyIncNumber = ngDriver.FindElement(By.Id("bcIncorporationNumber"));
                uiSocietyIncNumber.SendKeys(incorporationNumber);
            }

            // enter the date of incorporation in B.C. 
            if ((businessTypeShared == "private corporation") || (businessTypeShared == "society"))
            {
                NgWebElement uiCalendar1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
                uiCalendar1.Click();

                NgWebElement uiCalendar2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[1]/td[2]/div"));
                uiCalendar2.Click();
            }

            // enter the physical street address 1
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiPhysStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
                uiPhysStreetAddress1.SendKeys(physStreetAddress1);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiPhysStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
                uiPhysStreetAddress1.SendKeys(physStreetAddress1);
            }
            else
            {
                NgWebElement uiPhysStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
                uiPhysStreetAddress1.SendKeys(physStreetAddress1);
            }

            // enter the physical street address 2
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiPhysStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
                uiPhysStreetAddress2.SendKeys(physStreetAddress2);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiPhysStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
                uiPhysStreetAddress2.SendKeys(physStreetAddress2);
            }
            else
            {
                NgWebElement uiPhysStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
                uiPhysStreetAddress2.SendKeys(physStreetAddress2);
            }

            // enter the physical city
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiPhysCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
                uiPhysCity.SendKeys(physCity);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiPhysCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
                uiPhysCity.SendKeys(physCity);
            }
            else
            {
                NgWebElement uiPhysCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
                uiPhysCity.SendKeys(physCity);
            }

            // enter the physical postal code
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiPhysPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
                uiPhysPostalCode.SendKeys(physPostalCode);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiPhysPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
                uiPhysPostalCode.SendKeys(physPostalCode);
            }
            else
            {
                NgWebElement uiPhysPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
                uiPhysPostalCode.SendKeys(physPostalCode);
            }

            /* switching off use of checkbox "Same as physical address" in order to test mailing address fields
            NgWebElement uiSameAsMailingAddress = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiSameAsMailingAddress.Click(); */

            // enter the mailing street address 1
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiMailingStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
                uiMailingStreetAddress1.SendKeys(mailStreet1);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiMailingStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
                uiMailingStreetAddress1.SendKeys(mailStreet1);
            }
            else
            {
                NgWebElement uiMailingStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
                uiMailingStreetAddress1.SendKeys(mailStreet1);
            }

            // enter the mailing street address 2
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiMailingStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[11]"));
                uiMailingStreetAddress2.SendKeys(mailStreet2);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiMailingStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
                uiMailingStreetAddress2.SendKeys(mailStreet2);
            }
            else
            {
                NgWebElement uiMailingStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
                uiMailingStreetAddress2.SendKeys(mailStreet2);
            }

            // enter the mailing city
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiMailingCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
                uiMailingCity.SendKeys(mailCity);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiMailingCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[11]"));
                uiMailingCity.SendKeys(mailCity);
            }
            else
            {
                NgWebElement uiMailingCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[14]"));
                uiMailingCity.SendKeys(mailCity);
            }

            // enter the mailing province
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiMailingProvince = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
                uiMailingProvince.SendKeys(mailProvince);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiMailingProvince = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
                uiMailingProvince.SendKeys(mailProvince);
            }
            else
            {
                NgWebElement uiMailingProvince = ngDriver.FindElement(By.XPath("(//input[@type='text'])[15]"));
                uiMailingProvince.SendKeys(mailProvince);
            }

            // enter the mailing postal code
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiMailingPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[14]"));
                uiMailingPostalCode.SendKeys(mailPostalCode);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiMailingPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
                uiMailingPostalCode.SendKeys(mailPostalCode);
            }
            else
            {
                NgWebElement uiMailingPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[16]"));
                uiMailingPostalCode.SendKeys(mailPostalCode);
            }

            // enter the mailing country
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiMailingCountry = ngDriver.FindElement(By.XPath("(//input[@type='text'])[15]"));
                uiMailingCountry.SendKeys(mailCountry);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiMailingCountry = ngDriver.FindElement(By.XPath("(//input[@type='text'])[14]"));
                uiMailingCountry.SendKeys(mailCountry);
            }
            else
            {
                NgWebElement uiMailingCountry = ngDriver.FindElement(By.XPath("(//input[@type='text'])[17]"));
                uiMailingCountry.SendKeys(mailCountry);
            }

            // enter the business phone number
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiBizPhoneNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[16]"));
                uiBizPhoneNumber.SendKeys(bizPhoneNumber);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiBizPhoneNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[15]"));
                uiBizPhoneNumber.SendKeys(bizPhoneNumber);
            }
            else
            {
                NgWebElement uiBizPhoneNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[18]"));
                uiBizPhoneNumber.SendKeys(bizPhoneNumber);
            }

            // enter the business email
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiBizEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[17]"));
                uiBizEmail.SendKeys(bizEmail);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiBizEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[16]"));
                uiBizEmail.SendKeys(bizEmail);
            }
            else
            {
                NgWebElement uiBizEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[19]"));
                uiBizEmail.SendKeys(bizEmail);
            }

            // (re)enter the first name of corporation contact
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiCorpGiven = ngDriver.FindElement(By.XPath("(//input[@type='text'])[18]"));
                uiCorpGiven.SendKeys(corpGiven);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiCorpGiven = ngDriver.FindElement(By.XPath("(//input[@type='text'])[17]"));
                uiCorpGiven.SendKeys(corpGiven);
            }
            else
            {
                NgWebElement uiCorpGiven = ngDriver.FindElement(By.XPath("(//input[@type='text'])[20]"));
                uiCorpGiven.SendKeys(corpGiven);
            }

            // (re)enter the last name of corporation contact
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiCorpSurname = ngDriver.FindElement(By.XPath("(//input[@type='text'])[19]"));
                uiCorpSurname.SendKeys(corpSurname);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiCorpSurname = ngDriver.FindElement(By.XPath("(//input[@type='text'])[18]"));
                uiCorpSurname.SendKeys(corpSurname);
            }
            else
            {
                NgWebElement uiCorpSurname = ngDriver.FindElement(By.XPath("(//input[@type='text'])[21]"));
                uiCorpSurname.SendKeys(corpSurname);
            }

            // enter the corporation contact title
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiCorpTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[20]"));
                uiCorpTitle.SendKeys(corpTitle);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiCorpTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[19]"));
                uiCorpTitle.SendKeys(corpTitle);
            }
            else
            {
                NgWebElement uiCorpTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[22]"));
                uiCorpTitle.SendKeys(corpTitle);
            }

            // enter the corporation contact phone number
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiCorpContactPhone = ngDriver.FindElement(By.XPath("(//input[@type='text'])[21]"));
                uiCorpContactPhone.SendKeys(corpContactPhone);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiCorpContactPhone = ngDriver.FindElement(By.XPath("(//input[@type='text'])[20]"));
                uiCorpContactPhone.SendKeys(corpContactPhone);
            }
            else
            {
                NgWebElement uiCorpContactPhone = ngDriver.FindElement(By.XPath("(//input[@type='text'])[23]"));
                uiCorpContactPhone.SendKeys(corpContactPhone);
            }

            // enter the corporation contact phone email
            if ((businessTypeShared == "partnership") || (businessTypeShared == "sole proprietorship"))
            {
                NgWebElement uiCorpContactEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[22]"));
                uiCorpContactEmail.SendKeys(corpContactEmail);
            }
            else if (businessTypeShared == "indigenous nation")
            {
                NgWebElement uiCorpContactEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[21]"));
                uiCorpContactEmail.SendKeys(corpContactEmail);
            }
            else
            {
                NgWebElement uiCorpContactEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[24]"));
                uiCorpContactEmail.SendKeys(corpContactEmail);
            }

            // select 'No' for corporation's connection to a federal producer
            NgWebElement corpConnectionFederalProducer = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[2]"));
            corpConnectionFederalProducer.Click();

            // select 'No' for federal producer's connection to corporation
            if ((businessTypeShared != "indigenous nation") && (businessTypeShared != "society"))
            {
                NgWebElement federalProducerConnectionToCorp = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[4]"));
                federalProducerConnectionToCorp.Click();
            }

            // click on Continue to Organization Review button
            NgWebElement continueApp_button = ngDriver.FindElement(By.Id("continueToApp"));
            continueApp_button.Click();
        }

        public void CRSReturnToDashboard()
        {
            /* 
            Page Title: Payment Approved
            */

            // confirm that payment receipt is for $7,500.00
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$7,500.00')]")).Displayed);

            // click on Return to Dashboard link
            string retDash = "Return to Dashboard";
            NgWebElement returnDash = ngDriver.FindElement(By.LinkText(retDash));
            returnDash.Click();
        }

        public void CateringReturnToDashboard()
        {
            /* 
            Page Title: Payment Approved
            */

            // confirm that payment receipt is for $475.00
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$475.00')]")).Displayed);

            string retDash = "Return to Dashboard";

            // click on the Return to Dashboard link
            NgWebElement returnDash = ngDriver.FindElement(By.LinkText(retDash));
            returnDash.Click();
        }

        public void CateringApplication()
        {
            /* 
                Page Title: Catering Licence Application
            */

            // create application info
            string prevAppDetails = "Here are the previous application details (test).";
            string liqConnectionDetails = "Here are the liquor industry connection details (test).";
            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A6X5";
            string estPID = "012345678";
            string estPhone = "2505555555";
            string otherBizDetails = "Here are the other business details (test).";
            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";

            System.Threading.Thread.Sleep(9000);

            // enter the establishment name
            NgWebElement uiEstabName = ngDriver.FindElement(By.Id("establishmentName"));
            uiEstabName.SendKeys(estName);

            // select 'No' for previous liquor licence
            //NgWebElement uiPreviousLicenceNo = ngDriver.FindElement(By.Id("mat-button-toggle-2-button"));
            //uiPreviousLicenceNo.Click();

            // select 'Yes' for previous liquor licence
            NgWebElement uiPreviousLicenceYes = ngDriver.FindElement(By.Id("mat-button-toggle-1-button"));
            uiPreviousLicenceYes.Click();

            // enter the previous application details
            NgWebElement uiPreviousApplicationDetails = ngDriver.FindElement(By.Id("previousApplicationDetails"));
            uiPreviousApplicationDetails.SendKeys(prevAppDetails);

            // select 'No' for Rural Agency Store Appointment
            //NgWebElement uiRuralStoreNo = ngDriver.FindElement(By.Id("mat-button-toggle-5-button"));
            //uiRuralStoreNo.Click();

            // select 'Yes' for Rural Agency Store Appointment
            NgWebElement uiRuralStoreYes = ngDriver.FindElement(By.Id("mat-button-toggle-4-button"));
            uiRuralStoreYes.Click();

            // select 'No' for distillery, brewery or winery connections
            //NgWebElement uiLiquorProductionNo = ngDriver.FindElement(By.Id("mat-button-toggle-8-button"));
            //uiLiquorProductionNo.Click();

            // select 'Yes' for distillery, brewery or winery connections
            NgWebElement uiLiquorProductionYes = ngDriver.FindElement(By.Id("mat-button-toggle-7-button"));
            uiLiquorProductionYes.Click();

            // enter the liquor industry connections details
            NgWebElement uiLiquorConnectionDetails = ngDriver.FindElement(By.Id("liquorIndustryConnectionsDetails"));
            uiLiquorConnectionDetails.SendKeys(liqConnectionDetails);

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

            // enter the store phone number
            NgWebElement uiEstabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            uiEstabPhone.SendKeys(estPhone);

            // select 'No' for other business on premises
            //NgWebElement uiOtherBusinessNo = ngDriver.FindElement(By.Id("mat-button-toggle-11-button"));
            //uiOtherBusinessNo.Click();

            // select 'Yes' for other business on premises
            NgWebElement uiOtherBusinessYes = ngDriver.FindElement(By.Id("mat-button-toggle-10-button"));
            uiOtherBusinessYes.Click();

            // enter the other business details
            NgWebElement uiOtherBusinessDetails = ngDriver.FindElement(By.Id("otherBusinessesDetails"));
            uiOtherBusinessDetails.SendKeys(otherBizDetails);

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
            NgWebElement uiContactRole = ngDriver.FindElement(By.Id("contactPersonRole"));
            uiContactRole.SendKeys(conRole);

            // enter the phone number of the application contact
            NgWebElement uiContactPhone = ngDriver.FindElement(By.Id("contactPersonPhone"));
            uiContactPhone.SendKeys(conPhone);

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgree.Click();
        }
    }
}
