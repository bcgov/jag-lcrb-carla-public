Feature: CRSApplicationTransferLicence
    As a logged in business user
    I want to submit a CRS Application for different business types
    And request a transfer of ownership for the approved application

@cannabis @privatecorporation @crstransferownership
Scenario: DEV Cannabis Transfer Ownership (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@cannabis @society @crstransferownership
Scenario: DEV Cannabis Transfer Ownership (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I complete the Cannabis Retail Store application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@cannabis @partnership @crstransferownership
Scenario: DEV Cannabis Transfer Ownership (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @crstransferownership
Scenario: DEV Cannabis Transfer Ownership (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

#@cannabis @indigenousnation @crstransferownership
#Scenario: UAT Cannabis Transfer Ownership (Indigenous Nation)
#    Given I am logged in to the dashboard as an indigenous nation
#    And I click on the Start Application button for a Cannabis Retail Store
#    And I complete the eligibility disclosure
#    And I review the account profile for an indigenous nation
#    And I review the organization structure for an indigenous nation
#    And I click on the button for Submit Organization Information
#    And I complete the Cannabis Retail Store application for an indigenous nation
#    And I click on the secondary Submit button
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And the application is approved
#    And I pay the licensing fee
#    And I request a transfer of ownership
#    And the account is deleted
#    Then I see the login page

#@cannabis @partnership @crstransferownership
#Scenario: UAT Cannabis Transfer Ownership (Partnership)
#    Given I am logged in to the dashboard as a partnership
#    And I click on the Start Application button for a Cannabis Retail Store
#    And I complete the eligibility disclosure
#    And I review the account profile for a partnership
#    And I review the organization structure for a partnership
#    And I click on the button for Submit Organization Information
#    And I complete the Cannabis Retail Store application for a partnership
#    And I click on the secondary Submit button
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And the application is approved
#    And I pay the licensing fee
#    And I request a transfer of ownership
#    And the account is deleted
#    Then I see the login page

#@cannabis @privatecorporation @crstransferownership
#Scenario: UAT Cannabis Transfer Ownership (Private Corporation)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the Start Application button for a Cannabis Retail Store
#    And I complete the eligibility disclosure
#    And I review the account profile for a private corporation
#    And I review the organization structure for a private corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Cannabis Retail Store application for a private corporation
#    And I click on the secondary Submit button
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And the application is approved
#    And I pay the licensing fee 
#    And I request a transfer of ownership
#    And the account is deleted
#    Then I see the login page

#@cannabis @publiccorporation @crstransferownership
#Scenario: UAT Cannabis Transfer Ownership (Public Corporation)
#    Given I am logged in to the dashboard as a public corporation
#    And I click on the Start Application button for a Cannabis Retail Store
#    And I complete the eligibility disclosure
#    And I review the account profile for a public corporation
#    And I review the organization structure for a public corporation
#    And I click on the button for Submit Organization Information
#    And I complete the Cannabis Retail Store application for a public corporation
#    And I click on the secondary Submit button
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And the application is approved
#    And I pay the licensing fee 
#    And I request a transfer of ownership
#    And the account is deleted
#    Then I see the login page

#@cannabis @society @crstransferownership
#Scenario: UAT Cannabis Transfer Ownership (Society)
#    Given I am logged in to the dashboard as a society
#    And I click on the Start Application button for a Cannabis Retail Store
#    And I complete the eligibility disclosure
#    And I review the account profile for a society
#    And I review the organization structure for a society
#    And I click on the button for Submit Organization Information
#    And I complete the Cannabis Retail Store application for a society
#    And I click on the secondary Submit button
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And the application is approved
#    And I pay the licensing fee 
#    And I request a transfer of ownership
#    And the account is deleted
#    Then I see the login page

#@cannabis @soleproprietorship @crstransferownership
#Scenario: UAT Cannabis Transfer Ownership (Sole Proprietorship)
#    Given I am logged in to the dashboard as a sole proprietorship
#    And I click on the Start Application button for a Cannabis Retail Store
#    And I complete the eligibility disclosure
#    And I review the account profile for a sole proprietorship
#    And I review the organization structure for a sole proprietorship
#    And I click on the button for Submit Organization Information
#    And I complete the Cannabis Retail Store application for a sole proprietorship
#    And I click on the secondary Submit button
#    And I click on the button for Pay for Application
#    And I enter the payment information
#    And the application is approved
#    And I pay the licensing fee
#    And I request a transfer of ownership
#    And the account is deleted
#    Then I see the login page