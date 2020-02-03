Feature: CRSApplication
    As a logged in business user
    I want to submit a CRS Application

Scenario: Start Application
    Given I SEE the Dashboard
    And I CLICK Start Application
    # And I SEE Review Account Profile - not needed
    And I CLICK on Continue to Application
    Then I COMPLETE the Application