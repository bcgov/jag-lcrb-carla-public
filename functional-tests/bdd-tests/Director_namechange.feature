Feature: Director_namechange.feature
    As a logged in business user
    I want to change the name of a director
    And pay the associated fee

Scenario: Change director name and pay fee
    # Given the CRS application has been approved
    Given I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And I pay the licensing fee
    And I return to the dashboard
    And I review the organization structure
    And I modify the director name
    And I submit the organization structure
    And I pay the name change fee
    Then the director name is now updated