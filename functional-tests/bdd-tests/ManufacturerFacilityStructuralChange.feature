Feature: ManufacturerFacilityStructuralChange
    As a logged in business user
    I want to request a facility structural change for a manufacturer licence

#@manufacturer @winery @structuralchange
#Scenario: UAT Facility Structural Change Application (Winery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a winery
#    And I review the security screening requirements for a private corporation
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And I request a facility structural change
#    And I click on the Dashboard tab
#    And the dashboard status is updated as Application Under Review
#    And the account is deleted
#    Then I see the login page

#@manufacturer @brewery @structuralchange
#Scenario: UAT Facility Structural Change Application (Brewery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a brewery
#    And I review the security screening requirements for a private corporation
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And I request a facility structural change
#    And I click on the Dashboard tab
#    And the dashboard status is updated as Application Under Review
#    And the account is deleted
#    Then I see the login page

#@manufacturer @distillery @structuralchange
#Scenario: UAT Facility Structural Change Application (Distillery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a distillery
#    And I review the security screening requirements for a private corporation
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And I request a facility structural change
#    And I click on the Dashboard tab
#    And the dashboard status is updated as Application Under Review
#    And the account is deleted
#    Then I see the login page

#@manufacturer @copacker @structuralchange
#Scenario: UAT Facility Structural Change Application (Co-packer)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a co-packer
#    And I review the security screening requirements for a private corporation
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And I request a facility structural change
#    And I click on the Dashboard tab
#    And the dashboard status is updated as Application Under Review
#    And the account is deleted
#    Then I see the login page

@manufacturer @winery @structuralchange
Scenario: DEV Facility Structural Change Application (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I request a facility structural change
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@manufacturer @brewery @structuralchange
Scenario: DEV Facility Structural Change Application (Brewery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I request a facility structural change
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@manufacturer @distillery @structuralchange
Scenario: DEV Facility Structural Change Application (Distillery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I request a facility structural change
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@manufacturer @copacker @structuralchange
Scenario: DEV Facility Structural Change Application (Co-packer)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I request a facility structural change
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page