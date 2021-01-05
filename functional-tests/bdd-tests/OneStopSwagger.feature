Feature: OneStopSwagger
    As a logged in business user
    I want to test the OneStop features via Swagger

@onestopswagger
Scenario: OneStop Send Change of Address
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendChangeAddress
    And I click on the Try it out button for SendChangeAddress 
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button for SendChangeAddress
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Change of Name
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendChangeName
    And I click on the Try it out button for SendChangeName
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button for SendChangeName
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Change of Status
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendChangeStatus
    And I click on the Try it out button for SendChangeStatus
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button for SendChangeStatus
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Licence Creation Message
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendLicenceCreationMessage
    And I click on the Try it out button for SendLicenceCreationMessage
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button for SendLicenceCreationMessage
    Then the correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Program Account Details Broadcast
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendProgramAccountDetailsBroadcastMessage
    And I click on the Try it out button for SendProgramAccountDetailsBroadcastMessage
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button for SendProgramAccountDetailsBroadcastMessage
    Then the correct 200 response is displayed   
   
@onestopswagger
Scenario: OneStop LDB Export
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for LdbExport
    And I click on the Try it out button for LdbExport
    And I enter the licence GUID 31b08509-909b-ea11-b818-00505683fbf4
    And I click on the Execute button for LdbExport
    Then the correct 200 response is displayed  