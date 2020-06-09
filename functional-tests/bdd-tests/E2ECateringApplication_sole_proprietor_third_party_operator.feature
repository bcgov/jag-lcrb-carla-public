Feature: E2ECateringApplication_sole_proprietor_third_party_operator
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a third party operator application for a sole proprietorship

Scenario: Pay First Year Catering Licence and Submit Third Party Operator Application
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
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for Catering
    And I pay the licensing fee for Catering
    And I request a third party operator
    And the account is deleted
    Then I see the login page