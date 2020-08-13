Feature: CRSApplicationPersonnelNameChanges
    As a logged in business user
    I want to submit a CRS Application for different business types
    And request personnel email and name changes for the approved application

@e2e @cannabis @partnership @crsemail
Scenario: Partnership CRS Personnel Name Changes
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the Pay for Application button for an indigenous nation
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I request a personnel name change for a partnership
    And I confirm the correct personnel name change fee for a Cannabis licence
    And I confirm that the director name has been updated
    And I change a personnel email address for a partnership
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @crsemail
Scenario: Private Corporation CRS Personnel Name Changes
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I request a personnel name change for a private corporation
    And I confirm the correct personnel name change fee for a Cannabis licence
    And I confirm that the director name has been updated
    And I change a personnel email address for a private corporation
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @crsemailpubcorp
Scenario: Public Corporation CRS Personnel Name Changes
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I request a personnel name change for a public corporation
    And I confirm the correct personnel name change fee for a Cannabis licence
    And I confirm that the director name has been updated
    And I change a personnel email address for a public corporation
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @crsemail2
Scenario: Society CRS Personnel Name Changes
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a society
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I request a personnel name change for a society
    And I confirm the correct personnel name change fee for a Cannabis licence
    And I confirm that the director name has been updated
    And I change a personnel email address for a society
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @crsemail
Scenario: Sole Proprietorship CRS Personnel Name Changes
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the Pay for Application button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee for Cannabis
    And I request a personnel name change for a sole proprietorship
    And I confirm the correct personnel name change fee for a Cannabis licence
    And I confirm that the director name has been updated
    And I change a personnel email address for a sole proprietorship
    And the account is deleted
    Then I see the login page