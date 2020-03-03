Feature: CRSApplication
    As a logged in business user
    I want to submit a CRS Application

Scenario: Start Application
    Given I SEE the Dashboard
    And I CLICK Start Application
    And I CLICK on Continue to Application
    And I COMPLETE the Application
    And I CLICK on 'SUBMIT & PAY'
    And I enter the payment information
    And I return to the dashboard
    And I delete my account
    Then I see login