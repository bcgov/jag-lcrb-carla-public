Feature: ManufacturerSpecialEventAreaEndorsement
    As a logged in business user
    I want to request special event area endorsement for a manufacturer licence

Scenario: Special Event Area Endorsement Application (Winery)
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
    And I click on the link for Special Event Area Endorsement Application
    And I click on the Continue to Application button
    And I request a special event area endorsement
    And the account is deleted
    Then I see the login page

Scenario: Special Event Area Endorsement Application (Brewery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Special Event Area Endorsement Application
    And I click on the Continue to Application button
    And I request a special event area endorsement
    And the account is deleted
    Then I see the login page

Scenario: Special Event Area Endorsement Application (Distillery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Special Event Area Endorsement Application
    And I click on the Continue to Application button
    And I request a special event area endorsement
    And the account is deleted
    Then I see the login page

Scenario: Special Event Area Endorsement Application (Co-packer)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Special Event Area Endorsement Application
    And I click on the Continue to Application button
    And I request a special event area endorsement
    And the account is deleted
    Then I see the login page