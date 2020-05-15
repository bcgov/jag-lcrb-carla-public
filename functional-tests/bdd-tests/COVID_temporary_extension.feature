Feature: COVID_temporary_extension.feature
    As a business user who is not logged in
    I want to submit a COVID temporary extension application

Scenario: Complete COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application
    And I click on the Submit button
    Then the application is submitted