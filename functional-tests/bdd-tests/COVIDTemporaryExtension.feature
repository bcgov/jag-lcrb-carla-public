Feature: COVIDTemporaryExtension
    As a business user who is not logged in
    I want to submit a COVID temporary extension application for different licence types

@covid
Scenario: Food Primary COVID Temp Extension
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Food Primary licence
    And I click on the Submit button
    Then the application is submitted

@covid
Scenario: Liquor Primary COVID Temp Extension
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Liquor Primary licence
    And I click on the Submit button
    Then the application is submitted

@covid
Scenario: Liquor Primary Club COVID Temp Extension 
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Liquor Primary Club licence
    And I click on the Submit button
    Then the application is submitted

@covid
Scenario: Manufacturer COVID Temp Extension 
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Manufacturer licence
    And I click on the Submit button
    Then the application is submitted