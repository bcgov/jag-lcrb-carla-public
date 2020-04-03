Feature: CRS_transfer_ownership
    As a logged in business user
    I would like to transfer the Cannabis Retail Store ownership

Scenario: Transfer CRS Ownership
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Transfer Ownership link
    And I identify the proposed licensee
    And I complete the consent and declarations
    And I submit the transfer
    Then **outcome to be confirmed**