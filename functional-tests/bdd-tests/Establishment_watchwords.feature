Feature: Establishment_watchwords
    As a logged in business user
    I want to submit an establishment name
    And confirm that watch words are not used

Scenario:
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    Then I confirm the correct watchword error messages are displayed 