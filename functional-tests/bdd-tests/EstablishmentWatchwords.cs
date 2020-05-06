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
Feature: Establishment_watchwords
    As a logged in business user
    I want to submit an establishment name
    And confirm that watch words are not used

Scenario:
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    Then I confirm the correct watchword error messages are displayed 
   
    List of watchwords:
    - Antidote      
    - Apothecary    
    - Compassion    
    - Cure          
    - Dispensary    
    - Doctor        
    - Dr.           
    - Elixir        
    - Heal          
    - Healing       
    - Health        
    - Herbal        
    - Hospital      
    - Med           
    - Medi          
    - Medical       
    - Medicinal     
    - Medicine      
    - Pharmacy      
    - Potion        
    - Prescription  
    - Relief        
    - Remedy        
    - Restore       
    - Solution      
    - Therapeutics  
    - Therapy       
    - Tonics        
    - Treatment     
*/

namespace bdd_tests
{
    [FeatureFile("./Establishment_watchwords.feature")]
    public sealed class EstablishmentWatchwords : TestBase
    {
        [Given(@"I am logged in to the dashboard as a (.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CarlaLoginNoCheck();
        }

        [And(@"I am logged in to the dashboard as a (.*)")]
        public void And_I_view_the_dashboard(string businessType)
        {
            CarlaLogin(businessType);
        }

        [And(@"the account is deleted")]
        public void Delete_my_account()
        {
            this.CarlaDeleteCurrentAccount();
        }

        [And(@"I click on the Start Application button")]
        public void I_start_application()
        {
            /* 
            Page Title: Welcome to Cannabis Licensing
            */

            // click on the Start Application button
            NgWebElement startApp_button = ngDriver.FindElement(By.XPath("//button[text()='START APPLICATION']"));
            startApp_button.Click();
        }

        [And(@"I complete the eligibility disclosure")]
        public void complete_eligibility_disclosure()
        {
            CRSEligibilityDisclosure();
        }

        [And(@"I review the account profile")]
        public void review_account_profile()
        {
            ReviewAccountProfile();
        }

        [And(@"I review the organization structure")]
        public void I_continue_to_organization_review()
        {
            ReviewOrgStructure();
        }

        [And(@"I submit the organization structure")]
        public void submit_org_structure()
        {
            SubmitOrgInfoButton();
        }

        [Then(@"I confirm the correct watchword error messages are displayed")]
        public void watch_warning_displayed()
        {
            /* 
            Page Title: Submit the Cannabis Retail Store Application
            */
            
            System.Threading.Thread.Sleep(7000);

            string watchword = "Antidote";

            /*Antidote
              Apothecary
              Compassion
              Cure
              Dispensary
              Doctor
              Dr.
              Elixir
              Heal
              Healing
              Health
              Herbal
              Hospital
              Med
              Medi
              Medical
              Medicinal
              Medicine
              Pharmacy
              Potion
              Prescription
              Relief
              Remedy
              Restore
              Solution
              Therapeutics
              Therapy
              Tonics
              Treatment*/

            // enter the establishment name
            NgWebElement estabName = ngDriver.FindElement(By.Id("establishmentName"));
            estabName.SendKeys(watchword);

            // click on the establishment address to trigger the warning
            NgWebElement estabAddress = ngDriver.FindElement(By.Id("establishmentAddressStreet"));
            estabAddress.Click();

            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'at least one word that doesn’t comply']")).Displayed);
        }
    }
}
