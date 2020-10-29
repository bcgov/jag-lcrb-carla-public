Feature: LicencePDFVerification
    As a logged in business user
    I want to create different licences and manually verify the pdf formats

Scenario: Private Corporation Cannabis Licence PDF Verification
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I click on the Licences tab
    And I click on the link for Download Licence
    Then the correct licence PDF is generated with terms and conditions and without endorsements or hours of sales
