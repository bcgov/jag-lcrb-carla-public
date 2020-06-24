Feature: CRSApplicationShowStoreOnMap
    As a logged in business user
    I want to submit a CRS Application for different business types
    And view the store on the map for the approved application

@e2e @cannabis @indigenousnation
Scenario: Indigenous Nation Show Store On Map
    Given I am logged in to the dashboard as an indigenous nation
    And the account is deleted
    And I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Cannabis Retail Store application
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for a Cannabis Retail Store
    # And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership
Scenario: Partnership Show Store On Map
    Given I am logged in to the dashboard as a partnership
    And the account is deleted
    And I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Cannabis Retail Store application
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for a Cannabis Retail Store
    # And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation
Scenario: Private Corporation Show Store On Map
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Cannabis Retail Store application
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for a Cannabis Retail Store
    # And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation
Scenario: Public Corporation Show Store On Map
    Given I am logged in to the dashboard as a public corporation
    And the account is deleted
    And I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Cannabis Retail Store application
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for a Cannabis Retail Store
    # And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society
Scenario: Society Show Store On Map
    Given I am logged in to the dashboard as a society
    And the account is deleted
    And I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Cannabis Retail Store application
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for a Cannabis Retail Store
    # And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship
Scenario: Sole Proprietorship Show Store On Map
    Given I am logged in to the dashboard as a sole proprietorship
    And the account is deleted
    And I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I submit the organization structure
    And I complete the Cannabis Retail Store application
    And I click on the Pay for Application button
    And I enter the payment information
    And I return to the dashboard
    And the application is approved
    And I click on the Licences tab for a Cannabis Retail Store
    # And I pay the licensing fee for Cannabis
    And I show the store as open on the map
    And the account is deleted
    Then I see the login page