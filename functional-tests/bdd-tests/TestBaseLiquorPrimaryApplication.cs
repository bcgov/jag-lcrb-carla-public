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

            /*
            string patioCompDescription = "Sample patio comp description";
            string patioLocationDescription = "Sample patio location description";
            string patioAccessDescription = "Sample patio access description";
            string patioLiquorCarriedDescription = "Sample liquor carried description";
            string patioAccessControlDescription = "Sample patio access control description";
            */

            string conRole = "CEO";
            string conPhone = "2508888888";
            string conEmail = "contact@email.com";
            string indigenousNation = "Cowichan Tribes";
            string policeJurisdiction = "RCMP Shawnigan Lake";

            string floorAreaDescription = "Sample floor area.";
            string occupantLoad = "180";

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

            /*
            if ((bizType == "public corporation") || (bizType == "partnership") || (bizType == "society") || (bizType == "private corporation"))
            {
                // upload register of directors and officers
                FileUpload("register_of_directors_officers.pdf", "(//input[@type='file'])[3]");
            }

            if (bizType == "private corporation")
            {
                // upload supporting business documentation
                FileUpload("distribution_plan.pdf", "(//input[@type='file'])[6]");

                // upload register of directors and officers
                FileUpload("register_of_directors_officers.pdf", "(//input[@type='file'])[9]");

                // upload shareholders holding less than 10% interest
                FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[12]");
            }
            */

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
            uiIsPermittedInZoning.Click();

            // upload the letter of intent
            FileUpload("letter_of_intent.pdf", "(//input[@type='file'])[17]");

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

            /*
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

            // click fixed option
            // NgWebElement uiFixedOption = ngDriver.FindElement(By.Id("mat-button-toggle-9-button"));
            NgWebElement uiFixedOption = ngDriver.FindElement(By.XPath("//button[@id='mat-button-toggle-9-button']/div"));
            uiFixedOption.Click();

            // click portable option
            // NgWebElement uiPortableOption = ngDriver.FindElement(By.Id("mat-button-toggle-10-button"));
            // uiPortableOption.Click();

            // click interior option
            // NgWebElement uiInteriorOption = ngDriver.FindElement(By.Id("mat-button-toggle-11-button"));
            // uiInteriorOption.Click();
            */

            if ((bizType == "partnership") || (bizType == "public corporation") || (bizType == "society"))
            {
                // upload signage document
                FileUpload("signage.pdf", "(//input[@type='file'])[5]");
            }
            else if (bizType == "private corporation")
            {
                // upload signage document
                FileUpload("signage.pdf", "(//input[@type='file'])[20]");
            }
            else if (bizType == "sole proprietorship")
            {
                // upload signage document
                FileUpload("signage.pdf", "(//input[@type='file'])[2]");
            }

            if ((bizType == "partnership") || (bizType == "public corporation") || (bizType == "society"))
            {
                // upload floor plan
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[8]");
            }
            else if (bizType == "private corporation")
            {
                // upload floor plan
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[23]");
            }
            else if (bizType == "sole proprietorship")
            {
                // upload floor plan
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[5]");
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

            // upload the site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[26]");

            // select the owner checkbox
            NgWebElement uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            NgWebElement uiFutureValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='willhaveValidInterest']"));
            uiFutureValidInterest.Click();

            /*
            if ((bizType == "partnership") || (bizType == "public corporation") || (bizType == "society"))
            {
                // upload proof of valid interest
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[12]");
            }

            if (bizType == "private corporation")
            {
                // upload proof of valid interest
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[21]");
            }

            if (bizType == "sole proprietorship")
            {
                // upload proof of valid interest
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[9]");
            }
            */

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
