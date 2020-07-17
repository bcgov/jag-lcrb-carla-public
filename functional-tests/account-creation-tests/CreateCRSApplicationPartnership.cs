using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using System.IO;
using Xunit;

/*
Feature: Create_CRSApplication_partnership
    As a logged in business user
    I want to submit a CRS Application for a partnership
    To be used as test data

Scenario: Start Application
    Given I am logged in to the dashboard as a partnership
    And the account is deleted
    And I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the Pay for Application button
    And I enter the payment information
    Then I return to the dashboard   
*/

namespace bdd_tests
{
    [FeatureFile("./Create_CRSApplication_partnership.feature")]
    public sealed class CreateCRSApplicationPartnership : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void I_view_the_dashboard(string businessType)
        {
            CarlaLoginNoCheck();
        }
    }
}
