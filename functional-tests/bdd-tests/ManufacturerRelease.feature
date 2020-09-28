Feature: ManufacturerRelease
    As a logged in business user
    I want to confirm that the Manufacturer functionality is ready for release

@e2e @privatecorporation @release @winery
Scenario: Check Manufacturer Release status
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a valid store name or branding change for Manufacturing
    And I click on the Licences tab
    And I request a facility structural change
    And I click on the Licences tab
    And I request a location change
    And I click on the Licences tab
    And I request a lounge area endorsement
    And I click on the Licences tab
    And I request an on-site store endorsement
    And I request a picnic area endorsement
    And I request a special event area endorsement
    And I request structural alterations to an approved lounge or special events area
    And I request a third party operator
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page