Feature: FoodPrimaryNewOutdoorPatio
    As a logged in business user
    I want to submit a new outdoor patio application for a Food Primary licence

Scenario: Food Primary New Outdoor Patio (Private Corporation)
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
    And I click on the link for New Outdoor Patio
    And I click on the Continue to Application button
    And I request a new outdoor patio application
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page