Feature: CRS_licence_download
    As a logged in business user
    I would like to download a cannabis retail store licence

Scenario: Download CRS Licence
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Download Licence link
    And the licence is downloaded
    Then the correct information is displayed