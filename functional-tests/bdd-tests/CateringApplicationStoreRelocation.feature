Feature: CateringApplicationStoreRelocation
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a store relocation for different business types

 @catering @partnership @cateringrelocation
 Scenario: Partnership Catering Store Relocation Request
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
    And I request a store relocation for Catering
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

 @catering @privatecorporation @cateringrelocation
 Scenario: Private Corporation Catering Store Relocation Request
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
    And I request a store relocation for Catering
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

 @catering @publiccorporation @cateringrelocation
 Scenario: Public Corporation Catering Store Relocation Request
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
    And I request a store relocation for Catering
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

 @catering @society @cateringrelocation
 Scenario: Society Catering Store Relocation Request
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
    And I request a store relocation for Catering
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

 @catering @soleproprietorship @cateringrelocation
 Scenario: Sole Proprietorship Catering Store Relocation Request
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
    And I request a store relocation for Catering
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page