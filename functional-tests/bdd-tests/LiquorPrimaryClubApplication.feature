Feature: LiquorPrimaryClubApplication
    As a logged in business user
    I want to submit Liquor Primary Club Applications for different business types

Scenario: Liquor Primary Club Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a LPC Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary Club application for a private corporation
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary club
    And I click on the Submit button
    And I log in as a return user
    And the account is deleted
    Then I see the login page

Scenario: Liquor Primary Club Application (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a LPC Licence
    And I review the account profile for a society
    And I complete the Liquor Primary Club application for a society
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary club
    And I click on the Submit button
    And I log in as a return user
    And the account is deleted
    Then I see the login page

Scenario: Liquor Primary Club Application (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a LPC Licence
    And I review the account profile for a partnership
    And I complete the Liquor Primary Club application for a partnership
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary club
    And I click on the Submit button
    And I log in as a return user
    And the account is deleted
    Then I see the login page

Scenario: Liquor Primary Club Application (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a LPC Licence
    And I review the account profile for a sole proprietorship
    And I complete the Liquor Primary Club application for a sole proprietorship
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary club
    And I click on the Submit button
    And I log in as a return user
    And the account is deleted
    Then I see the login page