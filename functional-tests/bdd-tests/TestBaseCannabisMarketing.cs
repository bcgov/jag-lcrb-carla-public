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
        [And(@"I complete the Cannabis Marketing application for (.*)")]
        public void CannabisMarketingApplication(string bizType)
        {
            string nameOfFederalProducer = "Canadian Cannabis";
            string marketerConnectionToCrsDetails = "Details of association (marketer to store)";
            string crsConnectionToMarketer = "Details of association (store to marketer)";
            string contactTitle = "VP Marketing";
            string contactPhone = "5555555555";
            string contactEmail = "vp@cannabis_marketing.com";

            if (bizType != "a local government")
            {
                // enter name of federal producer
                NgWebElement uiFederalProducer = ngDriver.FindElement(By.CssSelector("input[formcontrolname='federalProducerNames']"));
                uiFederalProducer.SendKeys(nameOfFederalProducer);

                // select 'Yes'
                // Does the corporation have any association, connection or financial interest in a B.C. non-medical cannabis retail store licensee or applicant of cannabis?
                NgWebElement uiMarketerConnectionToCrs = ngDriver.FindElement(By.CssSelector("input[formcontrolname='marketerConnectionToCrs'][type='radio'][value='Yes']"));
                uiMarketerConnectionToCrs.Click();

                // enter the details
                NgWebElement uiMarketerConnectionToCrsDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='marketerConnectionToCrsDetails']"));
                uiMarketerConnectionToCrsDetails.SendKeys(marketerConnectionToCrsDetails);

                // !society !indigenousnation
                if ((bizType == "a private corporation") || (bizType == "a partnership"))
                {
                    // select 'Yes'
                    // Does a B.C. non-medical cannabis retail store licensee or applicant of cannabis have any association, connection or financial interest in the corporation? 
                    NgWebElement uiCrsConnectionToMarketer = ngDriver.FindElement(By.CssSelector("input[formcontrolname='crsConnectionToMarketer'][type='radio'][value='Yes']"));
                    uiCrsConnectionToMarketer.Click();

                    // enter the details
                    NgWebElement uiCrsConnectionToMarketerDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='crsConnectionToMarketerDetails']"));
                    uiCrsConnectionToMarketerDetails.SendKeys(crsConnectionToMarketer);
                }

                if (bizType == "a public corporation")
                {
                    // select 'Yes'
                    // Does any shareholder with 20% or more voting shares have any association, connection or financial interest in a B.C. non-medical cannabis retail store licensee or applicant of cannabis?
                    NgWebElement uiCrsConnectionToMarketer = ngDriver.FindElement(By.CssSelector("input[formcontrolname='crsConnectionToMarketer'][type='radio'][value='Yes']"));
                    uiCrsConnectionToMarketer.Click();

                    // enter the details
                    NgWebElement uiCrsConnectionToMarketerDetails = ngDriver.FindElement(By.CssSelector("textarea[formcontrolname='crsConnectionToMarketerDetails']"));
                    uiCrsConnectionToMarketerDetails.SendKeys(crsConnectionToMarketer);
                }
            }

            // upload the Associates form
            FileUpload("associates.pdf", "(//input[@type='file'])[3]");

            if (bizType != "a society")
            {
                // upload the Notice of Articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[5]");

                // upload the Central Securities Register
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[5]");
            }
            else 
            { 
                // upload the society supporting documents
                // TODO
            }

            // enter the contact title
            NgWebElement uiContactPersonRole = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonRole']"));
            uiContactPersonRole.SendKeys(contactTitle);

            // enter the contact phone number
            NgWebElement uiContactPersonPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonPhone']"));
            uiContactPersonPhone.SendKeys(contactPhone);

            // enter the contact email address
            NgWebElement uiContactPersonEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonEmail']"));
            uiContactPersonEmail.SendKeys(contactEmail);

            // select authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("input[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // select signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("input[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}
