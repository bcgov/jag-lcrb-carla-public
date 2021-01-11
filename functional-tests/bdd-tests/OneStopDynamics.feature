Feature: OneStopDynamics
    As a logged in business user
    I want to test the OneStop features and confirm the status in Dynamics

@onestopdynamics @manualonly
# Note: The Dynamics workflow for OneStop New Licences must be enabled prior to running this test
* Name of this workflow?
Scenario: OneStop New Licence (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait five minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop New Licence Message is displayed
    And the sent date is populated
    And I click on the OneStop New Licence Message
    And I review the New Licence payload
    And the programAccountStatusCode is 01
    Then the programAccountReasonCode is not displayed

@onestopdynamics @manualonly
Scenario: OneStop Cancel Licence (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I log in to Dynamics
    And I cancel the licence
    And I click on Settings
    And I click on OneStop Message Items
    And I click on the corresponding message item
    Then the payload sets the programAccountStatusCode to 02 and the programAccountReasonCode to 111

@onestopdynamics @manualonly
Scenario: OneStop Remove Cancellation (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I log in to Dynamics
    And I remove the licence cancellation    
    And I click on Settings
    And I click on OneStop Message Items
    And I click on the corresponding message item
    Then the payload sets the programAccountStatusCode to 01 and the programAccountReasonCode to null

@onestopdynamics @manualonly
Scenario: OneStop Enter Licence Dormancy (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I log in to Dynamics
    And I make the licence dormant    
    And I click on Settings
    And I click on OneStop Message Items
    And I click on the corresponding message item
    Then the payload sets the programAccountStatusCode to 02 and the programAccountReasonCode to 115

@onestopdynamics @manualonly
Scenario: OneStop End Licence Dormancy (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I log in to Dynamics
    And I end the licence dormancy    
    And I click on Settings
    And I click on OneStop Message Items
    And I click on the corresponding message item
    Then the payload sets the programAccountStatusCode to 01 and the programAccountReasonCode to null

@onestopdynamics @manualonly
Scenario: OneStop Licence Expired (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I log in to Dynamics
    And I set the licence to expired  
    And I click on Settings
    And I click on OneStop Message Items
    And I click on the corresponding message item
    Then the payload sets the programAccountStatusCode to 02 and the programAccountReasonCode to 112

@onestopdynamics @manualonly
Scenario: OneStop Licence Renewed (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I log in to Dynamics
    And I set the licence to renewed  
    And I click on Settings
    And I click on OneStop Message Items
    And I click on the corresponding message item
    Then the payload sets the programAccountStatusCode to 01 and the programAccountReasonCode to null

@onestopdynamics @manualonly
Scenario: OneStop Licence Suspended (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I log in to Dynamics
    And I set the licence to suspended  
    And I click on Settings
    And I click on OneStop Message Items
    And I click on the corresponding message item
    Then the payload sets the programAccountStatusCode to 02 and the programAccountReasonCode to 114

@onestopdynamics @manualonly
Scenario: OneStop Licence End Suspension (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I log in to Dynamics
    And I end the licence suspension
    And I click on Settings
    And I click on OneStop Message Items
    And I click on the corresponding message item
    Then the payload sets the programAccountStatusCode to 01 and the programAccountReasonCode to null