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
    And the OneStop New Licence message is displayed
    And the sent date is populated
    And I click on the OneStop New Licence message
    # Correct payload to be confirmed
    Then the payload shows the programAccountStatusCode as 01 and the programAccountReasonCode is not displayed

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
    And I find the new licence
    And I update the licence Status to Cancelled
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait five minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Cancelled message is displayed
    And the sent date is populated
    And I click on Settings
    And I click on OneStop Message Items
    And I click on the OneStop Cancelled message
    Then the payload shows the programAccountStatusCode as 02 and the programAccountReasonCode as 111

@onestopdynamics @manualonly
Scenario: OneStop Remove Cancellation (Private Corporation)
    Given I am logged in to Dynamics
    And I find the cancelled licence
    And I update the licence Status to Active
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait five minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Cancellation Removed message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 01 and the programAccountReasonCode is not displayed

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
    And I update the licence Status to Dormant
    # Confirm if any other change needs to be made
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait five minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Entered Dormancy message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 02 and the programAccountReasonCode as 115

@onestopdynamics @manualonly
Scenario: OneStop End Licence Dormancy (Private Corporation)
    Given I am logged in to Dynamics
    And I find the dormant licence
    And I update the licence Status to Active
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait five minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Dormancy Ended message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 01 and the programAccountReasonCode is not displayed

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
    # The following workflow sets the expiry date to today
    And the expiry date is changed using the Dynamics workflow named 26E7E116-DACE-426A-A798-E9134D913F19
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait five minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Expired message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 02 and the programAccountReasonCode as 112

@onestopdynamics @manualonly
Scenario: OneStop Licence Renewed (Private Corporation)
    Given I am logged in to Dynamics
    And I find the expired licence
    And I update the licence Status to Active
    And I set the Renewal Date to be more than 1 year from the Effective Date
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait five minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Renewed message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 01 and the programAccountReasonCode is not displayed

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
    And I update the licence Status to Suspended
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait five minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Suspended message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 02 and the programAccountReasonCode as 114

@onestopdynamics @manualonly
Scenario: OneStop Licence End Suspension (Private Corporation)
    Given I am logged in to Dynamics
    And I find the expired licence
    And I update the licence Status to Active
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait five minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Suspension Ended message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 01 and the programAccountReasonCode is not displayed

@onestopdynamics @manualonly
Scenario: OneStop Licence Name Change (Private Corporation)
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
    And I request a valid store name or branding change for Cannabis
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait five minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Change of Name message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as ? and the programAccountReasonCode as ?

@onestopdynamics @manualonly
Scenario: OneStop Licence Address Change (Private Corporation)
# Steps to be confirmed

@onestopdynamics @manualonly
Scenario: OneStop Licence Transfer Ownership (Private Corporation)
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
    And I request a transfer of ownership
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait ? minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Licence Deemed at Transfer message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as ? and the programAccountReasonCode as ?