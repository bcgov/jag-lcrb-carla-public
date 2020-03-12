Feature: CRSApplication_society
    As a logged in business user
    I want to submit a CRS Application for a society

Scenario: Start Application
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button
    And I click on the Continue to Application button
    And I complete the application
    And I click on the Submit button
    And I click on the Pay button
    And I enter the payment information
    And I return to the dashboard
    And I delete my account
    Then I see login