Feature: AgentTermsAndConditions
    As a logged in business user
    I want to confirm the Terms and Conditions for an Agent licence

Scenario: Agent Terms and Conditions (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for an Agent Licence
    And I review the account profile for a private corporation
    And I complete the Agent Licence application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for an Agent Licence
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I confirm the terms and conditions for an agent licence
    And the account is deleted
    Then I see the login page