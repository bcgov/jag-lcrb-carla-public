Feature: LiquorPrimaryClubAddLicenseeRepresentative
    As a logged in business user
    I want to add a licensee representative for a Liquor Primary Club Application

Scenario: Liquor Primary Club Licensee Representative (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a LPC Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary Club application for a private corporation
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary club
    And I click on the Submit button
    And I log in as a return user
    And I click on the link for Complete Application
    And I click on the Continue to Application button
    And I review the local government response for a liquor primary club licence
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Add Licensee Representative
    And I request a licensee representative
    And the account is deleted
    Then I see the login page