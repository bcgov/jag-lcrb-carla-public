Feature: ManufacturerLicensingRenewal
    As a logged in business user
    I want to pay the first year licensing fee for an approved Manucturer Application
    And renew the licence

#-----------------------
# Private Corporation
#-----------------------

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Negative Private Corporation Winery Licence Renewal
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
    And the expiry date is changed to today
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Positive Private Corporation Winery Licence Renewal
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
    And the expiry date is changed to today
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Negative Private Corporation Brewery Licence Renewal
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
    And the expiry date is changed to today
    And I renew the licence with negative responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Positive Private Corporation Brewery Licence Renewal
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
    And the expiry date is changed to today
    And I renew the licence with positive responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Negative Private Corporation Distillery Licence Renewal
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
    And the expiry date is changed to today
    And I renew the licence with negative responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Positive Private Corporation Distillery Licence Renewal
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
    And the expiry date is changed to today
    And I renew the licence with positive responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Negative Private Corporation Co-packer Licence Renewal
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
    And the expiry date is changed to today
    And I renew the licence with negative responses for a co-packer
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @privatecorporation @licencerenewal
Scenario:  Positive Private Corporation Co-packer Licence Renewal
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
    And the expiry date is changed to today
    And I renew the licence with positive responses for a co-packer
    And the account is deleted
    Then I see the login page

#-----------------------
# Public Corporation
#-----------------------

@e2e @manufacturer @publiccorporation @licencerenewal
Scenario:  Negative Public Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @publiccorporation @licencerenewal
Scenario:  Positive Public Corporation Winery Licence Renewal
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @publiccorporation @licencerenewal
Scenario:  Negative Public Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with negative responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @publiccorporation @licencerenewal
Scenario:  Positive Public Corporation Brewery Licence Renewal
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with positive responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @publiccorporation @licencerenewal
Scenario:  Negative Public Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with negative responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @publiccorporation @licencerenewal
Scenario:  Positive Public Corporation Distillery Licence Renewal
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with positive responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @publiccorporation @licencerenewal
Scenario:  Negative Public Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with negative responses for a co-packer
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @publiccorporation @licencerenewal
Scenario:  Positive Public Corporation Co-packer Licence Renewal
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with positive responses for a co-packer
    And the account is deleted
    Then I see the login page

#-----------------------
# Sole Proprietorship
#-----------------------

@e2e @manufacturer @soleproprietorship @licencerenewal
Scenario:  Negative Sole Proprietorship Winery Licence Renewal
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @soleproprietorship @licencerenewal
Scenario:  Positive Sole Proprietorship Winery Licence Renewal
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @soleproprietorship @licencerenewal
Scenario:  Negative Sole Proprietorship Brewery Licence Renewal
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with negative responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @soleproprietorship @licencerenewal
Scenario:  Positive Sole Proprietorship Brewery Licence Renewal
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with positive responses for a brewery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @soleproprietorship @licencerenewal
Scenario:  Negative Sole Proprietorship Distillery Licence Renewal
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with negative responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @soleproprietorship @licencerenewal
Scenario:  Positive Sole Proprietorship Distillery Licence Renewal
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with positive responses for a distillery
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @soleproprietorship @licencerenewal
Scenario:  Negative Sole Proprietorship Co-packer Licence Renewal
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with negative responses for a co-packer
    And the account is deleted
    Then I see the login page

@e2e @manufacturer @soleproprietorship @licencerenewal
Scenario:  Positive Sole Proprietorship Co-packer Licence Renewal
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And the expiry date is changed to today
    And I renew the licence with positive responses for a co-packer
    And the account is deleted
    Then I see the login page