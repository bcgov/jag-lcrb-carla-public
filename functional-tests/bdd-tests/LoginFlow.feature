Feature: LoginFlow
    As a business user
    I want to test out various log in scenarios

@manualonly @privatecorporation
Scenario: First Login
    # Account created → Account Creation Flow
    # Contact created
    # Login created and linked
    # Contact/Account shown in header
    Given I log in as a new user
    And this is a new account
    And the contact is created in Dynamics
    # And the login is created and linked
    And the contact/account is shown in the page header
    Then I see the login page


@manualonly @privatecorporation
Scenario: Existing Contact Logs into New Account
    # Account created → Account Creation Flow
    # Contact created
    # Login created and linked
    # Contact/Account shown in header
    Given I log in as a return user
    And this is a new account
    And the contact is created in Dynamics
    # And the login is created and linked
    And the contact/account is shown in the page header
    Then I see the login page

@manualonly @privatecorporation
Scenario: Existing Contact Logs into Existing Account for First time
    # Account created → Account Creation Flow
    # Contact created
    # Login created and linked
    # Contact/Account shown in header
    Given I log in as a return user
    And this is an existing account
    And the contact is created in Dynamics
    # And the login is created and linked
    And the contact/account is shown in the page header
    Then I see the login page

@manualonly @privatecorporation
Scenario: Re-Login
    # Account created → Account Creation Flow
    # Contact created
    # Login created and linked
    # Contact/Account shown in header
    Given I log in as a return user
    And this is an existing account
    And the contact is created in Dynamics
    # And the login is created and linked
    And the contact/account is shown in the page header
    Then I see the login page

@manualonly @privatecorporation
Scenario: New Contact for an existing account
    # Account created → Account Creation Flow
    # Contact created
    # Login created and linked
    # Contact/Account shown in header
    Given I log in as a new user
    And this is an existing account
    And the contact is created in Dynamics
    # And the login is created and linked
    And the contact/account is shown in the page header
    Then I see the login page