Feature: CrsLicensingRenewalDenied
    As a logged in business user
    I want to pay the first year licensing fee for an approved CRS Application
    And confirm that autorenewal is set to 'No' to prevent renewal of the licence

#-----------------------
# Expiry = Today
#-----------------------

Scenario: Deny CRS Licence Renewal Today (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    # Note: The following workflow sets the Dynamics autorenewal flag to 'No'
    And the expiry date is changed using the Dynamics workflow named 322d410b-f725-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = Yesterday
#-----------------------

Scenario: Deny CRS Licence Renewal Yesterday (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    # Note: The following workflow sets the Dynamics autorenewal flag to 'No'
    And the expiry date is changed using the Dynamics workflow named e1792ccf-e40b-491f-9a9a-ee8e977749e6
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = 45 Days Ago
#-----------------------

Scenario: Deny CRS Licence Renewal 45 Days Ago (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    # Note: The following workflow sets the Dynamics autorenewal flag to 'No'
    And the expiry date is changed using the Dynamics workflow named 65bfe79d-f825-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 60 Days From Today
#-----------------------------

Scenario: Deny CRS Licence Renewal 60 Days Future (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    # Note: The following workflow sets the Dynamics autorenewal flag to 'No'
    And the expiry date is changed using the Dynamics workflow named beb3243e-f825-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 30 Days From Today
#-----------------------------

Scenario: Deny CRS Licence Renewal 30 Days Future (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I pay the licensing fee 
    And I click on the link for Licences & Authorizations
    # Note: The following workflow sets the Dynamics autorenewal flag to 'No'
    And the expiry date is changed using the Dynamics workflow named 10eaae77-f725-eb11-b821-00505683fbf4
    And I am unable to renew the licence
    And the account is deleted
    Then I see the login page