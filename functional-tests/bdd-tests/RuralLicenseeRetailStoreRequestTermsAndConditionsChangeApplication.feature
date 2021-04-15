Feature: RuralLicenseeRetailStoreRequestTermsAndConditionsChangeApplication
    As a logged in business user
    I want to request a T&C change application for a rural LRS licence

Scenario: Rural LRS T&C Change Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a private corporation
    And I complete the Rural LRS application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Request T&C Change Application
    And I click on the Continue to Application button
    And I request a T&C change application
    And I click on the Submit button
    And the account is deleted
    Then I see the login page