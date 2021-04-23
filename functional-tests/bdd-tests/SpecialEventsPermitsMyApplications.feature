Feature: SpecialEventsPermitsMyApplications
    As a logged in business user
    I want to view my Special Events Permits applications

Scenario: My SEP Applications (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for Go To My Applications
    And the Current Applications label is displayed
    And the account is deleted
    Then I see the login page