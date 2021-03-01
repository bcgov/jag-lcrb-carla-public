Feature: AgentApplication
    As a logged in business user
    I want to submit an Agent application for different business types

@agent @privatecorporation
Scenario: Agent Application (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for an Agent Licence
    And I review the account profile for a private corporation
    And I complete the Agent Licence application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for an Agent Licence
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@agent @society 
Scenario: Agent Application (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for an Agent Licence
    And I review the account profile for a society
    And I complete the Agent Licence application for a society
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for an Agent Licence
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@agent @partnership 
Scenario: Agent Application (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for an Agent Licence
    And I review the account profile for a partnership
    And I complete the Agent Licence application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for an Agent Licence
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@agent @soleproprietorship 
Scenario: Agent Application (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for an Agent Licence
    And I review the account profile for a sole proprietorship
    And I complete the Agent Licence application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for an Agent Licence
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page