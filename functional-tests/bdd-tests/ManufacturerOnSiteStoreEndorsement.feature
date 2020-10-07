Feature: ManufacturerOnSiteStoreEndorsement
    As a logged in business user
    I want to request an on-site store endorsement for a manufacturer licence

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery On-Site Store Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    # And I review the organization structure for a private corporation
    # And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery On-Site Store Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    # And I review the organization structure for a private corporation
    # And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery On-Site Store Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    # And I review the organization structure for a private corporation
    # And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer On-Site Store Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    # And I review the organization structure for a private corporation
    # And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @validation
Scenario: Validation for Manufacturer On-Site Store Endorsement
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    # And I review the organization structure for a private corporation
    # And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I click on the Licences tab
    And I click on the link for On-Site Store Endorsement Application
    And I click on the Continue to Application button
    And I do not complete the application correctly
    And the expected validation errors are thrown for an on-site store endorsement
    And the account is deleted
    Then I see the login page