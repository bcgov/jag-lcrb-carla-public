Feature: CateringEstablishmentNameChange
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a name branding change for different business types

Scenario: Catering Branding Change (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the button for Catering terms and conditions
    And the correct terms and conditions are displayed for Catering
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for Catering
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Catering Branding Change (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I complete the Catering application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the button for Catering terms and conditions
    And the correct terms and conditions are displayed for Catering
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for Catering
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Catering Branding Change (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I complete the Catering application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the button for Catering terms and conditions
    And the correct terms and conditions are displayed for Catering
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for Catering
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Catering Branding Change (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I complete the Catering application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the button for Catering terms and conditions
    And the correct terms and conditions are displayed for Catering
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for Catering
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page