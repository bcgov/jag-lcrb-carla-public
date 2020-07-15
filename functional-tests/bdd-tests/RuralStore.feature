Feature: RuralStore.feature
    As a logged in business user
    I want to submit a rural store application for a private corporation

@privatecorporation @ruralstore
Scenario: Start Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Rural Agency Store
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    Then I complete the Rural Agency Store application