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
Feature: LicenceCRSPrivateCorp.feature
    As a logged in business user
    I want to pay the Cannabis Retail Store Licence Fee
    And complete the available application types

Scenario: Pay CRS Licence Fee and Complete Applications
    #Given the CRS application has been approved
    #And I am logged in to the dashboard as a private corporation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And I pay the licensing fee
    #And I plan the store opening
    And I request a store relocation
    And I request a valid store name or branding change
    And I review the federal reports
    And I request a transfer of ownership
    And I show the store as open on the map
    Then the requested applications are visible on the dashboard
*/

namespace bdd_tests
{
    [FeatureFile("./LicenceCRSPrivateCorp.feature")]
    public sealed class LicenceCRSPrivateCorp : TestBase
    {
        /*[Given(@"the CRS application has been approved")]
        public void CRS_application_is_approved()
        {
        }*/

        [Given(@"I am logged in to the dashboard as a (.*)")]
        //[And(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLoginNoCheck(businessType);
        }

        [And(@"I click on the Licences tab")]
        public void click_on_licences_tab()
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }

        [And(@"I pay the licensing fee")]
        public void pay_licence_fee()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            string licenceFee = "Pay Licence Fee and Plan Store Opening";

            // click on the pay licence fee link
            NgWebElement uiLicenceFee = ngDriver.FindElement(By.LinkText(licenceFee));
            uiLicenceFee.Click();

            /* 
            Page Title:
            */

            string reasonDay = "Automated test: Reason for opening date.";

            // enter the estimated opening date
            NgWebElement uiCalendar1 = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-application-and-licence-fee/div/div[2]/div[2]/section[2]/div/app-field[1]/section/div[1]/section/input"));
            uiCalendar1.Click();

            NgWebElement uiCalendar2 = ngDriver.FindElement(By.XPath("//*[@id='mat-datepicker-0']/div/mat-month-view/table/tbody/tr[2]/td[3]/div"));
            uiCalendar2.Click();

            // enter the reason for the opening date
            NgWebElement uiReasonDate = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-application-and-licence-fee/div/div[2]/div[2]/section[2]/div/app-field[2]/section/div/section/textarea"));
            uiReasonDate.SendKeys(reasonDay);
       
            NgWebElement paymentButton = ngDriver.FindElement(By.XPath("//button[contains(.,' PAY LICENCE FEE AND RECEIVE LICENCE')]"));
            paymentButton.Click();

            System.Threading.Thread.Sleep(3000);
  
            MakePayment();
        }

        [And(@"I plan the store opening")]
        public void plan_store_opening()
        {
            /* 
            Page Title: Plan Your Store Opening
            */

            string storePlanningLink = "Plan Store Opening";

            // click on the store planning link
            NgWebElement uiStorePlanning = ngDriver.FindElement(By.LinkText(storePlanningLink));
            uiStorePlanning.Click();

            // select checkboxes to confirm store opening details
            NgWebElement check1 = ngDriver.FindElement(By.XPath("//input[@type='checkbox']"));
            check1.Click();

            NgWebElement check2 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[2]"));
            check2.Click();

            NgWebElement check3 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[3]"));
            check3.Click();

            NgWebElement check4 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[4]"));
            check4.Click();

            NgWebElement check5 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[5]"));
            check5.Click();

            NgWebElement check6 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[6]"));
            check6.Click();

            NgWebElement check7 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[7]"));
            check7.Click();

            NgWebElement check8 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[8]"));
            check8.Click();

            NgWebElement check9 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[9]"));
            check9.Click();

            NgWebElement check10 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[10]"));
            check10.Click();

            NgWebElement check11 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[11]"));
            check11.Click();

            NgWebElement check12 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[12]"));
            check12.Click();

            NgWebElement check13 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[13]"));
            check13.Click();

            NgWebElement check14 = ngDriver.FindElement(By.XPath("(//input[@type='checkbox'])[14]"));
            check14.Click();

            //click on the Save button
            NgWebElement saveButton = ngDriver.FindElement(By.XPath("//button[contains(.,'SAVE')]"));
            saveButton.Click();
        }

        [And(@"I request a store relocation")]
        public void request_store_relocation()
        {
            /* 
            Page Title: Licences
            Subtitle: Cannabis Retail Store Licences
            */

            string requestRelocationLink = "Request Relocation";

            // click on the request location link
            NgWebElement uiRequestRelocation = ngDriver.FindElement(By.LinkText(requestRelocationLink));
            uiRequestRelocation.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            // click on the Continue to Application button
            NgWebElement continueButton = ngDriver.FindElement(By.XPath("//button[contains(.,'CONTINUE TO APPLICATION')]"));
            continueButton.Click();

            /* 
            Page Title: Submit a Licence Relocation Application
            */

            string proposedAddress = "Automated Test Street";
            string proposedCity = "Automated City";
            string proposedPostalCode = "A1A 1A1";

            // enter the proposed street address
            NgWebElement uiProposedAddress = ngDriver.FindElement(By.Id("establishmentAddressStreet"));
            uiProposedAddress.SendKeys(proposedAddress);

            // enter the proposed city
            NgWebElement uiProposedCity = ngDriver.FindElement(By.Id("establishmentAddressCity"));
            uiProposedCity.SendKeys(proposedCity);

            // enter the postal code
            NgWebElement uiProposedPostalCode = ngDriver.FindElement(By.Id("establishmentAddressPostalCode"));
            uiProposedPostalCode.SendKeys(proposedPostalCode);

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a supporting document
            string supportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "checklist.pdf");
            NgWebElement uploadSupportingDoc = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSupportingDoc.SendKeys(supportingDocument);

            // select the authorized to submit checkbox
            NgWebElement uiAuthToSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthToSubmit.Click();

            // select the signature agreement checkbox
            NgWebElement uiSigAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSigAgreement.Click();

            // click on the Submit & Pay button
            NgWebElement submitpayButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT')]"));
            submitpayButton.Click();

            // pay for the relocation application
            MakePayment();

            // return to the Licences tab
            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }

        [And(@"I request a valid store name or branding change")]
        public void request_name_branding_change()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            string nameBrandingLink = "Request Store Name or Branding Change";

            // click on the Request Store Name or Branding Change link
            NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText(nameBrandingLink));
            uiRequestChange.Click();

            /* 
            Page Title: Please Review the Account Profile
            */

            // click on the Continue to Application button
            NgWebElement continueButton = ngDriver.FindElement(By.XPath("//button[contains(.,'CONTINUE TO APPLICATION')]"));
            continueButton.Click();

            /*
            Page Title: Submit a Name or Branding Change Application
            */

            // find the upload test file in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload a supporting document
            string supportingDocument = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + "signage.pdf");
            NgWebElement uploadSupportingDoc = ngDriver.FindElement(By.XPath("(//input[@type='file'])[2]"));
            uploadSupportingDoc.SendKeys(supportingDocument);

            // click on the store exterior change button
            NgWebElement uiStoreExterior = ngDriver.FindElement(By.Id("mat-button-toggle-2-button"));
            uiStoreExterior.Click();

            // click on the authorized to submit checkbox
            NgWebElement uiAuthSubmit = ngDriver.FindElement(By.Id("authorizedToSubmit"));
            uiAuthSubmit.Click();

            // click on the signature agreement checkbox
            NgWebElement uiSigAgreement = ngDriver.FindElement(By.Id("signatureAgreement"));
            uiSigAgreement.Click();

            // click on the Submit & Pay button
            NgWebElement submitpayButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT')]"));
            submitpayButton.Click();

            // pay for the relocation application
            MakePayment();

            // return to the Licences tab
            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();
        }

        [And(@"I review the federal reports")]
        public void review_federal_reports()
        {   
            /* 
            Page Title:
            */
        }

        [And(@"I request a transfer of ownership")]
        public void request_ownership_transfer()
        {
            /* 
            Page Title:
            */
        }

        [And(@"I show the store as open on the map")]
        public void show_store_open_on_map()
        {
            /* 
            Page Title:
            */
        }

        [And(@"I transfer the ownership")]
        public void transfer_store_ownership()
        {
            /* 
            Page Title:
            */
        }

        [Then(@"the requested applications are visible on the dashboard")]
        public void licences_tab_updated()
        {
            /* 
            Page Title:
            */

            //Assert.True (ngDriver.FindElement(By.XPath("//a[text()='Log In']")).Displayed);
        }
    }
}
