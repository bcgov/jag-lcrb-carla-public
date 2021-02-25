Feature: RuralLRS
    As a logged in business user
    I want to submit a rural LRS application for a private corporation

@privatecorporation @ruralLRS @release2
Scenario: Rural LRS Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Rural LRS
    And I review the account profile for a private corporation
    And I complete the Rural LRS application
    And I click on the Submit button
    And I enter the payment information
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page