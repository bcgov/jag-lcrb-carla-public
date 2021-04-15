Feature: RuralLicenseeRetailStoreApplication
    As a logged in business user
    I want to submit a rural LRS application for different business types

Scenario: Rural LRS Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a private corporation
    And I complete the Rural LRS application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Rural LRS Application (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a public corporation
    And I complete the Rural LRS application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Rural LRS Application (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a partnership
    And I complete the Rural LRS application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Rural LRS Application (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a society
    And I complete the Rural LRS application for a society
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Rural LRS Application (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a sole proprietorship
    And I complete the Rural LRS application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page