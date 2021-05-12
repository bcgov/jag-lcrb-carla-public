using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a new outdoor patio application")]
        public void NewOutdoorPatioApplication()
        {
            /* 
            Page Title: New Outdoor Patio
            */

            // create test data
            var patioLocationDescription = "Sample patio location description.";
            var patioAccessDescription = "Sample patio access description.";
            var patioLiquorCarriedDescription = "Sample liquor carried description.";
            var areaDescription = "Sample area description";
            var occupantLoad = "99999";

            // select monitor and control checkbox	
            var uiIsBoundingSufficientForControl = ngDriver.FindElement(By.Id("isBoundingSufficientForControl"));
            uiIsBoundingSufficientForControl.Click();

            // select definition checkbox	
            var uiIsBoundingSufficientToDefine = ngDriver.FindElement(By.Id("isBoundingSufficientToDefine"));
            uiIsBoundingSufficientToDefine.Click();

            // select adequate care checkbox	
            var uiIsAdequateCare = ngDriver.FindElement(By.Id("isAdequateCare"));
            uiIsAdequateCare.Click();

            // enter patio location description
            var uiPatioLocationDescription = ngDriver.FindElement(By.CssSelector("textarea#patioLocationDescription"));
            uiPatioLocationDescription.SendKeys(patioLocationDescription);

            // select status of patio area construction
            var uiConstructionStatus = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-1-button"));
            uiConstructionStatus.Click();

            // select construction completion date
            var uiStoreOpenDate = ngDriver.FindElement(By.Id("storeOpenDate"));
            uiStoreOpenDate.Click();

            SharedCalendarDate();

            // select TESA checkbox
            var uiIsTESA = ngDriver.FindElement(By.Id("isTESA"));
            uiIsTESA.Click();

            // select January checkbox
            var uiJanuary = ngDriver.FindElement(By.Id("isMonth01"));
            uiJanuary.Click();

            // enter patio access description
            var uiPatioAccessDescription = ngDriver.FindElement(By.CssSelector("textarea#patioAccessDescription"));
            uiPatioAccessDescription.SendKeys(patioAccessDescription);

            // click on Is Liquor Carried checkbox
            var uiPatioIsLiquorCarried = ngDriver.FindElement(By.CssSelector("mat-checkbox#patioIsLiquorCarried"));
            JavaScriptClick(uiPatioIsLiquorCarried);

            // enter Is Liquor Carried description
            var uiPatioLiquorCarriedDescription =
                ngDriver.FindElement(By.CssSelector("textarea#patioLiquorCarriedDescription"));
            uiPatioLiquorCarriedDescription.SendKeys(patioLiquorCarriedDescription);

            // click Fixed service bar(s) on patio option
            var uiFixedOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-69-button"));
            uiFixedOption.Click();

            // click Portable service bar(s) on patio option
            var uiPortableOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-70-button"));
            uiPortableOption.Click();

            // click The interior service bar(s) option
            var uiInteriorOption = ngDriver.FindElement(By.CssSelector("#mat-button-toggle-71-button"));
            uiInteriorOption.Click();

            // upload floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[2]");

            // click on service area button
            var uiServiceAreas = ngDriver.FindElement(By.CssSelector("[formcontrolname= 'serviceAreas'] button"));
            uiServiceAreas.Click();

            // enter area description
            var uiAreaDescription = ngDriver.FindElement(By.CssSelector("input[formcontrolname='areaLocation']"));
            uiAreaDescription.SendKeys(areaDescription);

            // enter occupant load
            var uiOccupantLoad = ngDriver.FindElement(By.CssSelector("input[formcontrolname='capacity']"));
            uiOccupantLoad.SendKeys(occupantLoad);

            // upload exterior photos
            // FileUpload("exterior_photos.pdf", "(//input[@type='file'])[5]");

            // click on the isOwnerBusiness checkbox
            var uiIsOwnerBusiness =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiIsOwnerBusiness.Click();

            // click on the hasValidInterest checkbox
            var uiHasValidInterest =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='hasValidInterest']"));
            uiHasValidInterest.Click();

            // click on the willHaveValidInterest checkbox
            var uiWillHaveValidInterest =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='willHaveValidInterest']"));
            uiWillHaveValidInterest.Click();

            // click on the authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}