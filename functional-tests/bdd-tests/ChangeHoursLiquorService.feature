Feature: ChangeHoursLiquorService
    As a logged in business user
    I want to update the liquor hours of service for lounge areas and special events

@manufacturer @changehours
Scenario: DEV Lounge Area Within Service Hours (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I click on the Licences tab
    And I complete the change hours application for a lounge area within service hours
    And I click on the LG Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

@manufacturer @changehours
Scenario: DEV Lounge Area Outside Service Hours (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the Licences tab
    And I complete the change hours application for a lounge area outside of service hours
    And I click on the LG Submit button
    And I click on the link for Dashboard
    And the dashboard status is updated as Pending External Review
    And the account is deleted
    Then I see the login page

@manufacturer @changehours
Scenario: DEV Special Event Area Within Service Hours (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee
    And I click on the Licences tab
    And I complete the change hours application for a special event area within service hours
    And I click on the LG Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page

@manufacturer @changehours
Scenario: DEV Special Event Area Outside Service Hours (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee 
    And I click on the Licences tab
    And I complete the change hours application for a special event area outside of service hours
    And I click on the LG Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page