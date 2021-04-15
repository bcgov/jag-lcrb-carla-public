Feature: CrsStoreRequestRelocation
    As a logged in business user
    I want to submit a CRS Application for different business types
    And request a store relocation for the approved application

Scenario: Cannabis Store Relocation (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I request a store relocation for Cannabis
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

Scenario: Cannabis Store Relocation (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a society
    And I complete the Cannabis Retail Store application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I request a store relocation for Cannabis
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

Scenario: Cannabis Store Relocation (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I request a store relocation for Cannabis
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

Scenario: Cannabis Store Relocation (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I request a store relocation for Cannabis
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And the account is deleted
    Then I see the login page