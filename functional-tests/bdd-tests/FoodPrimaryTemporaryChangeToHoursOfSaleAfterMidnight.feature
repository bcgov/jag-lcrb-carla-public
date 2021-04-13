Feature: FoodPrimaryTemporaryChangeToHoursOfSaleAfterMidnight
    As a logged in business user
    I want to request a Food Primary licence temporary change to hours of sale after midnight

Scenario: Food Primary Temp Change to Hours of Sale After Midnight (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a private corporation
    And I complete the Food Primary application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I click on the link for Temporary Change to Hours of Sale (After Midnight)
    And I click on the Continue to Application button
    And I request an after midnight temporary change to hours of sale
    And I click on the Submit button
    And the account is deleted
    Then I see the login page