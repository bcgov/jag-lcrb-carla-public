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
        [And(@"I complete the Catering application for a (.*)")]
        public void CompleteCateringApplication(string bizType)
        {
            /* 
            Page Title: Catering Licence Application
            */

            // create application info
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
            string prevAppDetails = "Here are the previous application details (automated test).";
            string liqConnectionDetails = "Here are the liquor industry connection details (automated test).";
            string kitchenDetails = "Here are the details of the kitchen equipment.";
            string transportDetails = "Here are the transport details.";

            if (bizType == "partnership")
            {
                // upload a partnership agreement
                FileUpload("partnership_agreement.pdf", "(//input[@type='file'])[3]");

                // upload personal history summary
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[6]");
            }

            /*
            if ((bizType == "private corporation") || (bizType == "society"))
            {
                // upload a central securities register
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                // upload supporting business documentation
                FileUpload("associates.pdf", "(//input[@type='file'])[6]");

                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[9]");
            }

            if (bizType != "sole proprietorship")
            {
                // upload personal history summary documents
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[12]");
            }
            else
            {
                // upload personal history summary documents
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[3]");
            }

            if (bizType != "sole proprietorship")
            {
                // upload shareholders < 10% interest
                FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[15]");
            }
            */

            // enter the establishment name
            NgWebElement uiEstabName = null;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var names = ngDriver.FindElements(By.CssSelector("input[formcontrolname='establishmentName']"));
                    if (names.Count > 0)
                    {
                        uiEstabName = names[0];
                        break;
                    }
                }
                catch (Exception)
                {

                }
            }
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
            NgWebElement uiPreviousLicenceYes = ngDriver.FindElement(By.Id("mat-button-toggle-73-button"));
            uiPreviousLicenceYes.Click();

            // enter the previous application details
            NgWebElement uiPreviousApplicationDetails = ngDriver.FindElement(By.Id("previousApplicationDetails"));
            uiPreviousApplicationDetails.SendKeys(prevAppDetails);

            // select 'Yes'
            // Do you hold a Rural Agency Store Appointment?
            NgWebElement uiRuralAgencyStore = ngDriver.FindElement(By.Id("mat-button-toggle-76-button"));
            uiRuralAgencyStore.Click();

            // select 'Yes'
            // Do you, or any of your shareholders, have any connection, financial or otherwise, direct or indirect, with a distillery, brewery or winery?
            NgWebElement uiOtherBusinessYes = ngDriver.FindElement(By.Id("mat-button-toggle-79-button"));
            uiOtherBusinessYes.Click();

            // enter the connection details
            NgWebElement uiLiqIndConnection = ngDriver.FindElement(By.Id("liquorIndustryConnectionsDetails"));
            uiLiqIndConnection.SendKeys(liqConnectionDetails);

            // enter the kitchen details
            NgWebElement uiKitchenDescription = ngDriver.FindElement(By.CssSelector("textarea#description2"));
            uiKitchenDescription.SendKeys(kitchenDetails);

            // enter the transport details
            NgWebElement uiTransportDetails = ngDriver.FindElement(By.CssSelector("textarea#description3"));
            uiTransportDetails.SendKeys(transportDetails);

            if (bizType == "partnership")
            {
                // upload a store signage document
                FileUpload("signage.pdf", "(//input[@type='file'])[8]");

                // upload a valid interest document
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[12]");
            }

            /*if (bizType != "sole proprietorship")
            {
                // upload a store signage document
                FileUpload("signage.pdf", "(//input[@type='file'])[17]");
            }
            else 
            {
                // upload a store signage document
                FileUpload("signage.pdf", "(//input[@type='file'])[5]");
            }

            if (bizType != "sole proprietorship")
            {
                // upload a valid interest document
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[21]");
            }
            else
            {
                // upload a valid interest document
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[9]");
            }*/

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
