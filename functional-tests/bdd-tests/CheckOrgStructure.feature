Feature: CheckOrgStructure
    As a logged in business user
    I want to confirm that the organization structure page displays

@validation @checkorgstructure
Scenario: Check Organization Structure
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And the organization structure page is displayed
    And I click on the link for Dashboard
    And the account is deleted
    Then I see the login page