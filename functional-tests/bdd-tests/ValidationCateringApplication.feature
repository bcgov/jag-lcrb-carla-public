 Feature: ValidationCateringApplication
    As a logged in business user
    I want to confirm the validation messages for the Catering applications

Scenario: Validation for Catering Application 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Catering application
    And the account is deleted
    Then I see the login page

Scenario: Validation for Catering Branding Change 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Branding Change application
    And the account is deleted
    Then I see the login page

 Scenario: Validation for No Approval Event Authorization Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I request an event authorization being validated
    And the event history is updated correctly for an application being validated
    And I click on the link for Draft
    And I do not complete the event authorization application correctly
    And the expected validation errors are thrown for an event authorization
    And the account is deleted
    Then I see the login page

 Scenario: Validation for Licensee Representative Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Add Licensee Representative
    And I do not complete the application correctly
    And the expected validation errors are thrown for a licensee representative
    And the account is deleted
    Then I see the login page

Scenario: Validation for Catering Store Relocation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Catering store relocation application
    And the account is deleted
    Then I see the login page

Scenario: Validation for Catering Third Party Operator
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Add or Change a Third Party Operator
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Catering third party application
    And the account is deleted
    Then I see the login page

 Scenario: Validation for Catering Transfer of Ownership
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Transfer Licence
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Catering transfer of ownership
    And the account is deleted
    Then I see the login page