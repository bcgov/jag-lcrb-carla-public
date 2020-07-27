 Feature: CateringApplicationLicenseeRepresentative
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit a licensee representative request 

 @e2e @catering @privatecorporation @licenseerep
 Scenario: Private Corporation Licensee Representative Request
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    # And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Catering
    And I request a licensee representative
    And the account is deleted
    Then I see the login page