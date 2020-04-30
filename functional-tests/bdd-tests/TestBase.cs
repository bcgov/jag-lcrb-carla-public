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

            // select response: On or after March 1, 2020, did you or any of your associates own, operate, provide financial support to, or receive income from an unlicensed cannabis retail store or retailer?
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

            // complete field: Please indicate the name and location of the retailer or store
            try
            {
                string nameAndLocation = "Automated test name and location of retailer";

                if (businessTypeShared == "indigenous nation")
                {
                    NgWebElement uiNameAndLocation = ngDriver.FindElement(By.XPath("(//input[@type='text'])[36]"));
                    uiNameAndLocation.SendKeys(nameAndLocation);
                }
                else if ((businessTypeShared == "sole proprietorship") || (businessTypeShared == "partnership"))
                {
                    NgWebElement uiNameAndLocation = ngDriver.FindElement(By.XPath("(//input[@type='text'])[37]"));
                    uiNameAndLocation.SendKeys(nameAndLocation);
                }
                else
                {
                    NgWebElement uiNameAndLocation = ngDriver.FindElement(By.XPath("(//input[@type='text'])[39]"));
                    uiNameAndLocation.SendKeys(nameAndLocation);
                }       
            }
            catch (NoSuchElementException)
            {
            }

            // select response: Does the retailer or store continue to operate?
            try
            {
                // select Yes for Question 2 using radio button
                NgWebElement noRadio2 = ngDriver.FindElement(By.Id("mat-radio-5"));
                noRadio2.Click();
            }
            catch (NoSuchElementException)
            {
            }

            // select response: On or after March 1, 2020, were you or any of your associates involved with the distribution or supply of cannabis to a licensed or unlicensed cannabis retail store or retailer?
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

            // complete field: Please indicate the details of your involvement
            try
            {
                string involvementDetails = "Automated test - details of the involvement";
                
                NgWebElement matCheckbox = ngDriver.FindElement(By.XPath("//textarea"));
                matCheckbox.SendKeys(involvementDetails);
            }
            catch (NoSuchElementException)
            {
            }

            // complete field: Please indicate the name and location of the retailer or store
            try
            {
                string nameAndLocation2 = "Automated test name and location of retailer (2)";

                if ((businessTypeShared == "sole proprietorship") || (businessTypeShared == "partnership"))
                {
                    NgWebElement uiNameAndLocation2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[39]"));
                    uiNameAndLocation2.SendKeys(nameAndLocation2);
                }
                else if (businessTypeShared == "indigenous nation")
                {
                    NgWebElement uiNameAndLocation2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[38]"));
                    uiNameAndLocation2.SendKeys(nameAndLocation2);
                }
                else
                {
                    NgWebElement uiNameAndLocation2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[41]"));
                    uiNameAndLocation2.SendKeys(nameAndLocation2);
                }
            }
            catch (NoSuchElementException)
            {
            }

            // select response: Do you continue to be involved?
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

            // enter the business number
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

            // (re)enter the first name of contact
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

            // (re)enter the last name of contact
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

            // enter the contact title
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

            // enter the contact phone number
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

            // enter the contact email
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

            /*// select 'No' for connection to a federal producer - switched off to test text areas
            NgWebElement corpConnectionFederalProducer = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[2]"));
            corpConnectionFederalProducer.Click();*/

            // select 'Yes' for connection to a federal producer
            NgWebElement corpConnectionFederalProducer = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[1]"));
            corpConnectionFederalProducer.Click();

            // enter the name of the federal producer and details of the connection 
            string nameAndDetails = "Name and details of federal producer (automated test).";

            NgWebElement uiDetailsFederalProducer = ngDriver.FindElement(By.XPath("//textarea"));
            uiDetailsFederalProducer.SendKeys(nameAndDetails);

            /*// select 'No' for federal producer's connection to business - switched off to test text areas
            if ((businessTypeShared != "indigenous nation") && (businessTypeShared != "society"))
            {
                NgWebElement federalProducerConnectionToCorp = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[4]"));
                federalProducerConnectionToCorp.Click();
            }*/

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
            NgWebElement continueApp_button = ngDriver.FindElement(By.Id("continueToApp"));
            continueApp_button.Click();
        }

        public void ReviewOrgStructure()
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
                string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
                uploadSignage.SendKeys(noticeOfArticles);

                // upload a central securities register document
                string centralSecuritiesRegister = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uploadCentralSecReg = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
                uploadCentralSecReg.SendKeys(centralSecuritiesRegister);

                // upload a special rights and restrictions document
                string specialRightsRestrictions = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uploadSpecialRightsRes = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
                uploadSpecialRightsRes.SendKeys(specialRightsRestrictions);

                /********** Key Personnel **********/

                // create the key personnel data
                string keyPersonnelFirstName = "Jane";
                string keyPersonnelLastName = "Bond";
                string keyPersonnelTitle = "Adventurer";
                string keyPersonnelEmail = "jane@bond.com";

                // open key personnel form   
                NgWebElement openKeyPersonnelForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[4]/section/app-associate-list/div/button"));
                openKeyPersonnelForm.Click();

                // enter key personnel first name
                NgWebElement uiKeyPersonFirst = ngDriver.FindElement(By.XPath("//input[@type='text']"));
                uiKeyPersonFirst.SendKeys(keyPersonnelFirstName);

                // enter key personnel last name
                NgWebElement uiKeyPersonLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
                uiKeyPersonLast.SendKeys(keyPersonnelLastName);

                // select key personnel role
                NgWebElement uiKeyPersonRole = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
                uiKeyPersonRole.Click();

                // enter key personnel title
                NgWebElement uiKeyPersonTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
                uiKeyPersonTitle.SendKeys(keyPersonnelTitle);

                // enter key personnel email
                NgWebElement uiKeyPersonEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
                uiKeyPersonEmail.SendKeys(keyPersonnelEmail);

                // enter key personnel DOB
                NgWebElement openKeyPersonnelDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
                openKeyPersonnelDOB.Click();

                NgWebElement openKeyPersonnelDOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-3']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                openKeyPersonnelDOB1.Click();

                /********** Individual Shareholder **********/

                // create the shareholder data
                string shareholderFirstName = "Jacqui";
                string shareholderLastName = "Chan";
                string shareholderVotingShares = "500";
                string shareholderEmail = "jacqui@chan.com";

                // open shareholder form    
                NgWebElement uiOpenShare = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section/app-associate-list/div/button"));
                uiOpenShare.Click();

                // enter shareholder first name
                NgWebElement uiShareFirst = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
                uiShareFirst.SendKeys(shareholderFirstName);

                // enter shareholder last name
                NgWebElement uiShareLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
                uiShareLast.SendKeys(shareholderLastName);

                // enter number of voting shares
                NgWebElement uiShareVotes = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
                uiShareVotes.SendKeys(shareholderVotingShares);

                // enter shareholder email
                NgWebElement uiShareEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
                uiShareEmail.SendKeys(shareholderEmail);

                // enter shareholder DOB
                NgWebElement uiCalendarS1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
                uiCalendarS1.Click();

                NgWebElement uiCalendarS2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-4']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                uiCalendarS2.Click();

                /********** Business Shareholder **********/

                // create the business shareholder data
                string businessName = "Business Shareholder 1";
                string businessVotingShares = "50";
                string businessEmail = "bourne@enterprises.com";

                // open business shareholder form    
                NgWebElement uiOpenShareBiz = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/button"));
                uiOpenShareBiz.Click();

                // enter business name
                NgWebElement uiShareFirstBiz = ngDriver.FindElement(By.XPath("(//input[@type='text'])[11]"));
                uiShareFirstBiz.SendKeys(businessName);

                // enter business voting shares
                NgWebElement uiShareVotesBiz = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
                uiShareVotesBiz.SendKeys(businessVotingShares);

                // select the business type
                NgWebElement uiShareBizType = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/select/option[2]"));
                uiShareBizType.Click();

                // enter business shareholder email
                NgWebElement uiShareEmailBiz = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
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
                string keyPersonnelFirstNameBiz = "Ethel";
                string keyPersonnelLastNameBiz = "Hunt";
                string keyPersonnelTitleBiz = "Climbing Enthusiast";
                string keyPersonnelEmailBiz = "ethel@hunt.com";

                // open business shareholder > key personnel form
                NgWebElement openKeyPersonnelFormBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/button"));
                openKeyPersonnelFormBiz.Click();

                // enter business shareholder > key personnel first name
                NgWebElement uiKeyPersonFirstBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
                uiKeyPersonFirstBiz.SendKeys(keyPersonnelFirstNameBiz);

                // enter business shareholder > key personnel last name
                NgWebElement uiKeyPersonLastBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
                uiKeyPersonLastBiz.SendKeys(keyPersonnelLastNameBiz);

                // select business shareholder > key personnel role
                NgWebElement uiKeyPersonRoleBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
                uiKeyPersonRoleBiz.Click();

                // enter business shareholder > key personnel title
                NgWebElement uiKeyPersonTitleBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
                uiKeyPersonTitleBiz.SendKeys(keyPersonnelTitleBiz);

                // enter business shareholder > key personnel email
                NgWebElement uiKeyPersonEmailBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
                uiKeyPersonEmailBiz.SendKeys(keyPersonnelEmailBiz);

                // enter business shareholder > key personnel DOB
                NgWebElement uiKeyPersonnelDOB1Biz1 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
                uiKeyPersonnelDOB1Biz1.Click();

                NgWebElement uiKeyPersonnelDOB1Biz2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-5']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                uiKeyPersonnelDOB1Biz2.Click();

                /********** Business Shareholder - Individual Shareholder **********/

                // create the business shareholder > individual shareholder data
                string shareholderFirstNameBiz = "Jacintha";
                string shareholderLastNameBiz = "Ryan";
                string shareholderVotingSharesBiz = "500";
                string shareholderEmailBiz = "jacintha@cia.com";

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

                NgWebElement uiCalendarIndyS2Biz = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-6']/div/mat-month-view/table/tbody/tr[4]/td[4]/div"));
                uiCalendarIndyS2Biz.Click();
            }

            if (businessTypeShared == "sole proprietorship")
            {
                // open the leader row                                                           
                NgWebElement openLeaderForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/app-associate-list/div/button"));
                openLeaderForm.Click();

                // create the leader info
                string firstName = "Jane";
                string lastName = "Bond";
                string email = "jane@bond.com";

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

                NgWebElement openLeaderDOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-2']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                openLeaderDOB1.Click();
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
                string firstName = "Jane";
                string lastName = "Bond";
                string title = "Adventurer";
                string email = "jane@bond.com";

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

                NgWebElement openKeyPersonnelDOB1 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-3']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                openKeyPersonnelDOB1.Click();
            }

            if (businessTypeShared == "public corporation")
            {
                // create the key personnel data
                string keyPersonnelFirst = "Jane";
                string keyPersonnelLast = "Bond";
                string keyPersonnelTitle = "Adventurer";
                string keyPersonnelEmail = "jane@bond.com";

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
                NgWebElement openPartnerDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
                openPartnerDOB.Click();

                NgWebElement openKeyPartnerDOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-3']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                openKeyPartnerDOB1.Click();
            }

            if (businessTypeShared == "partnership")
            {
                // create individual partner info
                string partnerFirstName = "Automated";
                string partnerLastName = "Test";
                string partnerPercentage = "50";
                string partnerEmail = "automated@test.com";

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

                NgWebElement openKeyPartnerDOB1 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-2']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                openKeyPartnerDOB1.Click();

                // open business partner row
                NgWebElement openPartnerRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[3]/section[2]/app-associate-list/div/button"));
                openPartnerRow.Click();

                // create business partner info
                string bizPartnerName = "Automated Test";
                string bizPartnerPercentage = "50";
                string bizPartnerEmail = "automated@test.com";

                // enter the business partner name
                NgWebElement uiBizPartnerName = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[3]/section[2]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
                uiBizPartnerName.SendKeys(bizPartnerName);

                // enter the business partner percentage
                NgWebElement uiBizPartnerPercentage = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[3]/section[2]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
                uiBizPartnerPercentage.SendKeys(bizPartnerPercentage);

                // select the business type using dropdown
                NgWebElement uiShareBizType = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[3]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/select/option[4]"));
                uiShareBizType.Click();

                // enter the business partner email
                NgWebElement uiBizPartnerEmail = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[3]/section[2]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
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
                string partner2FirstName = "Automated2";
                string partner2LastName = "Test2";
                string partner2Percentage = "502";
                string partner2Email = "automated2@test.com";

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

                NgWebElement openKeyPartner2DOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-3']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                openKeyPartner2DOB1.Click();
            }
        }

        public void SubmitOrgInfoButton()
        {
            // click on the Submit Org Info button
            NgWebElement submitOrgInfoButton = ngDriver.FindElement(By.XPath("//button[text()=' SUBMIT ORGANIZATION INFORMATION']"));
            submitOrgInfoButton.Click();
        }

        public void CRSApplication()
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
                NgWebElement uiSelectNation = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-multi-stage-application-flow/div/mat-horizontal-stepper/div[2]/div[3]/app-application/div/div[2]/div[2]/section/div/app-field[2]/section/div[1]/section/select"));
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

            // click on the Submit button
            NgWebElement submit_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT')]"));
            submit_button.Click();
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

        public void CateringOrgStructure()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            if (businessTypeShared == "sole proprietorship")
            {
                // open the leader row                                                           
                NgWebElement openLeaderForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/app-associate-list/div/button"));
                openLeaderForm.Click();

                // create the leader info
                string firstName = "Jane";
                string lastName = "Bond";
                string email = "jane@bond.com";

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

                NgWebElement openLeaderDOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                openLeaderDOB1.Click();
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
                string firstName = "Jane";
                string lastName = "Bond";
                string title = "Adventurer";
                string email = "jane@bond.com";

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

                NgWebElement openKeyPersonnelDOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-1']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                openKeyPersonnelDOB1.Click();
            }

            if (businessTypeShared == "public corporation")
            {
                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload NOA form
                string NOAPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uploadNOA = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
                uploadNOA.SendKeys(NOAPath);

                /********** Key Personnel *********/

                // create the key personnel data
                string keyPersonnelFirst = "Jane";
                string keyPersonnelLast = "Bond";
                string keyPersonnelTitle = "Adventurer";
                string keyPersonnelEmail = "jane@bond.com";

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
                NgWebElement openKeyPersonnelDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
                openKeyPersonnelDOB.Click();

                NgWebElement openKeyPersonnelDOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-1']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                openKeyPersonnelDOB1.Click();
            }

            if (businessTypeShared == "partnership")
            {
                // create individual partner info
                string partnerFirstName = "Automated";
                string partnerLastName = "Test";
                string partnerPercentage = "50";
                string partnerEmail = "automated@test.com";

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

                NgWebElement openKeyPartnerDOB1 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                openKeyPartnerDOB1.Click(); 
            }

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

                /********** Key Personnel *********/

                // create the key personnel data
                string keyPersonnelFirstName = "Jane";
                string keyPersonnelLastName = "Bond";
                string keyPersonnelTitle = "Adventurer";
                string keyPersonnelEmail = "jane@bond.com";

                // open key personnel form   
                NgWebElement openKeyPersonnelForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[4]/section/app-associate-list/div/button"));
                openKeyPersonnelForm.Click();

                // enter key personnel first name
                NgWebElement uiKeyPersonFirst = ngDriver.FindElement(By.XPath("//input[@type='text']"));
                uiKeyPersonFirst.SendKeys(keyPersonnelFirstName);

                // enter key personnel last name
                NgWebElement uiKeyPersonLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
                uiKeyPersonLast.SendKeys(keyPersonnelLastName);

                // select key personnel role
                NgWebElement uiKeyPersonRole = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[3]"));
                uiKeyPersonRole.Click();

                // enter key personnel title
                NgWebElement uiKeyPersonTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
                uiKeyPersonTitle.SendKeys(keyPersonnelTitle);

                // enter key personnel email
                NgWebElement uiKeyPersonEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
                uiKeyPersonEmail.SendKeys(keyPersonnelEmail);

                // enter key personnel DOB
                NgWebElement openKeyPersonnelDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
                openKeyPersonnelDOB.Click();

                NgWebElement openKeyPersonnelDOB1 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-1']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                openKeyPersonnelDOB1.Click();

                /********** Individual Shareholder *********/

                // create the individual shareholder data
                string shareholderFirstName = "Jacqui";
                string shareholderLastName = "Chan";
                string shareholderVotingShares = "500";
                string shareholderEmail = "jacqui@chan.com";

                // open individual shareholder form    
                NgWebElement uiOpenShare = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section/app-associate-list/div/button"));
                uiOpenShare.Click();

                // enter individual shareholder first name
                NgWebElement uiShareFirst = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
                uiShareFirst.SendKeys(shareholderFirstName);

                // enter individual shareholder last name
                NgWebElement uiShareLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
                uiShareLast.SendKeys(shareholderLastName);

                // enter individual number of voting shares
                NgWebElement uiShareVotes = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
                uiShareVotes.SendKeys(shareholderVotingShares);

                // enter individual shareholder email
                NgWebElement uiShareEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
                uiShareEmail.SendKeys(shareholderEmail);

                // enter individual shareholder DOB
                NgWebElement uiCalendarS1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
                uiCalendarS1.Click();

                NgWebElement uiCalendarS2 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-2']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                uiCalendarS2.Click();

                /********** Business Shareholder *********/

                // create the business shareholder data
                string businessName = "Bourne Enterprises";
                string businessVotingShares = "50";
                string businessEmail = "bourne@enterprises.com";

                // open business shareholder form    
                NgWebElement uiOpenShareBiz = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/button"));
                uiOpenShareBiz.Click();

                // enter business name
                NgWebElement uiShareFirstBiz = ngDriver.FindElement(By.XPath("(//input[@type='text'])[11]"));
                uiShareFirstBiz.SendKeys(businessName);

                // enter business voting shares
                NgWebElement uiShareVotesBiz = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
                uiShareVotesBiz.SendKeys(businessVotingShares);

                // select the business type
                NgWebElement uiShareBizType = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/select/option[2]"));
                uiShareBizType.Click();

                // enter business shareholder email
                NgWebElement uiShareEmailBiz = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
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

                /********** Business Shareholder - Key Personnel ********/

                // create business shareholder key personnel data
                string keyPersonnelFirstNameBiz = "Ethel";
                string keyPersonnelLastNameBiz = "Hunt";
                string keyPersonnelTitleBiz = "Climbing Enthusiast";
                string keyPersonnelEmailBiz = "ethel@hunt.com";

                // open business shareholder > key personnel form
                NgWebElement openKeyPersonnelFormBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/button"));
                openKeyPersonnelFormBiz.Click();

                // enter business shareholder > key personnel first name
                NgWebElement uiKeyPersonFirstBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
                uiKeyPersonFirstBiz.SendKeys(keyPersonnelFirstNameBiz);

                // enter business shareholder > key personnel last name
                NgWebElement uiKeyPersonLastBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
                uiKeyPersonLastBiz.SendKeys(keyPersonnelLastNameBiz);

                // select business shareholder > key personnel role
                NgWebElement uiKeyPersonRoleBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
                uiKeyPersonRoleBiz.Click();

                // enter business shareholder > key personnel title
                NgWebElement uiKeyPersonTitleBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
                uiKeyPersonTitleBiz.SendKeys(keyPersonnelTitleBiz);

                // enter business shareholder > key personnel email
                NgWebElement uiKeyPersonEmailBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
                uiKeyPersonEmailBiz.SendKeys(keyPersonnelEmailBiz);

                // enter business shareholder > key personnel DOB
                NgWebElement uiKeyPersonnelDOB1Biz1 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
                uiKeyPersonnelDOB1Biz1.Click();

                NgWebElement uiKeyPersonnelDOB1Biz2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-3']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                uiKeyPersonnelDOB1Biz2.Click();

                /********** Business Shareholder - Individual Shareholder *********/

                // create the business shareholder > individual shareholder data
                string shareholderFirstNameBiz = "Jacintha";
                string shareholderLastNameBiz = "Ryan";
                string shareholderVotingSharesBiz = "500";
                string shareholderEmailBiz = "jacintha@cia.com";

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

                NgWebElement uiCalendarIndyS2Biz = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-4']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
                uiCalendarIndyS2Biz.Click();
            }
        }

        public void CateringApplication()
        {
            /* 
                Page Title: Catering Licence Application
            */

            // create application info
            string prevAppDetails = "Here are the previous application details (test).";
            //string liqConnectionDetails = "Here are the liquor industry connection details (test).";
            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A6X5";
            string estPID = "012345678";
            string estPhone = "2505555555";
            string estEmail = "test@automation.com";
            //string otherBizDetails = "Here are the other business details (test).";
            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";
            string conEmail = "test2@automation.com";

            System.Threading.Thread.Sleep(9000);

            // enter the establishment name
            NgWebElement uiEstabName = ngDriver.FindElement(By.Id("establishmentName"));
            uiEstabName.SendKeys(estName);

            // select 'No' for previous liquor licence
            //NgWebElement uiPreviousLicenceNo = ngDriver.FindElement(By.Id("mat-button-toggle-2-button"));
            //uiPreviousLicenceNo.Click();

            // select 'No' for Rural Agency Store Appointment
            //NgWebElement uiRuralStoreNo = ngDriver.FindElement(By.Id("mat-button-toggle-5-button"));
            //uiRuralStoreNo.Click();

            // select 'Yes' for Rural Agency Store Appointment
            //NgWebElement uiRuralStoreYes = ngDriver.FindElement(By.Id("mat-button-toggle-4-button"));
            //uiRuralStoreYes.Click();

            // select 'No' for distillery, brewery or winery connections
            //NgWebElement uiLiquorProductionNo = ngDriver.FindElement(By.Id("mat-button-toggle-8-button"));
            //uiLiquorProductionNo.Click();

            // select 'Yes' for distillery, brewery or winery connections
            //NgWebElement uiLiquorProductionYes = ngDriver.FindElement(By.Id("mat-button-toggle-7-button"));
            //uiLiquorProductionYes.Click();

            // enter the liquor industry connections details
            //NgWebElement uiLiquorConnectionDetails = ngDriver.FindElement(By.Id("liquorIndustryConnectionsDetails"));
            //uiLiquorConnectionDetails.SendKeys(liqConnectionDetails);

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

            // select 'Yes' for previous liquor licence
            NgWebElement uiPreviousLicenceYes = ngDriver.FindElement(By.Id("mat-button-toggle-1-button"));
            uiPreviousLicenceYes.Click();

            // enter the previous application details
            NgWebElement uiPreviousApplicationDetails = ngDriver.FindElement(By.Id("//textarea"));
            uiPreviousApplicationDetails.SendKeys(prevAppDetails);

            // select 'No' for other business on premises
            //NgWebElement uiOtherBusinessNo = ngDriver.FindElement(By.Id("mat-button-toggle-11-button"));
            //uiOtherBusinessNo.Click();

            // select 'Yes' for other business on premises
            //NgWebElement uiOtherBusinessYes = ngDriver.FindElement(By.Id("mat-button-toggle-10-button"));
            //uiOtherBusinessYes.Click();

            // enter the other business details
            //NgWebElement uiOtherBusinessDetails = ngDriver.FindElement(By.Id("otherBusinessesDetails"));
            //uiOtherBusinessDetails.SendKeys(otherBizDetails);

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

            // enter the email of the application contact
            NgWebElement uiContactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            uiContactEmail.SendKeys(conEmail);

            // click on the authorized to submit checkbox
            //NgWebElement uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            //uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            //NgWebElement uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            //uiSignatureAgree.Click();
        }
        public void Dispose()
        {
            ngDriver.Quit();

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        
    }
}
