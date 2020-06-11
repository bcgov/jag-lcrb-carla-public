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
Feature: COVID_temporary_extension_liquor_primary_club.feature
    As a business user who is not logged in
    I want to submit a COVID temporary extension application
    For a Liquor Primary Club licence

Scenario: Complete COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Liquor Primary Club licence
    And I click on the Submit button
    Then the application is submitted
*/

namespace bdd_tests
{
    [FeatureFile("./COVID_temporary_extension_liquor_primary_club.feature")]
    public sealed class COVIDTemporaryExtensionLiquorPrimaryClub : TestBase
    {
        

        [Given(@"I am not logged in to the Liquor and Cannabis Portal")]
        public void not_logged_in()
        {
            CheckFeatureFlagsCOVIDTempExtension();
        }

        [And(@"I click on the COVID Temporary Extension link")]
        public void click_on_covid_temp()
        {
            ngDriver.Navigate().GoToUrl($"{baseUri}covid-temporary-extension");
        }

        [And(@"I complete the temporary extension application for a Liquor Primary Club licence")]
        public void complete_application()
        {
            /* 
            Page Title: Covid Temporary Extension Application
            */

            // create test data
            string licencenumber = "123456";
            string licenceename = "Point Ellis Operations";
            string estname = "Point Ellis Greenhouse";
            string eststreet = "645 Tyee Road";
            string estcity = "Victoria";
            string estpostal = "V8V4Y3";
            string contactfirst = "ContactFirst";
            string contactlast = "ContactLast";
            string contacttitle = "Director";
            string contacttel = "2501811181";
            string contactemail = "contact@email.com";
            string mailingstreet = "MailingStreet";
            string mailingcity = "MailingCity";
            string mailingpostal = "V8V4Y3";

            // enter the licence number
            NgWebElement uiLicenceNumber = ngDriver.FindElement(By.XPath("//input[@type='text']"));
            uiLicenceNumber.SendKeys(licencenumber);
            
            // select the licence type for Liquor Primary Club
            NgWebElement uiLicenceType2 = ngDriver.FindElement(By.XPath("//mat-radio-button[@id='mat-radio-4']"));
            uiLicenceType2.Click();

            // enter the establishment name
            NgWebElement uiEstName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[2]"));
            uiEstName.SendKeys(estname);

            // enter the establishment street
            NgWebElement uiEstStreet = ngDriver.FindElement(By.XPath("(//input[@type='text'])[3]"));
            uiEstStreet.SendKeys(eststreet);

            // enter the establishment city
            NgWebElement uiEstCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[4]"));
            uiEstCity.SendKeys(estcity);

            // enter the establishment postal code
            NgWebElement uiEstPostal = ngDriver.FindElement(By.XPath("(//input[@type='text'])[5]"));
            uiEstPostal.SendKeys(estpostal);

            // enter the licencee name
            NgWebElement uiLicenceeName = ngDriver.FindElement(By.XPath("(//input[@type='text'])[6]"));
            uiLicenceeName.SendKeys(licenceename);

            // enter the contact first name
            NgWebElement uiContactFirst = ngDriver.FindElement(By.XPath("(//input[@type='text'])[7]"));
            uiContactFirst.SendKeys(contactfirst);

            // enter the contact last name
            NgWebElement uiContactLast = ngDriver.FindElement(By.XPath("(//input[@type='text'])[8]"));
            uiContactLast.SendKeys(contactlast);

            // enter the contact title
            NgWebElement uiContactTitle = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            uiContactTitle.SendKeys(contacttitle);

            // enter the contact phone number
            NgWebElement uiContactTel = ngDriver.FindElement(By.XPath("(//input[@type='text'])[10]"));
            uiContactTel.SendKeys(contacttel);

            // enter the contact email
            NgWebElement uiContactEmail = ngDriver.FindElement(By.XPath("(//input[@type='text'])[11]"));
            uiContactEmail.SendKeys(contactemail);

            // select mailing address 'Same as above' checkbox (and then deselect) to confirm checkbox is visible and clickable
            NgWebElement uiSameAsAbove = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            uiSameAsAbove.Click();
            uiSameAsAbove.Click();

            // enter the mailing street
            NgWebElement uiMailingStreet = ngDriver.FindElement(By.XPath("(//input[@type='text'])[12]"));
            uiMailingStreet.Clear();
            uiMailingStreet.SendKeys(mailingstreet);

            // enter the mailing city
            NgWebElement uiMailingCity = ngDriver.FindElement(By.XPath("(//input[@type='text'])[13]"));
            uiMailingCity.Clear();
            uiMailingCity.SendKeys(mailingcity);

            // enter the mailing postal code
            NgWebElement uiMailingPostal = ngDriver.FindElement(By.XPath("(//input[@type='text'])[14]"));
            uiMailingPostal.Clear();
            uiMailingPostal.SendKeys(mailingpostal);

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a floor plan document
            string floorplanPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uploadFloorplan = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadFloorplan.SendKeys(floorplanPath);

            // remove the floor plan 
            string delete = "Delete";
            NgWebElement uiDeleteLink = ngDriver.FindElement(By.LinkText(delete));
            uiDeleteLink.Click();

            // upload floor plan to confirm delete/reload functionality for same document
            string floorplanPath2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "floor_plan.pdf");
            NgWebElement uploadFloorplan2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadFloorplan2.SendKeys(floorplanPath2);

            // select the bounded status checkbox
            NgWebElement boundedStatus = ngDriver.FindElement(By.Id("mat-checkbox-1"));
            boundedStatus.Click();

            // upload a representative notification form 
            string repNotifyPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "licensee_rep_notification.pdf");
            NgWebElement uploadRepNotify = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uploadRepNotify.SendKeys(repNotifyPath);

            // delete the representative notification form
            NgWebElement uiDeleteLink2 = ngDriver.FindElement(By.XPath("//*[@id='licenseeRepresentativeNotficationFormDocuments']/div/div/section[1]/span[2]/a"));
            uiDeleteLink2.Click();
            
            // upload a new version of the representative notification form
            string repNotifyPath2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "licensee_rep_notification_2.pdf");
            NgWebElement uploadRepNotify2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[5]"));
            uploadRepNotify2.SendKeys(repNotifyPath2);

            // click on the local government / first nation comments checkbox
            NgWebElement uiLGIN = ngDriver.FindElement(By.XPath("//mat-radio-button[@id='mat-radio-8']"));
            uiLGIN.Click();

            // upload a LG/IN form 
            string LGINPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "LG_IN_approval.pdf");
            NgWebElement uploadLGIN = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uploadLGIN.SendKeys(LGINPath);

            // delete the LG/IN form 
            NgWebElement uiDeleteLink3 = ngDriver.FindElement(By.XPath("//*[@id='lGConfirmation']/div/div/section[1]/span[2]/a"));
            uiDeleteLink3.Click();

            // upload a new version of the LG/IN form
            string LGINPath2 = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "LG_IN_approval_2.pdf");
            NgWebElement uploadLGIN2 = ngDriver.FindElement(By.XPath("(//input[@type='file'])[8]"));
            uploadLGIN2.SendKeys(LGINPath2);

            // click on the signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[3]"));
            uiSignatureAgreement.Click();
        }

        [And(@"I click on the Submit button")]
        public void submit_button()
        {
            NgWebElement submitButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT APPLICATION')]"));
            submitButton.Click();
        }

        [Then(@"the application is submitted")]
        public void application_submitted()
        {
            System.Threading.Thread.Sleep(11000);

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Thank you for your submission.')]")).Displayed);
        }
    }
}
