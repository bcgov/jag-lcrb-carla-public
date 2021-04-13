Feature: LiquorPrimaryTemporaryChangeToHoursOfSale
    As a logged in business user
    I want to request a Liquor Primary licence temporary change to hours of sale 

Scenario: Liquor Primary Temporary Change To Hours Of Sale
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary application for a private corporation
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary
    And I click on the Submit button
    And I click on the overlay Submit button
    And No applications awaiting review is displayed
    And I log in as a return user
    And I click on the link for Complete Application
    And I click on the Continue to Application button
    And I review the local government response for a liquor primary licence
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Temporary Change to Hours of Sale
    And I click on the Continue to Application button
    And I request a temporary change to hours of sale
    And I click on the Submit button
    And the account is deleted
    Then I see the login page