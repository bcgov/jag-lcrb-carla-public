Feature: LiquorPrimaryLiveTheatreRequest
    As a logged in business user
    I want to submit a live theatre request for a Liquor Primary Application

@liquorprimary
Scenario: Liquor Primary Live Theatre Request (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary application for a private corporation
    And I click on the Submit button
    And the dashboard status is updated as Pending External Review
    # TODO
    And I click on the link for Live Theatre Request For Liquor Service
    And I click on the Continue to Application button
    And I complete the live theatre request
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page