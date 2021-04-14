Feature: ValidationRuralAgencyStore
    As a logged in business user
    I want to confirm the validation messages for a Rural Agency Store application

Scenario: Validation for Rural Agency Store Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Rural Agency Store
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Rural Store application
    And the account is deleted
    Then I see the login page