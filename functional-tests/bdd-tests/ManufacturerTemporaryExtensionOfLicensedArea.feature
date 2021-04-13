Feature: ManufacturerTemporaryExtensionOfLicensedArea
    As a logged in business user
    I want to request a temporary extension for a Liquor Primary Application

Scenario: Manufacturer Temporary Extension (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Temporary Extension of Licensed Area
    And I click on the Continue to Application button
    And I submit a liquor primary temporary extension of licensed area application
    And the account is deleted
    Then I see the login page