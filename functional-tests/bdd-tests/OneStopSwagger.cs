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
Feature: OneStopSwagger
    As a logged in business user
    I want to test the OneStop features via Swagger

@onestopswagger
Scenario: OneStop Send Change of Address
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendChangeAddress
    And I click on the Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Change of Name
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendChangeName
    And I click on the Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Change of Status
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendChangeStatus
    And I click on the Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Licence Creation Message
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendLicenceCreationMessage
    And I click on the Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Program Account Details Broadcast
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendProgramAccountDetailsBroadcastMessage
    And I click on the Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button
    Then the correct 200 response is displayed   
   
@onestopswagger
Scenario: OneStop LDB Export
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for LdbExport
    And I click on the Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button
    Then the correct 200 response is displayed         
*/

namespace bdd_tests
{
    [FeatureFile("./OneStopSwagger.feature")]
    [Collection("Liquor")]
    public sealed class OneStopSwagger : TestBase
    {
    }
}