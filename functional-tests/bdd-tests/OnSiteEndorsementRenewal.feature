Feature: OnSiteEndorsementRenewal
    As a logged in business user
    I want to renew a licence that expired yesterday and has an on-site endorsement

Scenario: On-Site Endorsement Licence Renewal (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I request an on-site store endorsement
    And I click on the link for Licences & Authorizations
    And the on-site endorsement application is approved
    And I click on the link for Dashboard
    And I click on the link for Pay First Year Fee
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page

Scenario: On-Site Endorsement Licence Renewal (Brewery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I request an on-site store endorsement
    And I click on the link for Licences & Authorizations
    And the on-site endorsement application is approved
    And I click on the link for Dashboard
    And I click on the link for Pay First Year Fee
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a brewery
    And the account is deleted
    Then I see the login page

Scenario: On-Site Endorsement Licence Renewal (Distillery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I request an on-site store endorsement
    And I click on the link for Licences & Authorizations
    And the on-site endorsement application is approved
    And I click on the link for Dashboard
    And I click on the link for Pay First Year Fee
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a distillery
    And the account is deleted
    Then I see the login page

Scenario: On-Site Endorsement Licence Renewal (Co-packer)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I request an on-site store endorsement
    And I click on the link for Licences & Authorizations
    And the on-site endorsement application is approved
    And I click on the link for Dashboard
    And I click on the link for Pay First Year Fee
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And the expiry date is changed using the Dynamics workflow named 26BE4A57-0066-4441-AC60-5910272C944C
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for a co-packer
    And the account is deleted
    Then I see the login page