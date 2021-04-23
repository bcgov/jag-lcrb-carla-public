Feature: ReviewAccountProfileValidation
    As a logged in business user
    I want to confirm the validation messages for the account profile

Scenario: Validation for Review Account Profile (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

Scenario: Validation for Review Account Profile (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

Scenario: Validation for Review Account Profile (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

Scenario: Validation for Review Account Profile (Society)
    Given I am logged in to the dashboard as a society
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

Scenario: Validation for Review Account Profile (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the link for Edit Account Profile
    And I click on the Submit button   
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

 Scenario: Validation for Review Account Profile (Indigenous Nation)
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the link for Edit Account Profile
    And I click on the Submit button   
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

 Scenario: Validation for Review Account Profile (Local Government)
    Given I am logged in to the dashboard as a local government
    And I click on the link for Edit Account Profile
    And I click on the Submit button   
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page