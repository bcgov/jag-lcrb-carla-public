Feature: AgentRelease
    As a logged in business user
    I want to run a release test for Agent licence

@4release
Scenario: Agent Release Test (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for an Agent Licence
    And I review the account profile for a private corporation
    And I complete the Agent Licence application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for an Agent Licence
    And the application is approved
    And I click on the link for Licences & Authorizations
    # And I click on the link for Download Licence
    # And I confirm the terms and conditions for an agent licence
    And I click on the link for Add Licensee Representative
    And I request a licensee representative
    And I click on the link for Licences & Authorizations
    And I click on the link for Request Take Home Sampling Event Authorization
    And I complete the Take Home Sampling Event Authorization request
    And I click on the secondary Submit button
    # And I click on the link for Licences & Authorizations
    # And I request a transfer of ownership for an agent
    And the account is deleted
    Then I see the login page

Scenario: Agent Release Test (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for an Agent Licence
    And I review the account profile for a sole proprietorship
    And I complete the Agent Licence application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for an Agent Licence
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    And I confirm the terms and conditions for an agent licence
    And I click on the link for Add Licensee Representative
    And I request a licensee representative
    And I click on the link for Licences & Authorizations
    And I click on the link for Request Take Home Sampling Event Authorization
    And I complete the Take Home Sampling Event Authorization request
    And I click on the secondary Submit button
    And I click on the link for Licences & Authorizations
    And I request a transfer of ownership for an agent
    And the account is deleted
    Then I see the login page