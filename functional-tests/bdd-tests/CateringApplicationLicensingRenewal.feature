Feature: CateringApplicationLicensingRenewal
    As a logged in business user
    I want to pay the first year licensing fee for an approved Catering Application
    And renew the licence

#-----------------------
# Expiry = Today
#-----------------------

@e2e @catering @privatecorporation @licencerenewal
Scenario: Negative Catering Licence Renewal Today
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
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @licencerenewal
Scenario: Positive Catering Licence Renewal Today
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
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = Yesterday
#-----------------------

@e2e @catering @privatecorporation @licencerenewal
Scenario: Negative Catering Licence Renewal Yesterday
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
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @licencerenewal
Scenario: Positive Catering Licence Renewal Yesterday
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
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = 45 Days Ago
#-----------------------

@e2e @catering @privatecorporation @licencerenewal
Scenario: Negative Catering Licence Renewal 45 Days Ago
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
    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I click on the link for Reinstate Licence
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @licencerenewal
Scenario: Positive Catering Licence Renewal 45 Days Ago
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
    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I click on the link for Reinstate Licence
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 60 Days From Today
#-----------------------------

@e2e @catering @privatecorporation @licencerenewal
Scenario: Negative Catering Licence Renewal 60 Days Future
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
    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @licencerenewal
Scenario: Positive Catering Licence Renewal 60 Days Future
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
    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 30 Days From Today
#-----------------------------

@e2e @catering @privatecorporation @licencerenewal
Scenario: Negative Catering Licence Renewal 30 Days Future
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
    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @licencerenewal
Scenario: Positive Catering Licence Renewal 30 Days Future
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
    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page