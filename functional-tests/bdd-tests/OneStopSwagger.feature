Feature: OneStopSwagger
    As a logged in business user
    I want to test the OneStop features via Swagger

Scenario: OneStop Send Change of Address
    Given I click on the Swagger link for OneStop
    And I click on the SwaggerUI Authorize button
    And I click on the SwaggerUI second Authorize button
    And I click on the SwaggerUI Close button
    And I click on the SwaggerUI Get button for SendChangeAddress
    And I click on the SwaggerUI Try it out button for SendChangeAddress 
    And I enter the licence GUID for SendChangeAddress
    And I click on the SwaggerUI Execute button for SendChangeAddress
    And the correct 200 response is displayed
    And I click on the SwaggerUI Clear button

Scenario: OneStop Send Change of Name
    Given I click on the Swagger link for OneStop
    And I click on the SwaggerUI Authorize button
    And I click on the SwaggerUI second Authorize button
    And I click on the SwaggerUI Close button
    And I click on the SwaggerUI Get button for SendChangeName
    And I click on the SwaggerUI Try it out button for SendChangeName
    And I enter the licence GUID for SendChangeName
    And I click on the SwaggerUI Execute button for SendChangeName
    And the correct 200 response is displayed
    And I click on the SwaggerUI Clear button

Scenario: OneStop Send Change of Status
    Given I click on the Swagger link for OneStop
    And I click on the SwaggerUI Authorize button
    And I click on the SwaggerUI second Authorize button
    And I click on the SwaggerUI Close button
    And I click on the SwaggerUI Get button for SendChangeStatus
    And I click on the SwaggerUI Try it out button for SendChangeStatus
    And I enter the licence GUID for SendChangeStatus
    And I click on the SwaggerUI Execute button for SendChangeStatus
    And the correct 200 response is displayed
    And I click on the SwaggerUI Clear button

Scenario: OneStop Send Licence Creation Message
    Given I click on the Swagger link for OneStop
    And I click on the SwaggerUI Authorize button
    And I click on the SwaggerUI second Authorize button
    And I click on the SwaggerUI Close button
    And I click on the SwaggerUI Get button for SendLicenceCreationMessage
    And I click on the SwaggerUI Try it out button for SendLicenceCreationMessage
    And I enter the licence GUID for SendLicenceCreationMessage
    And I click on the SwaggerUI Execute button for SendLicenceCreationMessage
    And the correct 200 response is displayed
    And I click on the SwaggerUI Clear button

Scenario: OneStop Send Program Account Details Broadcast
    Given I click on the Swagger link for OneStop
    And I click on the SwaggerUI Authorize button
    And I click on the SwaggerUI second Authorize button
    And I click on the SwaggerUI Close button
    And I click on the SwaggerUI Get button for SendProgramAccountDetailsBroadcastMessage
    And I click on the SwaggerUI Try it out button for SendProgramAccountDetailsBroadcastMessage
    And I enter the licence GUID for SendProgramAccountDetailsBroadcastMessage
    And I click on the SwaggerUI Execute button for SendProgramAccountDetailsBroadcastMessage
    And the correct 200 response is displayed
    And I click on the SwaggerUI Clear button

# Scenario: OneStop Release
    # Given I click on the Swagger link for OneStop
    # And I click on the Authorize button
    # And I click on the second Authorize button
    # And I click on the Close button
    # And I click on the Get button for SendChangeAddress
    # And I click on the Try it out button for SendChangeAddress 
    # And I enter the licence GUID for SendChangeAddress
    # And I click on the Execute button for SendChangeAddress
    # And the correct 200 response is displayed   
    # And I click on the Get button for SendChangeName
    # And I click on the Try it out button for SendChangeName
    # And I enter the licence GUID for SendChangeName
    # And I click on the Execute button for SendChangeName
    # And the correct 200 response is displayed   
    # And I click on the Get button for SendChangeStatus
    # And I click on the Try it out button for SendChangeStatus
    # And I enter the licence GUID for SendChangeStatus
    # And I click on the Execute button for SendChangeStatus
    # And the correct 200 response is displayed   
    # And I click on the Get button for SendLicenceCreationMessage
    # And I click on the Try it out button for SendLicenceCreationMessage
    # And I enter the licence GUID for SendLicenceCreationMessage
    # And I click on the Execute button for SendLicenceCreationMessage
    # And the correct 200 response is displayed   
    # And I click on the Get button for SendProgramAccountDetailsBroadcastMessage
    # And I click on the Try it out button for SendProgramAccountDetailsBroadcastMessage
    # And I enter the licence GUID for SendProgramAccountDetailsBroadcastMessage
    # And I click on the Execute button for SendProgramAccountDetailsBroadcastMessage
    # And the correct 200 response is displayed   
    # And I click on the Get button for LdbExport
    # And I click on the Try it out button for LdbExport
    # And I click on the Execute button for LdbExport
    # And the correct 200 response is displayed 
    # Then I click on the SwaggerUI Clear button