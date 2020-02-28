Feature: WorkerApplication
    As a logged in worker applicant
    I want to submit a cannabis worker application

Scenario: Worker Application
    Given I SEE the Dashboard
    And I click on my name
    And I complete Step 1 of the application
    And I complete Step 2 of the application
    And I CLICK on 'SUBMIT & PAY'
    And I enter the payment information
    And I return to the dashboard
    Then the dashboard has a new status