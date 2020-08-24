using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using Xunit;

/*
Feature: COVIDTemporaryExtension.feature
    As a business user who is not logged in
    I want to submit a COVID temporary extension application
    For different licence types and complete validation

@covid
Scenario: Food Primary COVID Temp Extension Application 
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Food Primary licence
    And I click on the Submit button for the COVID application
    Then the application is submitted

@covid
Scenario: Liquor Primary COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Liquor Primary licence
    And I click on the Submit button for the COVID application
    Then the application is submitted

@covid
Scenario: Liquor Primary Club COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Liquor Primary Club licence
    And I click on the Submit button for the COVID application
    Then the application is submitted

@covid
Scenario: Manufacturer COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Manufacturer licence
    And I click on the Submit button for the COVID application
    Then the application is submitted

@covid @validation
Scenario: Validate COVID Temp Extension Application
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I do not complete the temporary extension application
    And I click on the Submit button for the COVID application
    Then the required field messages are displayed
*/

namespace bdd_tests
{
    [FeatureFile("./COVIDTemporaryExtension.feature")]
    public sealed class COVIDTemporaryExtension : TestBase
    {
    }
}
