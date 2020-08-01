Feature: ManufacturerApplication.feature
    As a logged in business user
    I want to submit a Manufacturer Applications for different manufacturer types

@e2e @privatecorporation @manufacturerapp @winery
Scenario: Private Corporation Winery Manufacturer Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturerapp @distillery
Scenario: Private Corporation Distillery Manufacturer Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturerapp @brewery
Scenario: Private Corporation Brewery Manufacturer Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturerapp @copacker
Scenario: Private Corporation Co-packer Manufacturer Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the account is deleted
    Then I see the login page