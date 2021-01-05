Feature: Notices
    As a logged in business user
    I want to view notices on the Notices tab

@notices
Scenario: Notices Download (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Notices
    And I click on the Notices file
    And the account is deleted
    Then I see the login page