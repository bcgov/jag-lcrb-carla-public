Feature: CrsTermsAndConditions
    As a logged in business user
    I want to confirm the Terms and Conditions for a CRS licence 

Scenario: CRS Terms and Conditions (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I confirm the terms and conditions for a CRS licence
    And the account is deleted
    Then I see the login page