Feature: CreateCRSApplicationIndigenousNation
    As a logged in business user
    I want to submit a CRS Application for an indigenous nation
    To be used as test data

Scenario: Create CRS Application Indigenous Nation
    Given I am logged in to the dashboard as an indigenous nation
    And the account is deleted
    And I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the Pay for Application button
    And I enter the payment information
    Then I confirm the payment receipt for a Cannabis Retail Store application