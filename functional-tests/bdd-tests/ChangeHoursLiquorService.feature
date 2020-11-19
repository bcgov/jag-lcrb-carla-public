Feature: ChangeHoursLiquorService
    As a logged in business user
    I want to update the liquor hours of service for lounge areas and special events

@e2e @changehours
Scenario: Change Lounge Area Hours of Liquor Service Within Service Hours
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
    And I click on the Licences tab
    And I complete the change hours application for a lounge area within service hours
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

@e2e @changehours
Scenario: Change Lounge Area Hours of Liquor Service Outside Service Hours
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
    And I click on the Licences tab
    And I complete the change hours application for a lounge area outside of service hours
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

@e2e @changehours
Scenario: Change Special Event Area Hours of Liquor Service Within Service Hours
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
    And I click on the Licences tab
    And I complete the change hours application for a special event area within service hours
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

@e2e @changehours
Scenario: Change Special Event Area Hours of Liquor Service Outside Service Hours
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
    And I click on the Licences tab
    And I complete the change hours application for a special event area outside of service hours
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page