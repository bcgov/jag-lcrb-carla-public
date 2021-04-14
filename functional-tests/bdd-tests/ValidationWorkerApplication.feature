Feature: ValidationWorkerApplication
    As a logged in worker applicant
    I want to submit a cannabis worker application

Scenario: Validation for Worker Application
    Given I login with no terms
    And the account is deleted
    And I am logged in to the dashboard
    And I click on my name
    And I do not complete Step 1 of the application
    And the Step 1 error messages are displayed
    And I complete Step 1 of the application
    And I do not complete Step 2 of the application
    And I click on the Submit & Pay button
    And the Step 2 error messages are displayed
    And the account is deleted
    Then I see the login page