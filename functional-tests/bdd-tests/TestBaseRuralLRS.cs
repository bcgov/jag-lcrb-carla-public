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
        [And(@"I complete the Rural LRS application for a (.*)")]
        public void CompleteRuralLRSApplication(string businessType)
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

            string indigenousNation = "Cowichan Tribes";
            string policeJurisdiction = "RCMP Shawnigan Lake";

            string resortDescription = "Sample resort description";
            string otherBusinesses = "Sample other businesses";
            string legalOwners = "Sample legal owners";
            string businessName = "Sample business name";

            string contactPhone = "2505555556";
            string contactEmail = "contact@test.com";

            string numberOfResidents = "18";

            if (businessType == "private corporation")
            {
                // upload the central securities register
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                // upload supporting business documentation
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[6]");

                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[9]");

                // upload the personal history summary forms
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[12]");

                // upload Shareholders (individuals) Holding Less Than 10% Interest
                FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[15]");
            }

            if (businessType == "public corporation")
            {
                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[3]");

                // upload the personal history summary forms
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[6]");
            }

            if (businessType == "partnership")
            {
                // upload the partnership agreement
                FileUpload("partnership_agreement.pdf", "(//input[@type='file'])[3]");

                // upload the personal history summary forms
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[6]");
            }

            if (businessType == "sole proprietorship")
            {
                // upload the personal history summary forms
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[3]");
            }

            if (businessType == "society")
            {
                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[3]");

                // upload the personal history summary forms
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[6]");
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

            if (businessType == "private corporation")
            {
                // upload the zoning document
                FileUpload("proof_of_zoning.pdf", "(//input[@type='file'])[18]");
            }

            if ((businessType == "partnership") || (businessType == "public corporation") || (businessType == "society"))
            {
                // upload the zoning document
                FileUpload("proof_of_zoning.pdf", "(//input[@type='file'])[9]");
            }

            if (businessType == "sole proprietorship")
            {
                // upload the zoning document
                FileUpload("proof_of_zoning.pdf", "(//input[@type='file'])[6]");
            }

            // select 'Yes' for Treaty First Nation land
            NgWebElement uiIsOnINLand = ngDriver.FindElement(By.CssSelector("[formcontrolname='isOnINLand'] mat-radio-button#mat-radio-13"));
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

            // select 'Yes' for 'General store provides a range of products for residents to meet their basic grocery needs?'
            NgWebElement uiHasSufficientRangeOfProductsYes = ngDriver.FindElement(By.Id("mat-button-toggle-199-button"));
            uiHasSufficientRangeOfProductsYes.Click();

            // select 'Yes' for 'Does your general store provide other products such as hardware supplies and sporting goods?'
            NgWebElement uiHasOtherProductsYes = ngDriver.FindElement(By.Id("mat-button-toggle-268-button"));
            uiHasOtherProductsYes.Click();

            // select 'Yes' for 'General store provides services such as a post office, fishing/hunting licences, etc.'
            NgWebElement uiHasAdditionalServicesYes = ngDriver.FindElement(By.Id("mat-button-toggle-271-button"));
            uiHasAdditionalServicesYes.Click();

            // select today for date of store opening
            NgWebElement uiStoreOpenDate = ngDriver.FindElement(By.CssSelector("input#storeOpenDate"));
            uiStoreOpenDate.Click();

            NgWebElement uiStoreOpenDate2 = ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            JavaScriptClick(uiStoreOpenDate2);

            // select 'Yes' for 'Store business is solvent without liquor sales. Liquor sales will not be its primary business.'
            NgWebElement uiConfirmLiquorSalesIsNotPrimaryBusinessYes = ngDriver.FindElement(By.Id("mat-button-toggle-274-button"));
            JavaScriptClick(uiConfirmLiquorSalesIsNotPrimaryBusinessYes);

            // select 'Yes' for 'Is the proposed RLRS located in a rural community with no other RLRS?'
            NgWebElement uiIsRlrsLocatedInRuralCommunityAloneYes = ngDriver.FindElement(By.Id("mat-button-toggle-277-button"));
            JavaScriptClick(uiIsRlrsLocatedInRuralCommunityAloneYes);

            // select 'Yes' for 'Is the proposed RLRS located in a tourist destination resort with no other RLRS?'
            NgWebElement uiIsRlrsLocatedAtTouristDestinationAloneYes = ngDriver.FindElement(By.Id("mat-button-toggle-280-button"));
            JavaScriptClick(uiIsRlrsLocatedAtTouristDestinationAloneYes);

            // enter the resort description
            NgWebElement uirlrsResortCommunityDescription = ngDriver.FindElement(By.CssSelector("textarea#rlrsResortCommunityDescription"));
            uirlrsResortCommunityDescription.SendKeys(resortDescription);

            // select 'Yes' for 'Is there year-round all-weather road access to the community?'
            NgWebElement uiHasYearRoundAllWeatherRoadAccessYes = ngDriver.FindElement(By.Id("mat-button-toggle-283-button"));
            JavaScriptClick(uiHasYearRoundAllWeatherRoadAccessYes);

            // select 'Yes' for 'Does your general store operate seasonally?'
            NgWebElement uiDoesGeneralStoreOperateSeasonallyYes = ngDriver.FindElement(By.Id("mat-button-toggle-286-button"));
            JavaScriptClick(uiDoesGeneralStoreOperateSeasonallyYes);

            // enter number of residents within a 5km radius
            NgWebElement uiSurroundingResidentsOfRlrsYes = ngDriver.FindElement(By.CssSelector("input#surroundingResidentsOfRlrs"));
            uiSurroundingResidentsOfRlrsYes.SendKeys(numberOfResidents);

            // select 'Yes' for 'Is the proposed RLRS located at least 10 km, by all-weather road, from another RLRS, LRS, or GLS?'
            NgWebElement uiIsRlrsAtLeast10kmFromAnotherStoreYes = ngDriver.FindElement(By.Id("mat-button-toggle-289-button"));
            JavaScriptClick(uiIsRlrsAtLeast10kmFromAnotherStoreYes);

            // enter the other business info
            NgWebElement uiOtherBusinessesDetails = ngDriver.FindElement(By.CssSelector("textarea#otherBusinessesDetails"));
            uiOtherBusinessesDetails.SendKeys(otherBusinesses);

            // select 'No' for 'Is the applicant the legal and beneficial owner of the general store?'
            NgWebElement uiIsApplicantOwnerOfStoreYes = ngDriver.FindElement(By.Id("mat-button-toggle-292-button"));
            JavaScriptClick(uiIsApplicantOwnerOfStoreYes);

            // enter the legal owners
            NgWebElement uiLegalAndBeneficialOwnersOfStore = ngDriver.FindElement(By.CssSelector("textarea#legalAndBeneficialOwnersOfStore"));
            uiLegalAndBeneficialOwnersOfStore.SendKeys(legalOwners);

            // select 'Yes' for 'Is the applicant a franchisee or otherwise affiliated with another business?'
            NgWebElement uiIsApplicantFranchiseOrAffiliatedYes = ngDriver.FindElement(By.Id("mat-button-toggle-295-button"));
            JavaScriptClick(uiIsApplicantFranchiseOrAffiliatedYes);

            // enter the name of the franchise or affiliated business
            NgWebElement uiFranchiseOrAffiliatedBusiness = ngDriver.FindElement(By.CssSelector("input#franchiseOrAffiliatedBusiness"));
            uiFranchiseOrAffiliatedBusiness.SendKeys(businessName);

            if (businessType == "private corporation")
            {
                // upload the signage documents
                FileUpload("signage.pdf", "(//input[@type='file'])[20]");

                // upload the floor plan
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[23]");

                // upload the site plan
                FileUpload("site_plan.pdf", "(//input[@type='file'])[26]");

                // upload the exterior photos
                FileUpload("exterior_photos.pdf", "(//input[@type='file'])[29]");
            }

            if ((businessType == "partnership") || (businessType == "public corporation") || (businessType == "society"))
            {
                // upload the signage documents
                FileUpload("signage.pdf", "(//input[@type='file'])[11]");

                // upload the floor plan
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[14]");

                // upload the site plan
                FileUpload("site_plan.pdf", "(//input[@type='file'])[17]");

                // upload the exterior photos
                FileUpload("exterior_photos.pdf", "(//input[@type='file'])[20]");
            }

            if (businessType == "sole proprietorship")
            {
                // upload the signage documents
                FileUpload("signage.pdf", "(//input[@type='file'])[8]");

                // upload the floor plan
                FileUpload("floor_plan.pdf", "(//input[@type='file'])[11]");

                // upload the site plan
                FileUpload("site_plan.pdf", "(//input[@type='file'])[14]");

                // upload the exterior photos
                FileUpload("exterior_photos.pdf", "(//input[@type='file'])[17]");
            }

            // select the owner checkbox
            NgWebElement uiOwnerCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwnerCheckbox.Click();

            // select the owner's valid interest checkbox
            NgWebElement uiValidInterestCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterestCheckbox.Click();

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiZoningCheckbox.Click();

            // enter the contact phone number
            NgWebElement uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonPhone']"));
            uiContactPhone.SendKeys(contactPhone);

            // enter the contact email
            NgWebElement uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonEmail']"));
            uiContactEmail.SendKeys(contactEmail);

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
        }
    }
}
