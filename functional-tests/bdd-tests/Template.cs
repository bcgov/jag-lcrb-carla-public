using OpenQA.Selenium;
using Xunit.Gherkin.Quick;

/*
Feature: TemplateFeature
    As a logged in business user
    I want to ***

@suitabletags
Scenario: 
    Given I am logged in to the dashboard as ***
    And I click on the ***
    And I review the ***
    And I complete the ***
    And I enter the ***
    And I confirm the ***
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./***.feature")]
    public sealed class Template : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            // check correct feature flags

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }

        [And(@"I complete the step function")]
        public void SampleMethod()
        {
            /* 
            Page Title: 
            */

            // create test data
            var sampleTestData = " ";

            // sample input call
            var uiInputElement = ngDriver.FindElement(By.CssSelector("  "));
            uiInputElement.SendKeys(sampleTestData);

            // sample selection call
            var uiSelectElement = ngDriver.FindElement(By.CssSelector("  "));
            uiSelectElement.Click();
        }
    }
}