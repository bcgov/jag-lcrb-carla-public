Feature: WorkerApplicationNoLongerRequired
    As a logged in user
    I want to confirm that a cannabis worker application is no longer displayed

Scenario: Worker Application No Longer Required
    Given I login with no terms
    And the account is deleted
    And I am logged in to the dashboard
    And I click on the link for Cannabis Worker Security Verification
    And the Worker Application No Longer Required text is displayed
    And the account is deleted
    Then I see the login page