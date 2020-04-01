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
Feature: MultiLevelCRSApplicationPrivateCorp
    As a logged in business user
    I want to submit a CRS Application for a private corporation
    With multiple business shareholders

Scenario: Start Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I add in multiple business shareholders
    And I submit the organization information
    Then I see the application page
*/

namespace bdd_tests
{
    [FeatureFile("./MultiLevelCRSApplicationPrivateCorp.feature")]
    public sealed class MultiLevelCRSApplicationPrivateCorp : TestBase
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

            // click on the Start Application button
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

            // select No for Question 1 using radio button
            NgWebElement noRadio1 = ngDriver.FindElement(By.Id("mat-radio-3"));
            noRadio1.Click();

            // select No for Question 2 using radio button
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

            NgWebElement uiCalendar2 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[1]/td[2]/div"));
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

            // select 'No' for corporation's connection to a federal producer using radio button
            NgWebElement corpConnectionFederalProducer = ngDriver.FindElement(By.XPath("(//input[@type='radio'])[2]"));
            corpConnectionFederalProducer.Click();

            // select 'No' for federal producer's connection to corporation using radio button
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

            // select key personnel role using checkbox
            NgWebElement uiKeyPersonRole = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
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

            NgWebElement openKeyPersonnelDOB1 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-3']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            openKeyPersonnelDOB1.Click();

            /********** Individual Shareholder **********/

            // create the shareholder data
            string shareholderFirstName = "Jacqui";
            string shareholderLastName = "Chan";
            string shareholderVotingShares = "500";
            string shareholderEmail = "jacqui@chan.com";

            // open shareholder form 
            NgWebElement uiOpenShare = ngDriver.FindElement(By.XPath("//div[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/section/app-org-structure/div[5]/section/app-associate-list/div/button"));
            uiOpenShare.Click();

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
            NgWebElement uiCalendarS1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiCalendarS1.Click();

