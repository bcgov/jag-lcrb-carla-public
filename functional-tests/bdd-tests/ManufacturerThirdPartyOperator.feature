Feature: ManufacturerThirdPartyOperator
    As a logged in business user
    I want to request a third party operator for a manufacturer licence

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Third Party Operator
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
    And I pay the licensing fee for a Manufacturer application
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery Third Party Operator
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
    And I pay the licensing fee for a Manufacturer application
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery Third Party Operator
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
    And I pay the licensing fee for a Manufacturer application
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer Third Party Operator
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
    And I pay the licensing fee for a Manufacturer application
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page