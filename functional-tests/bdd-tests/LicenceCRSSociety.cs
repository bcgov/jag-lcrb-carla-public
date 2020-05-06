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
Feature: LicenceCRS_society.feature
    As a logged in business user
    I want to pay the Cannabis Retail Store Licence Fee
    And complete the available application types

Scenario: Pay CRS Licence Fee and Complete Applications
    # Given the CRS application has been approved
    # And I am logged in to the dashboard as a society
    Given I am logged in to the dashboard as a society
    And I click on the Licences tab
    And I pay the licensing fee
    And I click on the licence download link
    And I plan the store opening
    And I request a store relocation
    And I request a valid store name or branding change
    And I request a structural change
    And I review the federal reports
    And I show the store as open on the map
    And I request a transfer of ownership
    And I request a personnel name change
    And I change a personnel email address
    Then the requested applications are visible on the dashboard
*/

namespace bdd_tests
{
    [FeatureFile("./LicenceCRS_society.feature")]
    public sealed class LicenceCRSSociety : TestBase
    {
        /*[Given(@"the CRS application has been approved")]
        public void CRS_application_is_approved()
        {
        }*/

        [Given(@"I am logged in to the dashboard as a (.*)")]
        //[And(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLoginNoCheck();
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
            PayLicenceFee();
        }

        [And(@"I click on the licence download link")]
        public void click_licence_download_link()
        {
            DownloadLicence();
        }

        [And(@"I plan the store opening")]
        public void plan_store_opening()
        {
            PlanStoreOpening();
        }

        [And(@"I request a store relocation")]
        public void request_store_relocation()
        {
            RequestRelocation();
        }

        [And(@"I request a valid store name or branding change")]
        public void request_name_branding_change()
        {
            StoreNameBrandingChange();
        }

        [And(@"I request a structural change")]
        public void request_structural_change()
        {
            RequestStructuralChange();
        }

        [And(@"I review the federal reports")]
        public void review_federal_reports()
        {
            ReviewFederalReports();
        }   

        [And(@"I request a transfer of ownership")]
        public void request_ownership_transfer()
        {
            string licencesLink = "Licences";

            // click on the Licences link
            NgWebElement uiLicences = ngDriver.FindElement(By.LinkText(licencesLink));
            uiLicences.Click();

            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            string transferOwnership = "Transfer Ownership";

            // click on the Transfer Ownership link
            NgWebElement uiTransferOwnership = ngDriver.FindElement(By.LinkText(transferOwnership));
            uiTransferOwnership.Click();

            /* 
            Page Title: Transfer Your Cannabis Retail Store Licence
            */

            string thirdparty = "GunderCorp TestBusiness";

            // search for the proposed licensee
            NgWebElement thirdPartyOperator = ngDriver.FindElement(By.XPath("(//input[@type='text'])[9]"));
            thirdPartyOperator.SendKeys(thirdparty);

            NgWebElement thirdPartyOperatorOption = ngDriver.FindElement(By.XPath("//mat-option[@id='mat-option-0']/span"));
            thirdPartyOperatorOption.Click();

            // click on consent to licence transfer checkbox
            NgWebElement consentToTransfer = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-application-ownership-transfer/div/div[2]/div[2]/section[5]/app-field/section/div/section/section/input"));
            consentToTransfer.Click();

            // click on authorize signature checkbox
            NgWebElement authorizeSignature = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-application-ownership-transfer/div/div[2]/div[2]/div/app-field[1]/section/div/section/section/input"));
            authorizeSignature.Click();

            // click on signature agreement checkbox
            NgWebElement signatureAgreement = ngDriver.FindElement(By.XPath("/html/body/app-root/div/div/div/main/div/app-application-ownership-transfer/div/div[2]/div[2]/div/app-field[2]/section/div/section/section/input"));
            signatureAgreement.Click();

            // click on submit transfer button
            NgWebElement submitTransferButton = ngDriver.FindElement(By.XPath("//button[contains(.,' SUBMIT TRANSFER')]"));
            submitTransferButton.Click();

            // TODO: Confirm status change on Licences tab
        }

        [And(@"I show the store as open on the map")]
        public void show_store_open_on_map()
        {
            /* 
            Page Title: Licences
            Subtitle:   Cannabis Retail Store Licences
            */

            string showOpenOnMap = "Show Store as Open on Map";

            // click on the Transfer Ownership link
            NgWebElement uiShowOpenOnMap = ngDriver.FindElement(By.LinkText(showOpenOnMap));
            uiShowOpenOnMap.Click();

            // TODO: next steps?

            /* 
            Page Title: Apply for a cannabis licence
            */

            System.Threading.Thread.Sleep(7000);

            string dashboard = "Dashboard";

            // click on the Dashboard link
            NgWebElement uiDashboard = ngDriver.FindElement(By.LinkText(dashboard));
            uiDashboard.Click();
        }

        [And(@"I request a personnel name change")]
        public void request_personnel_name_change()
        {
            RequestPersonnelNameChange();
        }

        [And(@"I change a personnel email address")]
        public void request_personnel_email_change()
        {
            RequestPersonnelEmailChange();
        }

        [Then(@"the requested applications are visible on the dashboard")]
        public void licences_tab_updated()
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            System.Threading.Thread.Sleep(7000);

            string dashboard = "Dashboard";

            // click on the Dashboard link
            NgWebElement uiDashboard = ngDriver.FindElement(By.LinkText(dashboard));
            uiDashboard.Click();

            // confirm that relocation request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Relocation Request')]")).Displayed);

            // confirm that a name or branding change request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Name or Branding Change')]")).Displayed);

            // confirm that a structural change request is displayed
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Structural Change')]")).Displayed);
        }
    }
}
