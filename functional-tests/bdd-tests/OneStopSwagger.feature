Feature: OneStopSwagger
    As a logged in business user
    I want to test the OneStop features via Swagger

@onestopswagger
Scenario: OneStop Send Change of Address
    Given I click on the Swagger link for OneStop
    And I click on the SwaggerUI Authorize button
    And I click on the SwaggerUI Close button
    And I click on the SwaggerUI Get button for SendChangeAddress
    And I click on the SwaggerUI Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the SwaggerUI Execute button
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Change of Name
    Given I click on the Swagger link for OneStop
    And I click on the SwaggerUI Authorize button
    And I click on the SwaggerUI Close button
    And I click on the SwaggerUI Get button for SendChangeName
    And I click on the SwaggerUI Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the SwaggerUI Execute button
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Change of Status
    Given I click on the Swagger link for OneStop
    And I click on the SwaggerUI Authorize button
    And I click on the SwaggerUI Close button
    And I click on the SwaggerUI Get button for SendChangeStatus
    And I click on the SwaggerUI Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the SwaggerUI Execute button
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Licence Creation Message
    Given I click on the Swagger link for OneStop
    And I click on the SwaggerUI Authorize button
    And I click on the SwaggerUI Close button
    And I click on the SwaggerUI Get button for SendLicenceCreationMessage
    And I click on the SwaggerUI Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the SwaggerUI Execute button
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Program Account Details Broadcast
    Given I click on the Swagger link for OneStop
    And I click on the SwaggerUI Authorize button
    And I click on the SwaggerUI Close button
    And I click on the SwaggerUI Get button for SendProgramAccountDetailsBroadcastMessage
    And I click on the SwaggerUI Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the SwaggerUI Execute button
    Then the correct 200 response is displayed   
   
@onestopswagger
Scenario: OneStop LDB Export
    Given I click on the Swagger link for OneStop
    And I click on the SwaggerUI Authorize button
    And I click on the SwaggerUI Close button
    And I click on the SwaggerUI Get button for LdbExport
    And I click on the SwaggerUI Try it out button
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the SwaggerUI Execute button
    Then the correct 200 response is displayed  