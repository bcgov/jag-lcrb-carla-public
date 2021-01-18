Feature: ManufacturerLicensingRenewal
    As a logged in business user
    I want to pay the first year licensing fee for an approved Manucturer Application
    And renew the licence

#-----------------------
# Expiry = Today
#-----------------------

@manufacturer @licencerenewal
Scenario:  DEV Today Negative Licence Renewal (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  DEV Today Positive Licence Renewal (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Today Negative Licence Renewal (Winery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a winery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a winery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Today Positive Licence Renewal (Winery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a winery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a winery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Today Negative Licence Renewal (Brewery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a brewery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a brewery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Today Positive Licence Renewal (Brewery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a brewery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a brewery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Today Negative Licence Renewal (Distillery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a distillery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a distillery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Today Positive Licence Renewal (Distillery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a distillery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a distillery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Today Negative Licence Renewal (Co-packer)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a co-packer
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a co-packer
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Today Positive Licence Renewal (Co-packer)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a co-packer
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a co-packer
#    And the account is deleted
#    Then I see the login page

#-----------------------
# Expiry = Yesterday
#-----------------------

@manufacturer @licencerenewal
Scenario:  DEV Yesterday Negative Licence Renewal (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  DEV Yesterday Positive Licence Renewal (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C  
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Yesterday Negative Licence Renewal (Winery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a winery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a winery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Yesterday Positive Licence Renewal (Winery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a winery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C  
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a winery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Yesterday Negative Licence Renewal (Brewery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a brewery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C  
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a brewery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Yesterday Positive Licence Renewal (Brewery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a brewery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C 
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a brewery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Yesterday Negative Licence Renewal (Distillery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a distillery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C 
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a distillery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Yesterday Positive Licence Renewal (Distillery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a distillery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C  
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a distillery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Yesterday Negative Licence Renewal (Co-packer)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a co-packer
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C  
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a co-packer
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT Yesterday Positive Licence Renewal (Co-packer)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a co-packer
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C  
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a co-packer
#    And the account is deleted
#    Then I see the login page

#-----------------------
# Expiry = 45 Days Ago
#-----------------------

@manufacturer @licencerenewal
Scenario:  DEV 45 Days Ago Negative Licence Renewal (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I click on the link for Reinstate Licence
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  DEV 45 Days Ago Positive Licence Renewal (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I click on the link for Reinstate Licence
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 45 Days Ago Negative Licence Renewal (Winery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a winery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
#    And I click on the link for Reinstate Licence
#    And I renew the licence with negative responses for a winery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 45 Days Ago Positive Licence Renewal (Winery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a winery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
#    And I click on the link for Reinstate Licence
#    And I renew the licence with positive responses for a winery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 45 Days Ago Negative Licence Renewal (Brewery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a brewery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
#    And I click on the link for Reinstate Licence
#    And I renew the licence with negative responses for a brewery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 45 Days Ago Positive Licence Renewal (Brewery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a brewery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
#    And I click on the link for Reinstate Licence
#    And I renew the licence with positive responses for a brewery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 45 Days Ago Negative Licence Renewal (Distillery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a distillery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
#    And I click on the link for Reinstate Licence
#    And I renew the licence with negative responses for a distillery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 45 Days Ago Positive Licence Renewal (Distillery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a distillery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
#    And I click on the link for Reinstate Licence
#    And I renew the licence with positive responses for a distillery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 45 Days Ago Negative Licence Renewal (Co-packer)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a co-packer
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
#    And I click on the link for Reinstate Licence
#    And I renew the licence with negative responses for a co-packer
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 45 Days Ago Positive Licence Renewal (Co-packer)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a co-packer
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
#    And I click on the link for Reinstate Licence
#    And I renew the licence with positive responses for a co-packer
#    And the account is deleted
#    Then I see the login page

#-----------------------------
# Expiry = 60 Days From Today
#-----------------------------

@manufacturer @licencerenewal
Scenario:  DEV 60 Days From Today Negative Licence Renewal (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  DEV 60 Days From Today Positive Licence Renewal (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 60 Days From Today Negative Licence Renewal (Winery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a winery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a winery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 60 Days From Today Positive Licence Renewal (Winery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a winery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a winery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 60 Days From Today Negative Licence Renewal (Brewery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a brewery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a brewery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 60 Days From Today Positive Licence Renewal (Brewery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a brewery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a brewery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 60 Days From Today Negative Licence Renewal (Distillery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a distillery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a distillery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 60 Days From Today Positive Licence Renewal (Distillery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a distillery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a distillery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 60 Days From Today Negative Licence Renewal (Co-packer)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a co-packer
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a co-packer
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 60 Days From Today Positive Licence Renewal (Co-packer)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a co-packer
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a co-packer
#    And the account is deleted
#    Then I see the login page

#-----------------------------
# Expiry = 30 Days From Today
#-----------------------------

@manufacturer @licencerenewal
Scenario:  DEV 30 Days From Today Negative Licence Renewal (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  DEV 30 Days From Today Positive Licence Renewal (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 30 Days From Today Negative Licence Renewal (Winery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a winery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a winery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 30 Days From Today Positive Licence Renewal (Winery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a winery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a winery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 30 Days From Today Negative Licence Renewal (Brewery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a brewery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a brewery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 30 Days From Today Positive Licence Renewal (Brewery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a brewery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a brewery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 30 Days From Today Negative Licence Renewal (Distillery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a distillery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a distillery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 30 Days From Today Positive Licence Renewal (Distillery)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a distillery
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a distillery
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 30 Days From Today Negative Licence Renewal (Co-packer)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a co-packer
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee 
#    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
#    And I click on the link for Renew Licence
#    And I renew the licence with negative responses for a co-packer
#    And the account is deleted
#    Then I see the login page

#@manufacturer @licencerenewal
#Scenario:  UAT 30 Days From Today Positive Licence Renewal (Co-packer)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Manufacturer Licence
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Manufacturer application for a co-packer
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And I confirm the payment receipt for a Manufacturer Licence application
#    And the application is approved
#    And I pay the licensing fee
#    And the expiry date is changed using the Dynamics workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
#    And I click on the link for Renew Licence
#    And I renew the licence with positive responses for a co-packer
#    And the account is deleted
#    Then I see the login page