Feature: MultiLevelCRSApplicationPrivateCorp
    As a logged in business user
    I want to submit a CRS Application for a private corporation
    With multiple nested business shareholders

Scenario: Start Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I add in multiple nested business shareholders
    Then I submit the organization information