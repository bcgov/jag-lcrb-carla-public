Feature: CateringApplicationThirdParty
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a third party operator request for different business types

 @catering @partnership @cateringtpo
 Scenario: UAT Catering Third Party Operator Request (Partnership)
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
    And I pay the licensing fee 
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

 @catering @privatecorporation @cateringtpo
 Scenario: UAT Catering Third Party Operator Request (Private Corporation)
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
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

 @catering @privatecorporation @cateringtpo
 Scenario: DEV Catering Third Party Operator Request (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I complete the Catering application
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

 @catering @publiccorporation @cateringtpo2
 Scenario: UAT Catering Third Party Operator Request (Public Corporation)
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
    And I pay the licensing fee 
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

 @catering @society @cateringtpo2
 Scenario: UAT Catering Third Party Operator Request (Society)
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
    And I pay the licensing fee
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

 @catering @soleproprietorship @cateringtpo
 Scenario: UAT Catering Third Party Operator Request (Sole Proprietorship)
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
    And I pay the licensing fee
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page