Feature: CateringApplicationLicensingRenewal
    As a logged in business user
    I want to pay the first year licensing fee for an approved Catering Application
    And renew the licence

#-----------------------
# Expiry = Today
#-----------------------

@e2e @catering @privatecorporation @licencerenewal
Scenario: Negative Private Corporation Catering Licence Renewal Today
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @licencerenewal
Scenario: Positive Private Corporation Catering Licence Renewal Today
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @indigenousnation @licencerenewal
Scenario: Negative Indigenous Nation Catering Licence Renewal Today
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @indigenousnation @licencerenewal
Scenario: Positive Indigenous Nation Catering Licence Renewal Today
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @licencerenewal
Scenario: Negative Partnership Catering Licence Renewal Today
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @licencerenewal
Scenario: Positive Partnership Catering Licence Renewal Today
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @licencerenewal
Scenario: Negative Public Corporation Catering Licence Renewal Today
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @licencerenewal
Scenario: Positive Public Corporation Catering Licence Renewal Today
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @society @licencerenewal
Scenario: Negative Society Catering Licence Renewal Today
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @society @licencerenewal
Scenario: Positive Society Catering Licence Renewal Today
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @licencerenewal
Scenario: Negative Sole Proprietorship Catering Licence Renewal Today
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @licencerenewal
Scenario: Positive Sole Proprietorship Catering Licence Renewal Today
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @localgovernment @licencerenewal
Scenario: Negative Local Government Catering Licence Renewal Today
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for Catering
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @localgovernment @licencerenewal
Scenario: Positive Local Government Catering Licence Renewal Today
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for Catering
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @university @licencerenewal
Scenario: Negative University Catering Licence Renewal Today
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for Catering
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @university @licencerenewal
Scenario: Positive University Catering Licence Renewal Today
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for Catering
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = Yesterday
#-----------------------

@e2e @catering @privatecorporation @licencerenewal
Scenario: Negative Private Corporation Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @licencerenewal
Scenario: Positive Private Corporation Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @indigenousnation @licencerenewal
Scenario: Negative Indigenous Nation Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @indigenousnation @licencerenewal
Scenario: Positive Indigenous Nation Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @licencerenewal
Scenario: Negative Partnership Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @licencerenewal
Scenario: Positive Partnership Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @licencerenewal
Scenario: Negative Public Corporation Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @licencerenewal
Scenario: Positive Public Corporation Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @society @licencerenewal
Scenario: Negative Society Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @society @licencerenewal
Scenario: Positive Society Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @licencerenewal
Scenario: Negative Sole Proprietorship Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @licencerenewal
Scenario: Positive Sole Proprietorship Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @localgovernment @licencerenewal
Scenario: Negative Local Government Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for Catering
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @localgovernment @licencerenewal
Scenario: Positive Local Government Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for Catering
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @university @licencerenewal
Scenario: Negative University Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for Catering
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @university @licencerenewal
Scenario: Positive University Catering Licence Renewal Yesterday
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for Catering
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to yesterday
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

#-----------------------
# Expiry = 45 Days Ago
#-----------------------

@e2e @catering @privatecorporation @licencerenewal
Scenario: Negative Private Corporation Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @licencerenewal
Scenario: Positive Private Corporation Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @indigenousnation @licencerenewal
Scenario: Negative Indigenous Nation Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @indigenousnation @licencerenewal
Scenario: Positive Indigenous Nation Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @licencerenewal
Scenario: Negative Partnership Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @licencerenewal
Scenario: Positive Partnership Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @licencerenewal
Scenario: Negative Public Corporation Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @licencerenewal
Scenario: Positive Public Corporation Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @society @licencerenewal
Scenario: Negative Society Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @society @licencerenewal
Scenario: Positive Society Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @licencerenewal
Scenario: Negative Sole Proprietorship Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @licencerenewal
Scenario: Positive Sole Proprietorship Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @localgovernment @licencerenewal
Scenario: Negative Local Government Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for Catering
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @localgovernment @licencerenewal
Scenario: Positive Local Government Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for Catering
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @university @licencerenewal
Scenario: Negative University Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for Catering
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @university @licencerenewal
Scenario: Positive University Catering Licence Renewal 45 Days Ago
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for Catering
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 45 days ago
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 60 Days From Today
#-----------------------------

@e2e @catering @privatecorporation @licencerenewal
Scenario: Negative Private Corporation Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @licencerenewal
Scenario: Positive Private Corporation Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @indigenousnation @licencerenewal
Scenario: Negative Indigenous Nation Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @indigenousnation @licencerenewal
Scenario: Positive Indigenous Nation Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @licencerenewal
Scenario: Negative Partnership Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @licencerenewal
Scenario: Positive Partnership Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @licencerenewal
Scenario: Negative Public Corporation Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @licencerenewal
Scenario: Positive Public Corporation Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @society @licencerenewal
Scenario: Negative Society Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @society @licencerenewal
Scenario: Positive Society Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @licencerenewal
Scenario: Negative Sole Proprietorship Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @licencerenewal
Scenario: Positive Sole Proprietorship Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @localgovernment @licencerenewal
Scenario: Negative Local Government Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for Catering
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @localgovernment @licencerenewal
Scenario: Positive Local Government Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for Catering
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @university @licencerenewal
Scenario: Negative University Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for Catering
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @university @licencerenewal
Scenario: Positive University Catering Licence Renewal 60 Days Future
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for Catering
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 60 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

#-----------------------------
# Expiry = 30 Days From Today
#-----------------------------

@e2e @catering @privatecorporation @licencerenewal
Scenario: Negative Private Corporation Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @privatecorporation @licencerenewal
Scenario: Positive Private Corporation Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @indigenousnation @licencerenewal
Scenario: Negative Indigenous Nation Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @indigenousnation @licencerenewal
Scenario: Positive Indigenous Nation Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for Catering
    And I review the account profile for an indigenous nation
    And I review the organization structure for an indigenous nation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @licencerenewal
Scenario: Negative Partnership Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @partnership @licencerenewal
Scenario: Positive Partnership Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @licencerenewal
Scenario: Negative Public Corporation Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @publiccorporation @licencerenewal
Scenario: Positive Public Corporation Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @society @licencerenewal
Scenario: Negative Society Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @society @licencerenewal
Scenario: Positive Society Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @licencerenewal
Scenario: Negative Sole Proprietorship Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @soleproprietorship @licencerenewal
Scenario: Positive Sole Proprietorship Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @localgovernment @licencerenewal
Scenario: Negative Local Government Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for Catering
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @localgovernment @licencerenewal
Scenario: Positive Local Government Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for Catering
    And I review the account profile for a local government
    And I review the organization structure for a local government
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @university @licencerenewal
Scenario: Negative University Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for Catering
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with negative responses for Catering
    And the account is deleted
    Then I see the login page

@e2e @catering @university @licencerenewal
Scenario: Positive University Catering Licence Renewal 30 Days Future
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for Catering
    And I review the account profile for a university
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I complete the Catering application
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Catering
    And the expiry date is changed to 30 days from today
    And I renew the licence with positive responses for Catering
    And the account is deleted
    Then I see the login page