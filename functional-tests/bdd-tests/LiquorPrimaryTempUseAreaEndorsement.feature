Feature: LiquorPrimaryTempUseAreaEndorsement
    As a logged in business user
    I want to request a temporary use area endorsement for a Liquor Primary Application

@liquorprimary
Scenario: Liquor Primary Temp Use Area Endorsement (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary application for a private corporation
    And I click on the Submit button
    And the dashboard status is updated as Pending External Review
    # TODO
    And the account is deleted
    Then I see the login page