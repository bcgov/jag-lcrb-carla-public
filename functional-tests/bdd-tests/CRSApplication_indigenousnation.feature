Feature: CRSApplication_indigenousnation
    As a logged in business user
    I want to submit a CRS Application for an indigenous nation

Scenario: Start Application
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button
    And I click on the Continue to Organization Review button
    And I complete the application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And I delete my account
    Then I see login