Feature: FoodPrimaryDownloadLicence
    As a logged in business user
    I want to download a Food Primary licence for different business types

@foodprimarydownloadlicence @partnership 
Scenario: Partnership Food Primary Download Licence
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Food Primary
    And I review the account profile for a partnership
    And I complete the Food Primary application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Download Licence
    And the account is deleted
    Then I see the login page

@foodprimarydownloadlicence @privatecorporation
Scenario: Private Corporation Food Primary Download Licence
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a private corporation
    And I complete the Food Primary application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Download Licence
    And the account is deleted
    Then I see the login page

@foodprimarydownloadlicence @publiccorporation
Scenario: Public Corporation Food Primary Download Licence
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a public corporation
    And I complete the Food Primary application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Download Licence
    And the account is deleted
    Then I see the login page

@foodprimarydownloadlicence @society
Scenario: Society Food Primary Download Licence
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Food Primary
    And I review the account profile for a society
    And I complete the Food Primary application for a society
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Download Licence
    And the account is deleted
    Then I see the login page

@foodprimarydownloadlicence @soleproprietorship
Scenario: Sole Proprietorship Food Primary Download Licence
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Food Primary
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Food Primary application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Download Licence
    And the account is deleted
    Then I see the login page