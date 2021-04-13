Feature: PermanentChangeToLicensee
    As a logged in business user
    I want to submit a licensee changes for different business types

Scenario: Catering Licensee Changes (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a private corporation
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Catering Licensee Changes (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I complete the Catering application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a partnership
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Catering Licensee Changes (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I complete the Catering application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a sole proprietorship
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Catering Licensee Changes (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I complete the Catering application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a society
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Catering Licensee Changes (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I complete the Catering application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a public corporation
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Catering Licensee Changes (Co-op)
    Given I am logged in to the dashboard as a co-op
    And I click on the Start Application button for Catering
    And I review the account profile for a co-op
    And I complete the Catering application for a co-op
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a co-op
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Catering Licensee Changes (Military Mess)
    Given I am logged in to the dashboard as a military mess
    And I click on the Start Application button for Catering
    And I review the account profile for a military mess
    And I complete the Catering application for a military mess
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I click on the button for Submit a Change
    And I complete the Permanent Change to a Licensee application for a military mess
    And I click on the Submit button
    And the account is deleted
    Then I see the login page