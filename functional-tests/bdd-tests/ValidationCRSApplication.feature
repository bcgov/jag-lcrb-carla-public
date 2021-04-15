Feature: ValidationCrsApplication
    As a logged in business user
    I want to confirm the validation messages for CRS Applications

Scenario: Validation for Private Corporation CRS Application 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Cannabis application
    And the account is deleted
    Then I see the login page

Scenario: Validation for IN CRS Application
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I do not complete the application correctly
    And the expected validation errors are thrown for an indigenous nation Cannabis application
    And the account is deleted
    Then I see the login page

Scenario: Validation for CRS Branding Change 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Request Store Name or Branding Change
    And I click on the Continue to Application button
    And I do not complete the application correctly
    And the expected validation errors are thrown for a CRS Branding Change application
    And the account is deleted
    Then I see the login page

Scenario: Validation for CRS Licence Renewal
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I click on the link for Renew Licence
    And I do not complete the licence renewal application correctly
    And the expected validation errors are thrown for a licence renewal application
    And the account is deleted
    Then I see the login page

Scenario: Validation for CRS Store Relocation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I do not complete the application correctly
    And the expected validation errors are thrown for a store relocation application
    And the account is deleted
    Then I see the login page

Scenario: Validation for CRS Structural Change
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Request a Structural Change
    And I click on the Continue to Application button
    And I do not complete the application correctly
    And the expected validation errors are thrown for a structural change application
    And the account is deleted
    Then I see the login page

Scenario: Validation for CRS Transfer of Ownership
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Transfer Licence
    And I do not complete the application correctly
    And the expected validation errors are thrown for a CRS transfer of ownership
    And the account is deleted
    Then I see the login page