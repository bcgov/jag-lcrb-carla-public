Feature: LiquorPrimaryRelease
    As a logged in business user
    I want to run a release test for Liquor Primary

@liquorprimary 
Scenario: Liquor Primary Release Test #1 (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary application for a private corporation
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority
    And I click on the Submit button
    And I log in as a return user
    And I click on the link for Complete Application
    And I click on the Continue to Application button
    And I review the local government response for a liquor primary licence
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee
    And I click on the link for Download Licence
    And I click on the link for Application to Allow Family Food Service
    And I click on the Continue to Application button
    And I complete the Application to Allow Family Food Service
    And I click on the Submit button
    And I enter the payment information
    And I click on the Licences tab
    And I click on the link for Apply to Allow Minors in Recreation Facilities
    And I click on the Continue to Application button
    And I complete the allow minors request
    And I click on the Submit button
    And I enter the payment information
    And I click on the Licences tab
    And I click on the link for Catering Endorsement Application
    And I request a catering endorsement application
    And I click on the Submit button
    And I enter the payment information
    And I click on the Licences tab
    And I click on the link for Change to Hours of Liquor Service (within Service Hours)
    And I complete the change hours application for liquor service within service hours
    And I click on the Submit button
    And I enter the payment information
    And I click on the Licences tab
    And I click on the link for Live Theatre Request For Liquor Service
    And I click on the Continue to Application button
    And I complete the live theatre request
    And I click on the Submit button
    And I click on the Licences tab
    And I click on the link for New Outdoor Patio
    And I request a new outdoor patio application
    And I click on the Submit button
    And I click on the Licences tab
    And I click on the link for Liquor Primary Relocation Application
    And I complete a liquor primary relocation request
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

@liquorprimary 
Scenario: Liquor Primary Release Test #2 (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary application for a private corporation
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority
    And I click on the Submit button
    And I log in as a return user
    And I click on the link for Complete Application
    And I click on the Continue to Application button
    And I review the local government response for a liquor primary licence
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee
    And I click on the Licences tab
    And I click on the link for Request T&C Change Application
    And I request a T&C change application
    And I click on the Licences tab
    And I click on the link for Structural Change Application (Capacity Increase)
    And I request capacity increase structural change
    And I click on the Licences tab
    And I click on the link for Structural Change Application (No Capacity Increase)
    And I request a no capacity structural change
    And I click on the Licences tab
    And I click on the link for Temporary Extension of Licensed Area
    And I submit a liquor primary temporary extension of licensed area application
    And I click on the Licences tab
    And I click on the link for Temporary Suspension Request
    And I click on the Continue to Application button
    And I complete the temporary suspension request
    And I click on the Submit button
    And I enter the payment information
    And I click on the Licences tab
    And I click on the link for Temporary Use Area Endorsement Application
    And I submit a temporary use area endorsement application
    And I click on the Licences tab
    And I request a third party operator
    And I click on the Licences tab
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page