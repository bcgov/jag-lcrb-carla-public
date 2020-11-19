Feature: ManageOffsiteStorage
    As a logged in business user
    I want to ...

@e2e @privatecorporation @offsitestore
Scenario: Private Corporation Offsite Storage
    Given I am logged in to the dashboard as a private corporation

    And the account is deleted
    Then I see the login page