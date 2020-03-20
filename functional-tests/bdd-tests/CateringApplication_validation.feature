Feature: CateringApplication_validation
    As a logged in business user
    I want to confirm the page validation for a Catering Application

Scenario: Start Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Catering Start Application button
    And I review the account profile
    And I review the organization structure
    And I do not complete the catering application correctly
    Then the expected error messages are displayed