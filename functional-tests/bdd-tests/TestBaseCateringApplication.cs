using System;
using OpenQA.Selenium;
using Protractor;
using Xunit.Gherkin.Quick;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I complete the Catering application for a (.*)")]
        public void CompleteCateringApplication(string bizType)
        {
            /* 
            Page Title: Catering Licence Application
            */

            // create application info
            var estName = "Point Ellis Greenhouse";
            var estAddress = "645 Tyee Rd";
            var estCity = "Victoria";
            var estPostal = "V9A 6X5";
            var estPID = "012345678";
            var estPhone = "2505555555";
            var estEmail = "test@automation.com";
            var conGiven = "Given";
            var conSurname = "Surname";
            var conRole = "CEO";
            var conPhone = "2508888888";
            var conEmail = "test2@automation.com";
            var prevAppDetails = "Here are the previous application details (automated test).";
            var liqConnectionDetails = "Here are the liquor industry connection details (automated test).";
            var kitchenDetails = "Here are the details of the kitchen equipment.";
            var transportDetails = "Here are the transport details.";

            if (bizType == "partnership")
            {
                // upload a partnership agreement
                FileUpload("partnership_agreement.pdf", "(//input[@type='file'])[3]");

                // upload personal history summary
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[6]");
            }

            if (bizType == "public corporation")
            {
                // upload a central securities register
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                // upload supporting biz documents
                FileUpload("business_plan.pdf", "(//input[@type='file'])[6]");

                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[9]");

                // upload personal history summary documents
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[12]");

                // upload shareholders < 10% interest
                FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[15]");
            }

            if (bizType == "private corporation")
            {
                // upload a central securities register
                FileUpload("central_securities_register.pdf", "(//input[@type='file'])[3]");

                // upload supporting business documentation
                FileUpload("associates.pdf", "(//input[@type='file'])[6]");

                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[9]");

                // upload personal history summary documents
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[12]");

                // upload shareholders < 10% interest
                FileUpload("shareholders_less_10_interest.pdf", "(//input[@type='file'])[15]");
            }

            if (bizType == "society" || bizType == "military mess" || bizType == "co-op")
            {
                // upload notice of articles
                FileUpload("notice_of_articles.pdf", "(//input[@type='file'])[3]");

                // upload personal history summary documents
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[6]");
            }

            if (bizType == "sole proprietorship")
                // upload personal history summary documents
                FileUpload("personal_history_summary.pdf", "(//input[@type='file'])[3]");

            // enter the establishment name
            NgWebElement uiEstabName = null;
            for (var i = 0; i < 10; i++)
                try
                {
                    var names = ngDriver.FindElements(By.CssSelector("input[formcontrolname='establishmentName']"));
                    if (names.Count > 0)
                    {
                        uiEstabName = names[0];
                        break;
                    }
                }
                catch (Exception)
                {
                }

            uiEstabName.SendKeys(estName);

            // enter the establishment address
            var uiEstabAddress =
                ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            uiEstabAddress.SendKeys(estAddress);

            // enter the establishment city
            var uiEstabCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            uiEstabCity.SendKeys(estCity);

            // enter the establishment postal code
            var uiEstabPostal = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            uiEstabPostal.SendKeys(estPostal);

            // enter the PID
            var uiEstabPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            uiEstabPID.SendKeys(estPID);

            // enter the store email
            var uiEstabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            uiEstabEmail.SendKeys(estEmail);

            // enter the store phone number
            var uiEstabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            uiEstabPhone.SendKeys(estPhone);

            // select 'Yes'
            // Do you or any of your shareholders currently hold, have held, or have previously applied for a British Columbia liquor licence?
            if (bizType == "private corporation" || bizType == "partnership" || bizType == "society" ||
                bizType == "public corporation" || bizType == "sole proprietorship" || bizType == "military mess" || bizType == "co-op")
            {
                NgWebElement uiPreviousLicenceYes = null;
                for (var i = 0; i < 50; i++)
                    try
                    {
                        var names = ngDriver.FindElements(By.Id("mat-button-toggle-109-button"));
                        if (names.Count > 0)
                        {
                            uiPreviousLicenceYes = names[0];
                            break;
                        }
                    }
                    catch (Exception)
                    {
                    }

                JavaScriptClick(uiPreviousLicenceYes);
            }

            if (bizType == "combined application")
            {
                var uiPreviousLicenceYes = ngDriver.FindElement(By.Id("mat-button-toggle-55-button"));
                JavaScriptClick(uiPreviousLicenceYes);
            }

            // enter the previous application details
            var uiPreviousApplicationDetails = ngDriver.FindElement(By.Id("previousApplicationDetails"));
            uiPreviousApplicationDetails.SendKeys(prevAppDetails);

            // select 'Yes'
            // Do you hold a Rural Agency Store Appointment?
            if (bizType == "private corporation" || bizType == "partnership" || bizType == "society" ||
                bizType == "public corporation" || bizType == "sole proprietorship" || bizType == "military mess" || bizType == "co-op")
            {
                var uiRuralAgencyStore = ngDriver.FindElement(By.Id("mat-button-toggle-112-button"));
                JavaScriptClick(uiRuralAgencyStore);
            }

            if (bizType == "combined application")
            {
                var uiRuralAgencyStore = ngDriver.FindElement(By.Id("mat-button-toggle-58-button"));
                uiRuralAgencyStore.Click();
            }

            // select 'Yes'
            // Do you, or any of your shareholders, have any connection, financial or otherwise, direct or indirect, with a distillery, brewery or winery?
            if (bizType == "private corporation" || bizType == "partnership" || bizType == "society" ||
                bizType == "public corporation" || bizType == "sole proprietorship" || bizType == "military mess" || bizType == "co-op")
            {
                var uiOtherBusinessYes = ngDriver.FindElement(By.Id("mat-button-toggle-115-button"));
                JavaScriptClick(uiOtherBusinessYes);
            }

            if (bizType == "combined application")
            {
                var uiOtherBusinessYes = ngDriver.FindElement(By.Id("mat-button-toggle-61-button"));
                uiOtherBusinessYes.Click();
            }

            // enter the connection details
            var uiLiqIndConnection = ngDriver.FindElement(By.Id("liquorIndustryConnectionsDetails"));
            uiLiqIndConnection.SendKeys(liqConnectionDetails);

            // enter the kitchen details
            var uiKitchenDescription = ngDriver.FindElement(By.CssSelector("textarea#description2"));
            uiKitchenDescription.SendKeys(kitchenDetails);

            // enter the transport details
            var uiTransportDetails = ngDriver.FindElement(By.CssSelector("textarea#description3"));
            uiTransportDetails.SendKeys(transportDetails);

            if (bizType == "partnership" || bizType == "society" || bizType == "military mess" || bizType == "co-op")
            {
                // upload a store signage document
                FileUpload("signage.pdf", "(//input[@type='file'])[8]");

                // upload a valid interest document
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[12]");
            }

            if (bizType == "private corporation" || bizType == "combined application" ||
                bizType == "public corporation")
            {
                // upload a store signage document
                FileUpload("signage.pdf", "(//input[@type='file'])[17]");

                // upload a valid interest document
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[21]");
            }

            if (bizType == "sole proprietorship")
            {
                // upload a store signage document
                FileUpload("signage.pdf", "(//input[@type='file'])[5]");

                // upload a valid interest document
                FileUpload("valid_interest.pdf", "(//input[@type='file'])[9]");
            }

            // enter the first name of the application contact
            var uiContactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            uiContactGiven.SendKeys(conGiven);

            // enter the last name of the application contact
            var uiContactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            uiContactSurname.SendKeys(conSurname);

            // enter the role of the application contact
            var uiContactRole = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonRole']"));
            uiContactRole.SendKeys(conRole);

            // enter the phone number of the application contact
            var uiContactPhone = ngDriver.FindElement(By.CssSelector("input[formcontrolname='contactPersonPhone']"));
            uiContactPhone.SendKeys(conPhone);

            // enter the email of the application contact
            var uiContactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            uiContactEmail.SendKeys(conEmail);

            // click on the authorized to submit checkbox
            var uiAuthorizedToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthorizedToSubmit.Click();

            // click on the signature agreement checkbox
            var uiSignatureAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSignatureAgreement.Click();

            // retrieve the current URL to get the application ID (needed downstream)
            var URL = ngDriver.Url;

            // retrieve the application ID
            var parsedURL = URL.Split('/');

            var tempFix = parsedURL[5].Split(';');

            applicationID = tempFix[0];
        }
    }
}