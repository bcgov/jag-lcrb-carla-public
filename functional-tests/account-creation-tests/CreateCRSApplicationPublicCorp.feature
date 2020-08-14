Feature: CreateCRSApplicationPublicCorp
    As a logged in business user
    I want to submit a CRS Application for a public corporation
    To be used as test data

Scenario: Create CRS Application Public Corporation
    Given I am logged in to the dashboard as a public corporation
    And the account is deleted
    And I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    Then I confirm the payment receipt for a Cannabis Retail Store application