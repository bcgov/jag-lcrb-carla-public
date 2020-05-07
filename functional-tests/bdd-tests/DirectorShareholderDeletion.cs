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
Feature: Director_shareholder_deletion.feature
    As a logged in business user
    I want to delete a director who is also a shareholder
    And confirm the change in the organization structure

Scenario: Delete an individual who is both a director and shareholder
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I review the organization structure
    And I delete only the director record
    And I review the organization structure
    Then the director and shareholder are not displayed
*/

namespace bdd_tests
{
    [FeatureFile("./Director_shareholder_deletion.feature")]
    public sealed class DirectorShareholderDeletion : TestBase
    {
        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLogin(businessType);
        }

        [And(@"I click on the Complete Organization Information button")]
        public void complete_org_info()
        {
            // click on the complete organzation information button
            NgWebElement orgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,'COMPLETE ORGANIZATION INFORMATION')]"));
            orgInfoButton.Click();
        }

        [And(@"I enter the same individual as a director and a shareholder")]
        public void same_director_shareholder()
        {
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

            // click on the Add Key Personnel button
            NgWebElement uiAddKeyPersonnel = ngDriver.FindElement(By.XPath("//div/button"));
            uiAddKeyPersonnel.Click();

            // create data
            string sameIndividualFirstName = "Same";
            string sameIndividualLastName = "Individual";
            string sameIndividualEmail = "same@individual.com";
            string sameTitle = "CEO";
            string votingShares = "100";

            string sameIndividualEmail2 = "same@individual2.com";

            string sparePersonnelFirstName = "Spare";
            string sparePersonnelLastName = "KeyPersonnel";
            string sparePersonnelTitle = "CFO";
            string sparePersonnelEmail = "cfo@test.com";

            // enter the key personnel first name 
            NgWebElement uiSameIndividualFirstName = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiSameIndividualFirstName.SendKeys(sameIndividualFirstName);

            // enter the key personnel last name 
            NgWebElement uiSameIndividualLastName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiSameIndividualLastName.SendKeys(sameIndividualLastName);

            // click the key personnel checkbox
            NgWebElement uiSameRole = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiSameRole.Click();

            // enter the key personnel title
            NgWebElement uiSameTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiSameTitle.SendKeys(sameTitle);

            // enter the key personnel email 
            NgWebElement uiSameIndividualEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiSameIndividualEmail.SendKeys(sameIndividualEmail);

            // select the key personnel DOB
            NgWebElement openKeyPersonnelDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            openKeyPersonnelDOB.Click();

            NgWebElement openKeyPersonnelDOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[2]/td[2]/div"));
            openKeyPersonnelDOB1.Click();

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.XPath("//i/span"));
            uiConfirmButton.Click();

            // click on the add key personnel button for spare
            NgWebElement uiAddKeyPersonnel2 = ngDriver.FindElement(By.XPath("//div/button"));
            uiAddKeyPersonnel2.Click();

            // enter the spare key personnel first name
            NgWebElement uiSpareFirstName = ngDriver.FindElement(By.XPath("//app-org-structure/div[4]/section/app-associate-list/div/table/tr[2]/td[1]/app-field/section/div[1]/section/input"));
            uiSpareFirstName.SendKeys(sparePersonnelFirstName);

            // enter the key personnel last name 
            NgWebElement uiSpareLastName = ngDriver.FindElement(By.XPath("//app-org-structure/div[4]/section/app-associate-list/div/table/tr[2]/td[2]/app-field/section/div[1]/section/input"));
            uiSpareLastName.SendKeys(sparePersonnelLastName);

            // click the key personnel checkbox            
            NgWebElement uiSpareRole = ngDriver.FindElement(By.XPath("//app-org-structure/div[4]/section/app-associate-list/div/table/tr[2]/td[3]/app-field/section/div[1]/section/table/tr/td[2]/div/input"));
            uiSpareRole.Click();

            // enter the key personnel title
            NgWebElement uiSpareTitle = ngDriver.FindElement(By.XPath("//app-org-structure/div[4]/section/app-associate-list/div/table/tr[2]/td[4]/app-field/section/div/section/input"));
            uiSpareTitle.SendKeys(sparePersonnelTitle);

            // enter the key personnel email 
            NgWebElement uiSpareEmail = ngDriver.FindElement(By.XPath("//app-org-structure/div[4]/section/app-associate-list/div/table/tr[2]/td[5]/app-field/section/div[1]/section/input"));
            uiSpareEmail.SendKeys(sparePersonnelEmail);

            // select the key personnel DOB
            NgWebElement openSpareDOB = ngDriver.FindElement(By.XPath("//app-org-structure/div[4]/section/app-associate-list/div/table/tr[2]/td[6]/app-field/section/div[1]/section/input"));
            openSpareDOB.Click();

            NgWebElement openSpareDOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-1']/div/mat-month-view/table/tbody/tr[2]/td[5]/div"));
            openSpareDOB1.Click();

            // click on Add Individual Shareholder
            NgWebElement uiAddIndividualShareholder = ngDriver.FindElement(By.XPath("//div[5]/section/app-associate-list/div/button"));
            uiAddIndividualShareholder.Click();

            // enter the shareholder first name
            NgWebElement uiSameIndividualFirstName2 = ngDriver.FindElement(By.XPath("//app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/app-field/section/div[1]/section/input"));
            uiSameIndividualFirstName2.SendKeys(sameIndividualFirstName);

            // enter the last name
            NgWebElement uiSameIndividualLastName2 = ngDriver.FindElement(By.XPath("//app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[2]/app-field/section/div[1]/section/input"));
            uiSameIndividualLastName2.SendKeys(sameIndividualLastName);

            // enter the number of voting shares
            NgWebElement uiSameIndividualVotingShare = ngDriver.FindElement(By.XPath("//app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[3]/app-field/section/div[1]/section/div/input"));
            uiSameIndividualVotingShare.SendKeys(votingShares);

            // enter the email
            NgWebElement uiSameIndividualEmail2 = ngDriver.FindElement(By.XPath("//app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[4]/app-field/section/div[1]/section/input"));
            uiSameIndividualEmail2.SendKeys(sameIndividualEmail2);

            // enter the DOB
            NgWebElement uiCalendarS1 = ngDriver.FindElement(By.XPath("//app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[5]/app-field/section/div[1]/section/input"));
            uiCalendarS1.Click();

            NgWebElement uiCalendarS2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-2']/div/mat-month-view/table/tbody/tr[2]/td[2]/div"));
            uiCalendarS2.Click();

            // click on the Confirm button
            NgWebElement uiConfirmButton2 = ngDriver.FindElement(By.XPath("//td[6]/i/span"));
            uiConfirmButton2.Click();

            // click on submit org info button
            NgWebElement orgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT ORGANIZATION INFORMATION')]"));
            orgInfoButton.Click();
        }

        [And(@"I review the organization structure")]
        public void review_org_structure2()
        {
            // click on complete org info button
            complete_org_info();
        }
         
        [And(@"I delete only the director record")]
        public void delete_director_record()
        {
            // click on the delete button for key personnel 
            NgWebElement uiEditInfoButton = ngDriver.FindElement(By.XPath("//app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[7]/i[2]/span"));
            uiEditInfoButton.Click();

            // click on submit org info button
            NgWebElement orgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT ORGANIZATION INFORMATION')]"));
            orgInfoButton.Click();
        }

        [Then(@"only the shareholder record is displayed")]
        public void director_name_updated()
        {
            // check that the director email is not displayed
            Assert.False(ngDriver.FindElement(By.XPath("//body[contains(.,'same@individual.com')]")).Displayed);
        }
    }
}
