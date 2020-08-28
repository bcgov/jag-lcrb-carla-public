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
    }
}
