Feature: CRSApplicationBrandingChange
    As a logged in business user
    I want to submit a CRS Application for different business types
    And request a valid name or branding change for the approved application

@e2e @cannabis @indigenousnation @crsbranding2
Scenario: Indigenous Nation CRS Name Branding Change
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
    And I click on the button for CRS terms and conditions
    And the correct terms and conditions are displayed for CRS
    And I request a valid store name or branding change for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @partnership @crsbranding
Scenario: Partnership CRS Name Branding Change
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
    And I click on the button for CRS terms and conditions
    And the correct terms and conditions are displayed for CRS
    And I request a valid store name or branding change for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @crsbranding
Scenario: Private Corporation CRS Name Branding Change
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
    And I click on the button for CRS terms and conditions
    And the correct terms and conditions are displayed for CRS
    And I request a valid store name or branding change for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @publiccorporation @crsbranding2
Scenario: Public Corporation CRS Name Branding Change
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
    And I click on the button for CRS terms and conditions
    And the correct terms and conditions are displayed for CRS
    And I request a valid store name or branding change for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @society @crsbranding2
Scenario: Society CRS Name Branding Change
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
    And I click on the button for CRS terms and conditions
    And the correct terms and conditions are displayed for CRS
    And I request a valid store name or branding change for Cannabis
    And the account is deleted
    Then I see the login page

@e2e @cannabis @soleproprietorship @crsbranding
Scenario: Sole Proprietorship CRS Name Branding Change
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
    And I click on the button for CRS terms and conditions
    And the correct terms and conditions are displayed for CRS
    And I request a valid store name or branding change for Cannabis
    And the account is deleted
    Then I see the login page