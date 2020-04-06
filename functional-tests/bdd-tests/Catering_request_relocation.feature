Feature: Catering_request_relocation
    As a logged in business user
    I would like to request a catering relocation

Scenario: Request Catering Relocation
    Given the Catering application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Request Relocation link
    And I review the account profile
    And I submit a licence relocation application
    And I complete the payment
    And I return to the dashboard
    Then a relocation request under review is displayed