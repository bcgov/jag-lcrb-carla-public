Feature: CRSApplicationLicensingRenewal
    As a logged in business user
    I want to pay the first year licensing fee for an approved CRS Application
    And renew the licence

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Negative CRS Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with negative responses
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Positive CRS Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with positive responses
    And the account is deleted
    Then I see the login page