Feature: CRS_request_structural_change
    As a logged in business user
    I would like to request a Cannabis Retail Store structural change

Scenario: Request CRS Structural Change
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Request a Structural Change link
    And I review the account profile
    And I submit a CRS structural change application
    And I complete the payment
    And I click on the Licences tab
    Then **outcome to be confirmed**