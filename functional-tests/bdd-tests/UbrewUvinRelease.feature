Feature: UbrewUvinRelease
    As a logged in business user
    I want to submit a UBrew / UVin Licence application 
    And complete change requests 

@tenrelease
Scenario: UBrew / UVin Application Release Test (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a private corporation
    And I complete the UBrew / UVin application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the link for Dashboard
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    # And I click on the link for Download Licence
    # And I confirm the terms and conditions for a UBrew / UVin licence
    # And I click on the link for Add Licensee Representative
    # And I request a licensee representative
    # And I click on the link for Licences & Authorizations
    And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    And I click on the Continue to Application button
    And I request a change in terms and conditions application
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I request a store relocation for UBrew / UVin
    # And I click on the link for Licences & Authorizations
    # And I click on the link for Establishment Name Change Application
    # And I click on the Continue to Application button
    # And I request a valid store name or branding change for UBrew
    # And I click on the link for Licences & Authorizations
    # And I request a third party operator
    # And I click on the link for Cancel Application
    # And I cancel the third party operator application
    # And I click on the link for Licences & Authorizations
    # And I request a transfer of ownership for UBrew / UVin
    And the account is deleted
    Then I see the login page

Scenario: UBrew / UVin Application Release Test (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a sole proprietorship
    And I complete the UBrew / UVin application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the link for Dashboard
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    And I confirm the terms and conditions for a UBrew / UVin licence
    And I click on the link for Add Licensee Representative
    And I request a licensee representative
    And I click on the link for Licences & Authorizations
    And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    And I click on the Continue to Application button
    And I request a change in terms and conditions application
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I request a store relocation for UBrew / UVin
    And I click on the link for Licences & Authorizations
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for UBrew
    And I click on the link for Licences & Authorizations
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And I click on the link for Licences & Authorizations
    And I request a transfer of ownership for UBrew / UVin
    And the account is deleted
    Then I see the login page