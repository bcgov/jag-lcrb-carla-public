Feature: CRS_request_store_name_branding_change
    As a logged in business user
    I would like to request a Cannabis Retail Store name or branding change

Scenario: Request CRS Store Name or Branding Change
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Request Store Name or Branding Change link
    And I review the account profile
    And I submit a valid name or branding change application
    And I complete the payment
    And I return to the dashboard
    Then a name or branding change application under review is displayed