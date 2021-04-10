using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the Rural LRS relocation application")]
        public void CompleteRuralLRSRelocationApplication()
        {
            /* 
            Page Title: Relocation Application
            */

            // create test data
            var resortDescription = "Sample resort description.";
            var numberOfResidents = "18";
            var otherBusinesses = "Sample other businesses.";
            var legalOwners = "Sample legal owners.";
            var business = "Sample business.";

            // upload proof of zoning 
            FileUpload("proof_of_zoning.pdf", "(//input[@type='file'])[3]");

            // select 'Yes' for 'Is the proposed RLRS located in a rural community with no other RLRS?'
            var uiIsRlrsLocatedInRuralCommunityAloneYes = ngDriver.FindElement(By.Id("mat-button-toggle-169-button"));
            JavaScriptClick(uiIsRlrsLocatedInRuralCommunityAloneYes);

            // select 'Yes' for 'Is the proposed RLRS located in a tourist destination resort with no other RLRS?'
            var uiIsRlrsLocatedAtTouristDestinationAloneYes =
                ngDriver.FindElement(By.Id("mat-button-toggle-172-button"));
            JavaScriptClick(uiIsRlrsLocatedAtTouristDestinationAloneYes);

            // enter resort community description
            var uiRlrsResortCommunityDescription =
                ngDriver.FindElement(By.CssSelector("textarea#rlrsResortCommunityDescription"));
            uiRlrsResortCommunityDescription.SendKeys(resortDescription);

            // select 'Yes' for 'Is there year-round all-weather road access to the community?'
            var uiHasYearRoundAllWeatherRoadAccessYes = ngDriver.FindElement(By.Id("mat-button-toggle-175-button"));
            JavaScriptClick(uiHasYearRoundAllWeatherRoadAccessYes);

            // select 'Yes' for 'Does your general store operate seasonally?'
            var uiDoesGeneralStoreOperateSeasonallyYes = ngDriver.FindElement(By.Id("mat-button-toggle-178-button"));
            JavaScriptClick(uiDoesGeneralStoreOperateSeasonallyYes);

            // enter the number of residents
            var uiNumberOfResidents = ngDriver.FindElement(By.CssSelector("input#surroundingResidentsOfRlrs"));
            uiNumberOfResidents.SendKeys(numberOfResidents);

            // select 'Yes' for 'Is the proposed RLRS located at least 10 km, by all-weather road, from another RLRS, LRS, or GLS?'
            var uiIsRlrsAtLeast10kmFromAnotherStoreYes = ngDriver.FindElement(By.Id("mat-button-toggle-181-button"));
            JavaScriptClick(uiIsRlrsAtLeast10kmFromAnotherStoreYes);

            // enter the other businesses
            var uiOtherBusinesses = ngDriver.FindElement(By.CssSelector("textarea#otherBusinessesDetails"));
            uiOtherBusinesses.SendKeys(otherBusinesses);

            // select 'Yes' for 'Is the applicant the legal and beneficial owner of the general store?'
            var uiIsApplicantOwnerOfStoreYes = ngDriver.FindElement(By.Id("mat-button-toggle-184-button"));
            JavaScriptClick(uiIsApplicantOwnerOfStoreYes);

            // enter the legal owners
            var uiLegalOwners = ngDriver.FindElement(By.CssSelector("textarea#legalAndBeneficialOwnersOfStore"));
            uiLegalOwners.SendKeys(legalOwners);

            // select 'Yes' for 'Is the applicant a franchisee or otherwise affiliated with another business?'
            var uiIsApplicantFranchiseOrAffiliatedYes = ngDriver.FindElement(By.Id("mat-button-toggle-187-button"));
            JavaScriptClick(uiIsApplicantFranchiseOrAffiliatedYes);

            // enter the business
            var uiBusiness = ngDriver.FindElement(By.CssSelector("input#franchiseOrAffiliatedBusiness"));
            uiBusiness.SendKeys(business);

            // upload signage
            FileUpload("signage.pdf", "(//input[@type='file'])[5]");

            // upload floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[8]");

            // upload site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[11]");

            // upload exterior photos
            FileUpload("exterior_photos.pdf", "(//input[@type='file'])[14]");

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

            // upload proof of valid interest
            FileUpload("valid_interest.pdf", "(//input[@type='file'])[18]");

            // select the authorized to submit checkbox
            var uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            var uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();
        }
    }
}