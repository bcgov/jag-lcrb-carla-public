using System;
using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the Cannabis Marketing application for (.*)")]
        public void CannabisMarketingApplication(string bizType)
        {
            /* 
            Page Title: Submit the Cannabis Marketing Licence Application
            */

            var nameOfFederalProducer = "Canadian Cannabis";
            var marketerConnectionToCrsDetails = "Details of association (marketer to store)";
            var crsConnectionToMarketer = "Details of association (store to marketer)";
            var contactTitle = "VP Marketing";
            var contactPhone = "5555555555";
            var contactEmail = "vp@cannabis_marketing.com";

            if (bizType == "private corporation")
            {
                // upload a central securities register
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                // upload supporting business documentation
                FileUpload("associates.pdf", "(//input[@type='file'])[6]");

                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[9]");

                // upload cannabis associate security screening
                FileUpload("cannabis_associate_security_screening.pdf", "(//input[@type='file'])[12]");

                // upload financial integrity documents
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[15]");

                // upload shareholders < 10% interest
                FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[18]");
            }

            if (bizType == "sole proprietorship")
            {
                // upload cannabis associate security screening
                FileUpload("cannabis_associate_security_screening.pdf", "(//input[@type='file'])[3]");

                // upload financial integrity documents
                FileUpload("fin_integrity.pdf", "(//input[@type='file'])[6]");
            }

            if (bizType != "a local government" && bizType != "a university")
            {
                // enter name of federal producer
                var uiFederalProducer =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='federalProducerNames']"));
                uiFederalProducer.SendKeys(nameOfFederalProducer);

                // select 'Yes'
                // Does the corporation have any association, connection or financial interest in a B.C. non-medical cannabis retail store licensee or applicant of cannabis?
                var uiMarketerConnectionToCrs = ngDriver.FindElement(
                    By.CssSelector("input[formcontrolname='marketerConnectionToCrs'][type='radio'][value='Yes']"));
                uiMarketerConnectionToCrs.Click();

                // enter the details
                var uiMarketerConnectionToCrsDetails =
                    ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='marketerConnectionToCrsDetails']"));
                uiMarketerConnectionToCrsDetails.SendKeys(marketerConnectionToCrsDetails);

                // !society !indigenousnation
                if (bizType == "a private corporation" || bizType == "a partnership")
                {
                    // select 'Yes'
                    // Does a B.C. non-medical cannabis retail store licensee or applicant of cannabis have any association, connection or financial interest in the corporation? 
                    var uiCrsConnectionToMarketer = ngDriver.FindElement(
                        By.CssSelector("input[formcontrolname='crsConnectionToMarketer'][type='radio'][value='Yes']"));
                    uiCrsConnectionToMarketer.Click();

                    // enter the details
                    var uiCrsConnectionToMarketerDetails =
                        ngDriver.FindElement(
                            By.CssSelector("textarea[formcontrolname='crsConnectionToMarketerDetails']"));
                    uiCrsConnectionToMarketerDetails.SendKeys(crsConnectionToMarketer);
                }

                if (bizType == "a public corporation")
                {
                    // select 'Yes'
                    // Does any shareholder with 20% or more voting shares have any association, connection or financial interest in a B.C. non-medical cannabis retail store licensee or applicant of cannabis?
                    var uiCrsConnectionToMarketer = ngDriver.FindElement(
                        By.CssSelector("input[formcontrolname='crsConnectionToMarketer'][type='radio'][value='Yes']"));
                    uiCrsConnectionToMarketer.Click();

                    // enter the details
                    var uiCrsConnectionToMarketerDetails =
                        ngDriver.FindElement(
                            By.CssSelector("textarea[formcontrolname='crsConnectionToMarketerDetails']"));
                    uiCrsConnectionToMarketerDetails.SendKeys(crsConnectionToMarketer);
                }

                if (bizType == "a sole proprietorship")
                {
                    // select 'Yes'
                    // Does the sole proprietor have an immediate family member that has any interest in a licensee or applicant?
                    var uiCrsConnectionToMarketer = ngDriver.FindElement(
                        By.CssSelector("input[formcontrolname='crsConnectionToMarketer'][type='radio'][value='Yes']"));
                    uiCrsConnectionToMarketer.Click();

                    // enter the details
                    var uiCrsConnectionToMarketerDetails =
                        ngDriver.FindElement(
                            By.CssSelector("textarea[formcontrolname='crsConnectionToMarketerDetails']"));
                    uiCrsConnectionToMarketerDetails.SendKeys(crsConnectionToMarketer);
                }
            }

            if (bizType != "sole proprietorship")
            {
                // upload the Associates form
                FileUpload("associates.pdf", "(//input[@type='file'])[21]");

                // upload the Notice of Articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[23]");

                // upload the Central Securities Register
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[23]");
            }

            // enter the contact title
            var uiContactPersonRole =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonRole']"));
            uiContactPersonRole.SendKeys(contactTitle);

            // enter the contact phone number
            var uiContactPersonPhone =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonPhone']"));
            uiContactPersonPhone.SendKeys(contactPhone);

            // enter the contact email address
            var uiContactPersonEmail =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonEmail']"));
            uiContactPersonEmail.SendKeys(contactEmail);

            // select authorized to submit checkbox
            var uiAuthorizedToSubmit =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select signature agreement checkbox
            var uiSignatureAgreement =
                ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            // retrieve the application ID
            var parsedURL = ngDriver.Url.Split('/');

            applicationID = parsedURL[5];
        }
    }
}