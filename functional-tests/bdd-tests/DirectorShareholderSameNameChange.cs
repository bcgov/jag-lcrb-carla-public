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
Feature: Director_shareholder_samenamechange.feature
    As a logged in business user
    I want to change the name of a director who is also a shareholder
    And confirm the change in the organization structure

Scenario: Change director and shareholder same name 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I review the organization structure
    And I modify only the director record
    And I review the organization structure
    Then the director and shareholder name are identical 
*/

namespace bdd_tests
{
    [FeatureFile("./Director_shareholder_samenamechange.feature")]
    public sealed class DirectorShareholderSameNameChange : TestBase
    {
        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLoginNoCheck();
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
            // click on the Add Key Personnel button
            NgWebElement uiAddKeyPersonnel = ngDriver.FindElement(By.XPath("//div/button"));
            uiAddKeyPersonnel.Click();

            // create same individual data
            string sameIndividualFirstName = "Same";
            string sameIndividualLastName = "Individual";
            string sameIndividualEmail = "same@individual.com";
            string sameTitle = "CEO";
            string votingShares = "100";

            // enter the first name 
            NgWebElement uiSameIndividualFirstName = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiSameIndividualFirstName.SendKeys(sameIndividualFirstName);

            // enter the last name 
            NgWebElement uiSameIndividualLastName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiSameIndividualLastName.SendKeys(sameIndividualLastName);

            // click the Director checkbox
            NgWebElement uiSameRole = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiSameRole.Click();

            // enter the title
            NgWebElement uiSameTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiSameTitle.SendKeys(sameTitle);

            // enter the email 
            NgWebElement uiSameIndividualEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiSameIndividualEmail.SendKeys(sameIndividualEmail);

            // select the DOB
            NgWebElement openKeyPersonnelDOB = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            openKeyPersonnelDOB.Click();

            NgWebElement openKeyPersonnelDOB1 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-2']/div/mat-month-view/table/tbody/tr[2]/td[2]/div"));
            openKeyPersonnelDOB1.Click();

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.XPath("//i/span"));
            uiConfirmButton.Click();

            // click on Add Individual Shareholder
            NgWebElement uiAddIndividualShareholder = ngDriver.FindElement(By.XPath("//div[5]/section/app-associate-list/div/button"));
            uiAddIndividualShareholder.Click();

            // enter the first name
            NgWebElement uiSameIndividualFirstName2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiSameIndividualFirstName2.SendKeys(sameIndividualFirstName);

            // enter the last name
            NgWebElement uiSameIndividualLastName2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiSameIndividualLastName2.SendKeys(sameIndividualLastName);

            // enter the number of voting shares
            NgWebElement uiSameIndividualVotingShare = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiSameIndividualVotingShare.SendKeys(votingShares);

            // enter the email
            NgWebElement uiSameIndividualEmail2 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            uiSameIndividualEmail2.SendKeys(sameIndividualEmail);

            // enter the DOB
            NgWebElement uiCalendarS1 = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiCalendarS1.Click();

            NgWebElement uiCalendarS2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-4']/div/mat-month-view/table/tbody/tr[2]/td[2]/div"));
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
            // click on review org info button
            NgWebElement orgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,'REVIEW ORGANIZATION INFORMATION')]"));
            orgInfoButton.Click();
        }
         
        [And(@"I modify only the director record")]
        public void modify_director_record()
        {
            // create new name for same individual
            string newFirstName = "NewFirstName";
            string newLastName = "NewLastName";

            // click on the edit button for key personnel 
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

            // find the marriage certificate file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a marriage certificate document
            string marriageCertificate = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "marriage_certificate.pdf");
            NgWebElement uploadMarriageCert = ngDriver.FindElement(By.XPath("(//input[@type='file'])[12]"));
            uploadMarriageCert.SendKeys(marriageCertificate);

            // click on submit org info button
            NgWebElement orgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT ORGANIZATION INFORMATION')]"));
            orgInfoButton.Click();
        }

        [Then(@"the new director and shareholder name are identical")]
        public void director_name_updated()
        {
            // check that the director name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'NewFirstNamae')]")).Displayed);
        }
    }
}
