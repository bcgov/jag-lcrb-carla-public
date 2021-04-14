Feature: ReviewAccountProfileData
    As a logged in business user
    I want to confirm the saved data for the account profile

Scenario: Data for Review Account Profile (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Edit Account Profile
    And I review the account profile for a private corporation
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a private corporation account profile
    And the account is deleted
    Then I see the login page

Scenario: Data for Review Account Profile (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the link for Edit Account Profile
    And I review the account profile for a partnership
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a partnership account profile
    And the account is deleted
    Then I see the login page

Scenario: Data for Review Account Profile (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the link for Edit Account Profile
    And I review the account profile for a public corporation
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a public corporation account profile
    And the account is deleted
    Then I see the login page

Scenario: Data for Review Account Profile (Society)
    Given I am logged in to the dashboard as a society
    And I click on the link for Edit Account Profile
    And I review the account profile for a society
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a society account profile
    And the account is deleted
    Then I see the login page

Scenario: Data for Review Account Profile (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the link for Edit Account Profile
    And I review the account profile for a sole proprietorship
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a sole proprietorship account profile
    And the account is deleted
    Then I see the login page

 Scenario: Data for Review Account Profile (Indigenous Nation)
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the link for Edit Account Profile
    And I review the account profile for an indigenous nation
    And I click on the link for Edit Account Profile
    And the correct data is displayed for an indigenous nation account profile
    And the account is deleted
    Then I see the login page

 Scenario: Data for Review Account Profile (Local Government)
    Given I am logged in to the dashboard as a local government
    And I click on the link for Edit Account Profile
    And I review the account profile for a local government
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a local government account profile
    And the account is deleted
    Then I see the login page

 Scenario: Data for Review Account Profile (University)
    Given I am logged in to the dashboard as a university
    And I click on the link for Edit Account Profile
    And I review the account profile for a university
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a university account profile
    And the account is deleted
    Then I see the login page