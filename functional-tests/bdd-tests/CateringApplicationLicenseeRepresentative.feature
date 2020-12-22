 Feature: CateringApplicationLicenseeRepresentative
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a licensee representative request 

 @catering @licenseerep
 Scenario: Private Corporation Licensee Representative Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request a licensee representative
    And the account is deleted
    Then I see the login page