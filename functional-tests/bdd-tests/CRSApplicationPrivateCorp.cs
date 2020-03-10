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
Feature: CRSApplication_privatecorp
    As a logged in business user
    I want to submit a CRS Application for a private corporation

Scenario: Start Application
    Given I am logged in to the dashboard as a private corporation
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
    [FeatureFile("./CRSApplication_privatecorp.feature")]
    public sealed class CRSApplicationPrivateCorp : TestBaseCRS
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

            // click on the Start Application button
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
            string incorporationNumber = "BC1234567";
            
            string physStreetAddress1 = "645 Tyee Road";
            string physStreetAddress2 = "West of Victoria";
            string physCity = "Victoria";
            string physPostalCode = "V8V4Y3";
            
            string mailStreet1 = "P.O. Box 123";
            string mailStreet2 = "303 Prideaux St.";
            string mailCity = "Nanaimo";
            string mailProvince = "B.C.";
            string mailPostalCode = "V9R2N3";
            string mailCountry = "Switzerland";

            string bizPhoneNumber = "2501811818";
            string bizEmail = "test@automation.com";
            string corpTitle = "CEO";
            string corpContactPhone = "7781811818";
            string corpContactEmail = "automated@test.com";

            // enter the Business Number
            NgWebElement uiBizNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiBizNumber.SendKeys(bizNumber);

            // enter the Incorporation Number
            NgWebElement uiCorpNumber = ngDriver.FindElement(By.Id("bcIncorporationNumber"));
            uiCorpNumber.SendKeys(incorporationNumber);

            // enter the Date of Incorporation in B.C. 
            /*NgWebElement uiCalendar1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            uiCalendar1.Click();

            NgWebElement uiCalendar2 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/mat-calendar-header/div/div/button/span"));
            uiCalendar2.Click();

            NgWebElement uiCalendar3 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/div/mat-multi-year-view/table/tbody/tr[3]/td/div"));
            uiCalendar3.Click();

            NgWebElement uiCalendar4 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/div/mat-year-view/table/tbody/tr[3]/td[4]/div"));
            uiCalendar4.Click();

            NgWebElement uiCalendar5 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[4]/td[5]/div"));
            uiCalendar5.Click();*/

            // enter the physical street address 1
            NgWebElement uiPhysStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiPhysStreetAddress1.SendKeys(physStreetAddress1);

            // enter the physical street address 2
            NgWebElement uiPhysStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiPhysStreetAddress2.SendKeys(physStreetAddress2);

            // enter the physical city
            NgWebElement uiPhysCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiPhysCity.SendKeys(physCity);

            // enter the physical postal code
            NgWebElement uiPhysPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiPhysPostalCode.SendKeys(physPostalCode);

            /* switching off use of checkbox "Same as physical address" in order to test mailing address fields
            NgWebElement uiSameAsMailingAddress = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiSameAsMailingAddress.Click(); */

            // enter the mailing street address 1
            NgWebElement uiMailingStreetAddress1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
            uiMailingStreetAddress1.SendKeys(mailStreet1);

            // enter the mailing street address 2
            NgWebElement uiMailingStreetAddress2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
            uiMailingStreetAddress2.SendKeys(mailStreet2);

            // enter the mailing city
            NgWebElement uiMailingCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[14]"));
            uiMailingCity.SendKeys(mailCity);

            // enter the mailing province
            NgWebElement uiMailingProvince = ngDriver.FindElement(By.XPath("(//input[@type='text'])[15]"));
            uiMailingProvince.SendKeys(mailProvince);

            // enter the mailing postal code
            NgWebElement uiMailingPostalCode = ngDriver.FindElement(By.XPath("(//input[@type='text'])[16]"));
            uiMailingPostalCode.SendKeys(mailPostalCode);

            // enter the mailing country
            NgWebElement uiMailingCountry = ngDriver.FindElement(By.XPath("(//input[@type='text'])[17]"));
            uiMailingCountry.SendKeys(mailCountry);

            // enter the business phone number
            NgWebElement uiBizPhoneNumber = ngDriver.FindElement(By.XPath("(//input[@type='text'])[18]"));
            uiBizPhoneNumber.SendKeys(bizPhoneNumber);

            // enter the business email
            NgWebElement uiBizEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[19]"));
            uiBizEmail.SendKeys(bizEmail);

            // enter the corporation contact title
            NgWebElement uiCorpTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[22]"));
            uiCorpTitle.SendKeys(corpTitle);

            // enter the corporation contact phone number
            NgWebElement uiCorpContactPhone = ngDriver.FindElement(By.XPath("(//input[@type='text'])[23]"));
            uiCorpContactPhone.SendKeys(corpContactPhone);

            // enter the corporation contact phone email
            NgWebElement uiCorpContactEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[24]"));
            uiCorpContactEmail.SendKeys(corpContactEmail);

            // select 'No' for corporation's connection to a federal producer
            NgWebElement corpConnectionFederalProducer = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[2]"));
            corpConnectionFederalProducer.Click();

            // select 'No' for federal producer's connection to corporation
            NgWebElement federalProducerConnectionToCorp = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[4]"));
            federalProducerConnectionToCorp.Click();

            // click on CONTINUE TO ORGANIZATION REVIEW button
            NgWebElement continueApp_button = ngDriver.FindElement(By.Id("continueToApp"));
            continueApp_button.Click();

            ngDriver.WaitForAngular();
        }

        [And(@"I click on the Continue to Organization Review button")]
        public void I_continue_to_organization_review()
        {
            string keyPersonnelFirstName = "Jane";
            string keyPersonnelLastName = "Bond";
            string keyPersonnelTitle = "Adventurer";
            string keyPersonnelEmail = "jane@bond.com";
            string keyPersonnelDOB = "1998-01-01";

            string shareholderFirstName = "Jacqui";
            string shareholderLastName = "Chan";
            string shareholderVotingShares = "500";
            string shareholderEmail = "jacqui@chan.com";

            ngDriver.WaitForAngular();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document
            string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
            uploadSignage.SendKeys(noticeOfArticles);

            // upload a central securities register document
            string centralSecuritiesRegister = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadCentralSecReg = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uploadCentralSecReg.SendKeys(centralSecuritiesRegister);

            // upload a special rights and restrictions document
            string specialRightsRestrictions = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSpecialRightsRes = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
            uploadSpecialRightsRes.SendKeys(specialRightsRestrictions);

            // open key personnel form
            NgWebElement openKeyPersonnelForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/section[4]/app-associate-list/div/button"));
            openKeyPersonnelForm.Click();

            // enter key personnel first name
            NgWebElement uiKeyPersonFirst = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiKeyPersonFirst.SendKeys(keyPersonnelFirstName);

            // enter key personnel last name
            NgWebElement uiKeyPersonLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiKeyPersonLast.SendKeys(keyPersonnelLastName);

            // select key personnel role
            NgWebElement uiKeyPersonRole = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiKeyPersonRole.Click();

            // enter key personnel title
            NgWebElement uiKeyPersonTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiKeyPersonTitle.SendKeys(keyPersonnelTitle);

            // enter key personnel email
            NgWebElement uiKeyPersonEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiKeyPersonEmail.SendKeys(keyPersonnelEmail);

            // enter key personnel DOB
            NgWebElement uiKeyPersonDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            uiKeyPersonDOB.SendKeys(keyPersonnelDOB);

            // open shareholder form
            NgWebElement openShareholderForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/section[5]/app-associate-list/div/button"));
            openShareholderForm.Click();

            // enter shareholder first name
            NgWebElement uiShareFirst = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiShareFirst.SendKeys(shareholderFirstName);

            // enter shareholder last name
            NgWebElement uiShareLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiShareLast.SendKeys(shareholderLastName);

            // enter number of voting shares
            NgWebElement uiShareVotes = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiShareVotes.SendKeys(shareholderVotingShares);

            // enter shareholder email
            NgWebElement uiShareEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            uiShareEmail.SendKeys(shareholderEmail);

            // enter shareholder DOB
            // todo

            // click on Submit Organization Info button
            NgWebElement submitOrgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT ORGANIZATION INFORMATION')]"));
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

            /* 
            Page Title: Submit the Cannabis Retail Store Application
            */

            // enter the establishment name
            NgWebElement estabName = ngDriver.FindElement(By.Id("establishmentName"));
            estabName.SendKeys(estName);

            // enter the establishment address
            NgWebElement estabAddress = ngDriver.FindElement(By.Id("establishmentAddressStreet"));
            estabAddress.SendKeys(estAddress);

            // enter the establishment city
            NgWebElement estabCity = ngDriver.FindElement(By.Id("establishmentAddressCity"));
            estabCity.SendKeys(estCity);

            // enter the establishment postal code
            NgWebElement estabPostal = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            estabPostal.SendKeys(estPostal);

            // enter the PID
            NgWebElement estabPID = ngDriver.FindElement(By.Id("establishmentParcelId"));
            estabPID.SendKeys(estPID);

            // enter the store email
            NgWebElement estabEmail = ngDriver.FindElement(By.Id("establishmentEmail"));
            estabEmail.SendKeys(estEmail);

            // enter the store phone number
            NgWebElement estabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            estabPhone.SendKeys(estPhone);

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a store signage document
            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSignage.SendKeys(signagePath);

            // upload a valid interest document
            string validInterestPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "valid_interest.pdf");
            NgWebElement uploadValidInterest = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uploadValidInterest.SendKeys(validInterestPath);

            // upload a floor plan document
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uploadFloorplan.SendKeys(floorplanPath);

            // upload a site plan document
            string sitePlanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "site_plan.pdf");
            NgWebElement uploadSitePlan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
            uploadSitePlan.SendKeys(sitePlanPath);

            // upload a financial integrity form
            string finIntegrityPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "fin_integrity.pdf");
            NgWebElement uploadFinIntegrity = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uploadFinIntegrity.SendKeys(finIntegrityPath);

            // enter the first name of the application contact
            NgWebElement contactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            contactGiven.SendKeys(conGiven);

            // enter the last name of the application contact
            NgWebElement contactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            contactSurname.SendKeys(conSurname);

            // enter the role of the application contact
            NgWebElement contactRole = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonRole]"));
            contactRole.SendKeys(conRole);

            // enter the phone number of the application contact
            NgWebElement contactPhone = ngDriver.FindElement(By.CssSelector("input[formControlName=contactPersonPhone]"));
            contactPhone.SendKeys(conPhone);

            // enter the email of the application contact
            NgWebElement contactEmail = ngDriver.FindElement(By.Id("contactPersonEmail"));
            contactEmail.SendKeys(conEmail);

            // click on the authorized to submit checkbox
            NgWebElement authorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            authorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement signatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            signatureAgree.Click();
        }

        [And(@"I click on the Submit & Pay button")]
        public void click_on_submit_and_pay()
        {
            /* 
            Page Title: Submit the Cannabis Retail Store Application
            */

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
            /* 
            Page Title: Payment Approved
            */

            string retDash = "Return to Dashboard";

            // click on the Return to Dashboard link
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
