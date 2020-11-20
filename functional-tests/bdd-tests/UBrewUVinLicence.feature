Feature: UBrewUVinLicence
    As a logged in business user
    I want to submit a UBrew / UVin Licence for different business types

#-----------------------
# Application Only
#-----------------------

@e2e @ubrewuvinapplication @partnership 
Scenario: Partnership UBrew / UVin Application
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a partnership
    And I click on the Submit button
    And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvinapplication @privatecorporation @release
Scenario: Private Corporation UBrew / UVin Application
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
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvinapplication @publiccorporation 
Scenario: Public Corporation UBrew / UVin Application
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a public corporation
    And I click on the Submit button
    And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvinapplication @soleproprietorship 
Scenario: Sole Proprietorship UBrew / UVin Application
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a sole proprietorship
    And I click on the Submit button
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

#-----------------------
# Transfer Licence
#-----------------------

@e2e @ubrewuvin @partnership 
Scenario: Partnership UBrew / UVin Application Transfer Licence
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a partnership
    And I click on the Submit button
    And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvin @privatecorporation 
Scenario: Private Corporation UBrew / UVin Application Transfer Licence
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
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvin @publiccorporation 
Scenario: Public Corporation UBrew / UVin Application Transfer Licence
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a public corporation
    And I click on the Submit button
    And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvin @soleproprietorship 
Scenario: Sole Proprietorship UBrew / UVin Application Transfer Licence
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a sole proprietorship
    And I click on the Submit button
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

#-----------------------
# Third Party Operator
#-----------------------

@e2e @ubrewuvin @partnership 
Scenario: Partnership UBrew / UVin Application Third Party Operator
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a partnership
    And I click on the Submit button
    And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvin @privatecorporation 
Scenario: Private Corporation UBrew / UVin Application Third Party Operator
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
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvin @publiccorporation 
Scenario: Public Corporation UBrew / UVin Application Third Party Operator
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a public corporation
    And I click on the Submit button
    And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvin @soleproprietorship 
Scenario: Sole Proprietorship UBrew / UVin Application Third Party Operator
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a sole proprietorship
    And I click on the Submit button
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And the account is deleted
    Then I see the login page

#---------------------------
# Establishment Name Change
#---------------------------

@e2e @ubrewuvin @partnership 
Scenario: Partnership UBrew / UVin Application Establishment Name Change
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a partnership
    And I click on the Submit button
    And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a valid store name or branding change for UBrew
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvin @privatecorporation 
Scenario: Private Corporation UBrew / UVin Application Establishment Name Change
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
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a valid store name or branding change for UBrew
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvin @publiccorporation 
Scenario: Public Corporation UBrew / UVin Application Establishment Name Change
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a public corporation
    And I click on the Submit button
    And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a valid store name or branding change for UBrew
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @ubrewuvin @soleproprietorship 
Scenario: Sole Proprietorship UBrew / UVin Application Establishment Name Change
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the UBrew / UVin application for a sole proprietorship
    And I click on the Submit button
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for UBrew / UVin
    And I click on the Licences tab
    And I request a valid store name or branding change for UBrew
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page