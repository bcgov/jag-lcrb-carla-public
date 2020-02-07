Feature: CRSApplication
    As a logged in business user
    I want to submit a CRS Application

Scenario: Start Application
    Given I SEE the Dashboard
    And I CLICK Start Application
    And I CLICK on Continue to Application
    And I COMPLETE the Application
    And I CLICK on 'SUBMIT & PAY'
    Then I enter the payment information