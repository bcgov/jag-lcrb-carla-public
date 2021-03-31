using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

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

            var estName = "Point Ellis Greenhouse";
            var estAddress = "645 Tyee Rd";
            var estCity = "Victoria";
            var estPostal = "V9A 6X5";
            var estPID = "012345678";
            var estEmail = "test@test.com";
            var estPhone = "2505555555";

            var indigenousNation = "Cowichan Tribes";
            var policeJurisdiction = "RCMP Shawnigan Lake";

            var resortDescription = "Sample resort description";
            var otherBusinesses = "Sample other businesses";
            var legalOwners = "Sample legal owners";
            var businessName = "Sample business name";

            var contactPhone = "2505555556";
            var contactEmail = "contact@test.com";

            var numberOfResidents = "18";

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
                // upload the personal history summary forms
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[3]");

            if (businessType == "society")
            {
                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[3]");

                // upload the personal history summary forms
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[6]");
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

            if (businessType == "private corporation")
                // upload the zoning document
                FileUpload("proof_of_zoning.pdf", "(//input[@type='file'])[18]");

            if (businessType == "partnership" || businessType == "public corporation" || businessType == "society")
                // upload the zoning document
                FileUpload("proof_of_zoning.pdf", "(//input[@type='file'])[9]");

            if (businessType == "sole proprietorship")
                // upload the zoning document
                FileUpload("proof_of_zoning.pdf", "(//input[@type='file'])[6]");

            // select 'Yes' for Treaty First Nation land
            var uiIsOnINLand =
                ngDriver.FindElement(By.CssSelector("[formcontrolname='isOnINLand'] mat-radio-button#mat-radio-13"));
            uiIsOnINLand.Click();

            // search for and select the indigenous nation
            var uiIndigenousNation = ngDriver.FindElement(By.CssSelector("input[formcontrolname='indigenousNation']"));
            uiIndigenousNation.SendKeys(indigenousNation);

            var uiIndigenousNation2 = ngDriver.FindElement(By.CssSelector("#mat-option-0 span"));
            uiIndigenousNation2.Click();

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

            // select 'Yes' for 'General store provides a range of products for residents to meet their basic grocery needs?'
            var uiHasSufficientRangeOfProductsYes = ngDriver.FindElement(By.Id("mat-button-toggle-397-button"));
            uiHasSufficientRangeOfProductsYes.Click();

            // select 'Yes' for 'Does your general store provide other products such as hardware supplies and sporting goods?'
            var uiHasOtherProductsYes = ngDriver.FindElement(By.Id("mat-button-toggle-400-button"));
            uiHasOtherProductsYes.Click();

            // select 'Yes' for 'General store provides services such as a post office, fishing/hunting licences, etc.'
            var uiHasAdditionalServicesYes = ngDriver.FindElement(By.Id("mat-button-toggle-403-button"));
            uiHasAdditionalServicesYes.Click();

            // select today for date of store opening
            var uiStoreOpenDate = ngDriver.FindElement(By.CssSelector("input#storeOpenDate"));
            uiStoreOpenDate.Click();

            var uiStoreOpenDate2 =
                ngDriver.FindElement(By.CssSelector(".mat-calendar-body-cell-content.mat-calendar-body-today"));
            JavaScriptClick(uiStoreOpenDate2);

            // select 'Yes' for 'Store business is solvent without liquor sales. Liquor sales will not be its primary business.'
            var uiConfirmLiquorSalesIsNotPrimaryBusinessYes =
                ngDriver.FindElement(By.Id("mat-button-toggle-406-button"));
            JavaScriptClick(uiConfirmLiquorSalesIsNotPrimaryBusinessYes);

            // select 'Yes' for 'Is the proposed RLRS located in a rural community with no other RLRS?'
            var uiIsRlrsLocatedInRuralCommunityAloneYes = ngDriver.FindElement(By.Id("mat-button-toggle-409-button"));
            JavaScriptClick(uiIsRlrsLocatedInRuralCommunityAloneYes);

            // select 'Yes' for 'Is the proposed RLRS located in a tourist destination resort with no other RLRS?'
            var uiIsRlrsLocatedAtTouristDestinationAloneYes =
                ngDriver.FindElement(By.Id("mat-button-toggle-412-button"));
            JavaScriptClick(uiIsRlrsLocatedAtTouristDestinationAloneYes);

            // enter the resort description
            var uirlrsResortCommunityDescription =
                ngDriver.FindElement(By.CssSelector("textarea#rlrsResortCommunityDescription"));
            uirlrsResortCommunityDescription.SendKeys(resortDescription);

            // select 'Yes' for 'Is there year-round all-weather road access to the community?'
            var uiHasYearRoundAllWeatherRoadAccessYes = ngDriver.FindElement(By.Id("mat-button-toggle-415-button"));
            JavaScriptClick(uiHasYearRoundAllWeatherRoadAccessYes);

            // select 'Yes' for 'Does your general store operate seasonally?'
            var uiDoesGeneralStoreOperateSeasonallyYes = ngDriver.FindElement(By.Id("mat-button-toggle-418-button"));
            JavaScriptClick(uiDoesGeneralStoreOperateSeasonallyYes);

            // enter number of residents within a 5km radius
            var uiSurroundingResidentsOfRlrsYes =
                ngDriver.FindElement(By.CssSelector("input#surroundingResidentsOfRlrs"));
            uiSurroundingResidentsOfRlrsYes.SendKeys(numberOfResidents);

            // select 'Yes' for 'Is the proposed RLRS located at least 10 km, by all-weather road, from another RLRS, LRS, or GLS?'
            var uiIsRlrsAtLeast10kmFromAnotherStoreYes = ngDriver.FindElement(By.Id("mat-button-toggle-421-button"));
            JavaScriptClick(uiIsRlrsAtLeast10kmFromAnotherStoreYes);

            // enter the other business info
            var uiOtherBusinessesDetails = ngDriver.FindElement(By.CssSelector("textarea#otherBusinessesDetails"));
            uiOtherBusinessesDetails.SendKeys(otherBusinesses);

            // select 'No' for 'Is the applicant the legal and beneficial owner of the general store?'
            var uiIsApplicantOwnerOfStoreYes = ngDriver.FindElement(By.Id("mat-button-toggle-424-button"));
            JavaScriptClick(uiIsApplicantOwnerOfStoreYes);

            // enter the legal owners
            var uiLegalAndBeneficialOwnersOfStore =
                ngDriver.FindElement(By.CssSelector("textarea#legalAndBeneficialOwnersOfStore"));
            uiLegalAndBeneficialOwnersOfStore.SendKeys(legalOwners);

            // select 'Yes' for 'Is the applicant a franchisee or otherwise affiliated with another business?'
            var uiIsApplicantFranchiseOrAffiliatedYes = ngDriver.FindElement(By.Id("mat-button-toggle-427-button"));
            JavaScriptClick(uiIsApplicantFranchiseOrAffiliatedYes);

            // enter the name of the franchise or affiliated business
            var uiFranchiseOrAffiliatedBusiness =
                ngDriver.FindElement(By.CssSelector("input#franchiseOrAffiliatedBusiness"));
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

            if (businessType == "partnership" || businessType == "public corporation" || businessType == "society")
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
            var uiOwnerCheckbox =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwnerCheckbox.Click();

            // select the owner's valid interest checkbox
            var uiValidInterestCheckbox =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterestCheckbox.Click();

            // select the zoning checkbox
            var uiZoningCheckbox =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiZoningCheckbox.Click();

            // enter the contact phone number
            var uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonPhone']"));
            uiContactPhone.SendKeys(contactPhone);

            // enter the contact email
            var uiContactEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonEmail']"));
            uiContactEmail.SendKeys(contactEmail);

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
    }
}