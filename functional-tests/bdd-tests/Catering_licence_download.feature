Feature: Catering_licence_download
    As a logged in business user
    I would like to download a catering licence

Scenario: Download Catering Licence
    Given the Catering application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Download Licence link
    Then the licence is downloaded