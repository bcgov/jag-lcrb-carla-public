Feature: ManageOffsiteStorage
    As a logged in business user
    I want to manage offsite storage on the Licences & Authorizations tab

@e2e @privatecorporation @offsitestorage
Scenario: Add Rows Offsite Storage Private Corporation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I click on the Licences tab
    And I click on the link for Manage Off-Site Storage
    And I complete the offsite storage application
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @offsitestorage
Scenario: Add and Remove Rows Offsite Storage Private Corporation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Manage Off-Site Storage
    And I complete the offsite storage application
    And I remove a row from the offsite storage application
    And the updated offsite storage application is correct
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @offsitestorage
Scenario: Add Rows and Return to Offsite Storage Private Corporation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Manage Off-Site Storage
    And I complete the offsite storage application
    And the updated offsite storage application is correct for deletion
    And I add and delete more rows to the offsite storage application
    And the updated offsite storage application is correct for deletion and addition
    And I click on the Submit button
    And the account is deleted
    Then I see the login page