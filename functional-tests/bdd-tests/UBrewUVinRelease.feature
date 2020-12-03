Feature: UBrewUVinRelease
    As a logged in business user
    I want to submit a UBrew / UVin Licence application 
    And complete change requests 

@ubrewuvin @release
Scenario: UBrew / UVin Application Release Test
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a private corporation
    And I click on the Submit button
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Download Licence
    And I request a valid store name or branding change for UBrew
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And I click on the Licences tab
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And I click on the Licences tab
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page