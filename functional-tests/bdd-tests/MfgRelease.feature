Feature: MfgRelease
    As a logged in business user
    I want to confirm that the Manufacturer functionality is ready for release

@manufacturer @winery @release1 
Scenario: Manufacturer Release #1 (Winery/Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Manufacturing
    And I click on the Licences tab
    And I request a facility structural change
    And I click on the Licences tab
    And I request a relocation change
    And I click on the Licences tab
    And I request a lounge area endorsement
    And I click on the Licences tab
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for a winery
    And the account is deleted
    Then I see the login page

@manufacturer @winery @release1 
Scenario: Manufacturer Release #2 (Winery/Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I click on the Licences tab
    And I click on the link for Picnic Area Endorsement Application
    And I request a picnic area endorsement
    And I click on the Licences tab
    And I request a special event area endorsement
    And I click on the Licences tab
    And I request structural alterations to an approved lounge or special events area
    And I click on the Licences tab
    And I click on the link for Manage Off-Site Storage
    And I complete the offsite storage application
    And I click on the secondary Submit button
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event for one date only
    And I click on the secondary Submit button
    And I click on the Licences tab
    # And I click on the link for Temporary Expanded Service Areas Application
    # And I click on the Continue to Application button
    # And I complete the TESA application
    # And I click on the Submit button
    # And I click on the Licences tab
    And I request a third party operator
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page