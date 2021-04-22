using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the Food Primary application for a (.*)")]
        public void CompleteFoodPrimaryApplication(string bizType)
        {
            /* 
            Page Title: Food Primary Licence Application
            */

            // create test data
            var estName = "Point Ellis Greenhouse";
            var estAddress = "645 Tyee Rd";
            var estCity = "Victoria";
            var estPostal = "V9A 6X5";
            var estPID = "012345678";

            var estEmail = "test@test.com";
            var estPhone = "2505555555";

            var patioCompDescription = "Sample patio comp description";
            var patioLocationDescription = "Sample patio location description";
            var patioAccessDescription = "Sample patio access description";
            var patioLiquorCarriedDescription = "Sample liquor carried description";
            var patioAccessControlDescription = "Sample patio access control description";

            var conRole = "CEO";
            var conPhone = "2508888888";
            var conEmail = "contact@email.com";
            var indigenousNation = "Cowichan Tribes";
            var policeJurisdiction = "RCMP Shawnigan Lake";
            // var indigenousNation = "Parksville";
            // var policeJurisdiction = "RCMP Oceanside";

            if (bizType == "private corporation")
            {
                // upload a central securities register
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                // upload supporting business documentation
                FileUpload("associates.pdf", "(//input[@type='file'])[6]");

                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[9]");

                // upload personal history form
                FileUpload("associates.pdf", "(//input[@type='file'])[12]");

                // upload shareholders < 10% interest
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[15]");
            }

            if (bizType == "sole proprietorship")
            {
                // upload register of directors and officers
                FileUpload("register_of_directors_officers.pdf", "(//input[@type='file'])[3]");
            }

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

            // select the zoning checkbox
            var uiIsPermittedInZoning =
                ngDriver.FindElement(
                    By.CssSelector(
                        "mat-checkbox[formcontrolname='isPermittedInZoning'] .mat-checkbox-inner-container"));
            JavaScriptClick(uiIsPermittedInZoning);

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
            JavaScriptClick(uiPoliceJurisdiction2);

            // enter the store email
            var uiEstabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            uiEstabEmail.SendKeys(estEmail);

            // enter the store phone number
            var uiEstabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            uiEstabPhone.SendKeys(estPhone);

            // select 'Yes' for patio
            var uiHasPatio =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='isHasPatio'] mat-radio-button#mat-radio-13"));
            uiHasPatio.Click();

            // enter the patio comp description
            var uiPatioCompDescription = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioCompDescription.SendKeys(patioCompDescription);

            // enter the patio location description
            var uiPatioLocationDescription = ngDriver.FindElement(By.CssSelector("textarea#patioLocationDescription"));
            uiPatioLocationDescription.SendKeys(patioLocationDescription);

            // enter the patio access description
            var uiPatioAccessDescription = ngDriver.FindElement(By.CssSelector("textarea#patioAccessDescription"));
            uiPatioAccessDescription.SendKeys(patioAccessDescription);

            // click patio liquor is carried checkbox
            var uiPatioIsLiquorCarried = ngDriver.FindElement(By.CssSelector("mat-checkbox#patioIsLiquorCarried"));
            uiPatioIsLiquorCarried.Click();

            // enter patio liquor carried description
            var uiPatioLiquorCarriedDescription =
                ngDriver.FindElement(By.CssSelector("textarea#patioLiquorCarriedDescription"));
            uiPatioLiquorCarriedDescription.SendKeys(patioLiquorCarriedDescription);

            // enter patio access control description
            var uiPatioAccessControlDescription =
                ngDriver.FindElement(By.CssSelector("textarea#patioAccessControlDescription"));
            uiPatioAccessControlDescription.SendKeys(patioAccessControlDescription);

            // click Fixed option
            var uiFixedOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-1-button"));
            uiFixedOption.Click();

            // click Portable option
            var uiPortableOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-2-button"));
            uiPortableOption.Click();

            // click Interior option
            var uiInteriorOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-3-button"));
            uiInteriorOption.Click();

            // upload signage document
            if (bizType == "partnership" || bizType == "public corporation" || bizType == "society")
                FileUpload("signage.pdf", "(//input[@type='file'])[5]");
            else if (bizType == "private corporation")
                FileUpload("signage.pdf", "(//input[@type='file'])[17]");
            else if (bizType == "sole proprietorship")
                FileUpload("signage.pdf", "(//input[@type='file'])[5]");

            // upload floor plan
            if (bizType == "partnership" || bizType == "public corporation" || bizType == "society")
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[8]");
            else if (bizType == "private corporation")
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[20]");
            else if (bizType == "sole proprietorship")
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[8]");

            // select the owner checkbox
            var uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            var uiValidInterest =
                ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            var uiFutureValidInterest =
                ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiFutureValidInterest.Click();

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

            var tempFix = parsedURL[5].Split(';');

            applicationID = tempFix[0];
        }
    }
}