Feature: Create_CateringApplication_society
    As a logged in business user
    I want to submit a Catering Application for a society
    To be used as test data

Scenario: Create Catering Application Society
    Given I am logged in to the dashboard as a society
    And the account is deleted
    And I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering    
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    Then I confirm the payment receipt for a Catering application 