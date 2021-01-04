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
    And I enter the licence Guid
    And I click on the execute button
    Then correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Change of Name
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendChangeName
    And I click on the Try it out button
    And I enter the licence Guid
    And I click on the execute button
    Then correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Change of Status
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendChangeStatus
    And I click on the Try it out button
    And I enter the licence Guid
    And I click on the execute button
    Then correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Licence Creation Message
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendLicenceCreationMessage
    And I click on the Try it out button
    And I enter the licence Guid
    And I click on the execute button
    Then correct 200 response is displayed

@onestopswagger
Scenario: OneStop Send Program Account Details Broadcast
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for SendProgramAccountDetailsBroadcastMessage
    And I click on the Try it out button
    And I enter the licence Guid
    And I click on the execute button
    Then correct 200 response is displayed   
   
@onestopswagger
Scenario: OneStop LDB Export
    Given I click on the Swagger link for OneStop
    And I click on the Authorize button
    And I click on the Close button
    And I click on the Get button for LdbExport
    And I click on the Try it out button
    And I enter the licence Guid
    And I click on the execute button
    Then correct 200 response is displayed  