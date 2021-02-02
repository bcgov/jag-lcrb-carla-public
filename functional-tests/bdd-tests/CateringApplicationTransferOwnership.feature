 Feature: CateringApplicationTransferOwnership
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a transfer of ownership request for different business types

 @catering @privatecorporation @cateringeventtransfer
 Scenario: DEV Catering Transfer Ownership Request (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @catering @society @cateringeventtransfer
 Scenario: DEV Catering Transfer Ownership Request (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I complete the Catering application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 @catering @partnership @cateringeventtransfer
 Scenario: DEV Catering Transfer Ownership Request (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I complete the Catering application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

  @catering @soleproprietorship @cateringeventtransfer
  Scenario: DEV Catering Transfer Ownership Request (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I complete the Catering application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

 #@catering @partnership @cateringeventtransfer
 #Scenario: UAT Catering Transfer Ownership Request (Partnership)
 #   Given I am logged in to the dashboard as a partnership
 #   And I click on the Start Application button for Catering
 #   And I review the account profile for a partnership
 #   And I review the organization structure for a partnership
 #   And I click on the button for Submit Organization Information
 #   And I complete the Catering application
 #   And I click on the Submit button
 #   And I click on the button for Pay for Application
 #   And I enter the payment information
 #   And the application is approved
 #   And I pay the licensing fee 
 #   And I request a transfer of ownership
 #   And the account is deleted
 #   Then I see the login page

 #@catering @privatecorporation @cateringeventtransfer
 #Scenario: UAT Catering Transfer Ownership Request (Private Corporation)
 #   Given I am logged in to the dashboard as a private corporation
 #   And I click on the Start Application button for Catering
 #   And I review the account profile for a private corporation
 #   And I review the organization structure for a private corporation
 #   And I click on the button for Submit Organization Information
 #   And I complete the Catering application
 #   And I click on the Submit button
 #   And I click on the button for Pay for Application
 #   And I enter the payment information
 #   And the application is approved
 #   And I pay the licensing fee
 #   And I request a transfer of ownership
 #   And the account is deleted
 #   Then I see the login page

 #@catering @publiccorporation @cateringeventtransfer
 #Scenario: UAT Catering Transfer Ownership Request (Public Corporation)
 #   Given I am logged in to the dashboard as a public corporation
 #   And I click on the Start Application button for Catering
 #   And I review the account profile for a public corporation
 #   And I review the organization structure for a public corporation
 #   And I click on the button for Submit Organization Information
 #   And I complete the Catering application
 #   And I click on the Submit button
 #   And I click on the button for Pay for Application
 #   And I enter the payment information
 #   And the application is approved
 #   And I pay the licensing fee 
 #   And I request a transfer of ownership
 #   And the account is deleted
 #   Then I see the login page

 #@catering @society @cateringeventtransfer
 #Scenario: UAT Catering Transfer Ownership Request (Society)
 #   Given I am logged in to the dashboard as a society
 #   And I click on the Start Application button for Catering
 #   And I review the account profile for a society
 #   And I review the organization structure for a society
 #   And I click on the button for Submit Organization Information
 #   And I complete the Catering application
 #   And I click on the Submit button
 #   And I click on the button for Pay for Application
 #   And I enter the payment information
 #   And the application is approved
 #   And I pay the licensing fee 
 #   And I request a transfer of ownership
 #   And the account is deleted
 #   Then I see the login page

  #@catering @soleproprietorship @cateringeventtransfer
  #Scenario: UAT Catering Transfer Ownership Request (Sole Proprietorship)
  #  Given I am logged in to the dashboard as a sole proprietorship
  #  And I click on the Start Application button for Catering
  #  And I review the account profile for a sole proprietorship
  #  And I review the organization structure for a sole proprietorship
  #  And I click on the button for Submit Organization Information
  #  And I complete the Catering application
  #  And I click on the Submit button
  #  And I click on the button for Pay for Application
  #  And I enter the payment information
  #  And the application is approved
  #  And I pay the licensing fee 
  #  And I request a transfer of ownership
  #  And the account is deleted
  #  Then I see the login page