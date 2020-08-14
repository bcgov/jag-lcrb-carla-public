Feature: CreateCateringApplicationPrivateCorp
    As a logged in business user
    I want to submit a Catering Application for a private corporation
    To be used as test data

Scenario: Create Catering Application Private Corporation
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    Then I confirm the payment receipt for a Catering application