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
        [And(@"I complete the UBrew / UVin application for (.*)")]
        public void UBrewUVinApplication(string businessType)
        {
            string establishmentName = "Point Ellis Greenhouse";
            string streetAddress = "645 Tyee Road";
            string city = "Victoria";
            string postalCode = "V9A6X5";
            string PID = "999999999";
            string storeEmail = "auto@test.com";
            string storePhone = "2222222222";
            string contactTitle = "Brewmaster";
            string contactPhone = "3333333333";
            string contactEmail = "contact@email.com";

            if (businessType != " sole proprietorship")
            {
                // upload a central securities register
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                // upload supporting business documentation
                FileUpload("associates.pdf", "(//input[@type='file'])[6]");

                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[9]");
            }

            // upload cannabis associate screening form
            if (businessType == " sole proprietorship")
            {
                FileUpload("associates.pdf", "(//input[@type='file'])[3]");
            }
            else
            {
                FileUpload("associates.pdf", "(//input[@type='file'])[12]");
            }

            // upload financial integrity form
            if (businessType == " sole proprietorship")
            {
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[6]");
            }
            else
            {
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[15]");
            }

            // upload shareholders < 10% interest
            if (businessType != " sole proprietorship")
            {
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[18]");
            }

            // enter the establishment name
            NgWebElement uiEstablishmentName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentName']"));
            uiEstablishmentName.SendKeys(establishmentName);

            // enter the street address
            NgWebElement uiEstablishmentAddressStreet = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            uiEstablishmentAddressStreet.SendKeys(streetAddress);

            // enter the city
            NgWebElement uiEstablishmentAddressCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            uiEstablishmentAddressCity.SendKeys(city);

            // enter the postal code
            NgWebElement uiEstablishmentAddressPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressPostalCode']"));
            uiEstablishmentAddressPostalCode.SendKeys(postalCode);

            // enter the PID
            NgWebElement uiEstablishmentParcelId = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentParcelId']"));
            uiEstablishmentParcelId.SendKeys(PID);

            // enter the store email
            NgWebElement uiEstablishmentEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentEmail']"));
            uiEstablishmentEmail.SendKeys(storeEmail);

            // enter the store phone
            NgWebElement uiEstablishmentPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentPhone']"));
            uiEstablishmentPhone.SendKeys(storePhone);

            // upload the signage document
            FileUpload("signage.pdf", "(//input[@type='file'])[2]");

            // select owner business checkbox
            NgWebElement uiIsOwnerBusiness = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiIsOwnerBusiness.Click();

            // select has valid interest checkbox
            NgWebElement uiHasValidInterest = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='hasValidInterest']"));
            uiHasValidInterest.Click();

            // select will have valid interest checkbox
            NgWebElement uiWillhaveValidInterest = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiWillhaveValidInterest.Click();

            // upload the valid interest document
            FileUpload("valid_interest.pdf", "(//input[@type='file'])[6]");

            // enter the contact title
            NgWebElement uiContactPersonRole = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonRole']"));
            uiContactPersonRole.SendKeys(contactTitle);

            // enter the contact phone number
            NgWebElement uiContactPersonPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonPhone']"));
            uiContactPersonPhone.SendKeys(contactPhone);

            // enter the contact email address
            NgWebElement uiContactPersonEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonEmail']"));
            uiContactPersonEmail.SendKeys(contactEmail);

            // click on authorize signature checkbox
            NgWebElement uiAuthorizeSignature = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit']"));
            uiAuthorizeSignature.Click();

            // click on signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // retrieve the current URL to get the application ID (needed downstream)
            string URL = ngDriver.Url;

            // retrieve the application ID
            string[] parsedURL = URL.Split('/');

            applicationID = parsedURL[5];
        }
    }
}
