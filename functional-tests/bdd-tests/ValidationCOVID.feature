Feature: ValidationCovid
    As a logged in business user
    I want to confirm the validation messages for the COVID temporary extension

Scenario: Validate COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I do not complete the temporary extension application
    And I click on the Submit button
    Then the COVID validation messages are displayed