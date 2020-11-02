Feature: ManufacturerLicensingRenewal
    As a logged in business user
    I want to pay the first year licensing fee for an approved Manucturer Application
    And renew the licence

#-----------------------
# Expiry = Today
#-----------------------

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Negative Private Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Positive Private Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Negative Private Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with negative responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Positive Private Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with positive responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Negative Private Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with negative responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Positive Private Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with positive responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Negative Private Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with negative responses for a co-packer
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Today Positive Private Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I renew the licence with positive responses for a co-packer
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = Yesterday
#-----------------------

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Yesterday Negative Private Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Yesterday Positive Private Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Yesterday Negative Private Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I renew the licence with negative responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Yesterday Positive Private Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I renew the licence with positive responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Yesterday Negative Private Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I renew the licence with negative responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Yesterday Positive Private Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I renew the licence with positive responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Yesterday Negative Private Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I renew the licence with negative responses for a co-packer
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Yesterday Positive Private Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I renew the licence with positive responses for a co-packer
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = 45 Days Ago
#-----------------------

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  45 Days Ago Negative Private Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  45 Days Ago Positive Private Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  45 Days Ago Negative Private Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I renew the licence with negative responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  45 Days Ago Positive Private Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I renew the licence with positive responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  45 Days Ago Negative Private Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I renew the licence with negative responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  45 Days Ago Positive Private Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I renew the licence with positive responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  45 Days Ago Negative Private Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I renew the licence with negative responses for a co-packer
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  45 Days Ago Positive Private Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I renew the licence with positive responses for a co-packer
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 60 Days From Today
#-----------------------------

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  60 Days From Today Negative Private Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  60 Days From Today Positive Private Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  60 Days From Today Negative Private Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I renew the licence with negative responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  60 Days From Today Positive Private Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I renew the licence with positive responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  60 Days From Today Negative Private Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I renew the licence with negative responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  60 Days From Today Positive Private Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I renew the licence with positive responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  60 Days From Today Negative Private Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I renew the licence with negative responses for a co-packer
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  60 Days From Today Positive Private Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named aeb0a12f-ec61-4774-a7be-2cff9ffa1cd5
    And I renew the licence with positive responses for a co-packer
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 30 Days From Today
#-----------------------------

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  30 Days From Today Negative Private Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  30 Days From Today Positive Private Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  30 Days From Today Negative Private Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I renew the licence with negative responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  30 Days From Today Positive Private Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I renew the licence with positive responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  30 Days From Today Negative Private Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I renew the licence with negative responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  30 Days From Today Positive Private Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I renew the licence with positive responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  30 Days From Today Negative Private Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I renew the licence with negative responses for a co-packer
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  30 Days From Today Positive Private Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed using the workflow named 0EA6A9CA-AC55-44CB-A1BE-1B6E420DD69B
    And I renew the licence with positive responses for a co-packer
    And the account is deleted
    Then I see the login page