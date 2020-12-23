Feature: CannabisMarketingTransferLicence
    As a logged in business user
    I want to transfer a Cannabis Marketing Licence

@cannabismktglicencetransfer @privatecorporation
Scenario: Cannabis Marketing Application Transfer (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Marketing application for a private corporation
    And I click on the Submit button
    # And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application for Cannabis Marketing
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Marketing Licence
    And I click on the Dashboard tab
    And the application is approved
    And I click on the Licences tab
    And I click on the link for Transfer Licence
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page