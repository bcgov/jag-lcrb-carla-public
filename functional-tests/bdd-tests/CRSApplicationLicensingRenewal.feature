Feature: CRSApplicationLicensingRenewal
    As a logged in business user
    I want to pay the first year licensing fee for an approved CRS Application
    And renew the licence

#-----------------------
# Expiry = Today
#-----------------------

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Negative Private Corporation CRS Licence Renewal Today
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Positive Private Corporation CRS Licence Renewal Today
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = Yesterday
#-----------------------

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Negative Private Corporation CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed using the workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Positive Private Corporation CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed using the workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = 45 Days Ago
#-----------------------

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Negative Private Corporation CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed using the workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Positive Private Corporation CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed using the workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 60 Days From Today
#-----------------------------

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Negative Private Corporation CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed using the workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Positive Private Corporation CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed using the workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 30 Days From Today
#-----------------------------

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Negative Private Corporation CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed using the workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Positive Private Corporation CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed using the workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page