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
            NgWebElement uiEstabName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentName']"));
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

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the business plan
            string businessPlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "production_sales_forecast.pdf");
            NgWebElement uiUploadBusinessPlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uiUploadBusinessPlan.SendKeys(businessPlanPath);

            // upload the production sales forecast
            string productionSalesPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "production_sales_forecast.pdf");
            NgWebElement uiUploadProductionSales = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadProductionSales.SendKeys(productionSalesPath);

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
                string brewSheetsPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "brew_sheets.pdf");
                NgWebElement uiUploadBrewSheets = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
                uiUploadBrewSheets.SendKeys(brewSheetsPath);

                // upload the business insurance
                string insurancePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "business_insurance.pdf");
                NgWebElement uiUploadInsurance = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
                uiUploadInsurance.SendKeys(insurancePath);
            }

            if (manufacturerType == "brewery")
            {
                // upload the store signage
                string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[14]"));
                uiUploadSignage.SendKeys(signagePath);

                // upload the floor plan 
                string floorPlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
                NgWebElement uiUploadFloorPlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[17]"));
                uiUploadFloorPlan.SendKeys(floorPlanPath);

                // upload the site plan
                string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
                NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[20]"));
                uiUploadSitePlan.SendKeys(sitePlanPath);
            }
            else
            {
                // upload the store signage
                string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
                uiUploadSignage.SendKeys(signagePath);

                // upload the floor plan 
                string floorPlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
                NgWebElement uiUploadFloorPlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
                uiUploadFloorPlan.SendKeys(floorPlanPath);

                // upload the site plan 
                string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
                NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[14]"));
                uiUploadSitePlan.SendKeys(sitePlanPath);
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
