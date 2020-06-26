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
Feature: PrivateCorporationOrgStructure.feature
    As a logged in business user
    I want to change the name of a director
    And pay the associated fee

@e2e @cannabis @privatecorporation @validation
Scenario: Change director name and pay fee
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Cannabis Retail Store application
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab
    # And I pay the licensing fee for Cannabis
    # And I return to the dashboard
    And I click on the Dashboard link
    And I click on the Review Organization Information button
    And I modify the director name
    And I submit the organization structure
    And I pay the name change fee
    And the director name is now updated
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @validation
Scenario: Delete an individual who is both a director and shareholder
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I click on the Complete Organization Information button
    And I delete only the director record
    And I click on the Complete Organization Information button
    And only the shareholder record is displayed
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @validation
Scenario: Change director and shareholder same name 
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I click on the Complete Organization Information button
    And I modify only the director record
    And I click on the Complete Organization Information button
    And the director and shareholder name are identical
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @validation
Scenario: Confirm business shareholder org structure update
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I click on the Complete Organization Information button
    And I add a business shareholder with the same individual as a director and a shareholder
    And I submit the organization structure
    And I click on the Complete Organization Information button
    And I add a second business shareholder with the same individual as a director and a shareholder
    And I click on the Confirm Organization Information is Complete button
    And I click on the Complete Organization Information button
    And the org structure is correct
    And I remove the latest director and shareholder
    And I submit the organization structure
    And I click on the Complete Organization Information button
    And the latest director and shareholder is removed
    And I remove the business shareholder
    And I submit the organization structure
    And I click on the Complete Organization Information button
    And the business shareholder is removed
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @validation
Scenario: Confirm business shareholder org structure update after payment
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I click on the Complete Organization Information button
    And I add a business shareholder with the same individual as a director and a shareholder
    And I submit the organization structure
    And I click on the Complete Organization Information button
    And I add a second business shareholder with the same individual as a director and a shareholder
    And I click on the Confirm Organization Information is Complete button
    And I click on the Complete Organization Information button
    And the org structure is correct
    And I click on the Confirm Organization Information is Complete button
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile
    And I click on the Confirm Organization Information is Complete button
    And I complete the Cannabis Retail Store application
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Review Organization Information button
    And the org structure is correct after payment
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @validation
Scenario: Save for Later feature for org structure 
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    #And I enter the same individual as a director and a shareholder
    And I click on the Save for Later button
    And I click on the Complete Organization Information button
    And the saved org structure is present
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./PrivateCorporationOrgStructure.feature")]
    public sealed class PrivateCorporationOrgStructure : TestBase
    {
        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CarlaLogin(businessType);
        }

        [And(@"I modify the director name")]
        public void modify_director_name()
        {
            // click on the Edit button for Key Personnel
            NgWebElement uiEditInfoButton = ngDriver.FindElement(By.XPath("//i/span"));
            uiEditInfoButton.Click();

            // enter a new name for the director
            string newDirectorFirstName = "Updated Director";

            NgWebElement uiNewDirectorFirstName = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiNewDirectorFirstName.Clear();
            uiNewDirectorFirstName.SendKeys(newDirectorFirstName);

            // click on the Confirm button
            NgWebElement uiConfirmButton = ngDriver.FindElement(By.XPath("//i/span"));
            uiConfirmButton.Click();

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a marriage certificate document
            string marriageCertificate = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "marriage_certificate.pdf");
            NgWebElement uploadMarriageCert = ngDriver.FindElement(By.XPath("(//input[@type='file'])[12]"));
            uploadMarriageCert.SendKeys(marriageCertificate);
        }

        [And(@"I pay the name change fee")]
        public void name_change_fee()
        {
            MakePayment();

            // check payment fee
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'$500.00')]")).Displayed);
        }

        [And(@"the director name is now updated")]
        public void director_name_updated()
        {
            // click on Return to Dashboard link
            string retDash = "Return to Dashboard";
            NgWebElement returnDash = ngDriver.FindElement(By.LinkText(retDash));
            returnDash.Click();

            NgWebElement orgInfoButton = ngDriver.FindElement(By.XPath("//button[contains(.,'REVIEW ORGANIZATION INFORMATION')]"));
            orgInfoButton.Click();

            // check that the director name has been updated
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Updated Director')]")).Displayed);
        }
    }
}
