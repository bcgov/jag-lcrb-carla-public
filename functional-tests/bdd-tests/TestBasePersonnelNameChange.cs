using System;
using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a personnel name change for a (.*)")]
        public void RequestPersonnelNameChange(string businessType)
        {
            if (businessType != "indigenous nation")
            {
                /* 
                Page Title: Welcome to Liquor and Cannabis Licensing
                */

                // click on the review organization information button
                ClickReviewOrganizationInformation();

                /* 
                Page Title: [client name] Legal Entity Structure
                */

                // click on the Edit button for Leader (partnership, sole proprietorship, private corporation, or society)
                if (businessType == "partnership" || businessType == "sole proprietorship" ||
                    businessType == "private corporation" || businessType == "society")
                {
                    var uiEditInfoButtonShared = ngDriver.FindElement(By.CssSelector(".fas.fa-pencil-alt span"));
                    uiEditInfoButtonShared.Click();
                }

                // click on the Edit button for Leader (public corporation)
                if (businessType == "public corporation")
                {
                    var uiEditInfoButton = ngDriver.FindElement(By.CssSelector("td:nth-child(7) .ng-star-inserted"));
                    uiEditInfoButton.Click();
                }

                // enter a new name for the director
                var newDirectorFirstName = "UpdatedFirstName";
                var newDirectorLastName = "UpdatedLastName";

                var uiNewDirectorFirstName =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
                uiNewDirectorFirstName.Clear();
                uiNewDirectorFirstName.SendKeys(newDirectorFirstName);

                var uiNewDirectorLasttName =
                    ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastNameNew']"));
                uiNewDirectorLasttName.Clear();
                uiNewDirectorLasttName.SendKeys(newDirectorLastName);

                // click on the Confirm button
                var uiConfirmButton = ngDriver.FindElement(By.CssSelector(".fa-save span"));
                uiConfirmButton.Click();

                // upload a marriage certificate document
                if (businessType == "public corporation" || businessType == "partnership")
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[12]");

                if (businessType == "private corporation")
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[18]");

                if (businessType == "society" || businessType == "sole proprietorship")
                    FileUpload("marriage_certificate.pdf", "(//input[@type='file'])[9]");

                // click on Submit Organization Information button
                var uiSubmitOrgStructure =
                    ngDriver.FindElement(By.CssSelector("button.btn-primary[name='submit-application']"));
                uiSubmitOrgStructure.Click();

                MakePayment();
            }
        }


        [And(@"I change a personnel email address for a (.*)")]
        public void RequestPersonnelEmailChange(string businessType)
        {
            if (businessType != "indigenous nation")
            {
                /* 
                Page Title: Welcome to Liquor and Cannabis Licensing
                */

                // click on the review organization information button
                ClickReviewOrganizationInformation();

                /* 
                Page Title: [client name] Legal Entity Structure
                */

                // click on the Edit button for Leader (partnership, sole proprietorship, public corporation, or society)
                if (businessType == "partnership" || businessType == "sole proprietorship" ||
                    businessType == "public corporation" || businessType == "society")
                {
                    var uiEditInfoButtonPartner = ngDriver.FindElement(By.CssSelector(".fas.fa-pencil-alt span"));
                    uiEditInfoButtonPartner.Click();
                }

                // click on the Edit button for Leader (private corporation)
                if (businessType == "private corporation")
                {
                    var uiEditInfoButton = ngDriver.FindElement(By.CssSelector("td:nth-child(7) .ng-star-inserted"));
                    uiEditInfoButton.Click();
                }

                // enter a new email for the associate
                var newEmail = "newemail@test.com";

                var uiNewEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
                uiNewEmail.Clear();
                uiNewEmail.SendKeys(newEmail);

                // click on the Confirm button
                var uiConfirmButton = ngDriver.FindElement(By.CssSelector(".fa-save span"));
                uiConfirmButton.Click();

                // click on Confirm Organization Information is Complete button
                var uiOrgInfoButton2 = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
                uiOrgInfoButton2.Click();

                /* 
                Page Title: Welcome to Liquor and Cannabis Licensing
                */

                // check that dashboard is displayed (i.e. no payment has been required)
                Assert.True(ngDriver
                    .FindElement(By.XPath("//body[contains(.,'Welcome to Liquor and Cannabis Licensing')]")).Displayed);
            }
        }
    }
}