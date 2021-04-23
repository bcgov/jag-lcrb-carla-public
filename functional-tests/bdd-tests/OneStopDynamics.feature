Feature: OneStopDynamics
    As a logged in business user
    I want to test the OneStop features and confirm the status in Dynamics

# Note: The Dynamics workflow for OneStop New Licences must be enabled prior to running this test
# Name of this workflow: 'Licence: OneStopMessage - Issued'
# GUID: 463CF450-060B-404F-8487-004840A49D81
Scenario: OneStop New Licence (Private Corporation)
    Given I confirm that autotest account deletion is switched off
    And I am using the correct business number
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I confirm the OneStop message field is 'Yes' on the licence record
    Then the next steps are to be confirmed

Scenario: OneStop Cancel Licence (Private Corporation)
    Given I confirm that autotest account deletion is switched off
    And I am using the correct business number
    And I am logged in to the dashboard as a private corporation    
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I log in to Dynamics
    And I find the new licence
    And I update the licence Status to Cancelled
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Cancelled message is displayed
    And the sent date is populated
    And I click on Settings
    And I click on OneStop Message Items
    And I click on the OneStop Cancelled message
    Then the payload shows the programAccountStatusCode as 02 and the programAccountReasonCode as 111

Scenario: OneStop Remove Cancellation (Private Corporation)
    Given I am logged in to Dynamics
    And I find the cancelled licence
    And I update the licence Status to Active
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Cancellation Removed message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 01 and the programAccountReasonCode is not displayed

Scenario: OneStop Enter Licence Dormancy (Private Corporation)
    Given I confirm that autotest account deletion is switched off
    And I am using the correct business number
    And I am logged in to the dashboard as a private corporation 
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I log in to Dynamics
    And I select the 'Yes' option in the Dormant dropdown
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Entered Dormancy message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 02 and the programAccountReasonCode as 115

Scenario: OneStop End Licence Dormancy (Private Corporation)
    Given I am logged in to Dynamics
    And I find the dormant licence
    And I select the 'No' option in the Dormant dropdown
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Dormancy Ended message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 01 and the programAccountReasonCode is not displayed

Scenario: OneStop Licence Expired (Private Corporation)
    Given I confirm that autotest account deletion is switched off
    And I am using the correct business number
    And I am logged in to the dashboard as a private corporation 
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    # Select 45 days ago option
    And the expiry date is changed using the Dynamics workflow named 97c9eac3-9e8e-443d-83d1-6174b5a59676
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Expired message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 02 and the programAccountReasonCode as 112

Scenario: OneStop Licence Renewed (Private Corporation)
    Given I am logged in to Dynamics
    And I find the expired licence
    And I update the licence Status to Active
    And I set the Expiry Date to be 2 years from the Effective Date
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Renewed message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 01 and the programAccountReasonCode is not displayed

Scenario: OneStop Licence Suspended (Private Corporation)
    Given I confirm that autotest account deletion is switched off
    And I am using the correct business number
    And I am logged in to the dashboard as a private corporation 
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I log in to Dynamics
    And I select the 'Yes' option in the Suspended dropdown
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Suspended message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 02 and the programAccountReasonCode as 114

Scenario: OneStop Licence End Suspension (Private Corporation)
    Given I am logged in to Dynamics
    And I find the expired licence
    And I select the 'No' option in the Suspended dropdown
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Suspension Ended message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as 01 and the programAccountReasonCode is not displayed

Scenario: OneStop Licence Name Change (Private Corporation)
    Given I confirm that autotest account deletion is switched off
    And I am using the correct business number
    And I am logged in to the dashboard as a private corporation 
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for Cannabis
    And the application is approved
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Change of Name message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as ? and the programAccountReasonCode as ?

Scenario: OneStop Licence Address Change (Private Corporation)
    Given I confirm that autotest account deletion is switched off
    And I am using the correct business number
    And I am logged in to the dashboard as a private corporation 
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I request a store relocation for Cannabis
    And the application is approved
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Licence Deemed at Transfer message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as ? and the programAccountReasonCode as ?

Scenario: OneStop Licence Transfer Ownership (Private Corporation)
    Given I confirm that autotest account deletion is switched off
    And I am using the correct business number
    And I am logged in to the dashboard as a private corporation 
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I request a transfer of ownership for Cannabis
    And the application is approved
    And I go to https://one-stop-testing-b7aa30-dev.apps.silver.devops.gov.bc.ca/swagger/index.html
    And I run /api/OneStop/CheckQueue
    And I wait 2 minutes
    And I log in to Dynamics
    And I click on Settings
    And I click on OneStop Message Items
    And the OneStop Updated - Licence Deemed at Transfer message is displayed
    And the sent date is populated
    Then the payload shows the programAccountStatusCode as ? and the programAccountReasonCode as ?