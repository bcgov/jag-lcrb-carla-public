Feature: AgentLicenseeRepresentative
    As a logged in business user
    I want to request a licensee representative for an agent 

Scenario: Agent Licensee Representative (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for an Agent Licence
    And I review the account profile for a private corporation
    And I complete the Agent Licence application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for an Agent Licence
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I click on the link for Add Licensee Representative
    And I request a licensee representative
    And the account is deleted
    Then I see the login page