Feature: ReviewOrgStructureValidation
    As a logged in business user
    I want to confirm the validation messages for the org structure

@e2e @cannabis @partnership @orgstructure
Scenario: Validation for Partnership Org Structure
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I click on the button for Submit Organization Information
    And the expected validation errors are thrown for a partnership org structure
    And the account is deleted
    Then I see the login page

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

@e2e @cannabis @publiccorporation @orgstructure
Scenario: Validation for Public Corporation Org Structure
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I click on the button for Submit Organization Information
    And the expected validation errors are thrown for a public corporation org structure
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @orgstructure
Scenario: Validation for Society Org Structure
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I click on the button for Submit Organization Information
    And the expected validation errors are thrown for a society org structure
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @orgstructure
Scenario: Validation for Sole Proprietorship Org Structure
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I click on the button for Submit Organization Information
    And the expected validation errors are thrown for a sole proprietorship org structure
    And the account is deleted
    Then I see the login page

# Note: There is no validation configured for this biz type.
# @e2e @cannabis @indigenousnation @orgstructure
# Scenario: Validation for Indigenous Nation Org Structure
#    Given I am logged in to the dashboard as an indigenous nation
#    And I click on the Start Application button for a Cannabis Retail Store
#    And I complete the eligibility disclosure
#    And I review the account profile for an indigenous nation
#    And I click on the button for Submit Organization Information
#    And the expected validation errors are thrown for an indigenous nation org structure
#    And the account is deleted
#    Then I see the login page

# Note: There is no validation configured for this biz type.
# @e2e @cannabis @localgovernment @orgstructure
# Scenario: Validation for Local Government Org Structure
#    Given I am logged in to the dashboard as a local government
#    And I click on the Start Application button for a Cannabis Retail Store
#    And I complete the eligibility disclosure
#    And I review the account profile for a local government
#    And I click on the button for Submit Organization Information
#    And the expected validation errors are thrown for a local government org structure
#    And the account is deleted
#    Then I see the login page