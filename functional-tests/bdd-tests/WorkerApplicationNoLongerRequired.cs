using OpenQA.Selenium;
using Xunit;
using Xunit.Gherkin.Quick;

/*
Feature: WorkerApplicationNoLongerRequired
    As a logged in user
    I want to confirm that a cannabis worker application is no longer displayed

@workerapplicationnotreqd
Scenario: Worker Application No Longer Required
    Given I login with no terms
    And the account is deleted
    And I am logged in to the dashboard
    And I click on the link for Cannabis Worker Security Verification
    And the Worker Application No Longer Required text is displayed
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./WorkerApplicationNoLongerRequired.feature")]
    [Collection("General")]
    public sealed class WorkerApplicationNoLongerRequired : TestBaseWorker
    {
        [And(@"the dashboard has a new status")]
        public void DashboardHasNewStatus()
        {
            /* 
            Page Title: Worker Dashboard
            */

            // confirm the page title is correct
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Worker Application No Longer Required')]"))
                .Displayed);
        }
    }
}