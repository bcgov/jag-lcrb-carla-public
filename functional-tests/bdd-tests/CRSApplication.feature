Feature: CRSApplication
    As a logged in business user
    I want to submit a CRS Application

Scenario: Start Application
    Given I am logged in to the dashboard
    And I click on the Start Application button
    And I click on the Continue to Application button
    And I complete the application
    And I click on the Submit & Pay button
    And I enter the payment information
    Then I return to the dashboard