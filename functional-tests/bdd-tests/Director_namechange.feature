Feature: Director_namechange.feature
    As a logged in business user
    I want to change the name of a director
    And pay the associated fee

Scenario: Change director name and pay fee
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
    And I pay the licensing fee
    And I return to the dashboard
    And I review the organization structure again
    And I modify the director name
    And I submit the organization structure
    And I pay the name change fee
    And the director name is now updated
    And the account is deleted
    Then I see the login page
