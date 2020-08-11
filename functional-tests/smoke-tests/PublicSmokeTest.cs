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
Feature: Public_smoke_test
    As a business user who is not logged in
    I want to confirm that I can view the publicly available content

Scenario: View Public Content
    Given I am not logged in to the portal
    And I click on Home page
    And the Map of Cannabis Stores Header Text is displayed
    And I click on the Licence Types page
    And the Cannabis Retail Store Licence is displayed
    And I click on the Worker Information page
    Then the Cannabis Worker Information is displayed
*/

namespace bdd_tests
{
    [FeatureFile("./Public_smoke_test.feature")]
    public sealed class PublicSmokeTest : TestBase
    {
        [Given(@"I am not logged in to the portal")]
        public void not_logged_in_public()
        {
            NavigateToFeatures();

            CheckFeatureFlagsMaps();

            IgnoreSynchronization();
        }

        [And(@"the Map of Cannabis Stores Header Text is displayed")]
        public void cannabis_maps_displayed()
        {
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Map of Cannabis Stores')]")).Displayed);
        }

        [And(@"I click on the Licence Types page")]
        public void click_on_licence_types()
        {
            ngDriver.Navigate().GoToUrl($"{baseUri}policy-document/cannabis-retail-licence");
        }

        [And(@"the Cannabis Retail Store Licence is displayed")]
        public void CRS_licence_displayed()
        {
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Cannabis Retail Store Licence')]")).Displayed);
        }

        [And(@"I click on the Worker Information page")]
        public void click_on_worker_info()
        {
            ngDriver.Navigate().GoToUrl($"{baseUri}policy-document/worker-qualification-training");
        }

        [Then(@"the Cannabis Worker Information is displayed")]
        public void worker_info_displayed()
        {
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Cannabis Retail Store Licence: Worker Information')]")).Displayed);
        }
    }
}
