Feature: Catering_transfer_ownership
    As a logged in business user
    I want to transfer the Catering ownership

Scenario: Transfer Catering Ownership
    Given the Catering application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And I click on the Transfer Ownership link
    And I identify the proposed licensee
    And I complete the consent and declarations
    And I submit the transfer
    Then **outcome to be confirmed**