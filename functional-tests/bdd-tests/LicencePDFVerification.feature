Feature: LicencePDFVerification
    As a logged in business user
    I want to create different licences and manually verify the pdf formats

Scenario: Cannabis Retail Store Licence PDF Verification
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
    And I pay the licensing fee
    And I click on the Licences tab
    And I click on the link for Download Licence
    Then the correct Cannabis licence PDF is generated with terms and conditions and without endorsements or hours of sales

Scenario: Catering Licence PDF Verification
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee
    And I click on the Licences tab
    And I click on the link for Download Licence
    Then the correct Catering licence PDF is generated with terms and conditions and without endorsements or hours of sales

Scenario: Cannabis Marketing PDF Verification
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a private corporation
    And I complete the Cannabis Marketing application for a private corporation
    And I click on the Submit button
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Marketing Licence
    And the application is approved
    And I click on the Licences tab
    And I click on the link for Download Licence
    Then the correct licence PDF is generated with terms and conditions and without endorsements or hours of sales

Scenario: UBrew / UVin Licence PDF Verification
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a private corporation
    And I click on the Submit button
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I click on the link for Download Licence
    Then the correct UBrew / UVin licence PDF is generated with terms and conditions and without endorsements or hours of sales

Scenario: Rural Agency Store Licence PDF Verification
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Rural Agency Store
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Rural Agency Store application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the link for Download Licence
    Then the correct Rural Agency Store PDF is generated with terms and conditions and hours of sales and without endorsements