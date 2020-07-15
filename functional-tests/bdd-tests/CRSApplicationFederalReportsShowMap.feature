Feature: CRSApplicationFederalReportsShowMap
    As a logged in business user
    I want to submit a CRS Application for different business types
    And review the federal reports for the approved application

@e2e @cannabis @indigenousnation @crsfedreports2
Scenario: Indigenous Nation Federal Reports and Show Map
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And I review the federal reports
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @crsfedreports
Scenario: Partnership Federal Reports and Show Map
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And I review the federal reports
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @crsfedreports
Scenario: Private Corporation Federal Reports and Show Map
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And I review the federal reports
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @crsfedreports2
Scenario: Public Corporation Federal Reports and Show Map
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And I review the federal reports
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @crsfedreports2
Scenario: Society Federal Reports and Show Map
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a society
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And I review the federal reports
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @crsfedreports
Scenario: Sole Proprietorship Federal Reports and Show Map
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And I review the federal reports
    And the account is deleted
    Then I see the login page