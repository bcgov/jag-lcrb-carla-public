Feature: ManufacturerAddOrChangeAThirdPartyOperator
    As a logged in business user
    I want to request a third party operator for a manufacturer licence

Scenario: Third Party Operator (Winery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

Scenario: Third Party Operator (Brewery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

Scenario: Third Party Operator (Distillery)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

Scenario: Third Party Operator (Co-packer)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page