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
        [And(@"I complete the Rural LRS application")]
        public void CompleteRuralLRSApplication()
        {
            /* 
            Page Title: Rural Licensee Retail Store Application
            */

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

            string resortDescription = "Sample resort description";
            string otherBusinesses = "Sample other businesses";
            string legalOwners = "Sample legal owners";
            string businessName = "Sample business name";

            // upload the legal entity document requirements
            FileUpload("valid_interest.pdf", "(//input[@type='file'])[3]");

            // upload the personal history summary requirements
            FileUpload("valid_interest.pdf", "(//input[@type='file'])[6]");

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

            // upload the zoning document
            FileUpload("valid_interest.pdf", "(//input[@type='file'])[9]");

            // select 'Yes' for Treaty First Nation land
            NgWebElement uiIsOnINLand = ngDriver.FindElement(By.CssSelector("[formcontrolname='isOnINLand'] mat-radio-button#mat-radio-2"));
            uiIsOnINLand.Click();

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

            // select 'Yes' for 'Is the proposed RLRS located in a rural community with no other RLRS?'
            NgWebElement uiIsRlrsLocatedInRuralCommunityAloneYes = ngDriver.FindElement(By.CssSelector("#isRlrsLocatedInRuralCommunityAlone #mat-button-toggle-127 button"));
            uiIsRlrsLocatedInRuralCommunityAloneYes.Click();

            // select 'Yes' for 'Is the proposed RLRS located in a tourist destination resort with no other RLRS?'
            NgWebElement uiIsRlrsLocatedAtTouristDestinationAloneYes = ngDriver.FindElement(By.CssSelector("#isRlrsLocatedAtTouristDestinationAlone #mat-button-toggle-130 button"));
            uiIsRlrsLocatedAtTouristDestinationAloneYes.Click();

            // enter the resort description
            NgWebElement uirlrsResortCommunityDescription = ngDriver.FindElement(By.CssSelector("textarea#rlrsResortCommunityDescription"));
            uirlrsResortCommunityDescription.SendKeys(resortDescription);

            // select 'Yes' for 'Is there year-round all-weather road access to the community?'
            NgWebElement uiHasYearRoundAllWeatherRoadAccessYes = ngDriver.FindElement(By.CssSelector("#hasYearRoundAllWeatherRoadAccess #mat-button-toggle-133 button"));
            uiHasYearRoundAllWeatherRoadAccessYes.Click();

            // select 'Yes' for 'Does your general store operate seasonally?'
            NgWebElement uiDoesGeneralStoreOperateSeasonallyYes = ngDriver.FindElement(By.CssSelector("#doesGeneralStoreOperateSeasonally #mat-button-toggle-136 button"));
            uiDoesGeneralStoreOperateSeasonallyYes.Click();

            // select 'Yes' for 'Is the proposed RLRS located at least 10 km, by all-weather road, from another RLRS, LRS, or GLS?'
            NgWebElement uiIsRlrsAtLeast10kmFromAnotherStoreYes = ngDriver.FindElement(By.CssSelector("#isRlrsAtLeast10kmFromAnotherStore #mat-button-toggle-139 button"));
            uiIsRlrsAtLeast10kmFromAnotherStoreYes.Click();

            // enter the other business info
            NgWebElement uiOtherBusinessesDetails = ngDriver.FindElement(By.CssSelector("textarea#otherBusinessesDetails"));
            uiOtherBusinessesDetails.SendKeys(otherBusinesses);

            // select 'No' for 'Is the applicant the legal and beneficial owner of the general store?'
            NgWebElement uiIsApplicantOwnerOfStoreYes = ngDriver.FindElement(By.CssSelector("#isApplicantOwnerOfStore #mat-button-toggle-143 button"));
            uiIsApplicantOwnerOfStoreYes.Click();

            // enter the legal owners
            NgWebElement uiLegalAndBeneficialOwnersOfStore = ngDriver.FindElement(By.CssSelector("textarea#legalAndBeneficialOwnersOfStore"));
            uiLegalAndBeneficialOwnersOfStore.SendKeys(legalOwners);

            // select 'Yes' for 'Is the applicant a franchisee or otherwise affiliated with another business?'
            NgWebElement uiIsApplicantFranchiseOrAffiliatedYes = ngDriver.FindElement(By.CssSelector("#isApplicantFranchiseOrAffiliated #mat-button-toggle-145 button"));
            uiIsApplicantFranchiseOrAffiliatedYes.Click();

            // enter the name of the franchise or affiliated business
            NgWebElement uiFranchiseOrAffiliatedBusiness = ngDriver.FindElement(By.CssSelector("input#franchiseOrAffiliatedBusiness"));
            uiFranchiseOrAffiliatedBusiness.SendKeys(businessName);

            // upload the signage documents
            FileUpload("signage.pdf", "(//input[@type='file'])[11]");

            // upload the floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[14]");

            // upload the site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[17]");

            // upload the exterior photos
            FileUpload("exterior_photos.pdf", "(//input[@type='file'])[20]");

            // select the owner checkbox
            NgWebElement uiOwnerCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwnerCheckbox.Click();

            // select the owner's valid interest checkbox
            NgWebElement uiValidInterestCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterestCheckbox.Click();

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='willhaveValidInterest']"));
            uiZoningCheckbox.Click();

            // click on the authorized to submit checkbox
            NgWebElement uiAuthorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgree.Click();
        }
    }
}
