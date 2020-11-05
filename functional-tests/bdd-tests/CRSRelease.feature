Feature: CRSRelease
    As a logged in business user
    I want to confirm that the CRS functionality is ready for release

@validation @privatecorporation @crsrelease
Scenario: Private Corporation CRS Release 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I click on the Licences tab
    And I click on the button for CRS terms and conditions
    And the correct terms and conditions are displayed for CRS
    And I request a valid store name or branding change for Cannabis
    And I click on the Licences tab
    And I click on the link for Download Licence
    And I show the store as open on the map
    And I review the federal reports
    And I click on the Licences tab
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for Cannabis
    And I click on the link for Dashboard
    And I request a personnel name change for a private corporation
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And I click on the Licences tab
    And I request a store relocation for Cannabis
    # And I request a structural change
    And I request a transfer of ownership
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    # And I confirm the structural change request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

@validation @soleproprietorship @crsrelease
Scenario: Sole Proprietorship CRS Release 
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I click on the Licences tab
    And I click on the button for CRS terms and conditions
    And the correct terms and conditions are displayed for CRS
    And I request a valid store name or branding change for Cannabis
    And I click on the Licences tab
    And I click on the link for Download Licence
    And I show the store as open on the map
    And I review the federal reports
    And I click on the Licences tab
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for Cannabis
    And I click on the link for Dashboard
    And I request a personnel name change for a sole proprietorship
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And I click on the Licences tab
    And I request a store relocation for Cannabis
    # And I request a structural change
    And I request a transfer of ownership
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    # And I confirm the structural change request is displayed on the dashboard
    And the account is deleted
    Then I see the login page