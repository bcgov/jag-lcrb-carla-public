Feature: ManufacturerRequestTakeHomeSamplingEvent
    As a logged in business user
    I want to request a take home sampling event for a manufacturer licence

Scenario: Manufacturer Download Licence (Winery)
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
    And I click on the link for Request Take Home Sampling Event Authorization
    And I complete the Take Home Sampling Event Authorization request
    And I click on the secondary Submit button
    And the account is deleted
    Then I see the login page