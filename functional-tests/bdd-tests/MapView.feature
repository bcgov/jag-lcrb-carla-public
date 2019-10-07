Feature: MapSearch
	As a public website visitor
	I want to see Cannabis Licenses on a map

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


