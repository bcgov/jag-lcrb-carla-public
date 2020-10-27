 Feature: MarketEvent
    As a logged in business user
    I want to submit a market event for different manufacturer types

#-----------------------
# One Market Event
#-----------------------

 @e2e @privatecorporation @marketeventoneday @winery
 Scenario: Private Corporation Winery One Day Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event for one date only
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventoneday @distillery
 Scenario: Private Corporation Distillery One Day Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event for one date only
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventoneday @brewery
 Scenario: Private Corporation Brewery One Day Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event for one date only
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventoneday @copacker
 Scenario: Private Corporation Co-packer One Day Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event for one date only
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

#-----------------------
# Weekly Market Event
#-----------------------

 @e2e @privatecorporation @marketeventweekly @winery
 Scenario: Private Corporation Winery Weekly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event weekly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventweekly @distillery
 Scenario: Private Corporation Distillery Weekly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event weekly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventweekly @brewery
 Scenario: Private Corporation Brewery Weekly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event weekly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventweekly @copacker
 Scenario: Private Corporation Co-packer Weekly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event weekly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

#-----------------------
# Bi-Weekly Market Event
#-----------------------

 @e2e @privatecorporation @marketeventbiweekly @winery
 Scenario: Private Corporation Winery Bi-Weekly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event bi-weekly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventbiweekly @distillery
 Scenario: Private Corporation Distillery Bi-Weekly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event bi-weekly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventbiweekly @brewery
 Scenario: Private Corporation Brewery Bi-Weekly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event bi-weekly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventbiweekly @copacker
 Scenario: Private Corporation Co-packer Bi-Weekly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event bi-weekly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

#-----------------------
# Monthly Market Event
#-----------------------

 @e2e @privatecorporation @marketeventmonthly @winery
 Scenario: Private Corporation Winery Monthly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event monthly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventmonthly @distillery
 Scenario: Private Corporation Distillery Monthly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event monthly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventmonthly @brewery
 Scenario: Private Corporation Brewery Monthly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event monthly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page

 @e2e @privatecorporation @marketeventmonthly @copacker
 Scenario: Private Corporation Co-packer Monthly Market Event
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
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event monthly
    And I click on the event history for markets
    And the market event is approved
    And the account is deleted
    Then I see the login page