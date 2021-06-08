Feature: CateringRelease
    As a logged in business user
    I want to confirm that the Catering functionality is ready for release

@7release
Scenario: Catering Release (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    # And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    # And I click on the Continue to Application button
    # And I request a change in terms and conditions application
    # And I click on the Submit button
    # And I enter the payment information
    # And I click on the link for Licences & Authorizations
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for Catering
    And I click on the link for Licences & Authorizations
    # And I click on the link for Add Licensee Representative
    # And I request a licensee representative
    # And I click on the link for Licences & Authorizations
    # And I request a store relocation for Catering
    # And I click on the link for Licences & Authorizations
    # And I request an event authorization that doesn't require approval
    # And the event history is updated correctly for an application without approval
    # And I click on the link for Licences & Authorizations
    And I request a third party operator
    And I click on the link for Licences & Authorizations
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I click on the Continue to Application button
    And I renew the licence with positive responses for Catering
    # And I request a transfer of ownership for Catering
    And the account is deleted
    Then I see the login page

Scenario: Catering Release (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I complete the Catering application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the button for Catering terms and conditions
    And the correct terms and conditions are displayed for Catering
    And I click on the link for Download Licence
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for Catering
    And I click on the link for Licences & Authorizations
    And I click on the link for Add Licensee Representative
    And I request a licensee representative
    And I click on the link for Licences & Authorizations
    And I request a store relocation for Catering
    And I click on the link for Licences & Authorizations
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And I click on the link for Licences & Authorizations
    And I request a third party operator
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I click on the Continue to Application button
    And I renew the licence with positive responses for Catering
    And I click on the link for Licences & Authorizations
    And I request a transfer of ownership for Catering
    And the account is deleted
    Then I see the login page