Feature: ReviewOrgStructureValidation
    As a logged in business user
    I want to confirm the validation messages for the org structure

@e2e @cannabis @privatecorporation @orgstructure
Scenario: Validation for Private Corporation Org Structure
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I click on the button for Submit Organization Information
    And the expected validation errors are thrown for a private corporation org structure
    And the account is deleted
    Then I see the login page