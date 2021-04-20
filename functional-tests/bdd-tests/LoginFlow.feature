Feature: LoginFlow
    As a business user
    I want to test out various log in scenarios

Scenario: First Login
    Given I log in with the new contact and account (cantest4employee)
    And the new user contact is created in Dynamics 
    And the new account is created in Dynamics
    And the login is created and linked in Dynamics under Accounts > [account name] > Login
    And this login record is unique
    And the contact/account is shown in the page header in the portal
    Then I see the login page

Scenario: Existing Contact Logs into New Account*
    Given I log in as an existing contact for a new account
    And the contact is already created in Dynamics (belonging to another account)
    And the login is created and linked in Dynamics under Accounts > [new account name] > Login
    # Login might be available under the contact record
    # And the login is created and linked
    And the contact/account is shown in the page header
    Then I see the login page

Scenario: Re-Login
    Given I log in as an existing contact for an existing account
    And the contact is created in Dynamics
    And the login is created and linked in Dynamics under Accounts > [account name] > Login
    # And new accounts/contacts/logins are not being created!
    And the contact/account is shown in the page header
    Then I see the login page

Scenario: New Contact for an existing account
    Given I log in as a new contact (cantestemployee4)
    And this is an existing account (cantest4)
    And the new contact is created in Dynamics
    And the login is created and linked in Dynamics under Accounts > [account name] > Login
    And the contact/account is shown in the page header
    Then I see the login page

Scenario: Duplicate Check By Email For Duplicate Account Creation In Error
    Given I log in as a new contact (cantestemployee4)
    And I have the same first name, last name, and email address as an existing contact
    And I have logged in before
    And this is an existing account (cantest4)
    And the new contact is not created in Dynamics
    And the login is already created and linked in Dynamics under Accounts > [account name] > Login
    And the contact/account is shown in the page header
    Then I see the login page

Scenario: Duplicate Check By Email For New Contact
    Given I log in as a new contact (cantestemployee4)
    And I have the same first name, last name, and different email address as an existing contact
    And I have not logged in before
    And this is an existing account (cantest4)
    And the new contact is created in Dynamics
    And the login is already created and linked in Dynamics under Accounts > [account name] > Login
    And the contact/account is shown in the page header
    Then I see the login page

Scenario: Duplicate Check By Email For Existing Contact With a New Role
    Given I log in as a new contact (cantestemployee4)
    And I have the same first name, last name, and email address as an existing contact
    And I have not logged in before
    And this is an existing account (cantest4)
    And this is an existing contact 
    And the login is created and linked in Dynamics under Accounts > [account name] > Login
    And the contact/account is shown in the page header
    Then I see the login page