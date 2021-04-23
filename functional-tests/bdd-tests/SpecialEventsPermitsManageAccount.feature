Feature: SpecialEventsPermitsManageAccount
    As a logged in business user
    I want to manage my account for Special Events Permits

Scenario: Manage SEP Account (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for Edit Account Profile
    And Account Profile is displayed
    And the account is deleted
    Then I see the login page