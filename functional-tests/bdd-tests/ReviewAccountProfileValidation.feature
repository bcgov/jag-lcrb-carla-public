Feature: ReviewAccountProfileValidation
    As a logged in business user
    I want to confirm the validation messages for the account profile

@e2e @cannabis @privatecorporation @reviewaccount
Scenario: Validation for Private Corporation Review Account Profile
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page