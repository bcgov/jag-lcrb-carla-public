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
            string indigenousNation = "Cowichan Tribes";
            string policeJurisdiction = "RCMP Shawnigan Lake";
            string localGovernmentSaanich = "Saanich";
            string policeJurisdictionSaanich = "Saanich Police Department";

            // enter the establishment name
            NgWebElement uiEstabName = null;
            // try up to 10 times to get an element.
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
            NgWebElement uiProofOfZoning = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-1"));
            uiProofOfZoning.Click();

            // select 'yes' for ALR inclusion
            NgWebElement uiALRInclusion = ngDriver.FindElement(By.CssSelector("[formcontrolname='isAlr'] mat-radio-button#mat-radio-2"));
            uiALRInclusion.Click();

            if (manufacturerType == "winery in Saanich")
            {
                // search for and select the local government
                NgWebElement uiIndigenousNation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='indigenousNation']"));
                uiIndigenousNation.SendKeys(localGovernmentSaanich);

                NgWebElement uiIndigenousNation2 = ngDriver.FindElement(By.CssSelector("#mat-option-2 span"));
                uiIndigenousNation2.Click();

                // search for and select the police jurisdiction
                NgWebElement uiPoliceJurisdiction = ngDriver.FindElement(By.CssSelector("input[formcontrolname='policeJurisdiction']"));
                uiPoliceJurisdiction.SendKeys(policeJurisdictionSaanich);

                NgWebElement uiPoliceJurisdiction2 = ngDriver.FindElement(By.CssSelector("#mat-option-6 span"));
                uiPoliceJurisdiction2.Click();
            }
            else
            {
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
            }

            // enter the store email
            NgWebElement uiEstabEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentEmail']"));
            uiEstabEmail.SendKeys(storeEmail);

            // enter the store phone number
            NgWebElement uiEstabPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentPhone']"));
            uiEstabPhone.SendKeys(storePhone);

            if ((manufacturerType == "winery") || (manufacturerType == "winery in Saanich"))
            {
                // select winery radio button
                NgWebElement uiWinery = ngDriver.FindElement(By.CssSelector("mat-radio-button#mat-radio-5"));
                uiWinery.Click();
            }

            if (manufacturerType == "distillery")
            {
                // select distillery radio button
                NgWebElement uiDistillery = ngDriver.FindElement(By.CssSelector("mat-radio-button#mat-radio-6"));
                uiDistillery.Click();
            }

            if (manufacturerType == "brewery")
            {
                // select brewery radio button
                NgWebElement uiBrewery = ngDriver.FindElement(By.CssSelector("mat-radio-button#mat-radio-7"));
                uiBrewery.Click();
            }

            if (manufacturerType == "co-packer")
            {
                // select co-packer radio button
                NgWebElement uiCoPacker = ngDriver.FindElement(By.CssSelector("mat-radio-button#mat-radio-8"));
                uiCoPacker.Click();
            }

            // upload the business plan
            FileUpload("business_plan.pdf","(//input[@type='file'])[2]");

            // upload the production sales forecast
            FileUpload("production_sales_forecast.pdf","(//input[@type='file'])[5]");

            if ((manufacturerType == "winery") || (manufacturerType == "winery in Saanich"))
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
                NgWebElement uiBlending = ngDriver.FindElement(By.CssSelector("#mat-checkbox-10 .mat-checkbox-inner-container"));
                uiBlending.Click();

                // select the crushing checkbox
                NgWebElement uiCrushing = ngDriver.FindElement(By.CssSelector("#mat-checkbox-11 .mat-checkbox-inner-container"));
                uiCrushing.Click();

                // select the filtering checkbox
                NgWebElement uiFiltering = ngDriver.FindElement(By.CssSelector("#mat-checkbox-12 .mat-checkbox-inner-container"));
                uiFiltering.Click();

                // select the aging, for at least 3 months checkbox
                NgWebElement uiAging = ngDriver.FindElement(By.CssSelector("#mat-checkbox-13 .mat-checkbox-inner-container"));
                uiAging.Click();

                // select the secondary fermentation or carbonation checkbox
                NgWebElement uiSecondaryFermentation = ngDriver.FindElement(By.CssSelector("#mat-checkbox-14 .mat-checkbox-inner-container"));
                uiSecondaryFermentation.Click();

                // select the packaging checkbox
                NgWebElement uiPackaging = ngDriver.FindElement(By.CssSelector("#mat-checkbox-15 .mat-checkbox-inner-container"));
                uiPackaging.Click();
            }

            // select 'yes' for neutral grain spirits            
            NgWebElement uiNeutralGrains = ngDriver.FindElement(By.CssSelector("[formcontrolname='mfgUsesNeutralGrainSpirits'] mat-radio-button[value='Yes']"));
            uiNeutralGrains.Click();

            if (manufacturerType == "brewery")
            {
                // select 'Yes' for the brewery operating with brew pub on site
                NgWebElement uiPubOnSite = ngDriver.FindElement(By.CssSelector("[formcontrolname='mfgBrewPubOnSite'] mat-radio-button[value='Yes']"));
                uiPubOnSite.Click();

                // select 'Yes' for piping from brewery
                NgWebElement uiPiping = ngDriver.FindElement(By.CssSelector("[formcontrolname='mfgPipedInProduct'] mat-radio-button[value='Yes']"));
                uiPiping.Click();

                // upload brew sheets sample
                FileUpload("brew_sheets.pdf","(//input[@type='file'])[8]");

                // upload the business insurance
                FileUpload("business_insurance.pdf","(//input[@type='file'])[11]");
            }

            if (manufacturerType == "brewery")
            {
                // upload the store signage
                FileUpload("signage.pdf","(//input[@type='file'])[14]");

                // upload the floor plan 
                FileUpload("floor_plan.pdf","(//input[@type='file'])[17]");

                // upload the site plan
                FileUpload("site_plan.pdf","(//input[@type='file'])[20]");
            }
            else
            {
                // upload the store signage
                FileUpload("signage.pdf","(//input[@type='file'])[8]");

                // upload the floor plan 
                FileUpload("floor_plan.pdf","(//input[@type='file'])[11]");

                // upload the site plan 
                FileUpload("site_plan.pdf","(//input[@type='file'])[14]");
            }

            // select the owner checkbox
            NgWebElement uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            NgWebElement uiValidInterest = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            NgWebElement uiFutureValidInterest = ngDriver.FindElement(By.CssSelector("mat-checkbox#mat-checkbox-4[formcontrolname='willhaveValidInterest']"));
            uiFutureValidInterest.Click();

            if (manufacturerType == "winery")
            {
                // upload the valid interest document
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[18]");
            }

            if (manufacturerType == "brewery")
            {
                // upload the valid interest document
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[21]");
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
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
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
