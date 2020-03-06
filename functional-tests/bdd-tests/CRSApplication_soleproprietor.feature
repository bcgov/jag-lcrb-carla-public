Feature: CRSApplication_soleproprietor
    As a logged in business user
    I want to submit a CRS Application for a sole proprietorship

Scenario: Start Application
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button
    And I click on the Continue to Application button
    And I complete the application
    And I click on the Submit & Pay button
    And I enter the payment information
    And I return to the dashboard
    And I delete my account
    Then I see login