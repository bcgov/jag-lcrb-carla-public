Feature: CateringApplicationThirdParty
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a third party operator request for different business types

 @e2e @catering @partnership @cateringtpo
 Scenario: Partnership Catering Third Party Operator Request
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request a third party operator
    And the account is deleted
    Then I see the login page

 @e2e @catering @privatecorporation @cateringtpo
 Scenario: Private Corporation Catering Third Party Operator Request
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
    And I pay the licensing fee for Catering
    And I request a third party operator
    And the account is deleted
    Then I see the login page

 @e2e @catering @publiccorporation @cateringtpo2
 Scenario: Public Corporation Catering Third Party Operator Request
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request a third party operator
    And the account is deleted
    Then I see the login page

 @e2e @catering @society @cateringtpo2
 Scenario: Society Catering Third Party Operator Request
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request a third party operator
    And the account is deleted
    Then I see the login page

 @e2e @catering @soleproprietorship @cateringtpo
 Scenario: Sole Proprietorship Catering Third Party Operator Request
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request a third party operator
    And the account is deleted
    Then I see the login page