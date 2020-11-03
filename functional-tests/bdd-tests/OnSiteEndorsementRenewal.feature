Feature: OnSiteEndorsementRenewal
    As a logged in business user
    I want to renew a licence with an on-site endorsement

@e2e @onsiteendorsement @renewal 
Scenario: Winery On-Site Endorsement Licence Renewal
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
    And I click on the Licences tab
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I click on the Dashboard tab
    And I pay the on-site endorsement first year licensing fee
    And I click on the Licences tab
    And the expiry date is set to yesterday
    And I renew the licence with negative responses for a winery
    And the account is deleted
    Then I see the login page