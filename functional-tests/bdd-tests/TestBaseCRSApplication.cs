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
        [And(@"I complete the eligibility disclosure")]
        public void CompleteEligibilityDisclosure()
        {
            /* 
            Page Title: Cannabis Retail Store Licence Eligibility Disclosure
            */

            // select response: On or after March 1, 2020, did you or any of your associates own, operate, provide financial support to, or receive income from an unlicensed cannabis retail store or retailer?           
            // select Yes radio button 
            NgWebElement uiYesRadio1 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isConnectedToUnlicencedStore'] mat-radio-button"));
            uiYesRadio1.Click();

            // complete field: Please indicate the name and location of the retailer or store 
            string nameAndLocation = "Automated test name and location of retailer";

            NgWebElement uiNameAndLocation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='nameLocationUnlicencedRetailer']"));
            uiNameAndLocation.SendKeys(nameAndLocation);

            // select response: Does the retailer or store continue to operate?
            // select Yes for Question 2 using radio button
            NgWebElement uiYesRadio2 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isRetailerStillOperating'] mat-radio-button"));
            uiYesRadio2.Click();

            // select response: On or after March 1, 2020, were you or any of your associates involved with the distribution or supply of cannabis to a licensed or unlicensed cannabis retail store or retailer?
            // select Yes using radio button
            NgWebElement uiYesRadio3 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isInvolvedIllegalDistribution'] mat-radio-button"));
            uiYesRadio3.Click();

            // complete field: Please indicate the details of your involvement
            string involvementDetails = "Automated test - details of the involvement";

            NgWebElement uiInvolvementDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='illegalDistributionInvolvementDetails']"));
            uiInvolvementDetails.SendKeys(involvementDetails);

            // complete field: Please indicate the name and location of the retailer or store           
            string nameAndLocation2 = "Automated test name and location of retailer (2)";

            NgWebElement uiNameAndLocation2 = ngDriver.FindElement(By.CssSelector("input[formControlName='nameLocationRetailer']"));
            uiNameAndLocation2.SendKeys(nameAndLocation2);

            // select response: Do you continue to be involved?
            // select Yes for Question 2 using radio button
            NgWebElement uiYesRadio4 = ngDriver.FindElement(By.CssSelector("[formcontrolname='isInvolvementContinuing'] mat-radio-button"));
            uiYesRadio4.Click();

            // select certification checkbox
            NgWebElement uiCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isEligibilityCertified']"));
            uiCheckbox.Click();

            // enter the electronic signature
            string electricSignature = "Automated Test";

            NgWebElement uiSigCheckbox = ngDriver.FindElement(By.CssSelector("input[formcontrolname='eligibilitySignature']"));
            uiSigCheckbox.SendKeys(electricSignature);

            // click on the Submit button
            NgWebElement uiEligibilitySubmit = ngDriver.FindElement(By.CssSelector("app-eligibility-form button.btn-primary"));
            uiEligibilitySubmit.Click();
        }


        [And(@"I complete the Cannabis Retail Store application for a(.*)")]
        public void CompleteCannabisApplication(string businessType)
        {
            /* 
            Page Title: Submit the Cannabis Retail Store Application
            */

            // create application info
            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A 6X5";
            string estPID = "012345678";
            string estEmail = "test@test.com";
            string estPhone = "2505555555";
            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";
            string conEmail = "contact@email.com";
            string indigenousNation = "Cowichan Tribes";
            string policeJurisdiction = "RCMP Shawnigan Lake";

            

            // enter the establishment name
            NgWebElement uiEstabName = null;
            // try up to 10 times to get an element.
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    var names = ngDriver.FindElements(By.Id("establishmentName"));
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

            if (businessType == "n indigenous nation")
            {
                // select the IN 
                NgWebElement uiSelectNation = ngDriver.FindElement(By.CssSelector("[formcontrolname='indigenousNationId'] option[value='236686fc-d9d3-e811-90f0-005056832371']"));
                uiSelectNation.Click();
            }

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a proof of zoning form
            string zoningPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "fin_integrity.pdf");
            NgWebElement uiUploadZoning = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
            uiUploadZoning.SendKeys(zoningPath);

            // search for and select the indigenous nation
            NgWebElement uiIndigenousNation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='indigenousNation']"));
            uiIndigenousNation.SendKeys(indigenousNation);

            NgWebElement uiIndigenousNation2 = ngDriver.FindElement(By.CssSelector("#mat-option-0 span"));
            uiIndigenousNation2.Click();

            // search for and select the  police jurisdiction
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

            // upload a store signage document
            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uiUploadSignage.SendKeys(signagePath);

            // select not visible from outside checkbox
            NgWebElement uiVisibleFromOutside = ngDriver.FindElement(By.CssSelector(".mat-checkbox-inner-container"));
            uiVisibleFromOutside.Click();

            // upload a floor plan document
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uiUploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uiUploadFloorplan.SendKeys(floorplanPath);

            // upload a site plan document
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uiUploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
            uiUploadSitePlan.SendKeys(sitePlanPath);

            // upload a financial integrity form
            string finIntegrityPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "fin_integrity.pdf");
            NgWebElement uiUploadFinIntegrity = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uiUploadFinIntegrity.SendKeys(finIntegrityPath);

            // upload a ownership details document
            string ownershipDetailsPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "fin_integrity.pdf");
            NgWebElement uiOwnershipDetails = ngDriver.FindElement(By.XPath("(//input[@type='file'])[18]"));
            uiOwnershipDetails.SendKeys(ownershipDetailsPath);

            // enter the first name of the application contact
            NgWebElement uiContactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            uiContactGiven.SendKeys(conGiven);

            // enter the last name of the application contact
            NgWebElement uiContactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            uiContactSurname.SendKeys(conSurname);

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

            applicationID = parsedURL[5];

            // click on the submit button
            NgWebElement uiSubmitButton = ngDriver.FindElement(By.CssSelector(".application-wrapper button.btn-primary"));
            uiSubmitButton.Click();
        }
    }
}
