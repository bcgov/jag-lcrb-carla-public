Feature: CRS_plan_store_opening
    As a logged in business user
    I want to plan my Cannabis Retail Store opening 

Scenario: Plan CRS Opening
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Plan Store Opening link
    And I get ready for my inspection
    And I click on the Save button
    And I click on the Licences tab
    And I click on the Plan Store Opening link
    Then the store planning details have been saved