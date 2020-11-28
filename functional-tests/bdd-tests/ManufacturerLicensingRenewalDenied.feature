Feature: ManufacturerLicensingRenewalDenied
    As a logged in business user
    I want to pay the first year licensing fee for an approved Manucturer Application
    And confirm that autorenewal is set to 'No' to prevent renewal of the licence

#-----------------------
# Expiry = Today
#-----------------------

@manufacturer @licencerenewal
Scenario:  Deny Today Winery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 322d410b-f725-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny Today Brewery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 322d410b-f725-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny Today Distillery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 322d410b-f725-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny Today Co-packer Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 322d410b-f725-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = Yesterday
#-----------------------

@manufacturer @licencerenewal
Scenario:  Deny Yesterday Winery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow e1792ccf-e40b-491f-9a9a-ee8e977749e6
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny Yesterday Brewery Licence Renewal
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
    And I pay the licensing fee
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow e1792ccf-e40b-491f-9a9a-ee8e977749e6
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny Yesterday Distillery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow e1792ccf-e40b-491f-9a9a-ee8e977749e6
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny Yesterday Co-packer Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow e1792ccf-e40b-491f-9a9a-ee8e977749e6
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = 45 Days Ago
#-----------------------

@manufacturer @licencerenewal
Scenario:  Deny 45 Days Ago Winery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 65bfe79d-f825-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny 45 Days Ago Brewery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 65bfe79d-f825-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny 45 Days Ago Distillery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 65bfe79d-f825-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny 45 Days Ago Co-packer Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 65bfe79d-f825-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 60 Days From Today
#-----------------------------

@manufacturer @licencerenewal
Scenario:  Deny 60 Days From Today Winery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow beb3243e-f825-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny 60 Days From Today Brewery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow beb3243e-f825-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny 60 Days From Today Distillery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow beb3243e-f825-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny 60 Days From Today Co-packer Licence Renewal
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
    And I pay the licensing fee
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow beb3243e-f825-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 30 Days From Today
#-----------------------------

@manufacturer @licencerenewal
Scenario:  Deny 30 Days From Today Winery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 10eaae77-f725-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny 30 Days From Today Brewery Licence Renewal
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
    And I pay the licensing fee
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 10eaae77-f725-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny 30 Days From Today Distillery Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 10eaae77-f725-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

@manufacturer @licencerenewal
Scenario:  Deny 30 Days From Today Co-packer Licence Renewal
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
    And I pay the licensing fee 
    And I click on the Licences tab
    And autorenewal is set to 'No' using Dynamics workflow 10eaae77-f725-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page