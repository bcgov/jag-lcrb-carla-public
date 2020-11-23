 Feature: MarketEvent
    As a logged in business user
    I want to submit a market event for different manufacturer types

#-----------------------
# One Market Event
#-----------------------

 @e2e @privatecorporation @marketevent @winery @release
 Scenario: Winery One Day Market Event
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event for one date only
    And I click on the market event submit button
    And I click on the Licences tab
    And I click on the event history for markets
    And I click on the link for Approved
    And the market event data is correct for a one day event
    And the account is deleted
    Then I see the login page

#-----------------------
# Weekly Market Event
#-----------------------

 @e2e @privatecorporation @marketevent @winery
 Scenario: Winery Weekly Market Event
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event weekly
    And I click on the market event submit button
    And I click on the Licences tab
    And I click on the event history for markets
    And I click on the link for Approved
    And the market event data is correct for a weekly event
    And the account is deleted
    Then I see the login page

#-------------------------------------------------
# Bi-Weekly Market Event
# Note that 'bi-weekly' refers to twice per month
#-------------------------------------------------

 @e2e @privatecorporation @marketevent @winery
 Scenario: Winery Bi-Weekly Market Event
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event bi-weekly
    And I click on the market event submit button
    And I click on the Licences tab
    And I click on the event history for markets
    And I click on the link for Approved
    And the market event data is correct for a bi-weekly event
    And the account is deleted
    Then I see the login page

#-----------------------
# Monthly Market Event
#-----------------------

 @e2e @privatecorporation @marketevent @winery
 Scenario: Winery Monthly Market Event
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event monthly
    And I click on the market event submit button
    And I click on the Licences tab
    And I click on the event history for markets
    And I click on the link for Approved
    And the market event data is correct for a monthly event
    And the account is deleted
    Then I see the login page

#-----------------------------------
# One Market Event - Save for Later
#-----------------------------------

 @e2e @privatecorporation @marketeventonedaysave @winery
 Scenario: Winery One Day Market Event Save For Later
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event for one date only
    And I click on the market event save for later button
    And I click on the Licences tab
    And I click on the event history for markets
    And I click on the link for Draft
    And the market event data is correct for a one day event saved for later
    And I click on the signature checkbox
    And I click on the market event submit button
    And I click on the event history for markets
    And I click on the link for Approved
    And the market event data is correct for a one day event
    And the account is deleted
    Then I see the login page

#--------------------------------------
# Weekly Market Event - Save for Later
#--------------------------------------

 @e2e @privatecorporation @marketeventweeklysave @winery
 Scenario: Winery Weekly Market Event Save For Later
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event weekly
    And I click on the market event save for later button
    And I click on the Licences tab
    And I click on the event history for markets
    And I click on the link for Draft
    And the market event data is correct for a weekly event saved for later
    And I click on the signature checkbox
    And I click on the market event submit button
    And I click on the event history for markets
    And I click on the link for Approved
    And the market event data is correct for a weekly event
    And the account is deleted
    Then I see the login page

#-----------------------------------------
# Bi-Weekly Market Event - Save for Later
#-----------------------------------------

 @e2e @privatecorporation @marketeventbiweeklysave @winery
 Scenario: Winery Bi-Weekly Market Event Save For Later
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event bi-weekly
    And I click on the market event save for later button
    And I click on the Licences tab
    And I click on the event history for markets
    And I click on the link for Draft
    And the market event data is correct for a bi-weekly event saved for later
    And I click on the signature checkbox
    And I click on the market event submit button
    And I click on the event history for markets
    And I click on the link for Approved
    And the market event data is correct for a bi-weekly event
    And the account is deleted
    Then I see the login page

#---------------------------------------
# Monthly Market Event - Save for Later
#---------------------------------------

 @e2e @privatecorporation @marketeventmonthlysave @winery
 Scenario: Winery Monthly Market Event Save For Later
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Manufacturer application for a winery
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I request an on-site store endorsement
    And I click on the Licences tab
    And the on-site endorsement application is approved
    And I request a market event monthly
    And I click on the market event save for later button
    And I click on the Licences tab
    And I click on the event history for markets
    And I click on the link for Draft
    And the market event data is correct for a monthly event saved for later
    And I click on the signature checkbox
    And I click on the market event submit button
    And I click on the event history for markets
    And I click on the link for Approved
    And the market event data is correct for an approved monthly event
    And the account is deleted
    Then I see the login page