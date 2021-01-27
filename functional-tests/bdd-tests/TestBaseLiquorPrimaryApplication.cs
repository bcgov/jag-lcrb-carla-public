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
        [And(@"I complete the Liquor Primary application for a (.*)")]
        public void CompleteLiquorPrimaryApplication(string bizType)
        {
            /* 
            Page Title: Liquor Primary Licence Application
            */

            // create test data
            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A 6X5";
            string estPID = "012345678";

            string estEmail = "test@test.com";
            string estPhone = "2505555555";
            string estType = "Military Mess";

            string patioCompDescription = "Sample patio comp description";
            string patioLocationDescription = "Sample patio location description";
            string patioAccessDescription = "Sample patio access description";
            string patioLiquorCarriedDescription = "Sample liquor carried description";
            string patioAccessControlDescription = "Sample patio access control description";

            string conRole = "CEO";
            string conPhone = "2508888888";
            string conEmail = "contact@email.com";
            string indigenousNation = "Cowichan Tribes";
            string policeJurisdiction = "RCMP Shawnigan Lake";

            string floorAreaDescription = "Sample floor area.";
            string occupantLoad = "180";

            if (bizType != "sole proprietorship")
            {
                // upload a central securities register
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                // upload supporting business documentation
                FileUpload("associates.pdf", "(//input[@type='file'])[6]");

                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[9]");
            }

            // upload personal history form
            if (bizType == "sole proprietorship")
            {
                FileUpload("associates.pdf", "(//input[@type='file'])[3]");
            }
            else {
                FileUpload("associates.pdf", "(//input[@type='file'])[12]");
            }

            // upload shareholders < 10% interest
            if (bizType != "sole proprietorship")
            {
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[15]");
            }

            // enter the establishment name
            NgWebElement uiEstabName = ngDriver.FindElement(By.Id("establishmentName"));
            uiEstabName.SendKeys(estName);

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

            // select the zoning checkbox
            NgWebElement uiIsPermittedInZoning = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isPermittedInZoning'] .mat-checkbox-inner-container"));
            JavaScriptClick(uiIsPermittedInZoning);

            // upload the letter of intent
            if (bizType == "sole proprietorship")
            {
                FileUpload("letter_of_intent.pdf", "(//input[@type='file'])[5]");
            }
            else {
                FileUpload("letter_of_intent.pdf", "(//input[@type='file'])[17]");
            }

            // search for and select the indigenous nation
            NgWebElement uiIndigenousNation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='indigenousNation']"));
            uiIndigenousNation.SendKeys(indigenousNation);

            NgWebElement uiIndigenousNation2 = ngDriver.FindElement(By.CssSelector("#mat-option-0 span"));
            uiIndigenousNation2.Click();

            // search for and select the police jurisdiction
            NgWebElement uiPoliceJurisdiction = ngDriver.FindElement(By.CssSelector("input[formcontrolname='policeJurisdiction']"));
            uiPoliceJurisdiction.SendKeys(policeJurisdiction);

            NgWebElement uiPoliceJurisdiction2 = ngDriver.FindElement(By.CssSelector("#mat-option-2 span"));
            uiPoliceJurisdiction2.Click();

            // enter the store email
            NgWebElement uiEstabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            uiEstabEmail.SendKeys(estEmail);

            // enter the store phone number
            NgWebElement uiEstabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            uiEstabPhone.SendKeys(estPhone);

            // select 'No' for patio
            NgWebElement uiHasPatio = ngDriver.FindElement(By.CssSelector("[formcontrolname='isHasPatio'] mat-radio-button#mat-radio-3"));
            uiHasPatio.Click();

            // enter the establishment type
            NgWebElement uiEstabType = ngDriver.FindElement(By.CssSelector("input[formcontrolname='description1']"));
            uiEstabType.SendKeys(estType);

            // enter the patio comp description
            NgWebElement uiPatioCompDescription = ngDriver.FindElement(By.CssSelector("textarea#patioCompDescription"));
            uiPatioCompDescription.SendKeys(patioCompDescription);

            // enter the patio location description
            NgWebElement uiPatioLocationDescription = ngDriver.FindElement(By.CssSelector("textarea#patioLocationDescription"));
            uiPatioLocationDescription.SendKeys(patioLocationDescription);

            // enter the patio access description
            NgWebElement uiPatioAccessDescription = ngDriver.FindElement(By.CssSelector("textarea#patioAccessDescription"));
            uiPatioAccessDescription.SendKeys(patioAccessDescription);

            // click patio liquor is carried checkbox
            NgWebElement uiPatioIsLiquorCarried = ngDriver.FindElement(By.CssSelector("mat-checkbox#patioIsLiquorCarried"));
            uiPatioIsLiquorCarried.Click();

            // enter patio liquor carried description
            NgWebElement uiPatioLiquorCarriedDescription = ngDriver.FindElement(By.CssSelector("textarea#patioLiquorCarriedDescription"));
            uiPatioLiquorCarriedDescription.SendKeys(patioLiquorCarriedDescription);

            // enter patio access control description
            NgWebElement uiPatioAccessControlDescription = ngDriver.FindElement(By.CssSelector("textarea#patioAccessControlDescription"));
            uiPatioAccessControlDescription.SendKeys(patioAccessControlDescription);

            // click Fixed option
            NgWebElement uiFixedOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-1-button"));
            uiFixedOption.Click();

            // click Portable option
            NgWebElement uiPortableOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-2-button"));
            uiPortableOption.Click();

            // click Interior option
            NgWebElement uiInteriorOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-3-button"));
            uiInteriorOption.Click();

            // upload signage document
            if ((bizType == "partnership") || (bizType == "society"))
            {
                FileUpload("signage.pdf", "(//input[@type='file'])[5]");
            }
            else if (bizType == "private corporation")
            {
                FileUpload("signage.pdf", "(//input[@type='file'])[20]");
            }
            else if (bizType == "sole proprietorship")
            {
                FileUpload("signage.pdf", "(//input[@type='file'])[8]");
            }

            // upload floor plan
            if ((bizType == "partnership") || (bizType == "society"))
            {
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[8]");
            }
            else if (bizType == "private corporation")
            {
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[23]");
            }
            else if (bizType == "sole proprietorship")
            {
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[11]");
            }

            // click on the Add Area button
            NgWebElement uiAddArea = ngDriver.FindElement(By.CssSelector("[formcontrolname='serviceAreas'] button"));
            uiAddArea.Click();

            // enter floor area description
            NgWebElement uiFloorAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
            uiFloorAreaDescription.SendKeys(floorAreaDescription);

            // click on patio checkbox
            NgWebElement uiPatioCheckbox = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isPatio']"));
            uiPatioCheckbox.Click();

            // enter occupant load
            NgWebElement uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiOccupantLoad.SendKeys(occupantLoad);

            if (bizType == "sole proprietorship")
            {
                // upload the site plan
                FileUpload("site_plan.pdf", "(//input[@type='file'])[14]");
            }
            else
            {
                // upload the site plan
                FileUpload("site_plan.pdf", "(//input[@type='file'])[26]");
            }

            // select the owner checkbox
            NgWebElement uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            NgWebElement uiFutureValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiFutureValidInterest.Click();

            // enter the role of the application contact
            NgWebElement uiContactRole = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonRole]"));
            uiContactRole.SendKeys(conRole);

            // enter the phone number of the application contact
            NgWebElement uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonPhone]"));
            uiContactPhone.SendKeys(conPhone);

            // enter the email of the application contact
            NgWebElement uiContactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            uiContactEmail.SendKeys(conEmail);

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgree.Click();

            // retrieve the current URL to get the application ID (needed downstream)
            string URL = ngDriver.Url;

            // retrieve the application ID
            string[] parsedURL = URL.Split('/');

            string[] tempFix = parsedURL[5].Split(';');

            applicationID = tempFix[0];
        }
    }
}
