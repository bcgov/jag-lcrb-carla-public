Feature: CRS_request_relocation
    As a logged in business user
    I would like to request a Cannabis Retail Store relocation

Scenario: Request CRS Relocation
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Request Relocation link
    And I review the account profile
    And I submit a licence relocation application
    And I complete the payment
    And I return to the dashboard
    Then a relocation request under review is displayed