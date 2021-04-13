Feature: CovidTemporaryExtension
    As a business user who is not logged in
    I want to submit a COVID temporary extension application for different licence types

Scenario: COVID Temp Extension (Food Primary)
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Food Primary licence
    And I click on the secondary Submit button
    Then the application is submitted

Scenario: COVID Temp Extension (Liquor Primary)
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Liquor Primary licence
    And I click on the secondary Submit button
    Then the application is submitted

Scenario: COVID Temp Extension (Liquor Primary Club)
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Liquor Primary Club licence
    And I click on the secondary Submit button
    Then the application is submitted

Scenario: COVID Temp Extension (Manufacturer)
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Manufacturer licence
    And I click on the secondary Submit button
    Then the application is submitted