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
        [And(@"I complete the Manufacturer application for a (.*)")]
        public void CompleteManufacturerApplication(string manufacturerType)
        {
            /* 
            Page Title: Manufacturer (Winery, Brewery, Distillery or Co-Packer) Licence Application
            */

            // create test data
            string estName = "Manufacturer's Establishment";
            string streetLocation = "123 Innovation Street";
            string city = "Victoria";
            string postal = "V9A 6X5";
            string PID = "111111111";
            string additionalPIDs = "999999999, 000000000, 181818181";
            string storeEmail = "store@email.com";
            string storePhone = "250-012-3456";
            string contactTitle = "Sommelier";
            string contactPhone = "778-181-1818";
            string contactEmail = "contact@email.com";
            string indigenousNation = "Parksville";
            string policeJurisdiction = "RCMP Shawnigan Lake";
            string localGovernmentParksville = "Parksville";
            string policeJurisdictionParksville = "RCMP Oceanside";

            // upload central securities register
            FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

            // upload supporting business documentation
            FileUpload("associates.pdf", "(//input[@type='file'])[6]");

            // upload notice of articles
            FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[9]");

            // upload personal history summary documents
            FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[12]");

            // upload shareholders holding less than 10% interest
            FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[15]");

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

            // enter the establishment street address
            NgWebElement uiEstabStreetAddress = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            uiEstabStreetAddress.SendKeys(streetLocation);

            // enter the establishment city
            NgWebElement uiEstabCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            uiEstabCity.SendKeys(city);

            // enter the establishment postal code
            NgWebElement uiEstabPostal = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressPostalCode']"));
            uiEstabPostal.SendKeys(postal);

            // enter the PID
            NgWebElement uiEstabPID = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentParcelId']"));
            uiEstabPID.SendKeys(PID);

            // enter additional PIDs
            NgWebElement uiAdditionalEstabPID = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='pidList']"));
            uiAdditionalEstabPID.SendKeys(additionalPIDs);

            // select the proof of zoning checkbox
            NgWebElement uiProofOfZoning = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isPermittedInZoning']"));
            JavaScriptClick(uiProofOfZoning);

            // select 'yes' for Treaty First Nation Land
            NgWebElement uiTreatyFirstNationLand = ngDriver.FindElement(By.CssSelector("[formcontrolname='isOnINLand'] mat-radio-button"));
            ScrollToElement(uiTreatyFirstNationLand);
            JavaScriptClick(uiTreatyFirstNationLand);

            // select 'yes' for ALR inclusion
            NgWebElement uiALRInclusion = ngDriver.FindElement(By.CssSelector("[formcontrolname='isAlr'] mat-radio-button"));
            JavaScriptClick(uiALRInclusion);

            // search for and select the indigenous nation
            NgWebElement uiIndigenousNation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='indigenousNation']"));
            ScrollToElement(uiIndigenousNation);
            if (manufacturerType == "winery in Parksville")
            {
                uiIndigenousNation.SendKeys(localGovernmentParksville);

                NgWebElement uiIndigenousNation2 = ngDriver.FindElement(By.CssSelector("#mat-option-0 span"));
                uiIndigenousNation2.Click();

                // search for and select the police jurisdiction
                NgWebElement uiPoliceJurisdiction = ngDriver.FindElement(By.CssSelector("input[formcontrolname='policeJurisdiction']"));
                uiPoliceJurisdiction.SendKeys(policeJurisdictionParksville);

                NgWebElement uiPoliceJurisdiction2 = ngDriver.FindElement(By.CssSelector("#mat-option-2 span"));
                uiPoliceJurisdiction2.Click();
            }
            else
            {
                uiIndigenousNation.SendKeys(indigenousNation);

                NgWebElement uiIndigenousNation2 = ngDriver.FindElement(By.CssSelector("#mat-option-0 span"));
                JavaScriptClick(uiIndigenousNation2);

                // search for and select the police jurisdiction
                NgWebElement uiPoliceJurisdiction = ngDriver.FindElement(By.CssSelector("input[formcontrolname='policeJurisdiction']"));
                uiPoliceJurisdiction.SendKeys(policeJurisdiction);

                NgWebElement uiPoliceJurisdiction2 = ngDriver.FindElement(By.CssSelector("#mat-option-2 span"));
                JavaScriptClick(uiPoliceJurisdiction2);
            }

            // enter the store email
            NgWebElement uiEstabEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentEmail']"));
            uiEstabEmail.SendKeys(storeEmail);

            // enter the store phone number
            NgWebElement uiEstabPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentPhone']"));
            ScrollToElement(uiEstabPhone);
            uiEstabPhone.SendKeys(storePhone);

            // this will likely need to be refactored with better CSS selectors.
            if ((manufacturerType == "winery") || (manufacturerType == "winery in Parksville"))
            {
                // select winery radio button
                NgWebElement uiWinery = ngDriver.FindElement(By.CssSelector("[formcontrolname='licenceSubCategory'] mat-radio-button input[value='Winery']"));
                JavaScriptClick(uiWinery);
            }

            if (manufacturerType == "distillery")
            {
                // select distillery radio button
                NgWebElement uiDistillery = ngDriver.FindElement(By.CssSelector("[formcontrolname='licenceSubCategory'] mat-radio-button input[value='Distillery']"));
                JavaScriptClick(uiDistillery);
            }

            if (manufacturerType == "brewery")
            {
                // select brewery radio button
                NgWebElement uiBrewery = ngDriver.FindElement(By.CssSelector("[formcontrolname='licenceSubCategory'] mat-radio-button input[value='Brewery']"));
                JavaScriptClick(uiBrewery);
            }

            if (manufacturerType == "co-packer")
            {
                // select co-packer radio button
                NgWebElement uiCoPacker = ngDriver.FindElement(By.CssSelector("[formcontrolname='licenceSubCategory'] mat-radio-button input[value='Co-packer']"));
                JavaScriptClick(uiCoPacker);
            }

            // upload the business plan
            FileUpload("business_plan.pdf","(//input[@type='file'])[17]");

            if (manufacturerType != "co-packer")
            {
                // upload the production sales forecast
                FileUpload("production_sales_forecast.pdf", "(//input[@type='file'])[20]");
            }

            if ((manufacturerType == "winery") || (manufacturerType == "winery in Parksville"))
            {
                // create winery test data
                string grapesAcres = "100";
                string fruitAcres = "5";
                string honeyBeehives = "7";

                // enter the grapes acreage 
                NgWebElement uiGrapes = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mfgAcresOfGrapes']"));
                uiGrapes.SendKeys(grapesAcres);

                // enter the fruit acreage
                NgWebElement uiFruit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mfgAcresOfFruit']"));
                uiFruit.SendKeys(fruitAcres);

                // enter the number of beehives
                NgWebElement uiHoney = ngDriver.FindElement(By.CssSelector("input[formcontrolname='mfgAcresOfHoney']"));
                uiHoney.SendKeys(honeyBeehives);

                // select the blending checkbox
                NgWebElement uiBlending = ngDriver.FindElement(By.CssSelector("#mat-checkbox-15 .mat-checkbox-inner-container"));
                uiBlending.Click();

                // select the crushing checkbox
                NgWebElement uiCrushing = ngDriver.FindElement(By.CssSelector("#mat-checkbox-16 .mat-checkbox-inner-container"));
                JavaScriptClick(uiCrushing);

                // select the filtering checkbox
                NgWebElement uiFiltering = ngDriver.FindElement(By.CssSelector("#mat-checkbox-17 .mat-checkbox-inner-container"));
                uiFiltering.Click();

                // select the aging, for at least 3 months checkbox
                NgWebElement uiAging = ngDriver.FindElement(By.CssSelector("#mat-checkbox-18 .mat-checkbox-inner-container"));
                uiAging.Click();

                // select the secondary fermentation or carbonation checkbox
                NgWebElement uiSecondaryFermentation = ngDriver.FindElement(By.CssSelector("#mat-checkbox-19 .mat-checkbox-inner-container"));
                uiSecondaryFermentation.Click();

                // select the packaging checkbox
                NgWebElement uiPackaging = ngDriver.FindElement(By.CssSelector("#mat-checkbox-20 .mat-checkbox-inner-container"));
                uiPackaging.Click();
            }

            if (manufacturerType != "co-packer")
            {
                // select 'yes' for neutral grain spirits            
                NgWebElement uiNeutralGrains = ngDriver.FindElement(By.CssSelector("[formcontrolname='mfgUsesNeutralGrainSpirits'] mat-radio-button[value='Yes']"));
                uiNeutralGrains.Click();
            }

            if (manufacturerType == "brewery")
            {
                // select 'Yes' for the brewery operating with brew pub on site
                NgWebElement uiPubOnSite = ngDriver.FindElement(By.CssSelector("[formcontrolname='mfgBrewPubOnSite'] mat-radio-button[value='Yes']"));
                uiPubOnSite.Click();

                // select 'Yes' for piping from brewery
                NgWebElement uiPiping = ngDriver.FindElement(By.CssSelector("[formcontrolname='mfgPipedInProduct'] mat-radio-button[value='Yes']"));
                uiPiping.Click();

                // upload brew sheets sample
                FileUpload("brew_sheets.pdf","(//input[@type='file'])[23]");

                // upload the business insurance
                FileUpload("business_insurance.pdf","(//input[@type='file'])[26]");
            }

            if (manufacturerType == "brewery")
            {
                // upload the store signage
                FileUpload("signage.pdf","(//input[@type='file'])[29]");

                // upload the floor plan 
                FileUpload("floor_plan.pdf","(//input[@type='file'])[32]");

                // upload the site plan
                FileUpload("site_plan.pdf","(//input[@type='file'])[35]");
            }
            else
            {
                // upload the store signage
                FileUpload("signage.pdf","(//input[@type='file'])[23]");

                // upload the floor plan 
                FileUpload("floor_plan.pdf","(//input[@type='file'])[26]");

                // upload the site plan 
                FileUpload("site_plan.pdf","(//input[@type='file'])[29]");
            }

            // select the owner checkbox
            NgWebElement uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            NgWebElement uiFutureValidInterest = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiFutureValidInterest.Click();

            // upload the valid interest document
            if (manufacturerType == "brewery")
            {
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[39]");
            }
            else 
            {
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[33]");
            }

            // enter the contact title
            NgWebElement uiContactTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonRole']"));
            uiContactTitle.SendKeys(contactTitle);

            // enter the contact phone number
            NgWebElement uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonPhone']"));
            uiContactPhone.SendKeys(contactPhone);

            // enter the contact email address
            NgWebElement uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonEmail']"));
            uiContactEmail.SendKeys(contactEmail);

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            // retrieve the current URL to get the application ID (needed downstream)
            string URL = ngDriver.Url;

            // retrieve the application ID
            string[] parsedURL = URL.Split('/');

            applicationID = parsedURL[5];

            // click on the Submit & Pay button
            ClickOnSubmitButton();
        }
    }
}
