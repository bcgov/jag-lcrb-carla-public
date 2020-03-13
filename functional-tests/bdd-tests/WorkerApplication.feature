Feature: WorkerApplication
    As a logged in worker applicant
    I want to submit a cannabis worker application

Scenario: Worker Application
    Given I am logged in to the dashboard
    And I click on my name
    And I complete Step 1 of the application
    And I complete Step 2 of the application
    And I click on the Submit & Pay button
    And I enter the payment information
    And I return to the dashboard
    And the dashboard has a new status
    Then I sign out