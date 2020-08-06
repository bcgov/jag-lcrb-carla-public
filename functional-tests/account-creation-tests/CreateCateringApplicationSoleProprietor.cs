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
Feature: Create_CateringApplication_soleproprietor
    As a logged in business user
    I want to submit a Catering Application for a sole proprietor
    To be used as test data

Scenario: Start Application
    Given I am logged in to the dashboard as a sole proprietorship
    And the account is deleted
    And I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    Then I confirm the payment receipt for a Catering application  
*/

namespace bdd_tests
{
    [FeatureFile("./Create_CateringApplication_soleproprietor.feature")]
    public sealed class CreateCateringApplicationSoleProprietor : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CheckFeatureFlagsLiquor();

            CarlaLoginNoCheck();
        }
    }
}