Feature: Notices
    As a logged in business user
    I want to confirm the functionality on the Notices tab

@notices
Scenario: No Notices (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Notices
    And there are no notices attached to the account
    And the account is deleted
    Then I see the login page