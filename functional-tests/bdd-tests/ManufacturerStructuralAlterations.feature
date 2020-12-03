Feature: ManufacturerStructuralAlterations
    As a logged in business user
    I want to request structural alterations to an approved lounge or special events area for a manufacturer licence

@manufacturer @winery @structuralalterations
Scenario: Winery Structural Alterations
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
    And I request structural alterations to an approved lounge or special events area
    And the account is deleted
    Then I see the login page

@manufacturer @brewery @structuralalterations
Scenario: Brewery Structural Alterations
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I request structural alterations to an approved lounge or special events area
    And the account is deleted
    Then I see the login page

@manufacturer @distillery @structuralalterations
Scenario: Distillery Structural Alterations
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I request structural alterations to an approved lounge or special events area
    And the account is deleted
    Then I see the login page

@manufacturer @copacker @structuralalterations
Scenario: Co-packer Structural Alterations
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I request structural alterations to an approved lounge or special events area
    And the account is deleted
    Then I see the login page