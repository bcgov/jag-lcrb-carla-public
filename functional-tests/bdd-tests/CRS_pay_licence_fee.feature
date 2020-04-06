Feature: CRS_pay_licence_fee
    As a logged in business user
    I want to pay the Cannabis Retail Store Licence Fee

Scenario: Pay CRS Licence Fee
    #Given the CRS application has been approved
    #And I am logged in to the dashboard as a private corporation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And I click on the Pay Licence Fee and Plan Store Opening link
    And I enter the estimated opening date and the opening date reason
    And I click on the payment button
    And I complete the payment
    And I click on the Licences tab
    Then the Licences tab has been updated with expiry date, download link, and change jobs