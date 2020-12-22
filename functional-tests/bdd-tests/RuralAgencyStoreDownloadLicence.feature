Feature: RuralAgencyStoreDownloadLicence
    As a logged in business user
    I want to download a rural store licence for a private corporation

@ruralagencydownload @privatecorporation 
Scenario: Private Corporation Rural Agency Licence Download
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Rural Agency Store
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Rural Agency Store application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Download Licence
    And the account is deleted
    Then I see the login page