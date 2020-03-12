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

Scenario: Start Application
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button
    And I click on the Continue to Application button
    And I complete the application
    And I click on the Submit & Pay button
    And I enter the payment information
    And I return to the dashboard
    And I delete my account
    Then I see login
*/

namespace bdd_tests
{
    [FeatureFile("./CRSApplication_partnership.feature")]
    public sealed class CRSApplicationPartnership : TestBaseCRS
    {

        // Dashboard related common actions

        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CarlaLogin(businessType);
        }

        [And(@"I am not a marketer")]
        public void check_marketer()
        {
        }

        [And(@"I click on the Start Application button")]
        public void I_start_application()
        {
            ngDriver.WaitForAngular();

            //System.Threading.Thread.Sleep(7000);

            /* 
            Page Title: Welcome to Cannabis Licensing
            */

            NgWebElement startApp_button = ngDriver.FindElement(By.XPath("//button[text()='START APPLICATION']"));
            startApp_button.Click();

            ngDriver.WaitForAngular();

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

            ngDriver.WaitForAngular();

            /* 
            Page Title: Please Review the Account Profile
            */

            string bizNumber = "012345678";
            string streetAddress = "645 Tyee Road";
            string city = "Victoria";
            string postalCode = "V8V4Y3";
            string bizPhoneNumber = "2501811818";
            string bizEmail = "test@automation.com";
            string corpContactPhone = "7781811818";
            string corpContactEmail = "automated@test.com";

            NgWebElement uiBizNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiBizNumber.SendKeys(bizNumber);

            NgWebElement uiStreetAddress = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiStreetAddress.SendKeys(streetAddress);

            NgWebElement uiCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiCity.SendKeys(city);

            NgWebElement uiPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiPostalCode.SendKeys(postalCode);

            NgWebElement uiSameAsMailingAddress = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiSameAsMailingAddress.Click();

            NgWebElement uiBizPhoneNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[16]"));
            uiBizPhoneNumber.SendKeys(bizPhoneNumber);

            NgWebElement uiBizEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[17]"));
            uiBizEmail.SendKeys(bizEmail);

            NgWebElement uiCorpContactPhone = ngDriver.FindElement(By.XPath("(//input[@type='text'])[21]"));
            uiCorpContactPhone.SendKeys(corpContactPhone);

            NgWebElement uiCorpContactEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[22]"));
            uiCorpContactEmail.SendKeys(corpContactEmail);

            NgWebElement corpConnectionFederalProducer = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[2]"));
            corpConnectionFederalProducer.Click();

            NgWebElement federalProducerConnectionToCorp = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[4]"));
            federalProducerConnectionToCorp.Click();

            NgWebElement continueApp_button = ngDriver.FindElement(By.Id("continueToApp"));
            continueApp_button.Click();

            ngDriver.WaitForAngular();
        }

        [And(@"I click on the Continue to Application button")]
        public void I_continue_to_application()
        {
            ngDriver.WaitForAngular();

            // upload partnership agreement
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            string partnershipPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "partnership_agreement.pdf");
            NgWebElement uploadPartnershipAgreement = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadPartnershipAgreement.SendKeys(partnershipPath);

            // enter individual partner info

            string partnerFirstName = "Automated";
            string partnerLastName = "Test";
            string partnerPercentage = "50";
            string partnerEmail = "automated@test.com";

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

            // list business partners - to do

            NgWebElement submitOrgInfoButton = ngDriver.FindElement(By.XPath("//button[text()=' SUBMIT ORGANIZATION INFORMATION']"));
            submitOrgInfoButton.Click();

            ngDriver.WaitForAngular();
        }

        [And(@"I complete the application")]
        public void I_complete_the_application()
        {
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

            NgWebElement estabName = ngDriver.FindElement(By.Id("establishmentName"));
            estabName.SendKeys(estName);

            NgWebElement estabAddress = ngDriver.FindElement(By.Id("establishmentAddressStreet"));
            estabAddress.SendKeys(estAddress);

            NgWebElement estabCity = ngDriver.FindElement(By.Id("establishmentAddressCity"));
            estabCity.SendKeys(estCity);

            NgWebElement estabPostal = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            estabPostal.SendKeys(estPostal);

            NgWebElement estabPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            estabPID.SendKeys(estPID);

            NgWebElement estabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            estabEmail.SendKeys(estEmail);

            NgWebElement estabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            estabPhone.SendKeys(estPhone);

            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSignage.SendKeys(signagePath);

            string validInterestPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "valid_interest.pdf");
            NgWebElement uploadValidInterest = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uploadValidInterest.SendKeys(validInterestPath);

            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uploadFloorplan.SendKeys(floorplanPath);

            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
            uploadSitePlan.SendKeys(sitePlanPath);

            string finIntegrityPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "fin_integrity.pdf");
            NgWebElement uploadFinIntegrity = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uploadFinIntegrity.SendKeys(finIntegrityPath);

            NgWebElement contactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            contactGiven.SendKeys(conGiven);

            NgWebElement contactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            contactSurname.SendKeys(conSurname);

            NgWebElement contactRole = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonRole]"));
            contactRole.SendKeys(conRole);

            NgWebElement contactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonPhone]"));
            contactPhone.SendKeys(conPhone);

            NgWebElement contactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            contactEmail.SendKeys(conEmail);

            NgWebElement authorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            authorizedSubmit.Click();

            NgWebElement signatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            signatureAgree.Click();
        }

        [And(@"I click on the Submit & Pay button")]
        public void click_on_submit_and_pay()
        {
            NgWebElement submitpay_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT & PAY')]"));
            System.Threading.Thread.Sleep(7000);

            submitpay_button.Click();
            System.Threading.Thread.Sleep(7000);
        }

        [Then(@"I CLICK on 'SAVE FOR LATER'")]
        public void click_on_save_for_later()
        {
            NgWebElement saveforlater_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SAVE FOR LATER')]"));
            saveforlater_button.Click();
        }

        [And(@"I enter the payment information")]
        public void enter_payment_info()
        {
            MakeCRSPayment();
        }

        [And(@"I return to the dashboard")]
        public void return_to_dashboard()
        {
            string retDash = "Return to Dashboard";
            NgWebElement returnDash = ngDriver.FindElement(By.LinkText(retDash));
            returnDash.Click();
        }

        [And(@"I delete my account")]
        public void Delete_my_account()
        {
            this.CarlaDeleteCurrentAccount();
        }

        [Then(@"I see login")]
        public void I_see_login()
        {
            Assert.True (ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }
    }
}
