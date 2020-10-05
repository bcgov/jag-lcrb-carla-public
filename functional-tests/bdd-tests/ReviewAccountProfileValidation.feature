Feature: ReviewAccountProfileValidation
    As a logged in business user
    I want to confirm the validation messages for the account profile

@e2e @cannabis @privatecorporation @reviewaccount
Scenario: Validation for Private Corporation Review Account Profile
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @reviewaccount
Scenario: Validation for Partnership Review Account Profile
    Given I am logged in to the dashboard as a partnership
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @reviewaccount
Scenario: Validation for Public Corporation Review Account Profile
    Given I am logged in to the dashboard as a public corporation
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @reviewaccount
Scenario: Validation for Society Review Account Profile
    Given I am logged in to the dashboard as a society
    And I click on the link for Edit Account Profile
    And I click on the Submit button
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @reviewaccount
Scenario: Validation for Sole Proprietorship Review Account Profile
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the link for Edit Account Profile
    And I click on the Submit button   
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

 @e2e @cannabis @indigenousnation @reviewaccount
 Scenario: Validation for Indigenous Nation Review Account Profile
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the link for Edit Account Profile
    And I click on the Submit button   
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page

 @e2e @cannabis @localgovernment @reviewaccount
 Scenario: Validation for Local Government Review Account Profile
    Given I am logged in to the dashboard as a local government
    And I click on the link for Edit Account Profile
    And I click on the Submit button   
    And the expected validation errors are thrown for an account profile
    And the account is deleted
    Then I see the login page