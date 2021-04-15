Feature: CrsDownloadLicence
    As a logged in business user
    I want to submit a CRS Application for different business types
    And download the licence for the approved application

Scenario: Download Licence (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    And the account is deleted
    Then I see the login page