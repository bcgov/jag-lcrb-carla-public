Feature: MapSearch
	As a public website visitor
	I want to search by City

Scenario
	Given that search is for Victoria
	Then search results include Victoria City

Scenario
	Given that search is for Victoria
	Then search results include Victoria in the Establishment Column
