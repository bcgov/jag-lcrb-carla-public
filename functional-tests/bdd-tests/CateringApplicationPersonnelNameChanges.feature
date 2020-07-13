Feature: CateringApplicationPersonnelNameChanges
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit personnel name and email changes for different business types

@e2e @catering @partnership @cateringemailpartner
Scenario: Catering Partnership Personnel Email Change
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Catering
    And I change a personnel email address
    And I request a personnel name change
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @cateringemailprivcorp
Scenario: Catering Private Corporation Personnel Email Change
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Catering
    And I change a personnel email address
    And I request a personnel name change
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @cateringemailpubcorp
Scenario: Catering Public Corporation Personnel Email Change
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Catering
    And I change a personnel email address
    And I request a personnel name change
    And the account is deleted
    Then I see the login page

@e2e @catering @society @cateringemailsociety
Scenario: Catering Society Personnel Email Change
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Catering
    And I change a personnel email address
    And I request a personnel name change
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @cateringemailsoleprop
Scenario: Catering Sole Proprietorship Personnel Email Change
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile
    And I review the organization structure
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Catering
    And I change a personnel email address
    And I request a personnel name change
    And the account is deleted
    Then I see the login page