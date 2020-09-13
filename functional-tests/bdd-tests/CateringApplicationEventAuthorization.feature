 Feature: CateringApplicationEventAuthorization
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit an event authorization request for different business types

#-----------------------
# No Approval Requests
#-----------------------

 @e2e @catering @indigenousnation @cateringeventtransfer2
 Scenario: No Approval Indigenous Nation Event Authorization Request
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And the account is deleted
    Then I see the login page

 @e2e @catering @partnership @cateringeventtransfer
 Scenario: No Approval Partnership Event Authorization Request
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
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And the account is deleted
    Then I see the login page

 @e2e @catering @privatecorporation @cateringeventtransfer @hourlyTest
 Scenario: No Approval Private Corporation Event Authorization Request
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
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And the account is deleted
    Then I see the login page

 @e2e @catering @publiccorporation @cateringeventtransfer2
 Scenario: No Approval Public Corporation Event Authorization Request
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
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And the account is deleted
    Then I see the login page

 @e2e @catering @society @cateringeventtransfer2
 Scenario: No Approval Society Event Authorization Request
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
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And the account is deleted
    Then I see the login page

  @e2e @catering @soleproprietorship @cateringeventtransfer
  Scenario: No Approval Sole Proprietorship Event Authorization Request
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
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And the account is deleted
    Then I see the login page

 @e2e @catering @privatecorporation @validation
 Scenario: Validation for No Approval Event Authorization Request
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
    And I click on the Licences tab
    And I request an event authorization that doesn't require approval
    And I do not complete the event authorization application correctly
    And the expected validation errors are thrown for an event authorization
    And the account is deleted
    Then I see the login page

#-----------------------
# 500+ Attendees Request
#-----------------------

 Scenario: 500+ Attendees Event Authorization Request
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
    And I request an event authorization with more than 500 people
    And the event history is updated correctly for an application with more than 500 people
    And the account is deleted
    Then I see the login page

#-----------------------
# Outdoor Request
#-----------------------

 Scenario: Outdoor Event Authorization Request
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
    And I request an event authorization for an outdoor location
    And the event history is updated correctly for an application for an outdoor location
    And the account is deleted
    Then I see the login page

#-----------------------
# Indoor/Outdoor Request
#-----------------------

 Scenario: Both Indoor and Outdoor Event Authorization Request
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
    And I request an event authorization for an indoor and outdoor location
    And the event history is updated correctly for an application for an indoor and outdoor location
    And the account is deleted
    Then I see the login page

#-------------------------
# Past 2am (non-community)
#-------------------------

 Scenario: Past 2am Event Authorization Request
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
    And I request an event authorization for after 2am
    And the event history is updated correctly for an application for after 2am
    And the account is deleted
    Then I see the login page

#-------------------------
# Past 2am (community)
#-------------------------

 Scenario: Past 2am Community Event Authorization Request
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
    And I request an event authorization for a community event after 2am
    And the event history is updated correctly for an application for a community event after 2am
    And the account is deleted
    Then I see the login page

#-------------------------
# Save For Later
#-------------------------

 Scenario: Save For Later Event Authorization Request
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
    And I request an event authorization for a draft
    And the event history is updated correctly for an application for a draft
    And I click on the link for Draft
    And the saved event authorization details are correct
    And the account is deleted
    Then I see the login page