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
        public void UBrewUVinApplication()
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
            NgWebElement uiIsOwnerBusiness = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1[formcontrolname='isOwnerBusiness']"));
            uiIsOwnerBusiness.Click();

            // select has valid interest checkbox
            NgWebElement uiHasValidInterest = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-2[formcontrolname='hasValidInterest']"));
            uiHasValidInterest.Click();

            // select will have valid interest checkbox
            NgWebElement uiWillhaveValidInterest = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-3[formcontrolname='willhaveValidInterest']"));
            uiWillhaveValidInterest.Click();

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
