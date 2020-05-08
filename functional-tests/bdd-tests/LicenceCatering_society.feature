Feature: LicenceCatering_society
    As a logged in business user
    I want to pay the first year catering licence fee
    And complete the available application types for a society

Scenario: Pay First Year Catering Licence and Complete Applications
    # Given the Catering application has been approved
    # And I am logged in to the dashboard as a society
    Given I am logged in to the dashboard as a society
    And I click on the Licences tab
    And I pay the licensing fee
    And I click on the licence download link
    And I request an event authorization
    # And I request a valid store name or branding change
    And I request a store relocation
    And I request a third party operator
    And I request a transfer of ownership
    Then the requested applications are visible on the dashboard