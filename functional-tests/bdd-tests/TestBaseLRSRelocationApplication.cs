using System;
using System.Threading;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the LRS application")]
        public void CompleteLRSApplication()
        {
            /* 
            Page Title: LRS Relocation Application
            */

            // create test data
            var proposedAddress = "645 Tyee Road";
            var proposedCity = "Victoria";
            var proposedPostalCode = "V9A 6X5";
            var proposedPID = "111111111";

            // enter the proposed address
            var uiProposedAddress =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            uiProposedAddress.SendKeys(proposedAddress);

            // enter the proposed city
            var uiProposedCity =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            uiProposedCity.SendKeys(proposedCity);

            // enter the proposed postal code
            var uiProposedPostalCode =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressPostalCode']"));
            uiProposedPostalCode.SendKeys(proposedPostalCode);

            // enter the proposed PID
            var uiProposedPID = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentParcelId']"));
            uiProposedPID.SendKeys(proposedPID);

            // upload the signage document
            FileUpload("signage.pdf", "(//input[@type='file'])[2]");

            // upload the floor plan
            FileUpload("floor_plan.pdf", "(//input[@type='file'])[5]");

            // upload the site plan
            FileUpload("site_plan.pdf", "(//input[@type='file'])[8]");

            // upload the exterior photos
            FileUpload("exterior_photos.jpg", "(//input[@type='file'])[11]");

            // select 'Yes' for proposed LRS site located within a grocery store
            var uiProposedSiteInGrocery =
                ngDriver.FindElement(By.CssSelector("#mat-button-toggle-1 button#mat-button-toggle-1-button"));
            JavaScriptClick(uiProposedSiteInGrocery);

            // upload grocery declaration document
            FileUpload("grocery_declaration.pdf", "(//input[@type='file'])[15]");

            // select the owner checkbox
            var uiOwner = ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='isOwnerBusiness']"));
            uiOwner.Click();

            // select the valid interest checkbox
            var uiValidInterest =
                ngDriver.FindElement(By.CssSelector(".mat-checkbox[formcontrolname='hasValidInterest']"));
            uiValidInterest.Click();

            // select the future valid interest checkbox
            var uiFutureValidInterest =
                ngDriver.FindElement(
                    By.CssSelector("mat-checkbox#mat-checkbox-4[formcontrolname='willhaveValidInterest']"));
            uiFutureValidInterest.Click();

            // upload valid interest document
            FileUpload("valid_interest.pdf", "(//input[@type='file'])[18]");

            // select the authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit'][type='checkbox']"));
            uiAuthorizedToSubmit.Click();

            // select the signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement'][type='checkbox']"));
            uiSignatureAgreement.Click();

            // click on the submit for LG/IN review button
            ClickOnSubmitButton();
        }


        [And(@"an LRS licence has been created")]
        public void LRSApproval()
        {
            Thread.Sleep(4000);

            var testWines = "login/token/TestWines";
            ngDriver.Navigate().GoToUrl($"{baseUri}{testWines}");
        }
    }
}