Feature: SpecialEventsPermitsLearnMore
    As a logged in business user
    I want to learn more about Special Events Permits policies

Scenario: SEP Learn More (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for Learn More
    And the LCRB web site is displayed
    And the account is deleted
    Then I see the login page