Feature: FoodPrimaryApplication
    As a logged in business user
    I want to submit a Food Primary Application for different business types

Scenario: Food Primary Application (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Food Primary
    And I review the account profile for a partnership
    And I complete the Food Primary application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Food Primary Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a private corporation
    And I complete the Food Primary application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Food Primary Application (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a public corporation
    And I complete the Food Primary application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Food Primary Application (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Food Primary
    And I review the account profile for a society
    And I complete the Food Primary application for a society
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Food Primary Application (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Food Primary
    And I review the account profile for a sole proprietorship
    And I complete the Food Primary application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Food Primary Application (Co-op)
    Given I am logged in to the dashboard as a co-op
    And I click on the Start Application button for Food Primary
    And I review the account profile for a co-op
    And I complete the Food Primary application for a co-op
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

Scenario: Food Primary Application (Military Mess)
    Given I am logged in to the dashboard as a military mess
    And I click on the Start Application button for Food Primary
    And I review the account profile for a military mess
    And I complete the Food Primary application for a military mess
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page