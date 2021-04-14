Feature: CateringApplication
    As a logged in business user
    I want to submit a Catering Application for different business types

Scenario: Catering Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Catering Application (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I complete the Catering application for a society
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Catering Application (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I complete the Catering application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Catering Application (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I complete the Catering application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Catering Application (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I complete the Catering application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Catering Application (Co-op)
    Given I am logged in to the dashboard as a co-op
    And I click on the Start Application button for Catering
    And I review the account profile for a co-op
    And I complete the Catering application for a co-op
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Catering Application (Military Mess)
    Given I am logged in to the dashboard as a military mess
    And I click on the Start Application button for Catering
    And I review the account profile for a military mess
    And I complete the Catering application for a military mess
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page