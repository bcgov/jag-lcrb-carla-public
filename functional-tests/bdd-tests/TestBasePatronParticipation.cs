using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a Patron Participation Entertainment Endorsement application")]
        public void PatronParticipationApplication()
        {
            /* 
            Page Title: Please Review Your Account Profile
            */

            // click on Continue to Application button
            ContinueToApplicationButton();

            /* 
            Page Title: Patron Participation Entertainment Endorsement Application
            */

            // create test data
            var patronEntertainment = "Sample patron entertainment.";

            // enter the patron entertainment
            var uiPatronEntertainment = ngDriver.FindElement(By.CssSelector("textarea#description2"));
            uiPatronEntertainment.SendKeys(patronEntertainment);

            // click on authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}