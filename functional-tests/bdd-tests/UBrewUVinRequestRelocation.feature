Feature: UbrewUvinRequestRelocation
    As a logged in business user
    I want to submit a relocation request for a UBrew / UVin licence

Scenario: UBrew / UVin Request Relocation (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a private corporation
    And I complete the UBrew / UVin application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I request a store relocation for UBrew / UVin
    And the account is deleted
    Then I see the login page