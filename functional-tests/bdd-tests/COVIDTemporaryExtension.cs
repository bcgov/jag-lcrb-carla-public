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
Feature: COVIDTemporaryExtension
    As a business user who is not logged in
    I want to submit a COVID temporary extension application for different licence types

@covid @release2
Scenario: COVID Temp Extension (Food Primary)
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Food Primary licence
    And I click on the secondary Submit button
    Then the application is submitted

@covid 
Scenario: COVID Temp Extension (Liquor Primary)
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Liquor Primary licence
    And I click on the secondary Submit button
    Then the application is submitted

@covid 
Scenario: COVID Temp Extension (Liquor Primary Club)
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Liquor Primary Club licence
    And I click on the secondary Submit button
    Then the application is submitted

@covid 
Scenario: COVID Temp Extension (Manufacturer)
    Given I am not logged in to the Liquor and Cannabis Portal
    And I click on the COVID Temporary Extension link
    And I complete the temporary extension application for a Manufacturer licence
    And I click on the secondary Submit button
    Then the application is submitted
*/

namespace bdd_tests
{
    [FeatureFile("./COVIDTemporaryExtension.feature")]
    public sealed class COVIDTemporaryExtension : TestBase
    {
    }
}
