Feature: MapSearch
	As a public website visitor
	I want to see Cannabis Licenses on a map

Scenario
	Given I view the map
	Then the page shows search results in text form

Scenario
	Given that I view the map
	And I zoom in
	Then the page shows search results in text form for the given area
