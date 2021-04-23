Feature: SpecialEventsPermitsApplicationChecklist
    As a logged in business user
    I want to view the application checklist for Special Events Permits

Scenario: SEP Application Checklist (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for See Checklist 
    And the SEP Checklist content is displayed
    And the account is deleted
    Then I see the login page