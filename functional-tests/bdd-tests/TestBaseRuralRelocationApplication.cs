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
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

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
            string proposedAddress = "645 Tyee Road";
            string proposedCity = "Victoria";
            string proposedPostalCode = "V9A 6X5";
            string proposedPID = "111111111";

            string resortDescription = "Sample resort description.";
            string numberOfResidents = "18";
            string otherBusinesses = "Sample other businesses.";
            string legalOwners = "Sample legal owners.";
            string business = "Sample business.";

            // upload proof of zoning 
            FileUpload("proof_of_zoning.pdf", "(//input[@type='file'])[3]");

            // select 'Yes' for 'Is the proposed RLRS located in a rural community with no other RLRS?'
            NgWebElement uiIsRlrsLocatedInRuralCommunityAloneYes = ngDriver.FindElement(By.Id("mat-button-toggle-127-button"));
            JavaScriptClick(uiIsRlrsLocatedInRuralCommunityAloneYes);

            // select 'Yes' for 'Is the proposed RLRS located in a tourist destination resort with no other RLRS?'
            NgWebElement uiIsRlrsLocatedAtTouristDestinationAloneYes = ngDriver.FindElement(By.Id("mat-button-toggle-130-button"));
            JavaScriptClick(uiIsRlrsLocatedAtTouristDestinationAloneYes);

            // enter resort community description
            NgWebElement uiRlrsResortCommunityDescription = ngDriver.FindElement(By.CssSelector("textarea#rlrsResortCommunityDescription"));
            uiRlrsResortCommunityDescription.SendKeys(resortDescription);

            // select 'Yes' for 'Is there year-round all-weather road access to the community?'
            NgWebElement uiHasYearRoundAllWeatherRoadAccessYes = ngDriver.FindElement(By.Id("mat-button-toggle-133-button"));
            JavaScriptClick(uiHasYearRoundAllWeatherRoadAccessYes);

            // select 'Yes' for 'Does your general store operate seasonally?'
            NgWebElement uiDoesGeneralStoreOperateSeasonallyYes = ngDriver.FindElement(By.Id("mat-button-toggle-136-button"));
            JavaScriptClick(uiDoesGeneralStoreOperateSeasonallyYes);

            // enter the number of residents
            NgWebElement uiNumberOfResidents = ngDriver.FindElement(By.CssSelector("input#surroundingResidentsOfRlrs"));
            uiNumberOfResidents.SendKeys(numberOfResidents);

            // select 'Yes' for 'Is the proposed RLRS located at least 10 km, by all-weather road, from another RLRS, LRS, or GLS?'
            NgWebElement uiIsRlrsAtLeast10kmFromAnotherStoreYes = ngDriver.FindElement(By.Id("mat-button-toggle-139-button"));
            JavaScriptClick(uiIsRlrsAtLeast10kmFromAnotherStoreYes);

            // enter the other businesses
            NgWebElement uiOtherBusinesses = ngDriver.FindElement(By.CssSelector("textarea#otherBusinessesDetails"));
            uiOtherBusinesses.SendKeys(otherBusinesses);

            // select 'Yes' for 'Is the applicant the legal and beneficial owner of the general store?'
            NgWebElement uiIsApplicantOwnerOfStoreYes = ngDriver.FindElement(By.Id("mat-button-toggle-142-button"));
            JavaScriptClick(uiIsApplicantOwnerOfStoreYes);

            // enter the legal owners
            NgWebElement uiLegalOwners = ngDriver.FindElement(By.CssSelector("textarea#legalAndBeneficialOwnersOfStore"));
            uiLegalOwners.SendKeys(legalOwners);

            // select 'Yes' for 'Is the applicant a franchisee or otherwise affiliated with another business?'
            NgWebElement uiIsApplicantFranchiseOrAffiliatedYes = ngDriver.FindElement(By.Id("mat-button-toggle-145-button"));
            JavaScriptClick(uiIsApplicantFranchiseOrAffiliatedYes);

            // enter the business
            NgWebElement uiBusiness = ngDriver.FindElement(By.CssSelector("input#franchiseOrAffiliatedBusiness"));
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
            NgWebElement uiOwnerCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwnerCheckbox.Click();

            // select the owner's valid interest checkbox
            NgWebElement uiValidInterestCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterestCheckbox.Click();

            // select the zoning checkbox
            NgWebElement uiZoningCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiZoningCheckbox.Click();

            // upload proof of valid interest
            FileUpload("valid_interest.pdf", "(//input[@type='file'])[18]");

            // select the authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();
        }
    }
}
