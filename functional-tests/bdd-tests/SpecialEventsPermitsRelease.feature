Feature: SpecialEventsPermitsRelease
    As a logged in business user
    I want to run a Special Events Permits release test

@release11
Scenario: SEP Release (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    # Start New Application button
    And I click on the Submit button
    # Start Application button (splash)
    And I click on the button for SEP Start Application
    And I complete the special events permits applicant info
    # Next button
    And I click on the Submit button
    # Next button
    And I click on the Submit button
    # To be completed
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for See Checklist 
    And the SEP Checklist content is displayed
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
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for Edit Account Profile
    And Account Profile is displayed
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for Go To My Applications
    And the Current Applications label is displayed
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for Plan Your Drinks
    And the Plan Your Drinks label is displayed
    And the account is deleted
    Then I see the login page