            NgWebElement uiCalendarS2 = ngDriver.FindElement(By.XPath("//mat-calendar[@id='mat-datepicker-4']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiCalendarS2.Click();

            /********** Business Shareholder #1 **********/

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

            // select the business type using dropdown
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

            /********** Business Shareholder #1 - Key Personnel **********/

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

            // select business shareholder > key personnel role using checkbox
            NgWebElement uiKeyPersonRoleBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
            uiKeyPersonRoleBiz.Click();

            // enter business shareholder > key personnel title
            NgWebElement uiKeyPersonTitleBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
            uiKeyPersonTitleBiz.SendKeys(keyPersonnelTitleBiz);

            // enter business shareholder > key personnel email
            NgWebElement uiKeyPersonEmailBiz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiKeyPersonEmailBiz.SendKeys(keyPersonnelEmailBiz);

            // enter business shareholder > key personnel DOB
            NgWebElement uiKeyPersonnelDOB1Biz1 = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-multi-stage-application-flow/div/mat-horizontal-stepper/div[2]/div[2]/app-application-licensee-changes/div/section[1]/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr[2]/td/mat-expansion-panel/div/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
            uiKeyPersonnelDOB1Biz1.Click();

            NgWebElement uiKeyPersonnelDOB1Biz2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-5']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiKeyPersonnelDOB1Biz2.Click();

            /********** Business Shareholder #1 - Individual Shareholder **********/

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

            // enter business shareholder > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiCalendarIndyS1Biz.Click();

            NgWebElement uiCalendarIndyS2Biz = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-6']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiCalendarIndyS2Biz.Click();
        }

        [And(@"I add in multiple nested business shareholders")]
        public void add_multiple_business_shareholders()
        {
            // add in an additional five nested business shareholders
            business_shareholder_2();
            business_shareholder_3();
            business_shareholder_4();
            business_shareholder_5();
        }

        [Then(@"I submit the organization information")]
        public void submit_org_info()
        {
            // click on Submit Organization Info button
            System.Threading.Thread.Sleep(7000);
            NgWebElement submitOrgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT ORGANIZATION INFORMATION')]"));
            System.Threading.Thread.Sleep(7000);
            submitOrgInfoButton.Click();
        }

        public void business_shareholder_2()
        {
            /********** Business Shareholder #2 **********/

            // create the business shareholder data
            string businessName2 = "GardaWorld International Protective Services";
            string businessVotingShares2 = "100";
            string businessEmail2 = "garda@world.com";

            // open business shareholder form - GardaWorld
            NgWebElement uiOpenShareBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/button"));
            uiOpenShareBiz2.Click();

            // enter business name
            NgWebElement uiShareFirstBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiShareFirstBiz2.SendKeys(businessName2);

            // enter business voting shares
            NgWebElement uiShareVotesBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr/td[2]/app-field/section/div/section/input"));
            uiShareVotesBiz2.SendKeys(businessVotingShares2);

            // select the business type from dropdown
            NgWebElement uiShareBizType2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/select/option[2]"));
            uiShareBizType2.Click();

            // enter business shareholder email
            NgWebElement uiShareEmailBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiShareEmailBiz2.SendKeys(businessEmail2);

            // select the business shareholder confirm button
            NgWebElement uiShareBizConfirmButton2 = ngDriver.FindElement(By.XPath("//div[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr/td[5]/i/span"));
            uiShareBizConfirmButton2.Click();
            
            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uploadNoticeofArticlesBiz2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[21]"));
            uploadNoticeofArticlesBiz2.SendKeys(noticeOfArticlesBiz2);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uploadCentralSecRegBiz2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[24]"));
            uploadCentralSecRegBiz2.SendKeys(centralSecuritiesRegisterBiz2);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uploadSpecialRightsResBiz2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[27]"));
            uploadSpecialRightsResBiz2.SendKeys(specialRightsRestrictionsBiz2);

            /********** Business Shareholder #2 - Key Personnel **********/

            // create business shareholder key personnel data
            string keyPersonnelFirstNameBiz2 = "GardaWorld";
            string keyPersonnelLastNameBiz2 = "KeyPersonnel";
            string keyPersonnelTitleBiz2 = "Security Expert";
            string keyPersonnelEmailBiz2 = "gardaworld@keypersonnel.com";

            // open business shareholder > key personnel form                                   
            NgWebElement openKeyPersonnelFormBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[3]/section/app-associate-list/div/button"));
            openKeyPersonnelFormBiz2.Click();

            // enter business shareholder > key personnel first name
            NgWebElement uiKeyPersonFirstBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiKeyPersonFirstBiz2.SendKeys(keyPersonnelFirstNameBiz2);

            // enter business shareholder > key personnel last name
            NgWebElement uiKeyPersonLastBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiKeyPersonLastBiz2.SendKeys(keyPersonnelLastNameBiz2);

            // select business shareholder > key personnel role using checkbox
            NgWebElement uiKeyPersonRoleBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
            uiKeyPersonRoleBiz2.Click();

            // enter business shareholder > key personnel title
            NgWebElement uiKeyPersonTitleBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
            uiKeyPersonTitleBiz2.SendKeys(keyPersonnelTitleBiz2);

            // enter business shareholder > key personnel email
            NgWebElement uiKeyPersonEmailBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiKeyPersonEmailBiz2.SendKeys(keyPersonnelEmailBiz2);

            // enter business shareholder > key personnel DOB
            NgWebElement uiKeyPersonnelDOB1Biz12 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
            uiKeyPersonnelDOB1Biz12.Click();

            NgWebElement uiKeyPersonnelDOB1Biz22 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-7']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiKeyPersonnelDOB1Biz22.Click();

            /********** Business Shareholder #2 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz2 = "GardaWorld";
            string shareholderLastNameBiz2 = "IndividualShareholder";
            string shareholderVotingSharesBiz2 = "Security Expert";
            string shareholderEmailBiz2 = "gardaworld@individualshareholder.com";

            // open business shareholder > individual shareholder form
            NgWebElement uiOpenIndyShareBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/button"));
            uiOpenIndyShareBiz2.Click();

            // enter business shareholder > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiIndyShareFirstBiz2.SendKeys(shareholderFirstNameBiz2);

            // enter business shareholder > individual shareholder last name
            NgWebElement uiIndyShareLastBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiIndyShareLastBiz2.SendKeys(shareholderLastNameBiz2);

            // enter business shareholder > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
            uiIndyShareVotesBiz2.SendKeys(shareholderVotingSharesBiz2);

            // enter business shareholder > individual shareholder email
            NgWebElement uiIndyShareEmailBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiIndyShareEmailBiz2.SendKeys(shareholderEmailBiz2);

            // enter business shareholder > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiCalendarIndyS1Biz2.Click();

            NgWebElement uiCalendarIndyS2Biz2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-8']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiCalendarIndyS2Biz2.Click();
        }
        public void business_shareholder_3()
        {
            /********** Business Shareholder #3 **********/

            // create the business shareholder data
            string businessName3 = "Häagen-Dazs";
            string businessVotingShares3 = "3";
            string businessEmail3 = "hd@icecream.com";

            // open business shareholder form
            NgWebElement uiOpenShareBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/button"));
            uiOpenShareBiz3.Click();

            // enter business name
            NgWebElement uiShareFirstBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[31]"));
            uiShareFirstBiz3.SendKeys(businessName3);

            // enter business voting shares
            NgWebElement uiShareVotesBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[32]"));
            uiShareVotesBiz3.SendKeys(businessVotingShares3);

            // select the business type using dropdown
            NgWebElement uiShareBizType3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/select/option[2]"));
            uiShareBizType3.Click();

            // enter business shareholder email
            NgWebElement uiShareEmailBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[33]"));
            uiShareEmailBiz3.SendKeys(businessEmail3);

            // select the business shareholder confirm button
            NgWebElement uiShareBizConfirmButton3 = ngDriver.FindElement(By.XPath("//div[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr/td[5]/i/span"));
            uiShareBizConfirmButton3.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz3 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uploadNoticeofArticlesBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[30]"));
            uploadNoticeofArticlesBiz3.SendKeys(noticeOfArticlesBiz3);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz3 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uploadCentralSecRegBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[33]"));
            uploadCentralSecRegBiz3.SendKeys(centralSecuritiesRegisterBiz3);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz3 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uploadSpecialRightsResBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[36]"));
            uploadSpecialRightsResBiz3.SendKeys(specialRightsRestrictionsBiz3);

            /********** Business Shareholder #3 - Key Personnel **********/

            // create business shareholder key personnel data
            string keyPersonnelFirstNameBiz3 = "Häagen-Dazs";
            string keyPersonnelLastNameBiz3 = "KeyPersonnel";
            string keyPersonnelTitleBiz3 = "Frozen Goods";
            string keyPersonnelEmailBiz3 = "hd@keypersonnel.com";

            // open business shareholder > key personnel form
            NgWebElement openKeyPersonnelFormBiz3 = ngDriver.FindElement(By.XPath("//div[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[3]/section/app-associate-list/div/button"));
            openKeyPersonnelFormBiz3.Click();

            // enter business shareholder > key personnel first name
            NgWebElement uiKeyPersonFirstBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiKeyPersonFirstBiz3.SendKeys(keyPersonnelFirstNameBiz3);

            // enter business shareholder > key personnel last name
            NgWebElement uiKeyPersonLastBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiKeyPersonLastBiz3.SendKeys(keyPersonnelLastNameBiz3);

            // select business shareholder > key personnel role using checkbox
            NgWebElement uiKeyPersonRoleBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
            uiKeyPersonRoleBiz3.Click();

            // enter business shareholder > key personnel title
            NgWebElement uiKeyPersonTitleBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
            uiKeyPersonTitleBiz3.SendKeys(keyPersonnelTitleBiz3);

            // enter business shareholder > key personnel email
            NgWebElement uiKeyPersonEmailBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiKeyPersonEmailBiz3.SendKeys(keyPersonnelEmailBiz3);

            // enter business shareholder > key personnel DOB
            NgWebElement uiKeyPersonnelDOB1Biz13 = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-multi-stage-application-flow/div/mat-horizontal-stepper/div[2]/div[2]/app-application-licensee-changes/div/section[1]/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr[2]/td/mat-expansion-panel/div/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr[2]/td/mat-expansion-panel/div/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr[2]/td/mat-expansion-panel/div/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
            uiKeyPersonnelDOB1Biz13.Click();

            NgWebElement uiKeyPersonnelDOB1Biz23 = ngDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div/mat-datepicker-content/mat-calendar/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiKeyPersonnelDOB1Biz23.Click();

            /********** Business Shareholder #3 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz3 = "Häagen-Dazs";
            string shareholderLastNameBiz3 = "IndividualShareholder";
            string shareholderVotingSharesBiz3 = "1000";
            string shareholderEmailBiz3 = "hd@individualshareholder.com";

            // open business shareholder > individual shareholder form
            NgWebElement uiOpenIndyShareBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/button"));
            uiOpenIndyShareBiz3.Click();

            // enter business shareholder > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiIndyShareFirstBiz3.SendKeys(shareholderFirstNameBiz3);

            // enter business shareholder > individual shareholder last name
            NgWebElement uiIndyShareLastBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiIndyShareLastBiz3.SendKeys(shareholderLastNameBiz3);

            // enter business shareholder > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
            uiIndyShareVotesBiz3.SendKeys(shareholderVotingSharesBiz3);

            // enter business shareholder > individual shareholder email
            NgWebElement uiIndyShareEmailBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiIndyShareEmailBiz3.SendKeys(shareholderEmailBiz3);

            // enter business shareholder > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiCalendarIndyS1Biz3.Click();

            NgWebElement uiCalendarIndyS2Biz3 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-10']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiCalendarIndyS2Biz3.Click();
        }

        public void business_shareholder_4()
        {
            /********** Business Shareholder #4 **********/

            // create the business shareholder data
            string businessName4 = "General Mills, Inc.";
            string businessVotingShares4 = "2";
            string businessEmail4 = "generalmills@hotmail.com";

            // open business shareholder form
            NgWebElement uiOpenShareBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/button"));
            uiOpenShareBiz4.Click();

            // enter business name
            NgWebElement uiShareFirstBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[41]"));
            uiShareFirstBiz4.SendKeys(businessName4);

            // enter business voting shares
            NgWebElement uiShareVotesBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[42]"));
            uiShareVotesBiz4.SendKeys(businessVotingShares4);

            // select the business type using dropdown
            NgWebElement uiShareBizType4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/select/option[2]"));
            uiShareBizType4.Click();

            // enter business shareholder email
            NgWebElement uiShareEmailBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[43]"));
            uiShareEmailBiz4.SendKeys(businessEmail4);

            // select the business shareholder confirm button
            NgWebElement uiShareBizConfirmButton4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr/td[5]/i[1]/span"));
            uiShareBizConfirmButton4.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz4 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uploadNoticeofArticlesBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[39]"));
            uploadNoticeofArticlesBiz4.SendKeys(noticeOfArticlesBiz4);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz4 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uploadCentralSecRegBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[42]"));
            uploadCentralSecRegBiz4.SendKeys(centralSecuritiesRegisterBiz4);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz4 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uploadSpecialRightsResBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[45]"));
            uploadSpecialRightsResBiz4.SendKeys(specialRightsRestrictionsBiz4);

            /********** Business Shareholder #4 - Key Personnel **********/

            // create business shareholder key personnel data
            string keyPersonnelFirstNameBiz4 = "General Mills, Inc.";
            string keyPersonnelLastNameBiz4 = "KeyPersonnel";
            string keyPersonnelTitleBiz4 = "Manager";
            string keyPersonnelEmailBiz4 = "GeneralMills@keypersonnel.com";

            // open business shareholder > key personnel form
            NgWebElement openKeyPersonnelFormBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[3]/section/app-associate-list/div/button"));
            openKeyPersonnelFormBiz4.Click();
        
            // enter business shareholder > key personnel first name
            NgWebElement uiKeyPersonFirstBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiKeyPersonFirstBiz4.SendKeys(keyPersonnelFirstNameBiz4);

            // enter business shareholder > key personnel last name
            NgWebElement uiKeyPersonLastBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiKeyPersonLastBiz4.SendKeys(keyPersonnelLastNameBiz4);

            // select business shareholder > key personnel role using checkbox
            NgWebElement uiKeyPersonRoleBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
            uiKeyPersonRoleBiz4.Click();

            // enter business shareholder > key personnel title
            NgWebElement uiKeyPersonTitleBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
            uiKeyPersonTitleBiz4.SendKeys(keyPersonnelTitleBiz4);

            // enter business shareholder > key personnel email
            NgWebElement uiKeyPersonEmailBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiKeyPersonEmailBiz4.SendKeys(keyPersonnelEmailBiz4);

            // enter business shareholder > key personnel DOB
            NgWebElement uiKeyPersonnelDOB1Biz14 = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-multi-stage-application-flow/div/mat-horizontal-stepper/div[2]/div[2]/app-application-licensee-changes/div/section[1]/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr[2]/td/mat-expansion-panel/div/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr[2]/td/mat-expansion-panel/div/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr[2]/td/mat-expansion-panel/div/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr[2]/td/mat-expansion-panel/div/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
            uiKeyPersonnelDOB1Biz14.Click();

            NgWebElement uiKeyPersonnelDOB1Biz24 = ngDriver.FindElement(By.XPath("/html/body/div[2]/div[2]/div/mat-datepicker-content/mat-calendar/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiKeyPersonnelDOB1Biz24.Click();

            /********** Business Shareholder #4 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz4 = "General Mills, Inc.";
            string shareholderLastNameBiz4 = "IndividualShareholder";
            string shareholderVotingSharesBiz4 = "1";
            string shareholderEmailBiz4 = "GeneralMills@individualshareholder.com";

            // open business shareholder > individual shareholder form
            NgWebElement uiOpenIndyShareBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/button"));
            uiOpenIndyShareBiz4.Click();

            // enter business shareholder > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiIndyShareFirstBiz4.SendKeys(shareholderFirstNameBiz4);

            // enter business shareholder > individual shareholder last name
            NgWebElement uiIndyShareLastBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiIndyShareLastBiz4.SendKeys(shareholderLastNameBiz4);

            // enter business shareholder > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
            uiIndyShareVotesBiz4.SendKeys(shareholderVotingSharesBiz4);

            // enter business shareholder > individual shareholder email
            NgWebElement uiIndyShareEmailBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiIndyShareEmailBiz4.SendKeys(shareholderEmailBiz4);

            // enter business shareholder > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiCalendarIndyS1Biz4.Click();

            NgWebElement uiCalendarIndyS2Biz4 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-12']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiCalendarIndyS2Biz4.Click();
        }

        public void business_shareholder_5()
        {
            /********** Business Shareholder #5 **********/

            // create the business shareholder data
            string businessName5 = "Emirates Telecommunication Group Company PJSC";
            string businessVotingShares5 = "1";
            string businessEmail5 = "dubai@tele.com";

            // open business shareholder form
            NgWebElement uiOpenShareBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/button"));
            uiOpenShareBiz5.Click();

            // enter business name
            NgWebElement uiShareFirstBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[51]"));
            uiShareFirstBiz5.SendKeys(businessName5);

            // enter business voting shares
            NgWebElement uiShareVotesBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[52]"));
            uiShareVotesBiz5.SendKeys(businessVotingShares5);

            // select the business type using dropdown
            NgWebElement uiShareBizType5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/select/option[2]"));
            uiShareBizType5.Click();

            // enter business shareholder email
            NgWebElement uiShareEmailBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[53]"));
            uiShareEmailBiz5.SendKeys(businessEmail5);

            // select the business shareholder confirm button
            NgWebElement uiShareBizConfirmButton5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section[2]/app-associate-list/div/table/tr/td[5]/i[1]/span"));
            uiShareBizConfirmButton5.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz5 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uploadNoticeofArticlesBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[48]"));
            uploadNoticeofArticlesBiz5.SendKeys(noticeOfArticlesBiz5);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz5 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uploadCentralSecRegBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[51]"));
            uploadCentralSecRegBiz5.SendKeys(centralSecuritiesRegisterBiz5);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz5 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uploadSpecialRightsResBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[54]"));
            uploadSpecialRightsResBiz5.SendKeys(specialRightsRestrictionsBiz5);

            /********** Business Shareholder #5 - Key Personnel **********/

            // create business shareholder key personnel data
            string keyPersonnelFirstNameBiz5 = "Emirates Telecommunication Group Company PJSC";
            string keyPersonnelLastNameBiz5 = "KeyPersonnel";
            string keyPersonnelTitleBiz5 = "Engineer";
            string keyPersonnelEmailBiz5 = "emirates@keypersonnel.com";

            // open business shareholder > key personnel form
            NgWebElement openKeyPersonnelFormBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[3]/section/app-associate-list/div/button"));
            openKeyPersonnelFormBiz5.Click();

            // enter business shareholder > key personnel first name
            NgWebElement uiKeyPersonFirstBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiKeyPersonFirstBiz5.SendKeys(keyPersonnelFirstNameBiz5);

            // enter business shareholder > key personnel last name
            NgWebElement uiKeyPersonLastBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiKeyPersonLastBiz5.SendKeys(keyPersonnelLastNameBiz5);

            // select business shareholder > key personnel role using checkbox
            NgWebElement uiKeyPersonRoleBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
            uiKeyPersonRoleBiz5.Click();

            // enter business shareholder > key personnel title
            NgWebElement uiKeyPersonTitleBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
            uiKeyPersonTitleBiz5.SendKeys(keyPersonnelTitleBiz5);

            // enter business shareholder > key personnel email
            NgWebElement uiKeyPersonEmailBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiKeyPersonEmailBiz5.SendKeys(keyPersonnelEmailBiz5);

            // enter business shareholder > key personnel DOB
            NgWebElement uiKeyPersonnelDOB1Biz15 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[3]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
            uiKeyPersonnelDOB1Biz15.Click();

            NgWebElement uiKeyPersonnelDOB1Biz25 = ngDriver.FindElement(By.XPath("//html/body/div[2]/div[2]/div/mat-datepicker-content/mat-calendar/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiKeyPersonnelDOB1Biz25.Click();

            /********** Business Shareholder #5 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz5 = "Emirates Telecommunication Group Company PJSC";
            string shareholderLastNameBiz5 = "IndividualShareholder";
            string shareholderVotingSharesBiz5 = "1";
            string shareholderEmailBiz5 = "emirates@individualshareholder.com";

            // open business shareholder > individual shareholder form
            NgWebElement uiOpenIndyShareBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/button"));
            uiOpenIndyShareBiz5.Click();

            // enter business shareholder > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiIndyShareFirstBiz5.SendKeys(shareholderFirstNameBiz5);

            // enter business shareholder > individual shareholder last name
            NgWebElement uiIndyShareLastBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiIndyShareLastBiz5.SendKeys(shareholderLastNameBiz5);

            // enter business shareholder > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
            uiIndyShareVotesBiz5.SendKeys(shareholderVotingSharesBiz5);

            // enter business shareholder > individual shareholder email
            NgWebElement uiIndyShareEmailBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiIndyShareEmailBiz5.SendKeys(shareholderEmailBiz5);

            // enter business shareholder > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiCalendarIndyS1Biz5.Click();

            NgWebElement uiCalendarIndyS2Biz5 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-14']/div/mat-month-view/table/tbody/tr[2]/td[1]/div"));
            uiCalendarIndyS2Biz5.Click();
        }
    }
}
