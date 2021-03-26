using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the Cannabis Retail Store application for a(.*)")]
        public void CompleteCannabisApplication(string businessType)
        {
            /* 
            Page Title: Submit the Cannabis Retail Store Application
            */

            // create application info
            var estName = "Point Ellis Greenhouse";
            var estAddress = "645 Tyee Rd";
            var estCity = "Victoria";
            var estPostal = "V9A 6X5";
            var estPID = "012345678";
            var estEmail = "test@test.com";
            var estPhone = "2505555555";
            var conRole = "CEO";
            var conPhone = "2508888888";
            var conEmail = "contact@email.com";
            var indigenousNation = "Cowichan Tribes";
            var policeJurisdiction = "RCMP Shawnigan Lake";

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
                FileUpload("cannabis_associate_security_screening.pdf", "(//input[@type='file'])[3]");
            else
                FileUpload("cannabis_associate_security_screening.pdf", "(//input[@type='file'])[12]");

            // upload financial integrity form
            if (businessType == " sole proprietorship")
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[6]");
            else
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[15]");

            // upload shareholders < 10% interest
            if (businessType != " sole proprietorship")
                FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[18]");

            // enter the establishment name
            var uiEstabName = ngDriver.FindElement(By.Id("establishmentName"));
            uiEstabName.SendKeys(estName);

            // enter the establishment address
            var uiEstabAddress = ngDriver.FindElement(By.Id("establishmentAddressStreet"));
            uiEstabAddress.SendKeys(estAddress);

            // enter the establishment city
            var uiEstabCity = ngDriver.FindElement(By.Id("establishmentAddressCity"));
            uiEstabCity.SendKeys(estCity);

            // enter the establishment postal code
            var uiEstabPostal = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            uiEstabPostal.SendKeys(estPostal);

            // enter the PID
            var uiEstabPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            uiEstabPID.SendKeys(estPID);

            // search for and select the indigenous nation
            var uiIndigenousNation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='indigenousNation']"));
            uiIndigenousNation.SendKeys(indigenousNation);

            var uiIndigenousNation2 = ngDriver.FindElement(By.CssSelector("#mat-option-0 span"));
            JavaScriptClick(uiIndigenousNation2);

            // search for and select the police jurisdiction
            var uiPoliceJurisdiction =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='policeJurisdiction']"));
            uiPoliceJurisdiction.SendKeys(policeJurisdiction);

            var uiPoliceJurisdiction2 = ngDriver.FindElement(By.CssSelector("#mat-option-2 span"));
            uiPoliceJurisdiction2.Click();

            // enter the store email
            var uiEstabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            uiEstabEmail.SendKeys(estEmail);

            // enter the store phone number
            var uiEstabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            uiEstabPhone.SendKeys(estPhone);

            // upload a store signage document
            if (businessType == " sole proprietorship")
                FileUpload("signage.pdf", "(//input[@type='file'])[8]");
            else
                FileUpload("signage.pdf", "(//input[@type='file'])[20]");

            // select not visible from outside checkbox
            var uiVisibleFromOutside = ngDriver.FindElement(By.CssSelector(".mat-checkbox-inner-container"));
            uiVisibleFromOutside.Click();

            // upload a floor plan document
            if (businessType == " sole proprietorship")
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[11]");
            else
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[23]");

            // upload a site plan document
            if (businessType == " sole proprietorship")
                FileUpload("site_plan.pdf", "(//input[@type='file'])[14]");
            else
                FileUpload("site_plan.pdf", "(//input[@type='file'])[26]");

            // upload a financial integrity form
            if (businessType == " sole proprietorship")
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[18]");
            else
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[30]");

            // upload a ownership details document
            if (businessType == " private corporation")
                FileUpload("ownership_details.pdf", "(//input[@type='file'])[33]");

            // upload a ownership details document
            if (businessType != " sole proprietorship")
                FileUpload("ownership_details.pdf", "(//input[@type='file'])[33]");

            // upload a ownership details document
            if (businessType == " sole proprietorship")
                FileUpload("ownership_details.pdf", "(//input[@type='file'])[21]");

            // enter the role of the application contact
            var uiContactRole = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonRole]"));
            uiContactRole.SendKeys(conRole);

            // enter the phone number of the application contact
            var uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonPhone]"));
            uiContactPhone.SendKeys(conPhone);

            // enter the email of the application contact
            var uiContactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            uiContactEmail.SendKeys(conEmail);

            // click on the authorized to submit checkbox
            var uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgree.Click();

            // retrieve the current URL to get the application ID (needed downstream)
            var URL = ngDriver.Url;

            // retrieve the application ID
            var parsedURL = URL.Split('/');

            applicationID = parsedURL[5];
        }

        /*
        [And(@"I complete the eligibility disclosure")]
        public void CompleteEligibilityDisclosure()
        {
            /* 
            Page Title: Cannabis Retail Store Licence Eligibility Disclosure
            

            // select response: On or after March 1, 2020, did you or any of your associates own, operate, provide financial support to, or receive income from an unlicensed cannabis retail store or retailer?           
            // select Yes radio button 
            var uiYesRadio1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isConnectedToUnlicencedStore'] mat-radio-button"));
            uiYesRadio1.Click();

            // complete field: Please indicate the name and location of the retailer or store 
            string nameAndLocation = "Automated test name and location of retailer";

            var uiNameAndLocation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='nameLocationUnlicencedRetailer']"));
            uiNameAndLocation.SendKeys(nameAndLocation);

            // select response: Does the retailer or store continue to operate?
            // select Yes for Question 2 using radio button
            var uiYesRadio2 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isRetailerStillOperating'] mat-radio-button"));

            ScrollToElement(uiYesRadio2);
            uiYesRadio2.Click();

            // select response: On or after March 1, 2020, were you or any of your associates involved with the distribution or supply of cannabis to a licensed or unlicensed cannabis retail store or retailer?
            // select Yes using radio button
            var uiYesRadio3 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isInvolvedIllegalDistribution'] mat-radio-button"));
            uiYesRadio3.Click();

            // complete field: Please indicate the details of your involvement
            string involvementDetails = "Automated test - details of the involvement";

            var uiInvolvementDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='illegalDistributionInvolvementDetails']"));
            uiInvolvementDetails.SendKeys(involvementDetails);

            // scroll the view down.

            // complete field: Please indicate the name and location of the retailer or store           
            string nameAndLocation2 = "Automated test name and location of retailer (2)";

            var uiNameAndLocation2 = ngDriver.FindElement(By.CssSelector("input[formControlName='nameLocationRetailer']"));
            uiNameAndLocation2.SendKeys(nameAndLocation2);

            // select response: Do you continue to be involved?
            // select Yes for Question 2 using radio button
            var uiYesRadio4 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isInvolvementContinuing'] mat-radio-button"));
            uiYesRadio4.Click();

            // select certification checkbox
            var uiCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isEligibilityCertified']"));
            uiCheckbox.Click();

            // enter the electronic signature
            string electricSignature = "Automated Test";

            var uiSigCheckbox = ngDriver.FindElement(By.CssSelector("input[formcontrolname='eligibilitySignature']"));
            uiSigCheckbox.SendKeys(electricSignature);

            // click on the Submit button
            var uiEligibilitySubmit = ngDriver.FindElement(By.CssSelector("app-eligibility-form button.btn-primary"));
            uiEligibilitySubmit.Click();
        }
        */
    }
}