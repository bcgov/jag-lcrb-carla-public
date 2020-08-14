 Feature: CateringApplicationLicenseeRepresentative
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a licensee representative request 

 @e2e @catering @privatecorporation @licenseerep
 Scenario: Private Corporation Licensee Representative Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request a licensee representative
    And the account is deleted
    Then I see the login page

 @e2e @licenseerep @validation
 Scenario: Validation for Licensee Representative Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I click on the Licences tab
    And I click on the link for licensee representative
    And I do not complete the application correctly
    And the expected validation errors are thrown for a licensee representative
    And the account is deleted
    Then I see the login page