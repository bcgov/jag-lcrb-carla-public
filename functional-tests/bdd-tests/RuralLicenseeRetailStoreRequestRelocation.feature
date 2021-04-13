Feature: RuralLicenseeRetailStoreRequestRelocation
    As a logged in business user
    I want to request a relocation for a rural LRS application

Scenario: Rural LRS Request Relocation (Private Corporation)
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
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

Scenario: Rural LRS Request Relocation (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a public corporation
    And I complete the Rural LRS application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

Scenario: Rural LRS Request Relocation (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a partnership
    And I complete the Rural LRS application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

Scenario: Rural LRS Request Relocation (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a society
    And I complete the Rural LRS application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

Scenario: Rural LRS Request Relocation (Sole Proprietorship)
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
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the Rural LRS relocation application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page