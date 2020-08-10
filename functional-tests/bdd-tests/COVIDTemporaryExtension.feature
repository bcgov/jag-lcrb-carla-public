Feature: COVIDTemporaryExtension.feature
    As a business user who is not logged in
    I want to submit a COVID temporary extension application
    For different licence types and complete validation

@covid
Scenario: Food Primary COVID Temp Extension Application 
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Food Primary licence
    And I click on the Submit button
    Then the application is submitted

@covid
Scenario: Liquor Primary COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Liquor Primary licence
    And I click on the Submit button
    Then the application is submitted

@covid
Scenario: Liquor Primary Club COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Liquor Primary Club licence
    And I click on the Submit button
    Then the application is submitted

@covid
Scenario: Manufacturer COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Manufacturer licence
    And I click on the Submit button
    Then the application is submitted

@covid @validation
Scenario: Validate COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I do not complete the temporary extension application
    And I click on the Submit button
    Then the required field messages are displayed