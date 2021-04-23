Feature: RuralLicenseeRetailStoreRelease
    As a logged in business user
    I want to run a release test for a rural LRS application

@9release
Scenario: Rural LRS Release (Private Corporation)
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
    # And I click on the link for Download Licence
    # And I confirm the terms and conditions for a Rural LRS licence
    And I click on the link for Request T&C Change Application
    And I click on the Continue to Application button
    And I request a T&C change application
    And I click on the Submit button
    And I click on the link for Licences & Authorizations
    # And I click on the link for Add Licensee Representative
    # And I request a licensee representative
    # And I click on the link for Licences & Authorizations
    # And I click on the link for Establishment Name Change Application
    # And I click on the Continue to Application button
    # And I request a valid store name or branding change for Rural RLS
    # And I click on the link for Licences & Authorizations
    # And I click on the link for Request Relocation
    # And I click on the Continue to Application button
    # And I complete the Rural LRS relocation application
    # And I click on the Submit button
    # And I enter the payment information
    # And I click on the link for Licences & Authorizations
    And I click on the link for Sales to Hospitality Licensees and Special Event Permittees
    And I click on the Continue to Application button
    And I request a sales to hospitality licensees and special event permittees application
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I click on the link for Structural Alteration Application
    And I click on the Continue to Application button
    And I request a Rural LRS structural alteration application
    And I click on the Submit button
    And I enter the payment information
    # And I click on the link for Licences & Authorizations
    # And I request a third party operator
    # And I click on the link for Licences & Authorizations
    # And I request a transfer of ownership for RLRS
    And the account is deleted
    Then I see the login page

Scenario: Rural LRS Release (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a sole proprietorship
    And I complete the Rural LRS application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    And I confirm the terms and conditions for a Rural LRS licence
    And I click on the link for Request T&C Change Application
    And I click on the Continue to Application button
    And I request a T&C change application
    And I click on the Submit button
    And I click on the link for Licences & Authorizations
    And I click on the link for Add Licensee Representative
    And I request a licensee representative
    And I click on the link for Licences & Authorizations
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for Rural RLS
    And I click on the link for Licences & Authorizations
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I click on the link for Sales to Hospitality Licensees and Special Event Permittees
    And I click on the Continue to Application button
    And I request a sales to hospitality licensees and special event permittees application
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I click on the link for Structural Alteration Application
    And I click on the Continue to Application button
    And I request a Rural LRS structural alteration application
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I request a third party operator
    And I click on the link for Licences & Authorizations
    And I request a transfer of ownership for RLRS
    And the account is deleted
    Then I see the login page