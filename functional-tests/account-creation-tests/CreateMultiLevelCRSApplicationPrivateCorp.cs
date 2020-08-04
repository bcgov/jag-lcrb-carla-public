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
Feature: Create_MultiLevel_CRSApplication_PrivateCorp
    As a logged in business user
    I want to submit a CRS Application for a private corporation
    With multiple nested business shareholders
    To be used as test data

Scenario: Start Application
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I add in multiple nested business shareholders
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Pay for Application button
    And I enter the payment information
    Then I confirm the payment receipt for a Cannabis Retail Store application
*/

namespace bdd_tests
{
    [FeatureFile("./Create_MultiLevel_CRSApplication_PrivateCorp.feature")]
    public sealed class CreateMultiLevelCRSApplicationPrivateCorp : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CarlaLoginNoCheck();
        }

        [And(@"I add in multiple nested business shareholders")]
        public void add_multiple_business_shareholders()
        {
            // add in an additional four nested business shareholders
            business_shareholder_2();
            business_shareholder_3();
            business_shareholder_4();
            business_shareholder_5();
        }

        public void business_shareholder_2()
        {
            /********** Business Shareholder #2 **********/

            // create the business shareholder data
            string businessName2 = "Business Shareholder 2";
            string businessVotingShares2 = "100";
            string businessEmail2 = "businessshareholder2@email.com";

            // open business shareholder form - GardaWorld
            NgWebElement uiOpenShareBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/button"));
            uiOpenShareBiz2.Click();

            // enter business name
            NgWebElement uiShareFirstBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiShareFirstBiz2.SendKeys(businessName2);

            // enter business voting shares
            NgWebElement uiShareVotesBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[2]/app-field/section/div/section/input"));
            uiShareVotesBiz2.SendKeys(businessVotingShares2);

            // select the business type from dropdown
            NgWebElement uiShareBizType2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/select/option[2]"));
            uiShareBizType2.Click();

            // enter business shareholder email
            NgWebElement uiShareEmailBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiShareEmailBiz2.SendKeys(businessEmail2);

            // select the business shareholder confirm button
            NgWebElement uiShareBizConfirmButton2 = ngDriver.FindElement(By.XPath("//div[@id='cdk-accordion-child-0']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[5]/i/span"));
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
            string keyPersonnelFirstNameBiz2 = "KeyPersonnelBiz2First";
            string keyPersonnelLastNameBiz2 = "KeyPersonnelBiz2Last";
            string keyPersonnelTitleBiz2 = "KeyPersonnelBiz2Title";
            string keyPersonnelEmailBiz2 = "keypersonnel@biz2.com";

            // open business shareholder > key personnel form 
            NgWebElement openKeyPersonnelFormBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section/app-associate-list/div/button"));
            openKeyPersonnelFormBiz2.Click();

            // enter business shareholder > key personnel first name
            NgWebElement uiKeyPersonFirstBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiKeyPersonFirstBiz2.SendKeys(keyPersonnelFirstNameBiz2);

            // enter business shareholder > key personnel last name
            NgWebElement uiKeyPersonLastBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiKeyPersonLastBiz2.SendKeys(keyPersonnelLastNameBiz2);

            // select business shareholder > key personnel role using checkbox
            NgWebElement uiKeyPersonRoleBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
            uiKeyPersonRoleBiz2.Click();

            // enter business shareholder > key personnel title
            NgWebElement uiKeyPersonTitleBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
            uiKeyPersonTitleBiz2.SendKeys(keyPersonnelTitleBiz2);

            // enter business shareholder > key personnel email
            NgWebElement uiKeyPersonEmailBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiKeyPersonEmailBiz2.SendKeys(keyPersonnelEmailBiz2);

            // enter business shareholder > key personnel DOB
            NgWebElement uiKeyPersonnelDOB1Biz12 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
            uiKeyPersonnelDOB1Biz12.Click();

            SharedCalendarDate();

            /********** Business Shareholder #2 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz2 = "IndividualShareholderBiz2First";
            string shareholderLastNameBiz2 = "IndividualShareholderBiz2Last";
            string shareholderVotingSharesBiz2 = "1800";
            string shareholderEmailBiz2 = "individualshareholder@biz2.com";

            // open business shareholder > individual shareholder form
            NgWebElement uiOpenIndyShareBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/button"));
            uiOpenIndyShareBiz2.Click();

            // enter business shareholder > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiIndyShareFirstBiz2.SendKeys(shareholderFirstNameBiz2);

            // enter business shareholder > individual shareholder last name
            NgWebElement uiIndyShareLastBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiIndyShareLastBiz2.SendKeys(shareholderLastNameBiz2);

            // enter business shareholder > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
            uiIndyShareVotesBiz2.SendKeys(shareholderVotingSharesBiz2);

            // enter business shareholder > individual shareholder email
            NgWebElement uiIndyShareEmailBiz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiIndyShareEmailBiz2.SendKeys(shareholderEmailBiz2);

            // enter business shareholder > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz2 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiCalendarIndyS1Biz2.Click();

            SharedCalendarDate();
        }
        public void business_shareholder_3()
        {
            /********** Business Shareholder #3 **********/

            // create the business shareholder data
            string businessName3 = "Business Shareholder 3";
            string businessVotingShares3 = "3";
            string businessEmail3 = "businessshareholder3@email.com";

            // open business shareholder form
            NgWebElement uiOpenShareBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/button"));
            uiOpenShareBiz3.Click();

            // enter business name
            NgWebElement uiShareFirstBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiShareFirstBiz3.SendKeys(businessName3);

            // enter business voting shares
            NgWebElement uiShareVotesBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiShareVotesBiz3.SendKeys(businessVotingShares3);

            // select the business type using dropdown
            NgWebElement uiShareBizType3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/select/option[2]"));
            uiShareBizType3.Click();

            // enter business shareholder email
            NgWebElement uiShareEmailBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
            uiShareEmailBiz3.SendKeys(businessEmail3);

            // select the business shareholder confirm button
            NgWebElement uiShareBizConfirmButton3 = ngDriver.FindElement(By.XPath("//div[@id='cdk-accordion-child-1']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[5]/i/span"));
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
            string keyPersonnelFirstNameBiz3 = "KeyPersonnelBiz3First";
            string keyPersonnelLastNameBiz3 = "KeyPersonnelBiz3Last";
            string keyPersonnelTitleBiz3 = "KeyPersonnelBiz3Title";
            string keyPersonnelEmailBiz3 = "keypersonnel@biz3.com";

            // open business shareholder > key personnel form
            NgWebElement openKeyPersonnelFormBiz3 = ngDriver.FindElement(By.XPath("//div[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section/app-associate-list/div/button"));
            openKeyPersonnelFormBiz3.Click();

            // enter business shareholder > key personnel first name
            NgWebElement uiKeyPersonFirstBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiKeyPersonFirstBiz3.SendKeys(keyPersonnelFirstNameBiz3);

            // enter business shareholder > key personnel last name
            NgWebElement uiKeyPersonLastBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiKeyPersonLastBiz3.SendKeys(keyPersonnelLastNameBiz3);

            // select business shareholder > key personnel role using checkbox
            NgWebElement uiKeyPersonRoleBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
            uiKeyPersonRoleBiz3.Click();

            // enter business shareholder > key personnel title
            NgWebElement uiKeyPersonTitleBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
            uiKeyPersonTitleBiz3.SendKeys(keyPersonnelTitleBiz3);

            // enter business shareholder > key personnel email
            NgWebElement uiKeyPersonEmailBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiKeyPersonEmailBiz3.SendKeys(keyPersonnelEmailBiz3);

            // enter business shareholder > key personnel DOB
            NgWebElement uiKeyPersonnelDOB1Biz13 = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-multi-stage-application-flow/div/mat-horizontal-stepper/div[2]/div[2]/app-application-licensee-changes/div/section[1]/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr[2]/td/mat-expansion-panel/div/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr[2]/td/mat-expansion-panel/div/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr[2]/td/mat-expansion-panel/div/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
            uiKeyPersonnelDOB1Biz13.Click();

            SharedCalendarDate();

            /********** Business Shareholder #3 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz3 = "IndividualShareholderBiz3First";
            string shareholderLastNameBiz3 = "IndividualShareholderBiz3Last";
            string shareholderVotingSharesBiz3 = "1000";
            string shareholderEmailBiz3 = "individualshareholder@biz3.com";

            // open business shareholder > individual shareholder form
            NgWebElement uiOpenIndyShareBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/button"));
            uiOpenIndyShareBiz3.Click();

            // enter business shareholder > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiIndyShareFirstBiz3.SendKeys(shareholderFirstNameBiz3);

            // enter business shareholder > individual shareholder last name
            NgWebElement uiIndyShareLastBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiIndyShareLastBiz3.SendKeys(shareholderLastNameBiz3);

            // enter business shareholder > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
            uiIndyShareVotesBiz3.SendKeys(shareholderVotingSharesBiz3);

            // enter business shareholder > individual shareholder email
            NgWebElement uiIndyShareEmailBiz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiIndyShareEmailBiz3.SendKeys(shareholderEmailBiz3);

            // enter business shareholder > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz3 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiCalendarIndyS1Biz3.Click();

            SharedCalendarDate();
        }
        public void business_shareholder_4()
        {
            /********** Business Shareholder #4 **********/

            // create the business shareholder data
            string businessName4 = "Business Shareholder 4";
            string businessVotingShares4 = "2";
            string businessEmail4 = "businessshareholder4@email.com";

            // open business shareholder form
            NgWebElement uiOpenShareBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/button"));
            uiOpenShareBiz4.Click();

            // enter business name
            NgWebElement uiShareFirstBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[41]"));
            uiShareFirstBiz4.SendKeys(businessName4);

            // enter business voting shares
            NgWebElement uiShareVotesBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[42]"));
            uiShareVotesBiz4.SendKeys(businessVotingShares4);

            // select the business type using dropdown
            NgWebElement uiShareBizType4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/select/option[2]"));
            uiShareBizType4.Click();

            // enter business shareholder email
            NgWebElement uiShareEmailBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[43]"));
            uiShareEmailBiz4.SendKeys(businessEmail4);

            // select the business shareholder confirm button
            NgWebElement uiShareBizConfirmButton4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[5]/i[1]/span"));
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
            string keyPersonnelFirstNameBiz4 = "KeyPersonnelBiz4First";
            string keyPersonnelLastNameBiz4 = "KeyPersonnelBiz4Last";
            string keyPersonnelTitleBiz4 = "KeyPersonnelBiz4Title";
            string keyPersonnelEmailBiz4 = "keypersonnel@biz4.com";

            // open business shareholder > key personnel form
            NgWebElement openKeyPersonnelFormBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section/app-associate-list/div/button"));
            openKeyPersonnelFormBiz4.Click();
        
            // enter business shareholder > key personnel first name
            NgWebElement uiKeyPersonFirstBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiKeyPersonFirstBiz4.SendKeys(keyPersonnelFirstNameBiz4);

            // enter business shareholder > key personnel last name
            NgWebElement uiKeyPersonLastBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiKeyPersonLastBiz4.SendKeys(keyPersonnelLastNameBiz4);

            // select business shareholder > key personnel role using checkbox
            NgWebElement uiKeyPersonRoleBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
            uiKeyPersonRoleBiz4.Click();

            // enter business shareholder > key personnel title
            NgWebElement uiKeyPersonTitleBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
            uiKeyPersonTitleBiz4.SendKeys(keyPersonnelTitleBiz4);

            // enter business shareholder > key personnel email
            NgWebElement uiKeyPersonEmailBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiKeyPersonEmailBiz4.SendKeys(keyPersonnelEmailBiz4);

            // enter business shareholder > key personnel DOB
            NgWebElement uiKeyPersonnelDOB1Biz14 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
            uiKeyPersonnelDOB1Biz14.Click();

            SharedCalendarDate();

            /********** Business Shareholder #4 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz4 = "IndividualShareholderBiz4First";
            string shareholderLastNameBiz4 = "IndividualShareholderBiz4Last";
            string shareholderVotingSharesBiz4 = "1";
            string shareholderEmailBiz4 = "individualshareholder@biz4.com";

            // open business shareholder > individual shareholder form
            NgWebElement uiOpenIndyShareBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/button"));
            uiOpenIndyShareBiz4.Click();

            // enter business shareholder > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiIndyShareFirstBiz4.SendKeys(shareholderFirstNameBiz4);

            // enter business shareholder > individual shareholder last name
            NgWebElement uiIndyShareLastBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiIndyShareLastBiz4.SendKeys(shareholderLastNameBiz4);

            // enter business shareholder > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
            uiIndyShareVotesBiz4.SendKeys(shareholderVotingSharesBiz4);

            // enter business shareholder > individual shareholder email
            NgWebElement uiIndyShareEmailBiz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiIndyShareEmailBiz4.SendKeys(shareholderEmailBiz4);

            // enter business shareholder > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz4 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiCalendarIndyS1Biz4.Click();

            SharedCalendarDate();
        }
        public void business_shareholder_5()
        {
            /********** Business Shareholder #5 **********/

            // create the business shareholder data
            string businessName5 = "Business Shareholder 5";
            string businessVotingShares5 = "1";
            string businessEmail5 = "businessshareholder5@email.com";

            // open business shareholder form
            NgWebElement uiOpenShareBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/button"));
            uiOpenShareBiz5.Click();

            // enter business name
            NgWebElement uiShareFirstBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiShareFirstBiz5.SendKeys(businessName5);

            // enter business voting shares
            NgWebElement uiShareVotesBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiShareVotesBiz5.SendKeys(businessVotingShares5);

            // select the business type using dropdown
            NgWebElement uiShareBizType5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/select/option[2]"));
            uiShareBizType5.Click();

            // enter business shareholder email
            NgWebElement uiShareEmailBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiShareEmailBiz5.SendKeys(businessEmail5);

            // select the business shareholder confirm button
            NgWebElement uiShareBizConfirmButton5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div[5]/section[2]/app-associate-list/div/table/tr/td[5]/i[1]/span"));
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
            string keyPersonnelFirstNameBiz5 = "KeyPersonnelBiz5First";
            string keyPersonnelLastNameBiz5 = "KeyPersonnelBiz5Last";
            string keyPersonnelTitleBiz5 = "KeyPersonnelBiz5Title";
            string keyPersonnelEmailBiz5 = "keypersonnel@biz5.com";

            // open business shareholder > key personnel form
            NgWebElement openKeyPersonnelFormBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section/app-associate-list/div/button"));
            openKeyPersonnelFormBiz5.Click();

            // enter business shareholder > key personnel first name
            NgWebElement uiKeyPersonFirstBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiKeyPersonFirstBiz5.SendKeys(keyPersonnelFirstNameBiz5);

            // enter business shareholder > key personnel last name
            NgWebElement uiKeyPersonLastBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiKeyPersonLastBiz5.SendKeys(keyPersonnelLastNameBiz5);

            // select business shareholder > key personnel role using checkbox
            NgWebElement uiKeyPersonRoleBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[3]/app-field/section/div/section/table/tr/td[1]/input[1]"));
            uiKeyPersonRoleBiz5.Click();

            // enter business shareholder > key personnel title
            NgWebElement uiKeyPersonTitleBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[4]/app-field/section/div/section/input"));
            uiKeyPersonTitleBiz5.SendKeys(keyPersonnelTitleBiz5);

            // enter business shareholder > key personnel email
            NgWebElement uiKeyPersonEmailBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiKeyPersonEmailBiz5.SendKeys(keyPersonnelEmailBiz5);

            // enter business shareholder > key personnel DOB
            NgWebElement uiKeyPersonnelDOB1Biz15 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[6]/app-field/section/div[1]/section/input"));
            uiKeyPersonnelDOB1Biz15.Click();

            SharedCalendarDate();

            /********** Business Shareholder #5 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz5 = "IndividualShareholderBiz5First";
            string shareholderLastNameBiz5 = "IndividualShareholderBiz5Last";
            string shareholderVotingSharesBiz5 = "1";
            string shareholderEmailBiz5 = "individualshareholder@biz5.com";

            // open business shareholder > individual shareholder form
            NgWebElement uiOpenIndyShareBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/button"));
            uiOpenIndyShareBiz5.Click();

            // enter business shareholder > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiIndyShareFirstBiz5.SendKeys(shareholderFirstNameBiz5);

            // enter business shareholder > individual shareholder last name
            NgWebElement uiIndyShareLastBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiIndyShareLastBiz5.SendKeys(shareholderLastNameBiz5);

            // enter business shareholder > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
            uiIndyShareVotesBiz5.SendKeys(shareholderVotingSharesBiz5);

            // enter business shareholder > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiCalendarIndyS1Biz5.Click();

            SharedCalendarDate();

            // enter business shareholder > individual shareholder email
            NgWebElement uiIndyShareEmailBiz5 = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-4']/div/section/app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiIndyShareEmailBiz5.SendKeys(shareholderEmailBiz5);
        }
    }
}
