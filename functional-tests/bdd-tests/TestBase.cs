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

            string electricSignature = "Automated Test";

            try
            {
                // select No for Question 1 using radio button
                NgWebElement noRadio1 = ngDriver.FindElement(By.Id("mat-radio-3"));
                noRadio1.Click();
            }
            catch (NoSuchElementException)
            {
            }

            try
            {
                // select No for Question 2 using radio button
                NgWebElement noRadio2 = ngDriver.FindElement(By.Id("mat-radio-9"));
                noRadio2.Click();
            }
            catch (NoSuchElementException)
            {
            }

            try
            {
                // select the certification checkbox
                NgWebElement matCheckbox = ngDriver.FindElement(By.Id("mat-checkbox-1"));
                matCheckbox.Click();
            }
            catch (NoSuchElementException)
            {
            }

            try
            {
                // enter the electronic signature
                NgWebElement sigCheckbox = ngDriver.FindElement(By.Id("eligibilitySignature"));
                sigCheckbox.SendKeys(electricSignature);
            }
            catch (NoSuchElementException)
            {
            }

            try
            {
                // click on the Submit button
                NgWebElement submit_button = ngDriver.FindElement(By.XPath("//button[text()='SUBMIT']"));
                submit_button.Click();
            }
            catch (NoSuchElementException)
            {
            }
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
