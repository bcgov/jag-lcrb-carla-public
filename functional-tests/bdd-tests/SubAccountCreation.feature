Feature: SubAccountCreation
    As a logged in business user
    I want to confirm that I am a sub-account user with correct account details

Scenario: Sub-Account Log In (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I review the account profile for a private corporation
    # .../token/AT**::AT**
    And I log in as the sub-account user
    And I click on the link for Edit Account Profile
    And the user details for sub-account user is displayed
    And the account is deleted
    # return to parent
    And the account is deleted
    Then I see the login page