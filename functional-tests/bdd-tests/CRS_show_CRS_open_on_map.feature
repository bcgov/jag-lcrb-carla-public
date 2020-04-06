Feature: CRS_show_CRS_open_on_map
    As a logged in business user
    I want to show the Cannabis Retail Store as open on the map 

Scenario: Show CRS as Open on Map
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And I select the Show Store as Open on Map checkbox
    And I click on the Show Store as Open on Map link
    And I click on the maps page
    And I search for my store
    Then my store is shown as open