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
        [And(@"I review the organization structure for a(.*)")]
        public void ReviewOrganizationStructure(string businessType)
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            if (businessType == " private corporation")
            {
                // find the upload test files in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload a notice of articles document
                string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
                NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
                uiUploadSignage.SendKeys(noticeOfArticles);

                // upload a central securities register document
                string centralSecuritiesRegister = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
                NgWebElement uiUploadCentralSecReg = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
                uiUploadCentralSecReg.SendKeys(centralSecuritiesRegister);

                // upload a special rights and restrictions document
                string specialRightsRestrictions = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
                NgWebElement uiUploadSpecialRightsRes = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
                uiUploadSpecialRightsRes.SendKeys(specialRightsRestrictions);

                // upload an additional supporting document
                string additionalSupportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "associates.pdf");
                NgWebElement uiUploadAdditionalSupportingDocument = ngDriver.FindElement(By.XPath("(//input[@type='file'])[11]"));
                uiUploadAdditionalSupportingDocument.SendKeys(additionalSupportingDocument);

                /********** Leader #0 **********/

                // create the leader data
                string leaderFirstName = "Leader0First";
                string leaderLastName = "Leader0Last";
                string leaderTitle = "CTOLeader0";
                string leaderEmail = "leader0@privatecorp.com";

                // open leader #0 form  
                NgWebElement uiOpenLeaderForm = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leadership'][changetypesuffix='Leadership'] button"));
                uiOpenLeaderForm.Click();

                // enter leader #0 first name
                NgWebElement uiLeaderFirst = ngDriver.FindElement(By.CssSelector("input[formControlName='firstNameNew']"));
                uiLeaderFirst.SendKeys(leaderFirstName);

                // enter leader #0 last name
                NgWebElement uiLeaderLast = ngDriver.FindElement(By.CssSelector("input[formControlName='lastNameNew']"));
                uiLeaderLast.SendKeys(leaderLastName);

                // select leader #0 role
                NgWebElement uiLeaderRole = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leadership'][changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
                uiLeaderRole.Click();

                // enter leader #0 title
                NgWebElement uiLeaderTitle = ngDriver.FindElement(By.CssSelector("input[formControlName='titleNew']"));
                uiLeaderTitle.SendKeys(leaderTitle);

                // enter leader #0 email
                NgWebElement uiLeaderEmail = ngDriver.FindElement(By.CssSelector("input[formControlName='emailNew']"));
                uiLeaderEmail.SendKeys(leaderEmail);

                // enter leader #0 DOB
                NgWebElement uiOpenLeaderDOB = ngDriver.FindElement(By.CssSelector("input[formControlName='dateofBirthNew']"));
                uiOpenLeaderDOB.Click();

                // select the date
                SharedCalendarDate();

                // click on the Confirm button
                NgWebElement uiConfirmButtonLeader = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] .fa-save span"));
                uiConfirmButtonLeader.Click();

                /********** Individual Shareholder #0 **********/

                // create the shareholder data
                string shareholderFirstName = "IndyShareholder0First";
                string shareholderLastName = "IndyShareholder0Last";
                string shareholderVotingShares = "1001";
                string shareholderNonVotingShares = "1002";
                string shareholderEmail = "individualshareholder0@privatecorp.com";

                // open shareholder #0 form
                NgWebElement uiOpenShare = ngDriver.FindElement(By.CssSelector("[changetypesuffix='IndividualShareholder'][addlabel='Add Individual Shareholder'] button"));
                uiOpenShare.Click();

                // enter shareholder #0 first name
                NgWebElement uiShareFirst = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName=\"firstNameNew\"]"));
                uiShareFirst.SendKeys(shareholderFirstName);

                // enter shareholder #0 last name
                NgWebElement uiShareLast = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='lastNameNew']"));
                uiShareLast.SendKeys(shareholderLastName);

                // enter shareholder #0 number of voting shares
                NgWebElement uiShareVotes = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='numberofSharesNew']"));
                uiShareVotes.SendKeys(shareholderVotingShares);

                // enter shareholder #0 number of non voting shares
                NgWebElement uiShareNonVotes = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='numberOfNonVotingSharesNew']"));
                uiShareNonVotes.SendKeys(shareholderNonVotingShares);

                // enter shareholder #0 email
                NgWebElement uiShareEmail = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='emailNew']"));
                uiShareEmail.SendKeys(shareholderEmail);

                // enter shareholder #0 DOB
                NgWebElement uiCalendarS1 = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='dateofBirthNew']"));
                uiCalendarS1.Click();

                // select the date
                SharedCalendarDate();

                // click on the Confirm button
                NgWebElement uiConfirmButtonIndyShareholder = ngDriver.FindElement(By.CssSelector("[changetypesuffix='IndividualShareholder'] .fa-save span"));
                uiConfirmButtonIndyShareholder.Click();

                /********** Business Shareholder #1 **********/

                // create the business shareholder data
                string businessName = "Business Shareholder 1";
                string businessVotingShares = "1003";
                string businessNonVotingShares = "1004";
                string businessEmail = "business@shareholder1.com";

                // open business shareholder #1 form    
                NgWebElement uiOpenShareBiz = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] button"));
                uiOpenShareBiz.Click();

                // enter business shareholder #1 name
                NgWebElement uiShareFirstBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='businessNameNew']"));
                uiShareFirstBiz.SendKeys(businessName);

                // enter business shareholder #1 voting shares
                NgWebElement uiShareVotesBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='numberofSharesNew']"));
                uiShareVotesBiz.SendKeys(businessVotingShares);

                // enter business shareholder #1 non voting shares
                NgWebElement uiShareNonVotesBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='numberOfNonVotingSharesNew']"));
                uiShareNonVotesBiz.SendKeys(businessNonVotingShares);

                // select the business shareholder #1 type
                NgWebElement uiShareBizType = ngDriver.FindElement(By.CssSelector("[formcontrolname='businessType'] option[value='PrivateCorporation']"));
                uiShareBizType.Click();

                // enter business shareholder #1 email
                NgWebElement uiShareEmailBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='emailNew']"));
                uiShareEmailBiz.SendKeys(businessEmail);

                // select the business shareholder #1 confirm button
                NgWebElement uiShareBizConfirmButton = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] .fa-save.ng-star-inserted span"));
                uiShareBizConfirmButton.Click();

                // upload a notice of articles document for business shareholder #1 
                string noticeOfArticlesBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
                NgWebElement uiUploadNoticeofArticlesBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
                uiUploadNoticeofArticlesBiz.SendKeys(noticeOfArticlesBiz);

                // upload a central securities register document for business shareholder #1 
                string centralSecuritiesRegisterBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
                NgWebElement uiUploadCentralSecRegBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[18]"));
                uiUploadCentralSecRegBiz.SendKeys(centralSecuritiesRegisterBiz);

                // upload a special rights and restrictions document for business shareholder #1 
                string specialRightsRestrictionsBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
                NgWebElement uiUploadSpecialRightsResBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[21]"));
                uiUploadSpecialRightsResBiz.SendKeys(specialRightsRestrictionsBiz);

                /********** Business Shareholder #1 - Leader #1 **********/

                // create business shareholder #1 leader data
                string leaderFirstNameBiz = "Leader1BizFirst";
                string leaderLastNameBiz = "Leader1BizLast";
                string leaderTitleBiz = "Leader1BizTitle";
                string leaderEmailBiz = "leader1bizshareholder@privatecorp.com";

                // open business shareholder #1 > leader form #1
                NgWebElement uiOpenLeaderFormBiz = ngDriver.FindElement(By.CssSelector("#cdk-accordion-child-0 .padded-section:nth-child(1) .btn-secondary"));
                uiOpenLeaderFormBiz.Click();

                // enter business shareholder #1 > leader #1 first name
                NgWebElement uiLeaderFirstBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
                uiLeaderFirstBiz.SendKeys(leaderFirstNameBiz);

                // enter business shareholder #1 > leader #1 last name
                NgWebElement uiLeaderLastBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='lastNameNew']"));
                uiLeaderLastBiz.SendKeys(leaderLastNameBiz);

                // select business shareholder #1 > leader #1 role
                NgWebElement uiLeaderRoleBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [addlabel='Add Leadership'][changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
                uiLeaderRoleBiz.Click();

                // enter business shareholder #1 > leader #1 title
                NgWebElement uiLeaderTitleBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='titleNew']"));
                uiLeaderTitleBiz.SendKeys(leaderTitleBiz);

                // enter business shareholder #1 > leader #1 email 
                NgWebElement uiLeaderEmailBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='emailNew']"));
                uiLeaderEmailBiz.SendKeys(leaderEmailBiz);

                // enter business shareholder #1 > leader #1 DOB
                NgWebElement uiLeaderDOB1Biz1 = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='dateofBirthNew']"));
                uiLeaderDOB1Biz1.Click();

                // select the date
                SharedCalendarDate();

                // click on the Confirm button
                NgWebElement uiConfirmButtonBusLeader = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] .fa-save span"));
                uiConfirmButtonBusLeader.Click();

                /********** Business Shareholder #1 - Individual Shareholder #1 **********/

                // create the business shareholder #1 > individual shareholder data
                string shareholderFirstNameBiz = "IndividualShareholder1Biz1First";
                string shareholderLastNameBiz = "IndividualShareholder1Biz1Last";
                string shareholderVotingSharesBiz = "1005";
                string shareholderNonVotingSharesBiz = "1006";
                string shareholderEmailBiz = "individualshareholder1bizshareholder@privatecorp.com";

                // open business shareholder #1 > individual shareholder #1 form
                NgWebElement uiOpenIndyShareBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [addlabel='Add Individual Shareholder'] button"));
                uiOpenIndyShareBiz.Click();

                // enter business shareholder #1 > individual shareholder #1 first name
                NgWebElement uiIndyShareFirstBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
                uiIndyShareFirstBiz.SendKeys(shareholderFirstNameBiz);

                // enter business shareholder #1 > individual shareholder #1 last name
                NgWebElement uiIndyShareLastBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
                uiIndyShareLastBiz.SendKeys(shareholderLastNameBiz);

                // enter business shareholder #1 > individual shareholder #1 number of voting shares
                NgWebElement uiIndyShareVotesBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
                uiIndyShareVotesBiz.SendKeys(shareholderVotingSharesBiz);

                // enter business shareholder #1 > individual shareholder #1 number of non voting shares
                NgWebElement uiIndyShareNonVotesBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberOfNonVotingSharesNew']"));
                uiIndyShareNonVotesBiz.SendKeys(shareholderNonVotingSharesBiz);

                // enter business shareholder #1 > individual shareholder #1 email
                NgWebElement uiIndyShareEmailBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
                uiIndyShareEmailBiz.SendKeys(shareholderEmailBiz);

                // enter business shareholder #1 > individual shareholder #1 DOB
                NgWebElement uiCalendarIndyS1Biz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
                uiCalendarIndyS1Biz.Click();

                // select the date
                SharedCalendarDate();

                // click on the Confirm button
                NgWebElement uiConfirmButtonBusIndyShareholder = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] .fa-save span"));
                uiConfirmButtonBusIndyShareholder.Click();
            }

            if (businessType == " sole proprietorship")
            {
                // find the upload test files in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload an additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
                NgWebElement uiUploadadditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
                uiUploadadditionalSupporting.SendKeys(additionalSupportingPath);

                /********** Sole Proprietor > Leader #1 **********/

                // open the leader row
                NgWebElement uiOpenLeaderForm = ngDriver.FindElement(By.CssSelector("button.btn.btn-secondary"));
                uiOpenLeaderForm.Click();

                // create the leader info
                string firstName = "Leader1First";
                string lastName = "Leader1Last";
                string email = "leader1@soleproprietor.com";

                // enter the leader first name
                NgWebElement uiFirstName = ngDriver.FindElement(By.CssSelector("[formControlName='firstNameNew']"));
                uiFirstName.SendKeys(firstName);

                // enter the leader last name
                NgWebElement uiLastName = ngDriver.FindElement(By.CssSelector("[formControlName='lastNameNew']"));
                uiLastName.SendKeys(lastName);

                // enter the leader email
                NgWebElement uiEmail = ngDriver.FindElement(By.CssSelector("[formControlName='emailNew']"));
                uiEmail.SendKeys(email);

                // select the leader DOB
                NgWebElement uiOpenLeaderDOB = ngDriver.FindElement(By.CssSelector("[formcontrolname='dateofBirthNew']"));
                uiOpenLeaderDOB.Click();

                // select the date
                SharedCalendarDate();

                /********** Sole Proprietor > Leader #2 **********/

                // open the leader #2 row
                NgWebElement uiOpenLeaderForm2 = ngDriver.FindElement(By.CssSelector("button.btn.btn-secondary"));
                uiOpenLeaderForm2.Click();

                // create the leader #2 info
                string firstName2 = "Leader2First";
                string lastName2 = "Leader2Last";
                string email2 = "leader2@soleproprietor.com";

                // enter the leader #2 first name
                NgWebElement uiFirstName2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiFirstName2.SendKeys(firstName2);

                // enter the leader #2 last name
                NgWebElement uiLastName2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiLastName2.SendKeys(lastName2);

                // enter the leader #2 email
                NgWebElement uiEmail2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(3) input[formControlName='emailNew']"));
                uiEmail2.SendKeys(email2);

                // select the leader #2 DOB
                NgWebElement uiOpenLeaderDOB2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(4) input[formControlName='dateofBirthNew']"));
                uiOpenLeaderDOB2.Click();

                // select the date
                SharedCalendarDate();

                /********** Sole Proprietor > Leader #3 **********/

                // open the leader #3 row
                NgWebElement uiOpenLeaderForm3 = ngDriver.FindElement(By.CssSelector("button.btn.btn-secondary"));
                uiOpenLeaderForm3.Click();

                // create the leader #2 info
                string firstName3 = "Leader3First";
                string lastName3 = "Leader3Last";
                string email3 = "leader3@soleproprietor.com";

                // enter the leader #2 first name
                NgWebElement uiFirstName3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiFirstName3.SendKeys(firstName3);

                // enter the leader #2 last name
                NgWebElement uiLastName3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiLastName3.SendKeys(lastName3);

                // enter the leader #3 email
                NgWebElement uiEmail3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(3) input[formControlName='emailNew']"));
                uiEmail3.SendKeys(email3);

                // select the leader #3 DOB
                NgWebElement uiOpenLeaderDOB3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(4) input[formControlName='dateofBirthNew']"));
                uiOpenLeaderDOB3.Click();

                // select the date
                SharedCalendarDate();
            }

            if (businessType == " society")
            {
                /********** Director #1 **********/

                // create society data
                string membershipFee = "2500";
                string membershipNumber = "200";

                // enter Annual Membership Fee
                NgWebElement uiMemberFee = ngDriver.FindElement(By.CssSelector("[formcontrolname='annualMembershipFee']"));
                uiMemberFee.SendKeys(membershipFee);

                // enter Number of Members
                NgWebElement uiMemberNumber = ngDriver.FindElement(By.CssSelector("[formcontrolname='numberOfMembers']"));
                uiMemberNumber.SendKeys(membershipNumber);

                // find the upload test files in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload an additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
                NgWebElement uiUploadAdditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
                uiUploadAdditionalSupporting.SendKeys(additionalSupportingPath);

                // open the director #1 row 
                NgWebElement uiOpenDirectorForm = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'][changetypesuffix='Leadership'] button"));
                uiOpenDirectorForm.Click();

                // create the director #1 info
                string firstName = "Director";
                string lastName = "Society";
                string title = "Chair";
                string email = "director@society.com";

                // enter the director #1 first name
                NgWebElement uiFirstName = ngDriver.FindElement(By.CssSelector("[formcontrolname='firstNameNew']"));
                uiFirstName.SendKeys(firstName);

                // enter the director #1 last name
                NgWebElement uiLastName = ngDriver.FindElement(By.CssSelector("[formcontrolname='lastNameNew']"));
                uiLastName.SendKeys(lastName);

                // select the director #1 position
                NgWebElement uiPosition = ngDriver.FindElement(By.CssSelector("[formcontrolname='isDirectorNew']"));
                uiPosition.Click();

                // enter the director #1 title
                NgWebElement uiTitle = ngDriver.FindElement(By.CssSelector("[formcontrolname='titleNew']"));
                uiTitle.SendKeys(title);

                // enter the director #1 email
                NgWebElement uiEmail = ngDriver.FindElement(By.CssSelector("[formcontrolname='emailNew']"));
                uiEmail.SendKeys(email);

                // select the director #1 DOB
                NgWebElement uiOpenDirectorDOB = ngDriver.FindElement(By.CssSelector("[formcontrolname='dateofBirthNew']"));
                uiOpenDirectorDOB.Click();

                // select the date
                SharedCalendarDate();

                /********** Director #2 **********/

                // open the director #2 row 
                NgWebElement uiOpenDirectorForm2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] button"));
                uiOpenDirectorForm2.Click();

                // create the director #2 info
                string firstName2 = "Director2";
                string email2 = "director2@society.com";

                // enter the director #2 first name
                NgWebElement uiFirstName2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiFirstName2.SendKeys(firstName2);

                // enter the director #2 last name
                NgWebElement uiLastName2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiLastName2.SendKeys(lastName);

                // select the director #2 position
                NgWebElement uiPosition2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(3) input[formControlName='isDirectorNew']"));
                uiPosition2.Click();

                // enter the director #2 title
                NgWebElement uiTitle2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(4) input[formControlName='titleNew']"));
                uiTitle2.SendKeys(title);

                // enter the director #2 email
                NgWebElement uiEmail2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(5) input[formControlName='emailNew']"));
                uiEmail2.SendKeys(email2);

                // select the director #2 DOB
                NgWebElement uiOpenDirectorDOB2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(6) input[formControlName='dateofBirthNew']"));
                uiOpenDirectorDOB2.Click();

                // select the date
                SharedCalendarDate();

                /********** Director #3 **********/

                // open the director #3 row 
                NgWebElement uiOpenDirectorForm3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] button"));
                uiOpenDirectorForm3.Click();

                // create the director #3 info
                string firstName3 = "Director3";
                string email3 = "director3@society.com";

                // enter the director #3 first name
                NgWebElement uiFirstName3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiFirstName3.SendKeys(firstName3);

                // enter the director #3 last name
                NgWebElement uiLastName3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiLastName3.SendKeys(lastName);

                // select the director #3 position
                NgWebElement uiPosition3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(3) input[formControlName='isDirectorNew']"));
                uiPosition3.Click();

                // enter the director #3 title
                NgWebElement uiTitle3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(4) input[formControlName='titleNew']"));
                uiTitle3.SendKeys(title);

                // enter the director #3 email
                NgWebElement uiEmail3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(5) input[formControlName='emailNew']"));
                uiEmail3.SendKeys(email3);

                // select the director #2 DOB
                NgWebElement uiOpenDirectorDOB3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(6) input[formControlName='dateofBirthNew']"));
                uiOpenDirectorDOB3.Click();

                // select the date
                SharedCalendarDate();
            }

            if (businessType == " public corporation")
            {
                /********** Leader #1 **********/

                // create the leader #1 data
                string leaderFirst = "Leader1";
                string leaderLast = "Public Corp";
                string leaderTitle = "CEO";
                string leaderEmail = "leader1@publiccorp.com";

                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload NOA form
                string NOAPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uiUploadNOA = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
                uiUploadNOA.SendKeys(NOAPath);

                // upload additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
                NgWebElement uiUploadAdditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
                uiUploadAdditionalSupporting.SendKeys(additionalSupportingPath);

                // open leader #1 form
                NgWebElement uiOpenLeaderForm = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] button"));
                uiOpenLeaderForm.Click();

                // enter leader #1 first name
                NgWebElement uiLeaderFirst = ngDriver.FindElement(By.CssSelector("[formcontrolname='firstNameNew']"));
                uiLeaderFirst.SendKeys(leaderFirst);

                // enter leader #1 last name
                NgWebElement uiLeaderLast = ngDriver.FindElement(By.CssSelector("[formcontrolname='lastNameNew']"));
                uiLeaderLast.SendKeys(leaderLast);

                // select leader #1 role
                NgWebElement uiLeaderRole = ngDriver.FindElement(By.CssSelector("[formcontrolname='isDirectorNew']"));
                uiLeaderRole.Click();

                // enter leader #1 title
                NgWebElement uiLeaderTitle = ngDriver.FindElement(By.CssSelector("[formcontrolname='titleNew']"));
                uiLeaderTitle.SendKeys(leaderTitle);

                // enter leader #1 email
                NgWebElement uiLeaderEmail = ngDriver.FindElement(By.CssSelector("[formcontrolname='emailNew']"));
                uiLeaderEmail.SendKeys(leaderEmail);

                // select leader #1 DOB
                NgWebElement uiLeaderDOB = ngDriver.FindElement(By.CssSelector("[formcontrolname='dateofBirthNew']"));
                uiLeaderDOB.Click();

                // select the date
                SharedCalendarDate();

                /********** Leader #2 **********/

                // create the leader #2 data
                string leaderFirst2 = "Leader2";
                string leaderEmail2 = "leader@publiccorp.com";

                // open leader #2 form
                NgWebElement uiOpenLeaderForm2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] button"));
                uiOpenLeaderForm2.Click();

                // enter leader #2 first name
                NgWebElement uiLeaderFirst2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiLeaderFirst2.SendKeys(leaderFirst2);

                // enter leader #2 last name
                NgWebElement uiLeaderLast2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiLeaderLast2.SendKeys(leaderLast);

                // select leader #2 role
                NgWebElement uiLeaderRole2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(3) input[formControlName='isDirectorNew']"));
                uiLeaderRole2.Click();

                // enter leader #2 title
                NgWebElement uiLeaderTitle2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(4) input[formControlName='titleNew']"));
                uiLeaderTitle2.SendKeys(leaderTitle);

                // enter leader #2 email
                NgWebElement uiLeaderEmail2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(5) input[formControlName='emailNew']"));
                uiLeaderEmail2.SendKeys(leaderEmail2);

                // select leader #2 DOB
                NgWebElement uiLeaderDOB2 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(3) td:nth-child(6) input[formControlName='dateofBirthNew']"));
                uiLeaderDOB2.Click();

                // select the date
                SharedCalendarDate();

                /********** Leader #3 **********/

                // create the leader #3 data
                string leaderFirst3 = "Leader3";
                string leaderEmail3 = "leader3@publiccorp.com";

                // open leader #3 form
                NgWebElement uiOpenLeaderForm3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] button"));
                uiOpenLeaderForm3.Click();

                // enter leader #3 first name
                NgWebElement uiLeaderFirst3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(1) input[formControlName='firstNameNew']"));
                uiLeaderFirst3.SendKeys(leaderFirst3);

                // enter leader #3 last name
                NgWebElement uiLeaderLast3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(2) input[formControlName='lastNameNew']"));
                uiLeaderLast3.SendKeys(leaderLast);

                // select leader #3 role
                NgWebElement uiLeaderRole3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(3) input[formControlName='isDirectorNew']"));
                uiLeaderRole3.Click();

                // enter leader #3 title
                NgWebElement uiLeaderTitle3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(4) input[formControlName='titleNew']"));
                uiLeaderTitle3.SendKeys(leaderTitle);

                // enter leader #3 email
                NgWebElement uiLeaderEmail3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(5) input[formControlName='emailNew']"));
                uiLeaderEmail3.SendKeys(leaderEmail3);

                // select leader #3 DOB
                NgWebElement uiKeyPersonDOB3 = ngDriver.FindElement(By.CssSelector("app-associate-list tr:nth-child(4) td:nth-child(6) input[formControlName='dateofBirthNew']"));
                uiKeyPersonDOB3.Click();

                // select the date
                SharedCalendarDate();
            }

            if (businessType == " partnership")
            {
                // create individual partner info
                string partnerFirstName = "Individual";
                string partnerLastName = "Partner";
                string partnerPercentage = "50";
                string partnerEmail = "individual@partner.com";

                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload the partnership agreement
                string partnershipPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "partnership_agreement.pdf");
                NgWebElement uiUploadPartnershipAgreement = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
                uiUploadPartnershipAgreement.SendKeys(partnershipPath);

                // upload the additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "partnership_agreement.pdf");
                NgWebElement uiUploadAdditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
                uiUploadAdditionalSupporting.SendKeys(additionalSupportingPath);

                // open partner row
                NgWebElement uiPartnerRow = ngDriver.FindElement(By.CssSelector("[changetypesuffix='IndividualShareholder'] button"));
                uiPartnerRow.Click();

                // enter partner first name
                NgWebElement uiPartnerFirst = ngDriver.FindElement(By.CssSelector("[formcontrolname='firstNameNew']"));
                uiPartnerFirst.SendKeys(partnerFirstName);

                // enter partner last name
                NgWebElement uiPartnerLast = ngDriver.FindElement(By.CssSelector("[formcontrolname='lastNameNew']"));
                uiPartnerLast.SendKeys(partnerLastName);

                // enter partner percentage
                NgWebElement uiPartnerPercentage = ngDriver.FindElement(By.CssSelector("[formcontrolname='interestPercentageNew']"));
                uiPartnerPercentage.SendKeys(partnerPercentage);

                // enter partner email
                NgWebElement uiPartnerEmail = ngDriver.FindElement(By.CssSelector("[formcontrolname='emailNew']"));
                uiPartnerEmail.SendKeys(partnerEmail);

                // enter partner DOB
                NgWebElement uiOpenPartnerDOB = ngDriver.FindElement(By.CssSelector("[formcontrolname='dateofBirthNew']"));
                uiOpenPartnerDOB.Click();

                // select the date
                SharedCalendarDate();

                // open business partner row
                NgWebElement uiOpenPartnerRow = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] button"));
                uiOpenPartnerRow.Click();

                // create business partner info
                string bizPartnerName = "Business Partner";
                string bizPartnerPercentage = "50";
                string bizPartnerEmail = "business@partner.com";

                // enter the business partner name
                NgWebElement uiBizPartnerName = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='businessNameNew']"));
                uiBizPartnerName.SendKeys(bizPartnerName);

                // enter the business partner percentage
                NgWebElement uiBizPartnerPercentage = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='interestPercentageNew']"));
                uiBizPartnerPercentage.SendKeys(bizPartnerPercentage);

                // select the business type using dropdown
                NgWebElement uiShareBizType = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='Partnership']"));
                uiShareBizType.Click();

                // enter the business partner email
                NgWebElement uiBizPartnerEmail = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='emailNew']"));
                uiBizPartnerEmail.SendKeys(bizPartnerEmail);

                // click on the business shareholder confirm button
                NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'][addlabel='Add Business Partner'] .fa-save span"));
                uiConfirmButton.Click();

                // upload a second partnership agreement
                string partnershipPath2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "partnership_agreement.pdf");
                NgWebElement uiUploadPartnership2Agreement = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
                uiUploadPartnership2Agreement.SendKeys(partnershipPath2);

                // open individual partner 2 row
                NgWebElement uiOpenPartner2Row = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
                uiOpenPartner2Row.Click();

                // create individual partner 2 info
                string partner2FirstName = "Individual";
                string partner2LastName = "Partner2";
                string partner2Percentage = "50";
                string partner2Email = "individual@partner2.com";

                // enter individual partner2 first name
                NgWebElement uiPartner2First = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formControlName='firstNameNew']"));
                uiPartner2First.SendKeys(partner2FirstName);

                // enter individual partner2 last name
                NgWebElement uiPartner2Last = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formControlName='lastNameNew']"));
                uiPartner2Last.SendKeys(partner2LastName);

                // enter individual partner2 percentage
                NgWebElement uiPartner2Percentage = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formControlName='interestPercentageNew']"));
                uiPartner2Percentage.SendKeys(partner2Percentage);

                // enter individual partner2 email
                NgWebElement uiPartner2Email = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formControlName='emailNew']"));
                uiPartner2Email.SendKeys(partner2Email);

                // enter individual partner2 DOB
                NgWebElement uiOpenPartner2DOB = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formControlName='dateofBirthNew']"));
                uiOpenPartner2DOB.Click();

                // select the date
                SharedCalendarDate();

                // click on individual partner2 confirm button
                NgWebElement uiConfirmButton2 = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Partner'][changetypesuffix='BusinessShareholder'] .fa-save span"));
                uiConfirmButton2.Click();
            }

            if (businessType == "n indigenous nation")
            {
                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload the associates document
                string associatesPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "associates.pdf");
                NgWebElement uiUploadAssociates = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
                uiUploadAssociates.SendKeys(associatesPath);

                // upload the additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "associates.pdf");
                NgWebElement uiUploadAdditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
                uiUploadAdditionalSupporting.SendKeys(additionalSupportingPath);
            }

            if (businessType == " local government")
            {
                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload the additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "associates.pdf");
                NgWebElement uiUploadAdditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
                uiUploadAdditionalSupporting.SendKeys(additionalSupportingPath);
            }

            if (businessType == " university")
            {
                // find the upload test file in the bdd-tests\upload_files folder
                var environment = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(environment).Parent.FullName;
                string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

                // upload an official document
                string officialDocumentPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "associates.pdf");
                NgWebElement uiUploadOfficialDocument = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
                uiUploadOfficialDocument.SendKeys(officialDocumentPath);

                // upload the additional supporting document
                string additionalSupportingPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "associates.pdf");
                NgWebElement uiUploadAdditionalSupporting = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
                uiUploadAdditionalSupporting.SendKeys(additionalSupportingPath);

                /********** Leader #0 **********/

                // create the leader data
                string leaderFirstName = "Leader0";
                string leaderLastName = "University";
                string leaderTitle = "CTO";
                string leaderEmail = "leader0@university.com";

                // open leader #0 form  
                NgWebElement uiOpenLeaderForm = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'][addlabel='Add Leadership'] button.btn-secondary"));
                uiOpenLeaderForm.Click();

                // enter leader #0 first name
                NgWebElement uiLeaderFirst = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
                uiLeaderFirst.SendKeys(leaderFirstName);

                // enter leader #0 last name
                NgWebElement uiLeaderLast = ngDriver.FindElement(By.CssSelector("input[formControlName='lastNameNew']"));
                uiLeaderLast.SendKeys(leaderLastName);

                // select leader #0 role
                NgWebElement uiLeaderRole = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leadership'][changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
                uiLeaderRole.Click();

                // enter leader #0 title
                NgWebElement uiLeaderTitle = ngDriver.FindElement(By.CssSelector("input[formControlName='titleNew']"));
                uiLeaderTitle.SendKeys(leaderTitle);

                // enter leader #0 email
                NgWebElement uiLeaderEmail = ngDriver.FindElement(By.CssSelector("input[formControlName='emailNew']"));
                uiLeaderEmail.SendKeys(leaderEmail);

                // enter leader #0 DOB
                NgWebElement uiOpenLeaderDOB = ngDriver.FindElement(By.CssSelector("input[formControlName='dateofBirthNew']"));
                uiOpenLeaderDOB.Click();

                // select the date
                SharedCalendarDate();

                // click on the Confirm button
                NgWebElement uiConfirmButtonLeader = ngDriver.FindElement(By.CssSelector("[changetypesuffix='Leadership'] .fa-save span"));
                uiConfirmButtonLeader.Click();
            }
        }
    }
}
