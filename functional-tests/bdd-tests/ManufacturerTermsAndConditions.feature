Feature: ManufacturerTermsAndConditions
    As a logged in business user
    I want to confirm the Terms and Conditions for a Manufacturer licence

Scenario: Manufacturer Terms and Conditions (Private Corporation)
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
    And I confirm the terms and conditions for a Manufacturer licence
    And the account is deleted
    Then I see the login page