Feature: CateringRelease
    As a logged in business user
    I want to confirm that the Catering functionality is ready for release

@validation @privatecorporation @release
Scenario: Check Catering Release status
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for Catering
    And I click on the button for Catering terms and conditions
    And the correct terms and conditions are displayed for Catering
    And I click on the link for Download Licence
    And I request a valid store name or branding change for Catering
    And I request an event authorization that doesn't require approval
    And the event history is updated correctly for an application without approval
    And I request a licensee representative
    And I click on the link for Dashboard
    And I request a personnel name change for a private corporation
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And I click on the Licences tab
    And I request a store relocation for Catering
    And the expiry date is changed to today
    And I renew the licence with positive responses for Catering
    # And I request a transfer of ownership
    And I click on the Licences tab
    And I request a third party operator
    And the account is deleted
    Then I see the login page