using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using Xunit;

/*
Feature: CRSApplication_partnership
    As a logged in business user
    I want to submit a CRS Application for a partnership

SScenario: Start Application
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I complete the application
    And I review the security screening requirements
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CRSApplication_partnership.feature")]
    public sealed class CRSApplicationPartnership : TestBaseCRS
    {
        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CarlaLogin(businessType);
        }

        [And(@"I click on the Start Application button")]
        public void I_start_application()
        {
            /* 
            Page Title: Welcome to Cannabis Licensing
            */

            NgWebElement startApp_button = ngDriver.FindElement(By.XPath("//button[text()='START APPLICATION']"));
            startApp_button.Click();
        }

        [And(@"I complete the eligibility disclosure")]
        public void complete_eligibility_disclosure()
        {
            /* 
            Page Title: Cannabis Retail Store Licence Eligibility Disclosure
            */

            string electricSignature = "Automated Test";

            // select No for Question 1
            NgWebElement noRadio1 = ngDriver.FindElement(By.Id("mat-radio-3"));
            noRadio1.Click();

            // select No for Question 2
            NgWebElement noRadio2 = ngDriver.FindElement(By.Id("mat-radio-9"));
            noRadio2.Click();

            // select the certification checkbox
            NgWebElement matCheckbox = ngDriver.FindElement(By.Id("mat-checkbox-1"));
            matCheckbox.Click();

            // enter the electronic signature
            NgWebElement sigCheckbox = ngDriver.FindElement(By.Id("eligibilitySignature"));
            sigCheckbox.SendKeys(electricSignature);

            // click on the Submit button
            NgWebElement submit_button = ngDriver.FindElement(By.XPath("//button[text()='SUBMIT']"));
            submit_button.Click();
        }

        [And(@"I review the account profile")]
        public void review_account_profile()
        {
            /* 
            Page Title: Please Review the Account Profile
            */

            // create the business data
            string bizNumber = "012345678";
            string streetAddress = "645 Tyee Road";
            string city = "Victoria";
            string postalCode = "V8V4Y3";
            string bizPhoneNumber = "2501811818";
            string bizEmail = "test@automation.com";
            string corpContactPhone = "7781811818";
            string corpContactEmail = "automated@test.com";

            // enter the business number
            NgWebElement uiBizNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiBizNumber.SendKeys(bizNumber);

            // enter the business street address
            NgWebElement uiStreetAddress = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiStreetAddress.SendKeys(streetAddress);

            // enter the business city
            NgWebElement uiCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiCity.SendKeys(city);

            // enter the business postal code
            NgWebElement uiPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiPostalCode.SendKeys(postalCode);

            // click the same as mailing address checkbox - todo: switch off to include more fields
            NgWebElement uiSameAsMailingAddress = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiSameAsMailingAddress.Click();

            // enter the business phone number
            NgWebElement uiBizPhoneNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[16]"));
            uiBizPhoneNumber.SendKeys(bizPhoneNumber);

            // enter the business email
            NgWebElement uiBizEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[17]"));
            uiBizEmail.SendKeys(bizEmail);

            // enter the corporate contact phone number
            NgWebElement uiCorpContactPhone = ngDriver.FindElement(By.XPath("(//input[@type='text'])[21]"));
            uiCorpContactPhone.SendKeys(corpContactPhone);

            // enter the corporate contact email
            NgWebElement uiCorpContactEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[22]"));
            uiCorpContactEmail.SendKeys(corpContactEmail);

            // select 'No' for connection to federal producer
            NgWebElement corpConnectionFederalProducer = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[2]"));
            corpConnectionFederalProducer.Click();

            // select 'No' for connection from federal producer
            NgWebElement federalProducerConnectionToCorp = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[4]"));
            federalProducerConnectionToCorp.Click();

            // click on Continue to Organization Review button
            NgWebElement continueApp_button = ngDriver.FindElement(By.Id("continueToApp"));
            continueApp_button.Click();
        }

        [And(@"I review the organization structure")]
        public void review_org_structure()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // create individual partner info
            string partnerFirstName = "Automated";
            string partnerLastName = "Test";
            string partnerPercentage = "50";
            string partnerEmail = "automated@test.com";

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the partnership agreement
            string partnershipPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "partnership_agreement.pdf");
            NgWebElement uploadPartnershipAgreement = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadPartnershipAgreement.SendKeys(partnershipPath);

            // open partner row
            NgWebElement uiPartnerRow = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[3]/section/app-associate-list/div/button"));
            uiPartnerRow.Click();

            // enter partner first name
            NgWebElement uiPartnerFirst = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiPartnerFirst.SendKeys(partnerFirstName);

            // enter partner last name
            NgWebElement uiPartnerLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiPartnerLast.SendKeys(partnerLastName);

            // enter partner percentage
            NgWebElement uiPartnerPercentage = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiPartnerPercentage.SendKeys(partnerPercentage);

            // enter partner email
            NgWebElement uiPartnerEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiPartnerEmail.SendKeys(partnerEmail);

            // enter partner DOB
            NgWebElement uiCalendar0 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            uiCalendar0.Click();

            NgWebElement uiCalendar1 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-2']/mat-calendar-header/div/div/button/span"));
            uiCalendar1.Click();

            NgWebElement uiCalendar2 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-2']/div/mat-multi-year-view/table/tbody/tr/td/div"));
            uiCalendar2.Click();

            NgWebElement uiCalendar3 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-2']/div/mat-year-view/table/tbody/tr[3]/td[4]/div"));
            uiCalendar3.Click();

            NgWebElement uiCalendar4 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-2']/div/mat-month-view/table/tbody/tr[4]/td[6]/div"));
            uiCalendar4.Click();

            NgWebElement uiCalendar5 = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[6]/i/span"));
            uiCalendar5.Click();

            NgWebElement uiCalendar6 = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/section/button[2]"));
            uiCalendar6.Click();
        }

        [And(@"I complete the application")]
        public void I_complete_the_application()
        {
            /* 
            Page Title: Submit the Cannabis Retail Store Application
            */

            // create application info
            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A6X5";
            string estPID = "012345678";
            string estEmail = "test@test.com";
            string estPhone = "2505555555";
            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";
            string conEmail = "contact@email.com";

            // enter the establishment name
            NgWebElement estabName = ngDriver.FindElement(By.Id("establishmentName"));
            estabName.SendKeys(estName);

            // enter the establishment street address
            NgWebElement estabAddress = ngDriver.FindElement(By.Id("establishmentAddressStreet"));
            estabAddress.SendKeys(estAddress);

            // enter the establishment city
            NgWebElement estabCity = ngDriver.FindElement(By.Id("establishmentAddressCity"));
            estabCity.SendKeys(estCity);

            // enter the establishment postal code
            NgWebElement estabPostal = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            estabPostal.SendKeys(estPostal);

            // enter the establishment's PID
            NgWebElement estabPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            estabPID.SendKeys(estPID);

            // enter the establishment email
            NgWebElement estabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            estabEmail.SendKeys(estEmail);

            // enter the establishment phone number
            NgWebElement estabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            estabPhone.SendKeys(estPhone);

            // find the upload_files folder in the repo
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the signage pdf
            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSignage.SendKeys(signagePath);

            // upload the valid interest pdf
            string validInterestPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "valid_interest.pdf");
            NgWebElement uploadValidInterest = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uploadValidInterest.SendKeys(validInterestPath);

            // upload the floor plan pdf
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uploadFloorplan.SendKeys(floorplanPath);

            // upload the site plan pdf
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
            uploadSitePlan.SendKeys(sitePlanPath);

            // upload the financial integrity pdf
            string finIntegrityPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "fin_integrity.pdf");
            NgWebElement uploadFinIntegrity = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uploadFinIntegrity.SendKeys(finIntegrityPath);

            // enter the contact's first name
            NgWebElement contactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            contactGiven.SendKeys(conGiven);

            // enter the contact's last name
            NgWebElement contactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            contactSurname.SendKeys(conSurname);

            // enter the contact's title
            NgWebElement contactRole = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonRole]"));
            contactRole.SendKeys(conRole);

            // enter the contact's phone number
            NgWebElement contactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonPhone]"));
            contactPhone.SendKeys(conPhone);

            // enter the contact's email address
            NgWebElement contactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            contactEmail.SendKeys(conEmail);

            // select the authorized to submit checkbox
            NgWebElement authorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            authorizedSubmit.Click();

            // select the signature checkbox
            NgWebElement signatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            signatureAgree.Click();

            // click on the Submit button
            NgWebElement submit_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT')]"));
            submit_button.Click();
        }

        [And(@"I review the security screening requirements")]
        public void review_security_screening_reqs()
        {
            /* 
            Page Title: Security Screening Requirements
                      : placeholder for future testing
            */
        }

        [And(@"I click on the Pay for Application button")]
        public void click_on_pay()
        {
            NgWebElement pay_button = ngDriver.FindElement(By.XPath("//button[contains(.,'Pay for Application')]"));
            pay_button.Click();
        }

        [And(@"I enter the payment information")]
        public void enter_payment_info()
        {
            MakeCRSPayment();
        }

        [And(@"I return to the dashboard")]
        public void return_to_dashboard()
        {
            /* 
            Page Title: Payment Approved
            */

            // confirm that payment receipt is for $7,500.00
            Assert.True (ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-payment-confirmation/mat-card/div/div[1]/div/div/table/tr[6]/td[2][text()='$7,500.00']")).Displayed);

            string retDash = "Return to Dashboard";
            NgWebElement returnDash = ngDriver.FindElement(By.LinkText(retDash));
            returnDash.Click();
        }

        [And(@"the account is deleted")]
        public void Delete_my_account()
        {
            this.CarlaDeleteCurrentAccount();
        }

        [Then(@"I see the login page")]
        public void I_see_login()
        {
            /* 
            Page Title: Apply for a cannabis licence
            */

            Assert.True (ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }
    }
}
