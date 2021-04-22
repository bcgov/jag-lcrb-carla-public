Feature: SpecialEventsPermitsHelpCentre
    As a logged in business user
    I want to access the Special Events Permits help pages

Scenario: SEP Help Centre (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Help Centre
    And I click on the link for Special Event Permit (SEP) application
    And I click on the link for Completing a SEP application
    And I click on the link for Permit and event types
    And I click on the link for Events on public property
    And I click on the link for Exemptions
    And I click on the link for Fees
    And I click on the link for Police and liquor inspectors
    And I click on the link for Permittee responsibilities
    And I click on the link for Social responsibility
    And I click on the link for Types and sources of liquor
    And I click on the link for Liquor, taxation and returns
    And I click on the link for Event security
    And the account is deleted
    Then I see the login page