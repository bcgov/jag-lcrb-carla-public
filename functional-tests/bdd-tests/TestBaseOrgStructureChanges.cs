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
using System.ComponentModel.DataAnnotations;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    { 
        [And(@"I confirm that the director name has been updated")]
        public void DirectorNameUpdated()
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            // click on the review organization information button
            ClickReviewOrganizationInformation();

            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // check that the director name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'UpdatedFirstName')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'UpdatedLastName')]")).Displayed);
        }


        [And(@"I enter the same individual as a director and a shareholder")]
        public void SameDirectorShareholder()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document
            string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[3]"));
            uiUploadSignage.SendKeys(noticeOfArticles);

            // upload a central securities register document
            string centralSecuritiesRegister = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadCentralSecReg = ngDriver.FindElement(By.XPath("(//input[@type='file'])[6]"));
            uiUploadCentralSecReg.SendKeys(centralSecuritiesRegister);

            // upload a special rights and restrictions document
            string specialRightsRestrictions = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uiUploadSpecialRightsRes = ngDriver.FindElement(By.XPath("(//input[@type='file'])[9]"));
            uiUploadSpecialRightsRes.SendKeys(specialRightsRestrictions);

            /***** Leader #1 *****/

            // click on the Add Leader button
            NgWebElement uiAddLeader = ngDriver.FindElement(By.CssSelector(".padded-section:nth-child(1) .btn-secondary"));
            uiAddLeader.Click();

            // create data
            string sameIndividualFirstName = "Same1";
            string sameIndividualLastName = "Individual";
            string sameIndividualEmail = "same@individual.com";
            string sameTitle = "CEO";
            string votingShares = "100";

            string sameIndividualEmail2 = "same@individual2.com";

            string sparePersonnelFirstName = "Spare";
            string sparePersonnelLastName = "Leader";
            string sparePersonnelTitle = "CFO";
            string sparePersonnelEmail = "cfo@test.com";

            // enter the leader first name 
            NgWebElement uiSameIndividualFirstName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
            uiSameIndividualFirstName.SendKeys(sameIndividualFirstName);

            // enter the leader last name 
            NgWebElement uiSameIndividualLastName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastNameNew']"));
            uiSameIndividualLastName.SendKeys(sameIndividualLastName);

            // click the leader checkbox
            NgWebElement uiSameRole = ngDriver.FindElement(By.CssSelector("input[formcontrolname='isDirectorNew']"));
            uiSameRole.Click();

            // enter the leader title
            NgWebElement uiSameTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='titleNew']"));
            uiSameTitle.SendKeys(sameTitle);

            // enter the leader email 
            NgWebElement uiSameIndividualEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
            uiSameIndividualEmail.SendKeys(sameIndividualEmail);

            // select the leader DOB
            NgWebElement uiOpenLeaderDOB = ngDriver.FindElement(By.CssSelector("input[formcontrolname='dateofBirthNew']"));
            uiOpenLeaderDOB.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector(".fa-save span"));
            uiConfirmButton.Click();

            /***** Leader #2 *****/
            /* This extra person has been added because the calendar selection is unreliable via SharedCalendarDate(). DO NOT REMOVE. */

            // click on the Add Leader button
            NgWebElement uiAddLeader2 = ngDriver.FindElement(By.CssSelector(".padded-section:nth-child(1) .btn-secondary"));
            uiAddLeader2.Click();

            // enter the leader first name 
            NgWebElement uiSameIndividualFirstName2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
            uiSameIndividualFirstName2.SendKeys(sameIndividualFirstName);

            // enter the leader last name 
            NgWebElement uiSameIndividualLastName2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastNameNew']"));
            uiSameIndividualLastName2.SendKeys(sameIndividualLastName);

            // click the leader checkbox
            NgWebElement uiSameRole2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='isDirectorNew']"));
            uiSameRole2.Click();

            // enter the leader title
            NgWebElement uiSameTitle2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='titleNew']"));
            uiSameTitle2.SendKeys(sameTitle);

            // enter the leader email 
            NgWebElement uiSameIndividualEmail2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
            uiSameIndividualEmail2.SendKeys(sameIndividualEmail);

            // select the leader DOB
            NgWebElement uiOpenLeaderDOB2 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='dateofBirthNew']"));
            uiOpenLeaderDOB2.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton1a = ngDriver.FindElement(By.CssSelector(".fa-save span"));
            uiConfirmButton1a.Click();

            /***** Leader #3 *****/

            // click on the add leader button for spare
            NgWebElement uiAddLeader3 = ngDriver.FindElement(By.CssSelector(".ng-touched .btn-secondary"));
            uiAddLeader3.Click();

            // enter the spare leader first name
            NgWebElement uiSpareFirstName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
            uiSpareFirstName.SendKeys(sparePersonnelFirstName);

            // enter the spare leader last name 
            NgWebElement uiSpareLastName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastNameNew']"));
            uiSpareLastName.SendKeys(sparePersonnelLastName);

            // click the spare leader checkbox            
            NgWebElement uiSpareRole = ngDriver.FindElement(By.CssSelector("input[formcontrolname='isDirectorNew']"));
            uiSpareRole.Click();

            // enter the spare leader title
            NgWebElement uiSpareTitle = ngDriver.FindElement(By.CssSelector("input[formcontrolname='titleNew']"));
            uiSpareTitle.SendKeys(sparePersonnelTitle);

            // enter the spare leader email 
            NgWebElement uiSpareEmail = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
            uiSpareEmail.SendKeys(sparePersonnelEmail);

            // select the spare leader DOB
            NgWebElement uiOpenSpareDOB = ngDriver.FindElement(By.CssSelector("input[formcontrolname='dateofBirthNew']"));
            uiOpenSpareDOB.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton2 = ngDriver.FindElement(By.CssSelector(".fa-save span"));
            uiConfirmButton2.Click();

            // delete the first same individual - DO NOT REMOVE
            NgWebElement uiDeleteButton = ngDriver.FindElement(By.XPath("//app-associate-list/div/table/tr[1]/td[7]/i[2]/span"));
            uiDeleteButton.Click();

            /***** Individual Shareholder *****/

            // click on Add Individual Shareholder
            NgWebElement uiAddIndividualShareholder = ngDriver.FindElement(By.CssSelector(".padded-section:nth-child(2) .btn-secondary"));
            uiAddIndividualShareholder.Click();

            // enter the shareholder first name
            NgWebElement uiSameIndividualFirstName3 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='firstNameNew']"));
            uiSameIndividualFirstName3.SendKeys(sameIndividualFirstName);

            // enter the shareholder last name
            NgWebElement uiSameIndividualLastName3 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='lastNameNew']"));
            uiSameIndividualLastName3.SendKeys(sameIndividualLastName);

            // enter the shareholder number of voting shares
            NgWebElement uiSameIndividualVotingShare = ngDriver.FindElement(By.CssSelector("input[formcontrolname='numberofSharesNew']"));
            uiSameIndividualVotingShare.SendKeys(votingShares);

            // enter the shareholder email
            NgWebElement uiSameIndividualEmail3 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
            uiSameIndividualEmail3.SendKeys(sameIndividualEmail2);

            // enter the shareholder DOB
            NgWebElement uiCalendarS1 = ngDriver.FindElement(By.CssSelector("input[formcontrolname='dateofBirthNew']"));
            uiCalendarS1.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton3 = ngDriver.FindElement(By.CssSelector(".fa-save span"));
            uiConfirmButton3.Click();
        }


        [And(@"I delete only the director record")]
        public void DeleteDirectorRecord()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // click on the delete button for leader > director record    
            NgWebElement uiEditInfoButton = ngDriver.FindElement(By.XPath("//app-associate-list/div/table/tr[1]/td[7]/i[2]/span"));
            uiEditInfoButton.Click();

            // click on the Submit Org Info button
            NgWebElement uiSubmitOrgInfoButton = ngDriver.FindElement(By.CssSelector("app-application-licensee-changes button.btn-primary"));
            uiSubmitOrgInfoButton.Click();
        }


        [And(@"only the shareholder record is displayed")]
        public void ShareholderRecordDisplayed()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // check that the director email is not displayed to confirm deletion
            Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'same@individual.com'))]")).Displayed);

            // check that the shareholder email is displayed to confirm remains
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'same@individual2.com')]")).Displayed);
        }


        [And(@"I modify only the director record")]
        public void ModifyOnlyDirectorRecord()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // create new name for same individual
            string newFirstName = "NewFirstName";
            string newLastName = "NewLastName";

            // click on the edit button for leader 
            NgWebElement uiEditInfoButton = ngDriver.FindElement(By.XPath("//i/span"));
            uiEditInfoButton.Click();

            // enter the new first name 
            NgWebElement uiNewFirstName = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiNewFirstName.Clear();
            uiNewFirstName.SendKeys(newFirstName);

            // enter the new last name 
            NgWebElement uiNewLastName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiNewLastName.Clear();
            uiNewLastName.SendKeys(newLastName);

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.XPath("//i/span"));
            uiConfirmButton.Click();

            // click on the Submit Org Info button
            NgWebElement uiSubmitOrgInfoButton = ngDriver.FindElement(By.CssSelector("app-application-licensee-changes button.btn-primary"));
            uiSubmitOrgInfoButton.Click();
        }


        [And(@"the director and shareholder name are identical")]
        public void DirectorShareholderNameIdentical()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // check that the director first name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[5]/section/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'NewFirstName')]")).Displayed);

            // check that the director last name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[5]/section/app-associate-list/div/table/tr[1]/td[2]/span[contains(.,'NewLastName')]")).Displayed);

            // check that the shareholder first name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[6]/section[1]/app-associate-list/div/table/tr/td[1]/span[contains(.,'NewFirstName')]")).Displayed);

            // check that the shareholder last name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[6]/section[1]/app-associate-list/div/table/tr/td[2]/span[contains(.,'NewLastName')]")).Displayed);
        }


        [And(@"the organization structure page is displayed")]
        public void OrgStructureDisplays()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // confirm that the page loads
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Legal Entity Structure')]")).Displayed);
        }


        [And(@"I add a business shareholder with the same individual as a director and a shareholder")]
        public void BusinessShareholderSameDirShare()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // create the business shareholder data
            string businessName = "Business Shareholder";
            string businessVotingShares = "50";
            string businessEmail = "business@shareholder.com";

            // open business shareholder form    
            NgWebElement uiOpenShareBiz = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] button"));
            uiOpenShareBiz.Click();

            // enter business shareholder name
            NgWebElement uiShareFirstBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='businessNameNew']"));
            uiShareFirstBiz.SendKeys(businessName);

            // enter business shareholder voting shares
            NgWebElement uiShareVotesBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='numberofSharesNew']"));
            uiShareVotesBiz.SendKeys(businessVotingShares);

            // select the business shareholder type
            NgWebElement uiShareBizType = ngDriver.FindElement(By.CssSelector("[formcontrolname='businessType'] option[value='PrivateCorporation']"));
            uiShareBizType.Click();

            // enter business shareholder email
            NgWebElement uiShareEmailBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] input[formControlName='emailNew']"));
            uiShareEmailBiz.SendKeys(businessEmail);

            // select the business shareholder confirm button
            NgWebElement uiShareBizConfirmButton = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] .fa-save.ng-star-inserted span"));
            uiShareBizConfirmButton.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder 
            string noticeOfArticlesBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadNoticeofArticlesBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[15]"));
            uiUploadNoticeofArticlesBiz.SendKeys(noticeOfArticlesBiz);

            // upload a central securities register document for business shareholder 
            string centralSecuritiesRegisterBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uiUploadCentralSecRegBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[18]"));
            uiUploadCentralSecRegBiz.SendKeys(centralSecuritiesRegisterBiz);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uiUploadSpecialRightsResBiz = ngDriver.FindElement(By.XPath("(//input[@type='file'])[21]"));
            uiUploadSpecialRightsResBiz.SendKeys(specialRightsRestrictionsBiz);

            /********** Business Shareholder - Leader #1 **********/

            // create business shareholder #1 leader data
            string leaderlFirstNameBiz = "Same2";
            string leaderLastNameBiz = "Individual2";
            string leaderTitleBiz = "Event Planner";
            string leaderEmailBiz = "sameindividual@privatecorp.com";

            // open business shareholder > leader form #1
            NgWebElement uiOpenLeaderFormBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] app-associate-list[addlabel='Add Key Personnel'][changetypesuffix='Leadership'] button"));
            uiOpenLeaderFormBiz.Click();

            // enter business shareholder > leader #1 first name
            NgWebElement uiLeaderFirstBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstBiz.SendKeys(leaderlFirstNameBiz);

            // enter business shareholder > leader #1 last name
            NgWebElement uiLeaderLastBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='lastNameNew']"));
            uiLeaderLastBiz.SendKeys(leaderLastNameBiz);

            // select business shareholder > leader #1 role
            NgWebElement uiLeaderRoleBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [addlabel='Add Key Personnel'][changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiLeaderRoleBiz.Click();

            // enter business shareholder > leader #1 title
            NgWebElement uiLeaderTitleBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='titleNew']"));
            uiLeaderTitleBiz.SendKeys(leaderTitleBiz);

            // enter business shareholder > leader #1 email 
            NgWebElement uiLeaderEmailBiz = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='emailNew']"));
            uiLeaderEmailBiz.SendKeys(leaderEmailBiz);

            // enter business shareholder > leader #1 DOB
            NgWebElement uiLeaderDOB1Biz1 = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='BusinessShareholder'] app-associate-list[changetypesuffix='Leadership'] input[formControlName='dateofBirthNew']"));
            uiLeaderDOB1Biz1.Click();

            // select the date
            SharedCalendarDate();

            /********** Business Shareholder - Individual Shareholder #1 **********/

            // open business shareholder #1 > individual shareholder #1 form
            NgWebElement uiOpenIndyShareBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [addlabel='Add Individual Shareholder'] button"));
            uiOpenIndyShareBiz.Click();

            // enter business shareholder #1 > individual shareholder #1 first name
            NgWebElement uiIndyShareFirstBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz.SendKeys(leaderlFirstNameBiz);

            // enter business shareholder #1 > individual shareholder #1 last name
            NgWebElement uiIndyShareLastBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz.SendKeys(leaderLastNameBiz);

            // enter business shareholder #1 > individual shareholder #1 number of voting shares
            string shareholderVotingSharesBiz = "10";
            NgWebElement uiIndyShareVotesBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz.SendKeys(shareholderVotingSharesBiz);

            // enter business shareholder #1 > individual shareholder #1 email
            NgWebElement uiIndyShareEmailBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz.SendKeys(leaderEmailBiz);

            // enter business shareholder #1 > individual shareholder #1 DOB
            NgWebElement uiCalendarIndyS1Biz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz.Click();

            // select the date
            SharedCalendarDate();
        }


        [And(@"I add a second individual as a director and a shareholder to the business shareholder")]
        public void BusinessShareholderSameDirShare2()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // create business shareholder leader #2 data
            string leaderFirstNameBiz = "Same3";
            string leaderLastNameBiz = "Individual3";
            string leaderTitleBiz = "Event Planner";
            string leaderEmailBiz = "sameindividual2@privatecorp.com";

            // open business shareholder > leader 2 form
            NgWebElement uiLeaderShareBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix= 'BusinessShareholder'] [changetypesuffix= 'Leadership'] button"));
            uiLeaderShareBiz.Click();

            // enter business shareholder > leader #2 first name
            NgWebElement uiLeaderFirstBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstBiz.SendKeys(leaderFirstNameBiz);

            // enter business shareholder > leader #2 last name
            NgWebElement uiLeaderLastBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiLeaderLastBiz.SendKeys(leaderLastNameBiz);

            // select business shareholder > leader #2 role
            NgWebElement uiLeaderRoleBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiLeaderRoleBiz.Click();

            // enter business shareholder > leader #2 title
            NgWebElement uiLeaderTitleBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiLeaderTitleBiz.SendKeys(leaderTitleBiz);

            // enter business shareholder > leader #2 email 
            NgWebElement uiLeaderEmailBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiLeaderEmailBiz.SendKeys(leaderEmailBiz);

            // enter business shareholder > leader #2 DOB
            NgWebElement uiLeaderDOB1Biz1 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiLeaderDOB1Biz1.Click();

            // select the date
            SharedCalendarDate();

            // open business shareholder > individual shareholder #2 form
            NgWebElement uiOpenIndyShareBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
            uiOpenIndyShareBiz.Click();

            // enter business shareholder > individual shareholder #2 first name
            NgWebElement uiIndyShareFirstBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz.SendKeys(leaderFirstNameBiz);

            // enter business shareholder > individual shareholder #2 last name
            NgWebElement uiIndyShareLastBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz.SendKeys(leaderLastNameBiz);

            // enter business shareholder > individual shareholder #2 number of voting shares
            string shareholderVotingSharesBiz = "10";
            NgWebElement uiIndyShareVotesBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz.SendKeys(shareholderVotingSharesBiz);

            // enter business shareholder > individual shareholder #2 email
            NgWebElement uiIndyShareEmailBiz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz.SendKeys(leaderEmailBiz);

            // enter business shareholder > individual shareholder #2 DOB
            NgWebElement uiCalendarIndyS1Biz = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz.Click();

            // select the date
            SharedCalendarDate();
        }


        [And(@"the org structure is correct")]
        public void OrgStructureCorrect()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // confirm that first individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same1')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/span[contains(.,'Same1')]")).Displayed);

            // confirm that second individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same2')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same2')]")).Displayed);

            // confirm that third individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[2]/td[1]/span[contains(.,'Same3')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[2]/td[1]/span[contains(.,'Same3')]")).Displayed);
        }


        [And(@"the org structure is correct after payment")]
        public void OrgStructureCorrectPayment()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // confirm that first individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same1')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/span[contains(.,'Same1')]")).Displayed);

            // confirm that second individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same2')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[1]/td[1]/span[contains(.,'Same2')]")).Displayed);

            // confirm that third individual is in correct positions
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[2]/td[1]/span[contains(.,'Same3')]")).Displayed);
            Assert.True(ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-0']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[2]/td[1]/span[contains(.,'Same3')]")).Displayed);
        }


        [And(@"I remove the latest director and shareholder")]
        public void RemoveLatestDirectorShareholder()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // delete the most recent director
            NgWebElement uiRemoveDirector = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[2]/td[7]/i[2]/span"));
            uiRemoveDirector.Click();

            // delete the most recent shareholder
            NgWebElement uiRemoveShareholder = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[2]/td[6]/i[2]/span"));
            uiRemoveShareholder.Click();
        }


        [And(@"I remove the latest director after saving")]
        public void RemoveLatestDirectorAfterSave()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // delete the most recent director
            NgWebElement uiRemoveDirector = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-2']/div/section/app-org-structure/div/div[4]/section/app-associate-list/div/table/tr[2]/td[7]/i[2]/span"));
            uiRemoveDirector.Click();
        }


        [And(@"I remove the latest shareholder after saving")]
        public void RemoveLatestShareholderAfterSave()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // delete the most recent shareholder 
            NgWebElement uiRemoveShareholder = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-3']/div/section/app-org-structure/div/div[5]/section[1]/app-associate-list/div/table/tr[2]/td[6]/i[2]/span"));
            uiRemoveShareholder.Click();
        }


        [And(@"the latest director and shareholder is removed")]
        public void LatestDirectorShareholderRemoved()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // confirm that the most recent director and shareholder not present
            Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'Same3'))]")).Displayed);
        }


        [And(@"I remove the business shareholder")]
        public void RemoveBusinessShareholder()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // delete the business shareholder
            NgWebElement uiRemoveBizShareholder = ngDriver.FindElement(By.XPath("//app-org-structure/div/div[5]/section[2]/app-associate-list/div/table/tr[1]/td[5]/i[2]/span"));
            uiRemoveBizShareholder.Click();
        }


        [And(@"the business shareholder is removed")]
        public void BusinessShareholderRemoved()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            // confirm that the business shareholder not present	
            Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'business@shareholder.com'))]")).Displayed);
        }


        [And(@"the saved org structure is present")]
        public void SaveOrgStructurePresent()
        {
            // TODO
        }


        [And(@"I add in business shareholders of different business types")]
        public void MixedBusinessShareholders()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            /********** Business Shareholder - Public Corporation **********/

            // create public corporation test data
            string businessNamePublicCorp = "Public Corporation";
            string sharesPublicCorp = "10";
            string emailAddressPublicCorp = "public@corporation.com";

            // click on the Add Business Shareholder button
            NgWebElement uiAddPublicCorporationRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[6]/section[2]/app-associate-list/div/button"));
            uiAddPublicCorporationRow.Click();

            // add the public corporation business name
            NgWebElement uiAddPublicCorporationBizName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='businessNameNew']"));
            uiAddPublicCorporationBizName.SendKeys(businessNamePublicCorp);

            // add the public corporation number of shares
            NgWebElement uiAddPublicCorporationShares = ngDriver.FindElement(By.CssSelector("input[formcontrolname='numberofSharesNew']"));
            uiAddPublicCorporationShares.SendKeys(sharesPublicCorp);

            // select the public corporation organization type
            NgWebElement uiAddOrganizationTypePublicCorp = ngDriver.FindElement(By.CssSelector("[formcontrolname='businessType'] option[value='PublicCorporation']"));
            uiAddOrganizationTypePublicCorp.Click();

            // add the public corporation email address
            NgWebElement uiAddEmailAddressPublicCorp = ngDriver.FindElement(By.CssSelector("input[formcontrolname='emailNew']"));
            uiAddEmailAddressPublicCorp.SendKeys(emailAddressPublicCorp);

            // click on the public corporation Confirm button
            NgWebElement uiConfirmButtonPublicCorp = ngDriver.FindElement(By.CssSelector(".fa-save span"));
            uiConfirmButtonPublicCorp.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document
            string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[24]"));
            uiUploadSignage.SendKeys(noticeOfArticles);

            // click on the Add Leader button
            NgWebElement uiAddPublicCorporationLeader = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] #cdk-accordion-child-1 [changetypesuffix='Leadership'] button"));
            uiAddPublicCorporationLeader.Click();

            // create public corp leader data
            string firstName = "LeaderPubCorp";
            string lastName = "Public Corporation";
            string title = "CTO";
            string email = "leader@pubcorp.com";

            // enter the leader first name 
            NgWebElement uiLeaderFirstName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstName.SendKeys(firstName);

            // enter the leader last name 
            NgWebElement uiLeaderLastName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiLeaderLastName.SendKeys(lastName);

            // click the leader checkbox
            NgWebElement uiLeaderRole = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isOfficerNew']"));
            uiLeaderRole.Click();

            // enter the leader title
            NgWebElement uiLeaderTitle = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiLeaderTitle.SendKeys(title);

            // enter the leader email 
            NgWebElement uiLeaderEmail = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiLeaderEmail.SendKeys(email);

            // select the leader DOB
            NgWebElement uiOpenLeaderDOB = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiOpenLeaderDOB.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] .fa-save span"));
            uiConfirmButton.Click();

            /********** Business Shareholder - Sole Proprietorship **********/

            // create sole proprietorship test data
            string businessNameSoleProprietorship = "Sole Proprietorship";
            string sharesSoleProprietorship = "10";
            string emailAddressSoleProprietorship = "sole@proprietorship.com";

            // click on the sole proprietorship Add Business Shareholder button
            NgWebElement uiAddSoleProprietorshipRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[6]/section[2]/app-associate-list/div/button"));
            uiAddSoleProprietorshipRow.Click();

            // add the sole proprietorship business name
            NgWebElement uiAddSoleProprietorshipBizName = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiAddSoleProprietorshipBizName.SendKeys(businessNameSoleProprietorship);

            // add the sole proprietorship number of shares
            NgWebElement uiAddSoleProprietorshipShares = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiAddSoleProprietorshipShares.SendKeys(sharesSoleProprietorship);

            // select the sole proprietorship organization type
            NgWebElement uiAddOrganizationTypeSoleProprietorship = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='SoleProprietorship']"));
            uiAddOrganizationTypeSoleProprietorship.Click();

            // add the sole proprietorship email address
            NgWebElement uiAddEmailAddressSoleProprietorship = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiAddEmailAddressSoleProprietorship.SendKeys(emailAddressSoleProprietorship);

            // click on the sole proprietorship Confirm button
            NgWebElement uiConfirmButtonSoleProprietorship = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiConfirmButtonSoleProprietorship.Click();

            // open the sole proprietorship leader row
            NgWebElement uiAddSoleProprietorshipLeaderRow = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'][addlabel='Add Leader'] button.btn-secondary"));
            uiAddSoleProprietorshipLeaderRow.Click();

            // create leader data
            string firstNameLeader = "LeaderSoleProp";
            string lastNameLeader = "LastName";
            string leaderEmail = "leader@soleprop.com";

            // add the leader first name
            NgWebElement uiAddSoleProprietorshipFirstName = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leader'][changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiAddSoleProprietorshipFirstName.SendKeys(firstNameLeader);

            // add the leader last name
            NgWebElement uiAddSoleProprietorshipLastName = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leader'][changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiAddSoleProprietorshipLastName.SendKeys(lastNameLeader);

            // add the leader email
            NgWebElement uiAddSoleProprietorshipEmail = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leader'][changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiAddSoleProprietorshipEmail.SendKeys(leaderEmail);

            // add the leader DOB
            NgWebElement uiAddSoleProprietorshipDOB = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leader'][changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiAddSoleProprietorshipDOB.Click();

            SharedCalendarDate();

            // click on leader confirm button
            NgWebElement uiConfirmSoleProprietorship = ngDriver.FindElement(By.CssSelector("[addlabel='Add Leader'][changetypesuffix='Leadership'] .fa-save span"));
            uiConfirmSoleProprietorship.Click();

            /********** Business Shareholder - Society **********/

            // create society test data
            string businessNameSociety = "Society";
            string sharesSociety = "10";
            string emailAddressSociety = "society@test.com";

            // click on the society Add Business Shareholder button
            NgWebElement uiAddSocietyRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[6]/section[2]/app-associate-list/div/button"));
            uiAddSocietyRow.Click();

            // add the society business name
            NgWebElement uiAddSocietyBizName = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiAddSocietyBizName.SendKeys(businessNameSociety);

            // add the society number of shares
            NgWebElement uiAddSocietyShares = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiAddSocietyShares.SendKeys(sharesSociety);

            // select the society organization type
            NgWebElement uiAddOrganizationTypeSociety = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='Society']"));
            uiAddOrganizationTypeSociety.Click();

            // add the society email address
            NgWebElement uiAddEmailAddressSociety = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiAddEmailAddressSociety.SendKeys(emailAddressSociety);

            // click on the society Confirm button
            NgWebElement uiConfirmButtonSociety = ngDriver.FindElement(By.CssSelector("[addlabel='Add Business Shareholder'][changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiConfirmButtonSociety.Click();

            // create society data
            string membershipFee = "1000";
            string memberCount = "100";
            string directorFirstName = "DirectorSociety";
            string directorLastName = "Society";
            string directorTitle = "CFO";
            string directorEmail = "cfo@society.com";

            // add the society's annual membership fee
            NgWebElement uiAddMembershipFee = ngDriver.FindElement(By.CssSelector("input[formcontrolname='annualMembershipFee']"));
            uiAddMembershipFee.SendKeys(membershipFee);

            // add the society's number of members
            NgWebElement uiAddMembershipCount = ngDriver.FindElement(By.CssSelector("input[formcontrolname='numberOfMembers']"));
            uiAddMembershipCount.SendKeys(memberCount);

            // click on the director/officer row
            NgWebElement uiOpenDirectorRow = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] button"));
            uiOpenDirectorRow.Click();

            // add the director first name
            NgWebElement uiAddDirectorFirst = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='firstNameNew']"));
            uiAddDirectorFirst.SendKeys(directorFirstName);

            // add the director last name
            NgWebElement uiAddDirectorLast = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='lastNameNew']"));
            uiAddDirectorLast.SendKeys(directorLastName);

            // select the director position
            NgWebElement uiAddDirectorPosition = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='isOfficerNew']"));
            uiAddDirectorPosition.Click();

            // add the director title
            NgWebElement uiAddDirectorTitle = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='titleNew']"));
            uiAddDirectorTitle.SendKeys(directorTitle);

            // add the director email
            NgWebElement uiAddDirectorEmail = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='emailNew']"));
            uiAddDirectorEmail.SendKeys(directorEmail);

            // add the director DOB
            NgWebElement uiAddDirectorDOB = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] input[formcontrolname='dateofBirthNew']"));
            uiAddDirectorDOB.Click();

            SharedCalendarDate();

            // click on director confirm button
            NgWebElement uiConfirmDirector = ngDriver.FindElement(By.CssSelector("[addlabel='Add Director or Officer'] .fa-save span"));
            uiConfirmDirector.Click();

            /********** Business Shareholder - Trust **********/

            // create trust test data
            string businessNameTrust = "Trust";
            string sharesTrust = "10";
            string emailAddressTrust = "trust@test.com";

            // click on the trust Add Business Shareholder button
            NgWebElement uiAddTrustRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[6]/section[2]/app-associate-list/div/button"));
            uiAddTrustRow.Click();

            // add the trust business name
            NgWebElement uiAddTrustBizName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiAddTrustBizName.SendKeys(businessNameTrust);

            // add the trust number of shares
            NgWebElement uiAddTrustShares = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiAddTrustShares.SendKeys(sharesTrust);

            // select the trust organization type
            NgWebElement uiAddOrganizationTypeTrust = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='Trust']"));
            uiAddOrganizationTypeTrust.Click();

            // add the trust email address
            NgWebElement uiAddEmailAddressTrust = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiAddEmailAddressTrust.SendKeys(emailAddressTrust);

            // click on the trust Confirm button
            NgWebElement uiConfirmButtonTrust = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiConfirmButtonTrust.Click();

            // click on add trustee button
            NgWebElement uiAddTrustee = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] button"));
            uiAddTrustee.Click();

            // create trustee test data
            string firstNameTrustee = "TrusteeTrust";
            string lastNameTrustee = "Trust";
            string emailAddressTrustee = "trustee@test.com";

            // add trustee first name
            NgWebElement uiAddTrusteeFirst = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] input[formcontrolname='firstNameNew']"));
            uiAddTrusteeFirst.SendKeys(firstNameTrustee);

            // add trustee last name
            NgWebElement uiAddTrusteeLast = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] input[formcontrolname='lastNameNew']"));
            uiAddTrusteeLast.SendKeys(lastNameTrustee);

            // add trustee email
            NgWebElement uiAddTrusteeEmail = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] input[formcontrolname='emailNew']"));
            uiAddTrusteeEmail.SendKeys(emailAddressTrustee);

            // add trustee DOB
            NgWebElement uiAddTrusteeDOB = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] input[formcontrolname='dateofBirthNew']"));
            uiAddTrusteeDOB.Click();

            SharedCalendarDate();

            // click on trustee confirm button
            NgWebElement uiConfirmTrustee = ngDriver.FindElement(By.CssSelector("[addlabel='Add Trustee'] .fa-save span"));
            uiConfirmTrustee.Click();

            /********** Business Shareholder - Partnership **********

            // create partnership test data
            string businessNamePartnership = "Partnership";
            string sharesPartnership = "10";
            string emailAddressPartnership = "partnership@test.com";

            // click on the partnership Add Business Shareholder button
            NgWebElement uiAddPartnershipRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[6]/section[2]/app-associate-list/div/button"));
            uiAddPartnershipRow.Click();

            // add the partnership business name
            NgWebElement uiAddPartnershipBizName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiAddPartnershipBizName.SendKeys(businessNamePartnership);

            // add the partnership number of shares
            NgWebElement uiAddPartnershipShares = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiAddPartnershipShares.SendKeys(sharesPartnership);

            // select the partnership organization type
            NgWebElement uiAddOrganizationTypePartnership = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='Partnership']"));
            uiAddOrganizationTypePartnership.Click();

            // add the partnership email address
            NgWebElement uiAddEmailAddressPartnership = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiAddEmailAddressPartnership.SendKeys(emailAddressPartnership);

            // click on the partnership Confirm button
            NgWebElement uiConfirmButtonPartnership = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiConfirmButtonPartnership.Click();

            // click on the individual partner row
            NgWebElement uiAddPartnerRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-5']/div/section/app-org-structure/div/div[3]/section[1]/app-associate-list/div/button"));
            uiAddPartnerRow.Click();

            // create the individual partner data
            string firstNamePartner = "Individual";
            string lastNamePartner = "Partner";
            string percentage = "50";
            string emailPartner = "individual@partner.com";

            // add the individual partner first name
            NgWebElement uiFirstNamePartner = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiFirstNamePartner.SendKeys(firstNamePartner);

            // add the individual partner last name
            NgWebElement uiLastNamePartner = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiLastNamePartner.SendKeys(lastNamePartner);

            // add the individual partner percentage
            NgWebElement uiPartnerPercentage = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] input[formcontrolname='interestPercentageNew']"));
            uiPartnerPercentage.SendKeys(percentage);

            // add the individual partner email address
            NgWebElement uiEmailPartner = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiEmailPartner.SendKeys(emailPartner);

            // add the individual partner DOB
            NgWebElement uiPartnerDOB = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiPartnerDOB.Click();

            SharedCalendarDate();

            // click on the individual partner confirm button
            NgWebElement uiPartnerConfirm = ngDriver.FindElement(By.CssSelector("[addlabel='Add Individual Partner'][changetypesuffix='IndividualShareholder'] .fa-save span"));
            uiPartnerConfirm.Click();

            // upload partnership agreement
            string partnershipAgreement = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "partnership_agreement.pdf");
            NgWebElement uiUploadPartner = ngDriver.FindElement(By.XPath("//*[@id='cdk-accordion-child-5']/div/section/app-org-structure/div/div[2]/section/app-file-uploader/div/ngx-file-drop/div/div/input"));
            uiUploadPartner.SendKeys(partnershipAgreement);
            */
        }


        [And(@"the mixed business shareholder org structure is correct")]
        public void ReviewMixedBusinessShareholdersOrgStructure()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            //TODO
        }


        [And(@"I enter business shareholders of different business types to be saved for later")]
        public void SaveForLaterMixedBusinessShareholders()
        {
            /* 
            Page Title: [client name] Legal Entity Structure
            */

            /********** Business Shareholder - Public Corporation **********/

            // create public corporation test data
            string businessNamePublicCorp = "Public Corporation";
            string sharesPublicCorp = "10";
            string emailAddressPublicCorp = "public@corporation.com";

            // click on the Add Business Shareholder button
            NgWebElement uiAddPublicCorporationRow = ngDriver.FindElement(By.XPath("//*[@id='cdk-step-content-0-1']/app-application-licensee-changes/div/div[2]/section[1]/app-org-structure/div/div[6]/section[2]/app-associate-list/div/button"));
            uiAddPublicCorporationRow.Click();

            // add the public corporation business name
            NgWebElement uiAddPublicCorporationBizName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiAddPublicCorporationBizName.SendKeys(businessNamePublicCorp);

            // add the public corporation number of shares
            NgWebElement uiAddPublicCorporationShares = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiAddPublicCorporationShares.SendKeys(sharesPublicCorp);

            // select the public corporation organization type
            NgWebElement uiAddOrganizationTypePublicCorp = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='PublicCorporation']"));
            uiAddOrganizationTypePublicCorp.Click();

            // add the public corporation email address
            NgWebElement uiAddEmailAddressPublicCorp = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiAddEmailAddressPublicCorp.SendKeys(emailAddressPublicCorp);

            // click on the public corporation Confirm button
            NgWebElement uiConfirmButtonPublicCorp = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiConfirmButtonPublicCorp.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document
            string noticeOfArticles = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadSignage = ngDriver.FindElement(By.XPath("(//input[@type='file'])[21]"));
            uiUploadSignage.SendKeys(noticeOfArticles);

            // click on the Add Leader button
            NgWebElement uiAddPublicCorporationLeader = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] #cdk-accordion-child-1 [changetypesuffix='Leadership'] button"));
            uiAddPublicCorporationLeader.Click();

            // create public corp leader data
            string firstName = "LeaderPubCorp";
            string lastName = "Public Corporation";
            string title = "CTO";
            string email = "leader@pubcorp.com";

            // enter the leader first name 
            NgWebElement uiLeaderFirstName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstName.SendKeys(firstName);

            // enter the leader last name 
            NgWebElement uiLeaderLastName = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiLeaderLastName.SendKeys(lastName);

            // click the leader checkbox
            NgWebElement uiLeaderRole = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isOfficerNew']"));
            uiLeaderRole.Click();

            // enter the leader title
            NgWebElement uiLeaderTitle = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiLeaderTitle.SendKeys(title);

            // enter the leader email 
            NgWebElement uiLeaderEmail = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiLeaderEmail.SendKeys(email);

            // select the leader DOB
            NgWebElement uiOpenLeaderDOB = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiOpenLeaderDOB.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] .fa-save span"));
            uiConfirmButton.Click();
        }


        [And(@"I confirm that no duplicates are shown in the org structure")]
        public void CheckOrgStructureDuplicates()
        {
            // check that Leader0 only displays once
            var leader0Elements = ngDriver.FindElements(By.XPath("//body[contains(.,'leader0@privatecorp.com')]")).Count;
            Assert.True(leader0Elements == 1);

            // check that IndividualShareholder0 only displays once
            var indyShareholder0Elements = ngDriver.FindElements(By.XPath("//body[contains(.,'individualshareholder0@privatecorp.com')]")).Count;
            Assert.True(indyShareholder0Elements == 1);

            // check that Business Shareholder 1 only displays once
            var bizShareholder1Elements = ngDriver.FindElements(By.XPath("//body[contains(.,'business@shareholder1.com')]")).Count;
            Assert.True(bizShareholder1Elements == 1);

            // check that Leader1 only displays once
            var leader1Elements = ngDriver.FindElements(By.XPath("//body[contains(.,'leader1bizshareholder@privatecorp.com')]")).Count;
            Assert.True(leader1Elements == 1);

            // check that IndividualShareholder1 only displays once
            var indyShareholder1Elements = ngDriver.FindElements(By.XPath("//body[contains(.,'individualshareholder1bizshareholder@privatecorp.com')]")).Count;
            Assert.True(indyShareholder1Elements == 1);
        }


        [And(@"I add in multiple nested business shareholders")]
        public void AddMultipleBusinessShareholders()
        {
            // add in an additional four nested business shareholders
            BusinessShareholder2();
            BusinessShareholder3();
            BusinessShareholder4();
            BusinessShareholder5();
        }

        public void BusinessShareholder2()
        {
            /********** Business Shareholder #2 **********/

            // create the business shareholder #2 data
            string businessName2 = "Business Shareholder 2";
            string businessVotingShares2 = "100";
            string businessEmail2 = "businessshareholder2@email.com";

            // open business shareholder #2 form
            NgWebElement uiOpenShareBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] button"));
            uiOpenShareBiz2.Click();

            // enter business sharedholder #2 name
            NgWebElement uiShareFirstBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiShareFirstBiz2.SendKeys(businessName2);

            // enter business shareholder #2 voting shares
            NgWebElement uiShareVotesBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiShareVotesBiz2.SendKeys(businessVotingShares2);

            // select business shareholder #2 business type (private corporation) from dropdown
            NgWebElement uiShareBizType2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='PrivateCorporation']"));
            uiShareBizType2.Click();

            // enter business shareholder #2 email
            NgWebElement uiShareEmailBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiShareEmailBiz2.SendKeys(businessEmail2);

            // select the business shareholder #2 confirm button
            NgWebElement uiShareBizConfirmButton2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiShareBizConfirmButton2.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadNoticeofArticlesBiz2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[21]"));
            uiUploadNoticeofArticlesBiz2.SendKeys(noticeOfArticlesBiz2);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uiUploadCentralSecRegBiz2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[24]"));
            uiUploadCentralSecRegBiz2.SendKeys(centralSecuritiesRegisterBiz2);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uiUploadSpecialRightsResBiz2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[27]"));
            uiUploadSpecialRightsResBiz2.SendKeys(specialRightsRestrictionsBiz2);

            /********** Business Shareholder #2 - Leader **********/

            // create business shareholder #2 leader data
            string leaderFirstNameBiz2 = "LeaderBiz2First";
            string leaderLastNameBiz2 = "LeaderBiz2Last";
            string leaderTitleBiz2 = "LeaderBiz2Title";
            string leaderEmailBiz2 = "leader@biz2.com";

            // open business shareholder #2 > leader form 
            NgWebElement uiOpenLeaderFormBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] button"));
            uiOpenLeaderFormBiz2.Click();

            // enter business shareholder #2 > leader first name
            NgWebElement uiLeaderFirstBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstBiz2.SendKeys(leaderFirstNameBiz2);

            // enter business shareholder #2 > leader last name
            NgWebElement uiLeaderLastBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiLeaderLastBiz2.SendKeys(leaderLastNameBiz2);

            // select business shareholder #2 > leader role using checkbox
            NgWebElement uiLeaderRoleBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiLeaderRoleBiz2.Click();

            // enter business shareholder #2 > leader title
            NgWebElement uiLeaderTitleBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiLeaderTitleBiz2.SendKeys(leaderTitleBiz2);

            // enter business shareholder #2 > leader email
            NgWebElement uiLeaderEmailBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiLeaderEmailBiz2.SendKeys(leaderEmailBiz2);

            // enter business shareholder #2 > leader DOB
            NgWebElement uiLeaderDOB1Biz12 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiLeaderDOB1Biz12.Click();

            SharedCalendarDate();

            /********** Business Shareholder #2 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz2 = "IndividualShareholderBiz2First";
            string shareholderLastNameBiz2 = "IndividualShareholderBiz2Last";
            string shareholderVotingSharesBiz2 = "1800";
            string shareholderEmailBiz2 = "individualshareholder@biz2.com";

            // open business shareholder #2 > individual shareholder form
            NgWebElement uiOpenIndyShareBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
            uiOpenIndyShareBiz2.Click();

            // enter business shareholder #2 > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz2.SendKeys(shareholderFirstNameBiz2);

            // enter business shareholder #2 > individual shareholder last name
            NgWebElement uiIndyShareLastBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz2.SendKeys(shareholderLastNameBiz2);

            // enter business shareholder #2 > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz2.SendKeys(shareholderVotingSharesBiz2);

            // enter business shareholder > individual shareholder email
            NgWebElement uiIndyShareEmailBiz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz2.SendKeys(shareholderEmailBiz2);

            // enter business shareholder > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz2 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz2.Click();

            SharedCalendarDate();
        }

        public void BusinessShareholder3()
        {
            /********** Business Shareholder #3 **********/

            // create the business shareholder data
            string businessName3 = "Business Shareholder 3";
            string businessVotingShares3 = "3";
            string businessEmail3 = "businessshareholder3@email.com";

            // open business shareholder #3 form
            NgWebElement uiOpenShareBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] button"));
            uiOpenShareBiz3.Click();

            // enter business shareholder #3 business name
            NgWebElement uiShareFirstBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiShareFirstBiz3.SendKeys(businessName3);

            // enter business shareholder #3 voting shares
            NgWebElement uiShareVotesBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiShareVotesBiz3.SendKeys(businessVotingShares3);

            // select the business shareholder #3 business type using dropdown
            NgWebElement uiShareBizType3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='PrivateCorporation']"));
            uiShareBizType3.Click();

            // enter business shareholder #3 email
            NgWebElement uiShareEmailBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiShareEmailBiz3.SendKeys(businessEmail3);

            // select the business shareholder #3 confirm button
            NgWebElement uiShareBizConfirmButton3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiShareBizConfirmButton3.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz3 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadNoticeofArticlesBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[30]"));
            uiUploadNoticeofArticlesBiz3.SendKeys(noticeOfArticlesBiz3);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz3 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uiUploadCentralSecRegBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[33]"));
            uiUploadCentralSecRegBiz3.SendKeys(centralSecuritiesRegisterBiz3);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz3 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uiUploadSpecialRightsResBiz3 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[36]"));
            uiUploadSpecialRightsResBiz3.SendKeys(specialRightsRestrictionsBiz3);

            /********** Business Shareholder #3 - Leader **********/

            // create business shareholder leader data
            string leaderFirstNameBiz3 = "LeaderBiz3First";
            string leaderLastNameBiz3 = "LeaderBiz3Last";
            string leaderTitleBiz3 = "LeaderBiz3Title";
            string leaderEmailBiz3 = "leader@biz3.com";

            // open business shareholder #3 > leader form
            NgWebElement uiOpenLeaderFormBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] button"));
            uiOpenLeaderFormBiz3.Click();

            // enter business shareholder #3 > leader first name
            NgWebElement uiLeaderFirstBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstBiz3.SendKeys(leaderFirstNameBiz3);

            // enter business shareholder #3 > leader last name
            NgWebElement uiLeaderLastBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiLeaderLastBiz3.SendKeys(leaderLastNameBiz3);

            // select business shareholder #3 > leader role using checkbox
            NgWebElement uiLeaderRoleBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiLeaderRoleBiz3.Click();

            // enter business shareholder #3 > leader title
            NgWebElement uiLeaderTitleBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiLeaderTitleBiz3.SendKeys(leaderTitleBiz3);

            // enter business shareholder #3 > leader email
            NgWebElement uiLeaderEmailBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiLeaderEmailBiz3.SendKeys(leaderEmailBiz3);

            // enter business shareholder #3 > leader DOB
            NgWebElement uiLeaderDOB1Biz13 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiLeaderDOB1Biz13.Click();

            SharedCalendarDate();

            /********** Business Shareholder #3 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz3 = "IndividualShareholderBiz3First";
            string shareholderLastNameBiz3 = "IndividualShareholderBiz3Last";
            string shareholderVotingSharesBiz3 = "1000";
            string shareholderEmailBiz3 = "individualshareholder@biz3.com";

            // open business shareholder #3 > individual shareholder form
            NgWebElement uiOpenIndyShareBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
            uiOpenIndyShareBiz3.Click();

            // enter business shareholder #3 > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz3.SendKeys(shareholderFirstNameBiz3);

            // enter business shareholder #3 > individual shareholder last name
            NgWebElement uiIndyShareLastBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz3.SendKeys(shareholderLastNameBiz3);

            // enter business shareholder #3 > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz3.SendKeys(shareholderVotingSharesBiz3);

            // enter business shareholder #3 > individual shareholder email
            NgWebElement uiIndyShareEmailBiz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz3.SendKeys(shareholderEmailBiz3);

            // enter business shareholder #3 > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz3 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz3.Click();

            SharedCalendarDate();
        }

        public void BusinessShareholder4()
        {
            /********** Business Shareholder #4 **********/

            // create the business shareholder data
            string businessName4 = "Business Shareholder 4";
            string businessVotingShares4 = "2";
            string businessEmail4 = "businessshareholder4@email.com";

            // open business shareholder #4 form
            NgWebElement uiOpenShareBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] button"));
            uiOpenShareBiz4.Click();

            // enter business shareholder #4 business name
            NgWebElement uiShareFirstBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiShareFirstBiz4.SendKeys(businessName4);

            // enter business shareholder #4 voting shares
            NgWebElement uiShareVotesBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiShareVotesBiz4.SendKeys(businessVotingShares4);

            // select the business shareholder #4 business type using dropdown
            NgWebElement uiShareBizType4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='PrivateCorporation']"));
            uiShareBizType4.Click();

            // enter business shareholder #4 email
            NgWebElement uiShareEmailBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiShareEmailBiz4.SendKeys(businessEmail4);

            // select the business shareholder #4 confirm button
            NgWebElement uiShareBizConfirmButton4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiShareBizConfirmButton4.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz4 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadNoticeofArticlesBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[39]"));
            uiUploadNoticeofArticlesBiz4.SendKeys(noticeOfArticlesBiz4);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz4 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uiUploadCentralSecRegBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[42]"));
            uiUploadCentralSecRegBiz4.SendKeys(centralSecuritiesRegisterBiz4);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz4 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uiUploadSpecialRightsResBiz4 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[45]"));
            uiUploadSpecialRightsResBiz4.SendKeys(specialRightsRestrictionsBiz4);

            /********** Business Shareholder #4 - Leader **********/

            // create business shareholder leader data
            string leaderFirstNameBiz4 = "LeaderBiz4First";
            string leaderLastNameBiz4 = "LeaderBiz4Last";
            string leaderTitleBiz4 = "LeaderBiz4Title";
            string leaderEmailBiz4 = "leader@biz4.com";

            // open business shareholder #4 > leader form
            NgWebElement uiOpenLeaderFormBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] button"));
            uiOpenLeaderFormBiz4.Click();

            // enter business shareholder #4 > leader first name
            NgWebElement uiLeaderFirstBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstBiz4.SendKeys(leaderFirstNameBiz4);

            // enter business shareholder #4 > leader last name
            NgWebElement uiLeaderLastBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiLeaderLastBiz4.SendKeys(leaderLastNameBiz4);

            // select business shareholder #4 > leader role using checkbox
            NgWebElement uiLeaderRoleBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiLeaderRoleBiz4.Click();

            // enter business shareholder #4 > leader title
            NgWebElement uiLeaderTitleBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiLeaderTitleBiz4.SendKeys(leaderTitleBiz4);

            // enter business shareholder #4 > leader email
            NgWebElement uiLeaderEmailBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiLeaderEmailBiz4.SendKeys(leaderEmailBiz4);

            // enter business shareholder #4 > leader DOB
            NgWebElement uiLeaderDOB1Biz14 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiLeaderDOB1Biz14.Click();

            SharedCalendarDate();

            /********** Business Shareholder #4 - Individual Shareholder **********/

            // create the business shareholder #4 > individual shareholder data
            string shareholderFirstNameBiz4 = "IndividualShareholderBiz4First";
            string shareholderLastNameBiz4 = "IndividualShareholderBiz4Last";
            string shareholderVotingSharesBiz4 = "1";
            string shareholderEmailBiz4 = "individualshareholder@biz4.com";

            // open business shareholder #4 > individual shareholder form
            NgWebElement uiOpenIndyShareBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
            uiOpenIndyShareBiz4.Click();

            // enter business shareholder #4 > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz4.SendKeys(shareholderFirstNameBiz4);

            // enter business shareholder #4 > individual shareholder last name
            NgWebElement uiIndyShareLastBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz4.SendKeys(shareholderLastNameBiz4);

            // enter business shareholder #4 > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz4.SendKeys(shareholderVotingSharesBiz4);

            // enter business shareholder #4 > individual shareholder email
            NgWebElement uiIndyShareEmailBiz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz4.SendKeys(shareholderEmailBiz4);

            // enter business shareholder #4 > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz4 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz4.Click();

            SharedCalendarDate();
        }

        public void BusinessShareholder5()
        {
            /********** Business Shareholder #5 **********/

            // create the business shareholder data
            string businessName5 = "Business Shareholder 5";
            string businessVotingShares5 = "1";
            string businessEmail5 = "businessshareholder5@email.com";

            // open business shareholder #5 form
            NgWebElement uiOpenShareBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] button"));
            uiOpenShareBiz5.Click();

            // enter business shareholder #5 business name
            NgWebElement uiShareFirstBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='businessNameNew']"));
            uiShareFirstBiz5.SendKeys(businessName5);

            // enter business shareholder #5  voting shares
            NgWebElement uiShareVotesBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiShareVotesBiz5.SendKeys(businessVotingShares5);

            // select business shareholder #5 business type using dropdown
            NgWebElement uiShareBizType5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [formcontrolname='businessType'] option[value='PrivateCorporation']"));
            uiShareBizType5.Click();

            // enter business shareholder #5 email
            NgWebElement uiShareEmailBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] input[formcontrolname='emailNew']"));
            uiShareEmailBiz5.SendKeys(businessEmail5);

            // select the business shareholder #5 confirm button
            NgWebElement uiShareBizConfirmButton5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] .fa-save span"));
            uiShareBizConfirmButton5.Click();

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a notice of articles document for business shareholder
            string noticeOfArticlesBiz5 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "notice_of_articles.pdf");
            NgWebElement uiUploadNoticeofArticlesBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[48]"));
            uiUploadNoticeofArticlesBiz5.SendKeys(noticeOfArticlesBiz5);

            // upload a central securities register document for business shareholder
            string centralSecuritiesRegisterBiz5 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "central_securities_register.pdf");
            NgWebElement uiUploadCentralSecRegBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[51]"));
            uiUploadCentralSecRegBiz5.SendKeys(centralSecuritiesRegisterBiz5);

            // upload a special rights and restrictions document for business shareholder
            string specialRightsRestrictionsBiz5 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "special_rights_restrictions.pdf");
            NgWebElement uiUploadSpecialRightsResBiz5 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[54]"));
            uiUploadSpecialRightsResBiz5.SendKeys(specialRightsRestrictionsBiz5);

            /********** Business Shareholder #5 - Leader **********/

            // create business shareholder #5 leader data
            string leaderFirstNameBiz5 = "LeaderBiz5First";
            string leaderLastNameBiz5 = "LeaderBiz5Last";
            string leaderTitleBiz5 = "LeaderBiz5Title";
            string leaderEmailBiz5 = "leader@biz5.com";

            // open business shareholder #5 > leader form
            NgWebElement uiOpenLeaderFormBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] button"));
            uiOpenLeaderFormBiz5.Click();

            // enter business shareholder #5 > leader first name
            NgWebElement uiLeaderFirstBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='firstNameNew']"));
            uiLeaderFirstBiz5.SendKeys(leaderFirstNameBiz5);

            // enter business shareholder #5 > leader last name
            NgWebElement uiLeaderLastBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='lastNameNew']"));
            uiLeaderLastBiz5.SendKeys(leaderLastNameBiz5);

            // select business shareholder #5 > leader role using checkbox
            NgWebElement uiLeaderRoleBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='isDirectorNew']"));
            uiLeaderRoleBiz5.Click();

            // enter business shareholder #5 > leader title
            NgWebElement uiLeaderTitleBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='titleNew']"));
            uiLeaderTitleBiz5.SendKeys(leaderTitleBiz5);

            // enter business shareholder #5 > leader email
            NgWebElement uiLeaderEmailBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='emailNew']"));
            uiLeaderEmailBiz5.SendKeys(leaderEmailBiz5);

            // enter business shareholder #5 > leader DOB
            NgWebElement uiLeaderDOB1Biz15 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='Leadership'] input[formcontrolname='dateofBirthNew']"));
            uiLeaderDOB1Biz15.Click();

            SharedCalendarDate();

            /********** Business Shareholder #5 - Individual Shareholder **********/

            // create the business shareholder > individual shareholder data
            string shareholderFirstNameBiz5 = "IndividualShareholderBiz5First";
            string shareholderLastNameBiz5 = "IndividualShareholderBiz5Last";
            string shareholderVotingSharesBiz5 = "1";
            string shareholderEmailBiz5 = "individualshareholder@biz5.com";

            // open business shareholder #5 > individual shareholder form
            NgWebElement uiOpenIndyShareBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] button"));
            uiOpenIndyShareBiz5.Click();

            // enter business shareholder #5 > individual shareholder first name
            NgWebElement uiIndyShareFirstBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='firstNameNew']"));
            uiIndyShareFirstBiz5.SendKeys(shareholderFirstNameBiz5);

            // enter business shareholder #5 > individual shareholder last name
            NgWebElement uiIndyShareLastBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='lastNameNew']"));
            uiIndyShareLastBiz5.SendKeys(shareholderLastNameBiz5);

            // enter business shareholder #5 > individual number of voting shares
            NgWebElement uiIndyShareVotesBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='numberofSharesNew']"));
            uiIndyShareVotesBiz5.SendKeys(shareholderVotingSharesBiz5);

            // enter business shareholder #5 > individual shareholder DOB
            NgWebElement uiCalendarIndyS1Biz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='dateofBirthNew']"));
            uiCalendarIndyS1Biz5.Click();

            SharedCalendarDate();

            // enter business shareholder #5 > individual shareholder email
            NgWebElement uiIndyShareEmailBiz5 = ngDriver.FindElement(By.CssSelector("[changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='BusinessShareholder'] [changetypesuffix='IndividualShareholder'] input[formcontrolname='emailNew']"));
            uiIndyShareEmailBiz5.SendKeys(shareholderEmailBiz5);
        }
    }
}
