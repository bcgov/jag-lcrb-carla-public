Feature: Director_shareholder_deletion.feature
    As a logged in business user
    I want to delete a director who is also a shareholder
    And confirm the change in the organization structure

Scenario: Delete an individual who is both a director and shareholder
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I review the organization structure
    And I delete only the director record
    And I click on the Complete Organization Information button
    And only the shareholder record is displayed
    Then the account is deleted