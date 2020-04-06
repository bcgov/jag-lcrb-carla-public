Feature: Catering_third_party_operator
    As a logged in business user
    I would like to request a catering third party operator

Scenario: Request Catering Third Party Operator
    Given the Catering application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Third-Party Operator Application link
    And I review the account profile
    And I submit the third-party operator application
    And I complete the payment
    And I return to the dashboard
    Then a third party operation application under review is displayed