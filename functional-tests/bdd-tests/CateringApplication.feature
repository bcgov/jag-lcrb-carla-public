Feature: CateringApplication
    As a logged in business user
    I want to submit a Catering Application for different business types

@e2e @catering @indigenousnation @cateringapp2
Scenario: Indigenous Nation Catering Application
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    # And I review the security screening requirements for an indigenous nation
    # And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @cateringapp
Scenario: Partnership Catering Application
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    # And I review the security screening requirements for a partnership
    # And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @cateringapp
Scenario: Private Corporation Catering Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    # And I review the security screening requirements for a private corporation
    # And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @cateringapp2
Scenario: Public Corporation Catering Application
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    # And I review the security screening requirements for a public corporation
    # And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

@e2e @catering @society @cateringapp2
Scenario: Society Catering Application
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    # And I review the security screening requirements for a society
    # And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @cateringapp
Scenario: Sole Proprietorship Catering Application
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the Submit Organization Information button
    And I complete the Catering application
    And I click on the Submit button
    # And I review the security screening requirements for a sole proprietorship
    # And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page

@catering @validation @cateringappvalidation
Scenario: Catering Application Validation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I do not complete the catering application correctly
    And the expected Catering error messages are displayed
    And the account is deleted
    Then I see the login page