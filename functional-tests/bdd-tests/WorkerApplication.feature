Feature: WorkerApplication
    As a logged in worker applicant
    I want to submit a cannabis worker application

@5release
Scenario: Worker Application
    Given I login with no terms
    And the account is deleted
    And I am logged in to the dashboard
    And I click on my name
    And I complete Step 1 of the application
    And I complete Step 2 of the application
    And I click on the Submit & Pay button
    And I enter the payment information
    And the account is deleted
    Then I see the login page