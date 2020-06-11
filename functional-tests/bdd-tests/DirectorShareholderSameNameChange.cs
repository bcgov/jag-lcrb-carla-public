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
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I review the organization structure
    And I modify only the director record
    And I click on the Complete Organization Information button
    And the director and shareholder name are identical
    And the account is deleted
    Then I see the login page
 */

namespace bdd_tests
{
    [FeatureFile("./Director_shareholder_samenamechange.feature")]
    public sealed class DirectorShareholderSameNameChange : TestBase
    {
        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void Given_I_view_the_dashboard(string businessType)
        {
            CarlaLogin(businessType);
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

            // create same individual data
            string sameIndividualFirstName = "Same";
            string sameIndividualLastName = "Individual";
            string sameIndividualEmail = "same@individual.com";
            string sameTitle = "CEO";
            string votingShares = "100";

            // enter the first name 
            NgWebElement uiSameIndividualFirstName = ngDriver.FindElement(By.CssSelector("input[formControlName=\"firstNameNew\"]"));
            uiSameIndividualFirstName.SendKeys(sameIndividualFirstName);

            // enter the last name 
            NgWebElement uiSameIndividualLastName = ngDriver.FindElement(By.CssSelector("input[formControlName=\"lastNameNew\"]"));
            uiSameIndividualLastName.SendKeys(sameIndividualLastName);

            // click the Director checkbox
            NgWebElement uiSameRole = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiSameRole.Click();

            // enter the title
            NgWebElement uiSameTitle = ngDriver.FindElement(By.CssSelector("input[formControlName=\"titleNew\"]"));
            uiSameTitle.SendKeys(sameTitle);

            // enter the email 
            NgWebElement uiSameIndividualEmail = ngDriver.FindElement(By.CssSelector("input[formControlName=\"emailNew\"]"));
            uiSameIndividualEmail.SendKeys(sameIndividualEmail);

            // select the DOB
            NgWebElement openKeyPersonnelDOB = ngDriver.FindElement(By.CssSelector("input[formControlName=\"dateofBirthNew\"]"));
            openKeyPersonnelDOB.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.XPath("//i/span"));
            uiConfirmButton.Click();

            // click on Add Individual Shareholder
            NgWebElement uiAddIndividualShareholder = ngDriver.FindElement(By.XPath("//div[5]/section/app-associate-list/div/button"));
            uiAddIndividualShareholder.Click();

            // enter shareholder first name
            NgWebElement uiShareFirst = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName=\"firstNameNew\"]"));
            uiShareFirst.SendKeys(sameIndividualFirstName);

            // enter shareholder last name
            NgWebElement uiShareLast = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='lastNameNew']"));
            uiShareLast.SendKeys(sameIndividualLastName);

            // enter number of voting shares
            NgWebElement uiShareVotes = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='numberofSharesNew']"));
            uiShareVotes.SendKeys(votingShares);

            // enter shareholder email
            NgWebElement uiShareEmail = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='emailNew']"));
            uiShareEmail.SendKeys(sameIndividualEmail);

            // enter shareholder DOB
            NgWebElement uiCalendarS1 = ngDriver.FindElement(By.CssSelector("app-associate-list[changetypesuffix='IndividualShareholder'] input[formControlName='dateofBirthNew']"));
            uiCalendarS1.Click();

            SharedCalendarDate();

            // click on the Confirm button
            NgWebElement uiConfirmButton2 = ngDriver.FindElement(By.XPath("//td[6]/i/span"));
            uiConfirmButton2.Click();

            // click on submit org info button
            NgWebElement orgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT ORGANIZATION INFORMATION')]"));
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

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.XPath("//i/span"));
            uiConfirmButton.Click();

            // click on submit org info button
            NgWebElement orgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT ORGANIZATION INFORMATION')]"));
            orgInfoButton.Click();
        }

        [And(@"the director and shareholder name are identical")]
        public void director_name_updated()
        {
            // check that the director first name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[1]/span[contains(.,'NewFirstName')]")).Displayed);

            // check that the director last name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div[4]/section/app-associate-list/div/table/tr/td[2]/span[contains(.,'NewLastName')]")).Displayed);

            // check that the shareholder first name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[1]/span[contains(.,'NewFirstName')]")).Displayed);

            // check that the shareholder last name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//app-org-structure/div[5]/section[1]/app-associate-list/div/table/tr/td[2]/span[contains(.,'NewLastName')]")).Displayed);
        }
    }
}
