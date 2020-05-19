Feature: COVID_temporary_extension_validation.feature
    As a business user who is not logged in
    I want to confirm the required field messages for a COVID temporary extension application

Scenario: Validate COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I do not complete the temporary extension application
    And I click on the Submit button
    Then the required field messages are displayed