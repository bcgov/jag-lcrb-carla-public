using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the UBrew / UVin application for (.*)")]
        public void UBrewUVinApplication(string businessType)
        {
            var establishmentName = "Point Ellis Greenhouse";
            var streetAddress = "645 Tyee Road";
            var city = "Victoria";
            var postalCode = "V9A6X5";
            var PID = "999999999";
            var storeEmail = "auto@test.com";
            var storePhone = "2222222222";
            var contactTitle = "Brewmaster";
            var contactPhone = "3333333333";
            var contactEmail = "contact@email.com";

            if (businessType != " sole proprietorship")
            {
                // upload a central securities register
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                // upload supporting business documentation
                FileUpload("associates.pdf", "(//input[@type='file'])[6]");

                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[9]");
            }

            // upload personal history summary form
            if (businessType == " sole proprietorship")
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[3]");
            else
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[12]");

            // upload shareholders < 10% interest
            if (businessType != " sole proprietorship")
                FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[15]");

            // enter the establishment name
            var uiEstablishmentName =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentName']"));
            uiEstablishmentName.SendKeys(establishmentName);

            // enter the street address
            var uiEstablishmentAddressStreet =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            uiEstablishmentAddressStreet.SendKeys(streetAddress);

            // enter the city
            var uiEstablishmentAddressCity =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            uiEstablishmentAddressCity.SendKeys(city);

            // enter the postal code
            var uiEstablishmentAddressPostalCode =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressPostalCode']"));
            uiEstablishmentAddressPostalCode.SendKeys(postalCode);

            // enter the PID
            var uiEstablishmentParcelId =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentParcelId']"));
            uiEstablishmentParcelId.SendKeys(PID);

            // enter the store email
            var uiEstablishmentEmail =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentEmail']"));
            uiEstablishmentEmail.SendKeys(storeEmail);

            // enter the store phone
            var uiEstablishmentPhone =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentPhone']"));
            uiEstablishmentPhone.SendKeys(storePhone);

            if (businessType == " sole proprietorship")
                // upload the signage document
                FileUpload("signage.pdf", "(//input[@type='file'])[5]");
            else
                // upload the signage document
                FileUpload("signage.pdf", "(//input[@type='file'])[17]");

            // select owner business checkbox
            var uiIsOwnerBusiness =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiIsOwnerBusiness.Click();

            // select has valid interest checkbox
            var uiHasValidInterest =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='hasValidInterest']"));
            uiHasValidInterest.Click();

            // select will have valid interest checkbox
            var uiWillhaveValidInterest =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiWillhaveValidInterest.Click();

            if (businessType == " sole proprietorship")
                // upload the valid interest document
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[9]");
            else
                // upload the valid interest document
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[21]");

            // enter the contact title
            var uiContactPersonRole =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonRole']"));
            uiContactPersonRole.SendKeys(contactTitle);

            // enter the contact phone number
            var uiContactPersonPhone =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonPhone']"));
            uiContactPersonPhone.SendKeys(contactPhone);

            // enter the contact email address
            var uiContactPersonEmail =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonEmail']"));
            uiContactPersonEmail.SendKeys(contactEmail);

            // click on authorize signature checkbox
            var uiAuthorizeSignature =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizeSignature.Click();

            // click on signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            // retrieve the current URL to get the application ID (needed downstream)
            var URL = ngDriver.Url;

            // retrieve the application ID
            var parsedURL = URL.Split('/');

            applicationID = parsedURL[5];
        }
    }
}