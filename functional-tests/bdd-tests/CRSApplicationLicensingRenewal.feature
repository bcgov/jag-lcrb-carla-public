Feature: CRSApplicationLicensingRenewal
    As a logged in business user
    I want to pay the first year licensing fee for an approved CRS Application
    And renew the licence

#-----------------------
# Expiry = Today
#-----------------------

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Negative Private Corporation CRS Licence Renewal Today
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Positive Private Corporation CRS Licence Renewal Today
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @indigenousnation @licencerenewal
Scenario: Negative Indigenous Nation CRS Licence Renewal Today
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @indigenousnation @licencerenewal
Scenario: Positive Indigenous Nation CRS Licence Renewal Today
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @licencerenewal
Scenario: Negative Partnership CRS Licence Renewal Today
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @licencerenewal
Scenario: Positive Partnership CRS Licence Renewal Today
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @licencerenewal
Scenario: Negative Public Corporation CRS Licence Renewal Today
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @licencerenewal
Scenario: Positive Public Corporation CRS Licence Renewal Today
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @licencerenewal
Scenario: Negative Society CRS Licence Renewal Today
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @licencerenewal
Scenario: Positive Society CRS Licence Renewal Today
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @licencerenewal
Scenario: Negative Sole Proprietorship CRS Licence Renewal Today
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @licencerenewal
Scenario: Positive Sole Proprietorship CRS Licence Renewal Today
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @localgovernment @licencerenewal
Scenario: Negative Local Government CRS Licence Renewal Today
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @localgovernment @licencerenewal
Scenario: Positive Local Government CRS Licence Renewal Today
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = Yesterday
#-----------------------

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Negative Private Corporation CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Positive Private Corporation CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @indigenousnation @licencerenewal
Scenario: Negative Indigenous Nation CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @indigenousnation @licencerenewal
Scenario: Positive Indigenous Nation CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @licencerenewal
Scenario: Negative Partnership CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @licencerenewal
Scenario: Positive Partnership CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @licencerenewal
Scenario: Negative Public Corporation CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @licencerenewal
Scenario: Positive Public Corporation CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @licencerenewal
Scenario: Negative Society CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @licencerenewal
Scenario: Positive Society CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @licencerenewal
Scenario: Negative Sole Proprietorship CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @licencerenewal
Scenario: Positive Sole Proprietorship CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @localgovernment @licencerenewal
Scenario: Negative Local Government CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @localgovernment @licencerenewal
Scenario: Positive Local Government CRS Licence Renewal Yesterday
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = 45 Days Ago
#-----------------------

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Negative Private Corporation CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Positive Private Corporation CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @indigenousnation @licencerenewal
Scenario: Negative Indigenous Nation CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @indigenousnation @licencerenewal
Scenario: Positive Indigenous Nation CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @licencerenewal
Scenario: Negative Partnership CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @licencerenewal
Scenario: Positive Partnership CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @licencerenewal
Scenario: Negative Public Corporation CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @licencerenewal
Scenario: Positive Public Corporation CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @licencerenewal
Scenario: Negative Society CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @licencerenewal
Scenario: Positive Society CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @licencerenewal
Scenario: Negative Sole Proprietorship CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @licencerenewal
Scenario: Positive Sole Proprietorship CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @localgovernment @licencerenewal
Scenario: Negative Local Government CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @localgovernment @licencerenewal
Scenario: Positive Local Government CRS Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 60 Days From Today
#-----------------------------

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Negative Private Corporation CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Positive Private Corporation CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @indigenousnation @licencerenewal
Scenario: Negative Indigenous Nation CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @indigenousnation @licencerenewal
Scenario: Positive Indigenous Nation CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @licencerenewal
Scenario: Negative Partnership CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @licencerenewal
Scenario: Positive Partnership CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @licencerenewal
Scenario: Negative Public Corporation CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @licencerenewal
Scenario: Positive Public Corporation CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @licencerenewal
Scenario: Negative Society CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @licencerenewal
Scenario: Positive Society CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @licencerenewal
Scenario: Negative Sole Proprietorship CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @licencerenewal
Scenario: Positive Sole Proprietorship CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @localgovernment @licencerenewal
Scenario: Negative Local Government CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @localgovernment @licencerenewal
Scenario: Positive Local Government CRS Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 30 Days From Today
#-----------------------------

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Negative Private Corporation CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @licencerenewal
Scenario: Positive Private Corporation CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @indigenousnation @licencerenewal
Scenario: Negative Indigenous Nation CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @indigenousnation @licencerenewal
Scenario: Positive Indigenous Nation CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @licencerenewal
Scenario: Negative Partnership CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @licencerenewal
Scenario: Positive Partnership CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @licencerenewal
Scenario: Negative Public Corporation CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @licencerenewal
Scenario: Positive Public Corporation CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @licencerenewal
Scenario: Negative Society CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @licencerenewal
Scenario: Positive Society CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @licencerenewal
Scenario: Negative Sole Proprietorship CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @licencerenewal
Scenario: Positive Sole Proprietorship CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @localgovernment @licencerenewal
Scenario: Negative Local Government CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @localgovernment @licencerenewal
Scenario: Positive Local Government CRS Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Cannabis
    And the account is deleted
    Then I see the login page