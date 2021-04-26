Feature: LoginManageProfile
    As a business user
    I want to manage my account after log in

Scenario: Manage Account Profile
    Given I log in with the new contact and account (cantest4employee)
    And I click on my user account
    And I click on the link for Manage Account    
    And I click on the Save and Continue button
    And the dashboard is displayed
    And the account is deleted
    Then I see the login page