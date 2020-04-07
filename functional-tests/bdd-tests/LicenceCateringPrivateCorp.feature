Feature: Catering_first_year_licence_fee
    As a logged in business user
    I want to pay the first year catering licence fee

Scenario: Pay First Year Catering Licence
    Given the Catering application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And I click on the Pay First Year Licensing Fee link
    And I complete the payment
    And I click on the Licences tab
    Then the Licences tab has been updated with expiry date, download link, and change jobs