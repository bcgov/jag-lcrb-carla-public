Feature: ManufacturerRequestChanges.feature
    As a logged in business user
    I want to request changes for a manufacturer licence

#-----------------------
# Winery Change Requests
#-----------------------

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Transfer Licence
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Third Party Operator
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a third party operator
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Picnic Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a picnic area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery On-Site Store Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Lounge Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a lounge area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Establishment Name Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a valid store name or branding change for Manufacturing
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Facility Structural Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a facility structural change
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Location Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a location change
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery Special Event Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a special event area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @winery
Scenario: Winery New Outdoor Patio Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a new outdoor patio endorsement
    And the account is deleted
    Then I see the login page

#------------------------
# Brewery Change Requests
#------------------------

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery Transfer Licence
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery Third Party Operator
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a third party operator
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery Picnic Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a picnic area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery On-Site Store Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery Lounge Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a lounge area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery Establishment Name Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a valid store name or branding change for Manufacturing
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery Facility Structural Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a facility structural change
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery Location Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a location change
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery Special Event Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a special event area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @brewery
Scenario: Brewery New Outdoor Patio Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a brewery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a new outdoor patio endorsement
    And the account is deleted
    Then I see the login page

#------------------------
# Distillery Change Requests
#------------------------

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery Transfer Licence
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery Third Party Operator
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a third party operator
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery Picnic Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a picnic area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery On-Site Store Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery Lounge Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a lounge area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery Establishment Name Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a valid store name or branding change for Manufacturing
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery Facility Structural Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a facility structural change
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery Location Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a location change
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery Special Event Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a special event area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @distillery
Scenario: Distillery New Outdoor Patio Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a distillery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a new outdoor patio endorsement
    And the account is deleted
    Then I see the login page

#------------------------
# Co-packer Change Requests
#------------------------

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer Transfer Licence
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer Third Party Operator
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a third party operator
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer Picnic Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a picnic area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer On-Site Store Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request an on-site store endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer Lounge Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a lounge area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer Establishment Name Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a valid store name or branding change for Manufacturing
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer Facility Structural Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a facility structural change
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer Location Change Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a location change
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer Special Event Area Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a special event area endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @copacker
Scenario: Co-packer New Outdoor Patio Endorsement Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a co-packer
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I request a new outdoor patio endorsement
    And the account is deleted
    Then I see the login page

@e2e @privatecorporation @manufacturer @validation
Scenario: Manufacturer Establishment Name Change Validation
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Manufacturer Licence
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Manufacturer application for a winery
    And I enter the payment information
    And I confirm the payment receipt for a Manufacturer Licence application
    And the application is approved
    And I pay the licensing fee for a Manufacturer application
    And I click on the branding change link for Manufacturer
    And I do not complete the application correctly
    And the expected validation errors are thrown for a Branding Change application
    And the account is deleted
    Then I see the login page