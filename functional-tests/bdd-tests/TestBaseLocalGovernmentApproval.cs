using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I specify my contact details as the approving authority for (.*)")]
        public void SpecifyContactDetails(string applicationType)
        {
            /* 
            Page Title: Provide Confirmation of Zoning
            */

            // create test data
            var nameOfOfficial = "Official Name";
            var title = "Title";
            var phone = "1811811818";
            var email = "test@automation.com";

            // enter the name of the official
            var uiOfficialName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lGNameOfOfficial']"));
            uiOfficialName.SendKeys(nameOfOfficial);

            // enter the official's title
            var uiOfficialTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lGTitlePosition']"));
            uiOfficialTitle.SendKeys(title);

            // enter the official's phone number
            var uiOfficialPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lGContactPhone']"));
            uiOfficialPhone.SendKeys(phone);

            // enter the official's email
            var uiOfficialEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lGContactEmail']"));
            uiOfficialEmail.SendKeys(email);

            // upload the supporting reports
            if (applicationType == "liquor primary" || applicationType == "relocation")
            {
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[14]");

                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[17]");
            }
            else if (applicationType == "live theatre" || applicationType == "T&C Change")
            {
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[5]");
            }
            else if (applicationType == "outdoor patio")
            {
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[8]");

                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[11]");
            }
            else if (applicationType == "structural change")
            {
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[5]");

                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[8]");
            }
            else if (applicationType == "TUA")
            {
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[11]");
            }
            else if (applicationType == "hawkers")
            {
                // click on no objection checkbox
                var uiNoObjection =
                    ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='lgNoObjection']"));
                uiNoObjection.Click();

                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[2]");
            }
            else
            {
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[2]");
            }
        }


        [And(@"I review the local government response for (.*)")]
        public void ReviewLocalGovernment(string responseType)
        {
            if (responseType == "a picnic area endorsement")
            {
                ContinueToApplicationButton();

                /* 
                Page Title: Manufacturer Picnic Area Endorsement Application (Sent to LG/IN)
                */

                Assert.True(ngDriver
                    .FindElement(By.XPath("//body[contains(.,'Manufacturer Picnic Area Endorsement Application ')]"))
                    .Displayed);

                ClickOnSubmitButton();
            }
        }
    }
}