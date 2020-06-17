Feature: CateringApplication
    As a logged in business user
    I want to submit a Catering Application for different business types

Scenario: Indigenous Nation Catering Application
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

Scenario: Partnership Catering Application
    Given I am logged in to the dashboard as a partnership
    And the account is deleted
    And I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

Scenario: Private Corporation Catering Application
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

Scenario: Public Corporation Catering Application
    Given I am logged in to the dashboard as a public corporation
    And the account is deleted
    And I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

Scenario: Society Catering Application
    Given I am logged in to the dashboard as a society
    And the account is deleted
    And I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

Scenario: Sole Proprietorship Catering Application
    Given I am logged in to the dashboard as a sole proprietorship
    And the account is deleted
    And I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

Scenario: Catering Application Validation
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I do not complete the catering application correctly
    Then the expected error messages are displayed