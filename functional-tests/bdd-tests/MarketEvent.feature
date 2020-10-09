Feature: MarketEvent
    As a logged in business user
    I want to submit a market event for different manufacturer types

@e2e @privatecorporation @marketevent @winery
Scenario: Private Corporation Winery Market Event
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
    And I click on the Licences tab
    And I pay the licensing fee for a Manufacturer application
    And I request a market event
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @marketevent @distillery
Scenario: Private Corporation Distillery Market Event
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
    And I click on the Licences tab
    And I pay the licensing fee for a Manufacturer application
    And I request a market event
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @marketevent @brewery
Scenario: Private Corporation Brewery Market Event
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
    And I click on the Licences tab
    And I pay the licensing fee for a Manufacturer application
    And I request a market event
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @marketevent @copacker
Scenario: Private Corporation Co-packer Market Event
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
    And I click on the Licences tab
    And I pay the licensing fee for a Manufacturer application
    And I request a market event
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @marketevent @validation
Scenario: Validation for Market Event Application 
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Manufacturing application
    And the application is approved
    And I click on the Licences tab
    And I click on the link for Request Market Event Authorization
    And I click on the Submit button
    And the expected validation errors are thrown for a market event
    And the account is deleted
    Then I see the login page