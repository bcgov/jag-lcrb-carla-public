Feature: AgentRequestTakeHomeSamplingEvent
    As a logged in business user
    I want to request a take home sampling event for an Agent licence

Scenario: Agent Take Home Sampling Event (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for an Agent Licence
    And I review the account profile for a private corporation
    And I complete the Agent Licence application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for an Agent Licence
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I click on the link for Request Take Home Sampling Event Authorization
    And I complete the Take Home Sampling Event Authorization request
    And I click on the secondary Submit button
    And the account is deleted
    Then I see the login page