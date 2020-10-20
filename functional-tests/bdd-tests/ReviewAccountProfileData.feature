Feature: ReviewAccountProfileData
    As a logged in business user
    I want to confirm the saved data for the account profile

@e2e @cannabis @privatecorporation @reviewaccountdata
Scenario: Data for Private Corporation Review Account Profile
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Edit Account Profile
    And I review the account profile for a private corporation
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a private corporation account profile
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @reviewaccountdata
Scenario: Data for Partnership Review Account Profile
    Given I am logged in to the dashboard as a partnership
    And I click on the link for Edit Account Profile
    And I review the account profile for a partnership
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a partnership account profile
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @reviewaccountdata
Scenario: Data for Public Corporation Review Account Profile
    Given I am logged in to the dashboard as a public corporation
    And I click on the link for Edit Account Profile
    And I review the account profile for a public corporation
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a public corporation account profile
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @reviewaccountdata
Scenario: Data for Society Review Account Profile
    Given I am logged in to the dashboard as a society
    And I click on the link for Edit Account Profile
    And I review the account profile for a society
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a society account profile
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @reviewaccountdata
Scenario: Data for Sole Proprietorship Review Account Profile
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the link for Edit Account Profile
    And I review the account profile for a sole proprietorship
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a sole proprietorship account profile
    And the account is deleted
    Then I see the login page

 @e2e @cannabis @indigenousnation @reviewaccountdata
 Scenario: Data for Indigenous Nation Review Account Profile
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the link for Edit Account Profile
    And I review the account profile for an indigenous nation
    And I click on the link for Edit Account Profile
    And the correct data is displayed for an indigenous nation account profile
    And the account is deleted
    Then I see the login page

 @e2e @cannabis @localgovernment @reviewaccountdata
 Scenario: Data for Local Government Review Account Profile
    Given I am logged in to the dashboard as a local government
    And I click on the link for Edit Account Profile
    And I review the account profile for a local government
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a local government account profile
    And the account is deleted
    Then I see the login page

 @e2e @cannabis @university @reviewaccountdata
 Scenario: Data for University Review Account Profile
    Given I am logged in to the dashboard as a university
    And I click on the link for Edit Account Profile
    And I review the account profile for a university
    And I click on the link for Edit Account Profile
    And the correct data is displayed for a university account profile
    And the account is deleted
    Then I see the login page