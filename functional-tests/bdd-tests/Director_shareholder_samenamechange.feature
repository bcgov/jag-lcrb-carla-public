Feature: Director_shareholder_samenamechange.feature
    As a logged in business user
    I want to change the name of a director who is also a shareholder
    And confirm the change in the organization structure

Scenario: Change director and shareholder same name 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I review the organization structure
    And I modify only the director record
    And I review the organization structure
    Then the director and shareholder name are identical 
