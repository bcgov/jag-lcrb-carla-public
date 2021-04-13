 Feature: CateringLicenseeRepresentative
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a licensee representative request 

 Scenario: Licensee Representative Request (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Add Licensee Representative
    And I request a licensee representative
    And the account is deleted
    Then I see the login page