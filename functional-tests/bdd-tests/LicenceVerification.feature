Feature: LicenceVerification
    As a logged in business user
    I want to create different licences and manually verify the pdf formats

Scenario: Cannabis Retail Store Licence PDF Verification (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation   
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee for Cannabis
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    Then the correct licence PDF is generated with terms and conditions and without endorsements or hours of sales

Scenario: Cannabis Marketing Licence PDF Verification (Private Corporation)
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
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    Then the correct licence PDF is generated with terms and conditions and without endorsements or hours of sales

Scenario: Catering Licence PDF Verification (Private Corporation)
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
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee for Catering
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    Then the correct Catering licence PDF is generated with terms and conditions and without endorsements or hours of sales

Scenario: UBrew / UVin Licence PDF Verification (Private Corporation)
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
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee for UBrew / UVin
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    Then the correct UBrew / UVin licence PDF is generated with terms and conditions and without endorsements or hours of sales

Scenario: Winery Licence PDF Verification (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    Then the correct winery licence PDF is generated with hours of sale and terms and conditions

Scenario: Brewery Licence PDF Verification (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a brewery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    Then the correct brewery licence PDF is generated with hours of sale and terms and conditions

Scenario: Distillery Licence PDF Verification (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a distillery
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    Then the correct distillery licence PDF is generated with hours of sale and terms and conditions

Scenario: Co-packer Licence PDF Verification (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a co-packer
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    Then the correct co-packer licence PDF is generated with hours of sale and terms and conditions

Scenario: Rural Agency Store Licence PDF Verification (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Rural Agency Store
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Rural Agency Store application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    Then the correct Rural Agency Store PDF is generated with terms and conditions and hours of sales and without endorsements

Scenario: Food Primary Licence PDF Verification (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a private corporation
    And I complete the Food Primary application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    Then the correct licence PDF is generated with hours of sale and terms and conditions

Scenario: Liquor Primary Licence PDF Verification
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary application for a private corporation
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary
    And I log in as a return user
    And I click on the link for Complete Application
    And I review the local government response for a liquor primary licence
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    Then the correct licence PDF is generated with hours of sale and terms and conditions