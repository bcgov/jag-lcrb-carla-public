Feature: CrsRelease
    As a logged in business user
    I want to confirm that the CRS functionality is ready for release

@2release
Scenario: CRS Release (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    # And I click on the button for CRS terms and conditions
    # And the correct terms and conditions are displayed for CRS
    # And I click on the link for Request Store Name or Branding Change
    # And I click on the Continue to Application button
    # And I request a valid store name or branding change for Cannabis
    # And I click on the link for Licences & Authorizations
    # And I click on the link for Download Licence
    # And I confirm the terms and conditions for a CRS licence
    And I show the store as open on the map
    And I click on the link for Review Federal Reports
    And I review the federal reports
    # And I click on the link for Licences & Authorizations
    # And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    # And I click on the link for Renew Licence
    # And I renew the licence with positive responses for Cannabis
    And I click on the link for Licences & Authorizations
    And I request a store relocation for Cannabis
    And I click on the link for Licences & Authorizations
    And I click on the link for Request a Structural Change
    And I click on the Continue to Application button
    And I request a structural change
    # And I click on the link for Licences & Authorizations
    # And I request a transfer of ownership for Cannabis
    And the account is deleted
    Then I see the login page

Scenario: CRS Release (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the button for CRS terms and conditions
    And the correct terms and conditions are displayed for CRS
    And I click on the link for Request Store Name or Branding Change
    And I click on the Continue to Application button
    And I request a valid store name or branding change for Cannabis
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    And I show the store as open on the map
    And I click on the link for Review Federal Reports
    And I review the federal reports
    And I click on the link for Licences & Authorizations
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for Cannabis
    And I click on the link for Licences & Authorizations
    And I request a store relocation for Cannabis
    And I click on the link for Licences & Authorizations
    And I click on the link for Request a Structural Change
    And I click on the Continue to Application button
    And I request a structural change
    And I click on the link for Licences & Authorizations
    And I request a transfer of ownership for Cannabis
    And the account is deleted
    Then I see the login page