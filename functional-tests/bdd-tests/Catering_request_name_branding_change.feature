Feature: Catering_request_name_branding_change
    As a logged in business user
    I would like to request a Catering name or branding change

Scenario: Request Catering Name or Branding Change
    Given the Catering application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Request Store Name or Branding Change link
    And I review the account profile
    And I submit a name or branding change application
    And I complete the payment
    And I click on the Licences tab
    Then **outcome to be confirmed**