Feature: LiquorPrimaryApplication
    As a logged in business user
    I want to submit Liquor Primary Applications for different business types

Scenario: Liquor Primary Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary application for a private corporation
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary
    And I click on the Submit button
    And I click on the overlay Submit button
    And No applications awaiting review is displayed
    And I log in as a return user
    And I click on the link for Complete Application
    And I click on the Continue to Application button
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Liquor Primary Application (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a society
    And I complete the Liquor Primary application for a society
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary
    And I click on the Submit button
    And I click on the overlay Submit button
    And No applications awaiting review is displayed
    And I log in as a return user
    And I click on the link for Complete Application
    And I click on the Continue to Application button
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Liquor Primary Application (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a partnership
    And I complete the Liquor Primary application for a partnership
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary
    And I click on the Submit button
    And I click on the overlay Submit button
    And No applications awaiting review is displayed
    And I log in as a return user
    And I click on the link for Complete Application
    And I click on the Continue to Application button
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Liquor Primary Application (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a sole proprietorship
    And I complete the Liquor Primary application for a sole proprietorship
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary
    And I click on the Submit button
    And I click on the overlay Submit button
    And No applications awaiting review is displayed
    And I log in as a return user
    And I click on the link for Complete Application
    And I click on the Continue to Application button
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Liquor Primary Application (Co-op)
    Given I am logged in to the dashboard as a co-op
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a co-op
    And I complete the Liquor Primary application for a co-op
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary
    And I click on the Submit button
    And I click on the overlay Submit button
    And No applications awaiting review is displayed
    And I log in as a return user
    And I click on the link for Complete Application
    And I click on the Continue to Application button
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Liquor Primary Application (Military Mess)
    Given I am logged in to the dashboard as a military mess
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a military mess
    And I complete the Liquor Primary application for a military mess
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary
    And I click on the Submit button
    And I click on the overlay Submit button
    And No applications awaiting review is displayed
    And I log in as a return user
    And I click on the link for Complete Application
    And I click on the Continue to Application button
    And I click on the Submit button
    And the account is deleted
    Then I see the login page

Scenario: Generate Liquor Primary Applications For Comment
    Given I am logged in to the dashboard as a private corporation
    And I generate 50 Liquor Primary Applications in Kamloops LGIN, RCMP Kamloops Police
