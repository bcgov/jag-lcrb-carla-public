Feature: LiquorPrimaryApplication
    As a logged in business user
    I want to submit Liquor Primary Applications for different manufacturer types

@liquorprimaryapp 
Scenario: DEV Liquor Primary Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a private corporation
    And I complete the Liquor application
    And I enter the payment information
    And I confirm the payment receipt for a Liquor Primary Licence application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page