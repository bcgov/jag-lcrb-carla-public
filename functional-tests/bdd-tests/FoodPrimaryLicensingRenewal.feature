Feature: FoodPrimaryLicensingRenewal
    As a logged in business user
    I want to pay the first year licensing fee for an approved Food Primary Application
    And renew the licence

#-----------------------
# Expiry = Today
#-----------------------

Scenario: Negative Catering Licence Renewal Today (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a private corporation
    And I complete the Food Primary application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Dashboard
    And the application is approved
    And I click on the link for Licences & Authorizations
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

Scenario: Positive Catering Licence Renewal Today (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a private corporation
    And I complete the Food Primary application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Dashboard
    And the application is approved
    And I click on the link for Licences & Authorizations
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page