Feature: RuralStore.feature
    As a logged in business user
    I want to submit a rural store application for a private corporation

@privatecorporation @ruralstore
Scenario: Rural Store Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Rural Agency Store
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Rural Agency Store application
    And the account is deleted
    Then I see the login page

@privatecorporation @ruralstore
Scenario: Validation for Rural Store Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Rural Agency Store
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Rural Store application
    And the account is deleted
    Then I see the login page