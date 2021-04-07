Feature: LoginFlow
    As a business user
    I want to test out various log in scenarios

@manualonly @privatecorporation
Scenario: First Login
    # Account created → Account Creation Flow
    # Contact created
    # Login created and linked
    # Contact/Account shown in header
    Given I log in with the new contact and account (cantest4 - everything deleted - employee deleted)
    And the new user contact is created in Dynamics
    And the new account is created in Dynamics
    And the login is created and linked in Dynamics under Accounts > [account name] > Login
    # And this login record should be unique
    And the contact/account is shown in the page header in the portal
    Then I see the login page

@manualonly @privatecorporation
Scenario: Existing Contact Logs into New Account*
    # Account created → Account Creation Flow
    # Contact created
    # Login created and linked
    # Contact/Account shown in header
    Given I log in as an existing contact for a new account
    And the contact is already created in Dynamics (belonging to another account)
    And the login is created and linked in Dynamics under Accounts > [new account name] > Login
    # Login might be available under the contact record
    # And the login is created and linked
    And the contact/account is shown in the page header
    Then I see the login page

@manualonly @privatecorporation
Scenario: Re-Login
    # Account created → Account Creation Flow
    # Contact created
    # Login created and linked
    # Contact/Account shown in header
    Given I log in as an existing contact for an existing account
    And the contact is created in Dynamics
    And the login is created and linked in Dynamics under Accounts > [account name] > Login
    # And new accounts/contacts/logins are not being created!
    And the contact/account is shown in the page header
    Then I see the login page

@manualonly @privatecorporation
Scenario: New Contact for an existing account
    # Account created → Account Creation Flow
    # Contact created
    # Login created and linked
    # Contact/Account shown in header
    Given I log in as a new contact (cantestemployee4)
    And this is an existing account (cantest4)
    And the new contact is created in Dynamics
    And the login is created and linked in Dynamics under Accounts > [account name] > Login
    And the contact/account is shown in the page header
    Then I see the login page