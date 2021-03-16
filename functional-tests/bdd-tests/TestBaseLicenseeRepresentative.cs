using System;
using System.Threading;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a licensee representative")]
        public void RequestLicenseeRepresentative()
        {
            /* 
            Page Title: Licences & Authorizations
            */

            // click on the Licensee Representative link
            var addLicensee = "Add Licensee Representative";
            var uiAddLicensee = ngDriver.FindElement(By.LinkText(addLicensee));
            uiAddLicensee.Click();

            // create test data
            var representativeName = "Automated Test";
            var telephone = "2005081818";
            var email = "automated@test.com";

            // enter the representative name
            var uiFullName = ngDriver.FindElement(By.CssSelector("input[formControlName='representativeFullName']"));
            uiFullName.SendKeys(representativeName);

            // enter the representative telephone number
            var uiPhoneNumber =
                ngDriver.FindElement(By.CssSelector("input[formControlName='representativePhoneNumber']"));
            uiPhoneNumber.SendKeys(telephone);

            // enter the representative email address
            var uiEmail = ngDriver.FindElement(By.CssSelector("input[formControlName='representativeEmail']"));
            uiEmail.SendKeys(email);

            Thread.Sleep(3000);

            try
            {
                // click on the submit permanent change applications checkbox
                var uiCheckbox =
                    ngDriver.FindElement(By.CssSelector(
                        "mat-checkbox[formcontrolname='representativeCanSubmitPermanentChangeApplications']"));
                uiCheckbox.Click();
            }
            catch
            {
                // click on the submit permanent change applications checkbox
                var uiCheckbox =
                    ngDriver.FindElement(By.CssSelector(
                        "mat-checkbox[formcontrolname='representativeCanSubmitPermanentChangeApplications']"));
                JavaScriptClick(uiCheckbox);
            }

            // click on the sign temporary change applications checkbox
            var uiCheckbox1 =
                ngDriver.FindElement(
                    By.CssSelector("mat-checkbox[formcontrolname='representativeCanSignTemporaryChangeApplications']"));
            JavaScriptClick(uiCheckbox1);

            // click on the obtain licence info from branch checkbox
            var uiCheckbox2 =
                ngDriver.FindElement(
                    By.CssSelector("mat-checkbox[formcontrolname='representativeCanObtainLicenceInformation']"));
            uiCheckbox2.Click();

            // click on sign grocery annual proof of sales revenue checkbox
            var uiCheckbox3 =
                ngDriver.FindElement(
                    By.CssSelector("mat-checkbox[formcontrolname='representativeCanSignGroceryStoreProofOfSale']"));
            uiCheckbox3.Click();

            // click on attend education sessions checkbox
            var uiCheckbox4 =
                ngDriver.FindElement(
                    By.CssSelector("mat-checkbox[formcontrolname='representativeCanAttendEducationSessions']"));
            uiCheckbox4.Click();

            // click on attend compliance meetings checkbox
            var uiCheckbox5 =
                ngDriver.FindElement(
                    By.CssSelector("mat-checkbox[formcontrolname='representativeCanAttendComplianceMeetings']"));
            uiCheckbox5.Click();

            // click on represent licensee at enforcement hearings checkbox
            var uiCheckbox6 =
                ngDriver.FindElement(
                    By.CssSelector("mat-checkbox[formcontrolname='representativeCanRepresentAtHearings']"));
            uiCheckbox6.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgree =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgree.Click();

            var uiSubmitButton = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
            JavaScriptClick(uiSubmitButton);
        }
    }
}