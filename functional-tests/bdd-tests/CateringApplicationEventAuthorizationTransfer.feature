 Feature: CateringApplicationEventAuthorizationTransfer
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit an event authorization and transfer of ownership request for different business types

 @e2e @catering @indigenousnation @cateringeventtransfer2
 Scenario: Indigenous Nation Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @partnership @cateringeventtransfer
 Scenario: Partnership Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @privatecorporation @cateringeventtransfer
 Scenario: Private Corporation Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @publiccorporation @cateringeventtransfer2
 Scenario: Public Corporation Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @society @cateringeventtransfer2
 Scenario: Society Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

  @e2e @catering @soleproprietorship @cateringeventtransfer
  Scenario: Sole Proprietorship Event Authorization Transfer Ownership Requests
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @e2e @catering @privatecorporation @validation
 Scenario: Validation for Event Authorization 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I click on the Licences tab
    And I click on the link for event authorization
    And I do not complete the application correctly
    And the expected validation errors are thrown for an event authorization
    And the account is deleted
    Then I see the login page

 @e2e @catering @privatecorporation @validation
 Scenario: Validation for Catering Transfer of Ownership
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I click on the Licences tab
    And I click on the link for transfer of ownership
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Catering transfer of ownership
    And the account is deleted
    Then I see the login page