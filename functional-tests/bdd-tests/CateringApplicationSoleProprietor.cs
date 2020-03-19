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
Feature: CateringApplication_soleproprietor
    As a logged in business user
    I want to submit a Catering Application for a sole proprietor

Scenario: Start Application
    Given I am logged in to the dashboard as a sole proprietor
    And I click on the Catering Start Application button
    And I review the account profile
    And I review the organization structure
    And I complete the application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CateringApplication_soleproprietor.feature")]
    public sealed class CateringApplicationSoleProprietor : TestBaseCatering
    {
        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CarlaLogin(businessType);
        }

        [And(@"I click on the Catering Start Application button")]
        public void I_start_application()
        {
            /* 
            Page Title: 
            */

            // click on the Catering Start Application button
            NgWebElement startApp_button = ngDriver.FindElement(By.Id("startCatering"));
            startApp_button.Click();
        }

        [And(@"I review the account profile")]
        public void review_account_profile()
        {
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
            NgWebElement uiCalendar1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            uiCalendar1.Click();

            NgWebElement uiCalendar2 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[2]/td[2]/div"));
            uiCalendar2.Click();

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

            // click on Continue to Organization Review button
            NgWebElement continueApp_button = ngDriver.FindElement(By.Id("continueToApp"));
            continueApp_button.Click();
        }

        [And(@"I review the organization structure")]
        public void I_continue_to_organization_review()
        {
            /* 
            Page Title: [client name] Detailed Organization Information
            */

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document
            string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
            uploadSignage.SendKeys(noticeOfArticles);

            // upload a central securities register document
            string centralSecuritiesRegister = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uploadCentralSecReg = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uploadCentralSecReg.SendKeys(centralSecuritiesRegister);

            // upload a special rights and restrictions document
            string specialRightsRestrictions = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uploadSpecialRightsRes = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
            uploadSpecialRightsRes.SendKeys(specialRightsRestrictions);

            /********** Key Personnel **********/

            // create the key personnel data
            string keyPersonnelFirstName = "Jane";
            string keyPersonnelLastName = "Bond";
            string keyPersonnelTitle = "Adventurer";
            string keyPersonnelEmail = "jane@bond.com";

            // open key personnel form   
            NgWebElement openKeyPersonnelForm = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[4]/section/app-associate-list/div/button"));
            openKeyPersonnelForm.Click();

            // enter key personnel first name
            NgWebElement uiKeyPersonFirst = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiKeyPersonFirst.SendKeys(keyPersonnelFirstName);

            // enter key personnel last name
            NgWebElement uiKeyPersonLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiKeyPersonLast.SendKeys(keyPersonnelLastName);

            // select key personnel role
            NgWebElement uiKeyPersonRole = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[3]"));
            uiKeyPersonRole.Click();
            
            // enter key personnel title
            NgWebElement uiKeyPersonTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiKeyPersonTitle.SendKeys(keyPersonnelTitle);

            // enter key personnel email
            NgWebElement uiKeyPersonEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiKeyPersonEmail.SendKeys(keyPersonnelEmail);

            // enter key personnel DOB
            NgWebElement openKeyPersonnelDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            openKeyPersonnelDOB.Click();

            NgWebElement openKeyPersonnelDOB1 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-1']/div/mat-month-view/table/tbody/tr[2]/td[2]/div"));
            openKeyPersonnelDOB1.Click();

            /********** Individual Shareholder **********/

            // create the individual shareholder data
            string shareholderFirstName = "Jacqui";
            string shareholderLastName = "Chan";
            string shareholderVotingShares = "500";
            string shareholderEmail = "jacqui@chan.com";

            // open individual shareholder form    
            NgWebElement uiOpenShare = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section/app-associate-list/div/button"));
            uiOpenShare.Click();

            // enter individual shareholder first name
            NgWebElement uiShareFirst = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiShareFirst.SendKeys(shareholderFirstName);

            // enter individual shareholder last name
            NgWebElement uiShareLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiShareLast.SendKeys(shareholderLastName);

            // enter individual number of voting shares
            NgWebElement uiShareVotes = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiShareVotes.SendKeys(shareholderVotingShares);

            // enter individual shareholder email
            NgWebElement uiShareEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            uiShareEmail.SendKeys(shareholderEmail);

            // enter individual shareholder DOB
            NgWebElement uiCalendarS1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiCalendarS1.Click();

            NgWebElement uiCalendarS2 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-2']/div/mat-month-view/table/tbody/tr[2]/td[2]/div"));
            uiCalendarS2.Click();

            /********** Business Shareholder **********

            // create the business shareholder data
            string businessName = "Bourne Enterprises";
            string businessVotingShares = "50";
            string businessEmail = "bourne@enterprises.com";

            // open business shareholder form    
            NgWebElement uiOpenShareBiz = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/button"));
            uiOpenShareBiz.Click();

            // enter business name
            NgWebElement uiShareFirstBiz = ngDriver.FindElement(By.XPath("(//input[@type='text'])[11]"));
            uiShareFirstBiz.SendKeys(businessName);

            // enter business voting shares
            NgWebElement uiShareVotesBiz = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
            uiShareVotesBiz.SendKeys(businessVotingShares);

            // select the business type
            NgWebElement uiShareBizType = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section[1]/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/select/option[2]"));
            uiShareBizType.Click();

            // enter business shareholder email
            NgWebElement uiShareEmailBiz = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
            uiShareEmailBiz.SendKeys(businessEmail);

            // select the business shareholder confirm button
            NgWebElement uiShareBizConfirmButton = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[5]/i/span"));
            uiShareBizConfirmButton.Click();

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uploadNoticeofArticlesBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[12]"));
            uploadNoticeofArticlesBiz.SendKeys(noticeOfArticlesBiz);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uploadCentralSecRegBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uploadCentralSecRegBiz.SendKeys(centralSecuritiesRegisterBiz);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uploadSpecialRightsResBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[18]"));
            uploadSpecialRightsResBiz.SendKeys(specialRightsRestrictionsBiz);

            /********** Business Shareholder - Key Personnel **********

            // create business shareholder key personnel data
            string keyPersonnelFirstNameBiz = "Ethel";
            string keyPersonnelLastNameBiz = "Hunt";
            string keyPersonnelTitleBiz = "Climbing Enthusiast";
            string keyPersonnelEmailBiz = "ethel@hunt.com";

            // open business shareholder > key personnel form
            NgWebElement openKeyPersonnelFormBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section/app-associate-list/div/button"));
            openKeyPersonnelFormBiz.Click();

            // enter business shareholder > key personnel first name
            NgWebElement uiKeyPersonFirstBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiKeyPersonFirstBiz.SendKeys(keyPersonnelFirstNameBiz);

            // enter business shareholder > key personnel last name
            NgWebElement uiKeyPersonLastBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiKeyPersonLastBiz.SendKeys(keyPersonnelLastNameBiz);

            // select business shareholder > key personnel role
            NgWebElement uiKeyPersonRoleBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
            uiKeyPersonRoleBiz.Click();

            // enter business shareholder > key personnel title
            NgWebElement uiKeyPersonTitleBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
            uiKeyPersonTitleBiz.SendKeys(keyPersonnelTitleBiz);

            // enter business shareholder > key personnel email
            NgWebElement uiKeyPersonEmailBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiKeyPersonEmailBiz.SendKeys(keyPersonnelEmailBiz);

            // enter business shareholder > key personnel DOB

            // Attempting to use SendKeys() for the textbox vs selecting a date from the calendar
            // Result: This pops open the calendar but then blows up
            // Question: How to get rid of the calendar picker (equivalent of hitting escape key in UI)?
            // Question: How to successfully select a date from the calendar?
            // Error message: System.InvalidOperationException : element not interactable
            string uiKeyPersonBizBD = "3/4/2001";
            NgWebElement uiKeyPersonnelDOB1Biz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
            uiKeyPersonnelDOB1Biz.SendKeys(uiKeyPersonBizBD);


            /********** Business Shareholder - Individual Shareholder **********

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz = "Jacintha";
            string shareholderLastNameBiz = "Ryan";
            string shareholderVotingSharesBiz = "500";
            string shareholderEmailBiz = "jacintha@cia.com";

            // open business shareholder > individual shareholder form
            NgWebElement uiOpenIndyShareBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/button"));
            uiOpenIndyShareBiz.Click();

            // enter business shareholder > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiIndyShareFirstBiz.SendKeys(shareholderFirstNameBiz);

            // enter business shareholder > individual shareholder last name
            NgWebElement uiIndyShareLastBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiIndyShareLastBiz.SendKeys(shareholderLastNameBiz);

            // enter business shareholder > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
            uiIndyShareVotesBiz.SendKeys(shareholderVotingSharesBiz);

            // enter business shareholder > individual shareholder email
            NgWebElement uiIndyShareEmailBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiIndyShareEmailBiz.SendKeys(shareholderEmailBiz);

            // enter business shareholder > individual shareholder DOB - todo
            //NgWebElement uiCalendarIndyS1Biz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            //uiCalendarIndyS1Biz.Click();

            //NgWebElement uiCalendarIndyS2Biz = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-4']/div/mat-month-view/table/tbody/tr[4]/td[1]/div"));
            //uiCalendarIndyS2Biz.Click();
            */

            // click on Submit Organization Info button
            NgWebElement submitOrgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT ORGANIZATION INFORMATION')]"));
            submitOrgInfoButton.Click();
        }

        [And(@"I complete the application")]
        public void I_complete_the_application()
        {
            /* 
            Page Title: [none]
            */

            // create application info
            string estName = "Point Ellis Greenhouse";
            string estAddress = "645 Tyee Rd";
            string estCity = "Victoria";
            string estPostal = "V9A6X5";
            string estPID = "012345678";
            string estPhone = "2505555555";
            string conGiven = "Given";
            string conSurname = "Surname";
            string conRole = "CEO";
            string conPhone = "2508888888";

            // select 'No' for previous liquor licence
            NgWebElement previousLicence = ngDriver.FindElement(By.Id("mat-button-toggle-2-button"));
            previousLicence.Click();

            // select 'No' for Rural Agency Store Appointment
            NgWebElement ruralStore = ngDriver.FindElement(By.Id("mat-button-toggle-5-button"));
            ruralStore.Click();

            // select 'No' for distillery, brewery or winery connections
            NgWebElement liquorProduction = ngDriver.FindElement(By.Id("mat-button-toggle-8-button"));
            liquorProduction.Click();

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

            // enter the store phone number
            NgWebElement estabPhone = ngDriver.FindElement(By.Id("establishmentPhone"));
            estabPhone.SendKeys(estPhone);

            // select 'No' for other business on premises
            NgWebElement otherBusiness = ngDriver.FindElement(By.Id("mat-button-toggle-11-button"));
            otherBusiness.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a store signage document
            string signagePath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSignage.SendKeys(signagePath);

            // enter the first name of the application contact
            NgWebElement contactGiven = ngDriver.FindElement(By.Id("contactPersonFirstName"));
            contactGiven.SendKeys(conGiven);

            // enter the last name of the application contact
            NgWebElement contactSurname = ngDriver.FindElement(By.Id("contactPersonLastName"));
            contactSurname.SendKeys(conSurname);

            // enter the role of the application contact
            NgWebElement contactRole = ngDriver.FindElement(By.Id("contactPersonRole"));
            contactRole.SendKeys(conRole);

            // enter the phone number of the application contact
            NgWebElement contactPhone = ngDriver.FindElement(By.Id("contactPersonPhone"));
            contactPhone.SendKeys(conPhone);

            // click on the authorized to submit checkbox
            NgWebElement authorizedSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            authorizedSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement signatureAgree = ngDriver.FindElement(By.Id("signatureAgreement"));
            signatureAgree.Click();
        }

        [And(@"I click on the Submit button")]
        public void click_on_submit()
        {
            NgWebElement submit_button = ngDriver.FindElement(By.XPath("//button[contains(.,'SUBMIT')]"));
            submit_button.Click();
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
            MakeCateringPayment();
        }

        [And(@"I return to the dashboard")]
        public void return_to_dashboard()
        {
            /* 
            Page Title: Payment Approved
            */

            // confirm that payment receipt is for $475.00
            Assert.True(ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-payment-confirmation/mat-card/div/div[1]/div/div/table/tr[6]/td[2][text()='$475.00']")).Displayed);

            string retDash = "Return to Dashboard";

            // click on the Return to Dashboard link
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
            Page Title: 
            */

            Assert.True(ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }
    }
}