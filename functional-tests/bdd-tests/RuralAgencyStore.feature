Feature: RuralAgencyStore
    As a logged in business user
    I want to submit a rural store application for a private corporation

@privatecorporation @ruralagencystore
Scenario: Rural Agency Store Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Rural Agency Store
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Rural Agency Store application
    And the account is deleted
    Then I see the login page