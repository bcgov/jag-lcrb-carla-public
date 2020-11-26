using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Protractor;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;

/*
  Feature: MapSearch
	As a public website visitor
	I want to see Cannabis Licenses on a map

@release
Scenario: Basic Map View
	Given I navigate to the map
	Then the page shows a map

Scenario: Search for Establishment
	Given I navigate to the map
	And I search for Victory
	Then the page shows search results including Victory

Scenario: Search for City
	Given I navigate to the map
	And I search for Victoria
	Then the page shows search results including Victoria
*/

namespace bdd_tests
{
    [FeatureFile("./MapView.feature")]
    public sealed class MapView : TestBase
    {

        [Given(@"I navigate to the map")]
        public void I_view_the_map()
        {
            // navigate to the map page.
            ngDriver.Navigate().GoToUrl($"{baseUri}/map");
        }

        [And(@"I search for (.*)")]
        public void I_search_for(string search)
        {
            //var elements = ngDriver.FindElements(NgBy.Repeater("todo in todoList.todos"));
        }

        [Then(@"the page shows a map")]
        public void The_page_shows_a_map()
        {
            // verify that the results are in the right area
            //var elements = ngDriver.FindElements(NgBy.Repeater("todo in todoList.todos"));
            //Assert.Equal("build an angular app", elements[1].Text);
        }
        
        [Then(@"the page shows search results including (.*)")]
        public void The_page_shows_search_results_including(string expectedResult)
        {
            // verify that the results are in the right area
            //var elements = ngDriver.FindElements(NgBy.Repeater("todo in todoList.todos"));
            //Assert.Equal("build an angular app", elements[1].Text);
        }
    }
}
