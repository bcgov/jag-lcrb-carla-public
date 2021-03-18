Feature: ManufacturerPicnicAreaEndorsement
    As a logged in business user
    I want to request picnic area endorsement for a manufacturer licence

@manufacturer @winery @picnicarea
Scenario: Picnic Area Endorsement Application (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Picnic Area Endorsement Application
    And I request a picnic area endorsement
    And the account is deleted
    Then I see the login page

@manufacturer @brewery @picnicarea
Scenario: Picnic Area Endorsement Application (Brewery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Picnic Area Endorsement Application
    And I request a picnic area endorsement
    And the account is deleted
    Then I see the login page

@manufacturer @distillery @picnicarea
Scenario: Picnic Area Endorsement Application (Distillery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Picnic Area Endorsement Application
    And I request a picnic area endorsement
    And the account is deleted
    Then I see the login page

@manufacturer @copacker @picnicarea
Scenario: Picnic Area Endorsement Application (Co-packer)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Picnic Area Endorsement Application
    And I request a picnic area endorsement
    And the account is deleted
    Then I see the login page