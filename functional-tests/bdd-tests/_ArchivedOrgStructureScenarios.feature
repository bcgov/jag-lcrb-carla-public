Feature: CateringApplicationPersonnelEmailChanges
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit personnel email changes for different business types

@catering @partnership @cateringemail
Scenario: UAT Catering Personnel Email Change (Partnership)
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
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I change a personnel email address for a partnership
    And the account is deleted
    Then I see the login page

@catering @privatecorporation @cateringemail
Scenario: UAT Catering Personnel Email Change (Private Corporation)
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
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I change a personnel email address for a private corporation
    And the account is deleted
    Then I see the login page

@catering @publiccorporation @cateringemail
Scenario: UAT Catering Personnel Email Change (Public Corporation)
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
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I change a personnel email address for a public corporation
    And the account is deleted
    Then I see the login page

@catering @society @cateringemail
Scenario: UAT Catering Personnel Email Change (Society)
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
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I change a personnel email address for a society
    And the account is deleted
    Then I see the login page

@catering @soleproprietorship @cateringemail
Scenario: UAT Catering Personnel Email Change (Sole Proprietorship)
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
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I change a personnel email address for a sole proprietorship
    And the account is deleted
    Then I see the login page

Feature: CateringApplicationPersonnelNameChanges
    As a logged in business user
    I want to pay the first year catering licence fee
    And submit personnel name changes for different business types

@catering @partnership @cateringpersonnelnamechange
Scenario: UAT Catering Personnel Name Change (Partnership)
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
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I request a personnel name change for a partnership
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And the account is deleted
    Then I see the login page

@catering @privatecorporation @cateringpersonnelnamechange
Scenario: UAT Catering Personnel Name Change (Private Corporation)
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
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I request a personnel name change for a private corporation
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And the account is deleted
    Then I see the login page

@catering @publiccorporation @cateringpersonnelnamechange
Scenario: UAT Catering Personnel Name Change (Public Corporation)
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
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I request a personnel name change for a public corporation
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And the account is deleted
    Then I see the login page

@catering @society @cateringpersonnelnamechange
Scenario: UAT Catering Personnel Name Change (Society)
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
    And I pay the licensing fee
    And I click on the link for Dashboard
    And I request a personnel name change for a society
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And the account is deleted
    Then I see the login page

@catering @soleproprietorship @cateringpersonnelnamechange
Scenario: UAT Catering Personnel Name Change (Sole Proprietorship)
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
    And I pay the licensing fee
    And I click on the link for Dashboard
    And I request a personnel name change for a sole proprietorship
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And the account is deleted
    Then I see the login page

Feature: CheckOrgStructure
    As a logged in business user
    I want to confirm that the organization structure page displays

@validation @checkorgstructure
Scenario: Check Organization Structure (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And the organization structure page is displayed
    And I click on the link for Dashboard
    And the account is deleted
    Then I see the login page

Feature: CRSApplicationPersonnelEmailChanges
    As a logged in business user
    I want to submit a CRS Application for different business types
    And request personnel email changes for the approved application

@cannabis @partnership @crsemail
Scenario: UAT CRS Personnel Email Changes (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Dashboard
    And I change a personnel email address for a partnership
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @crsemail
Scenario: UAT CRS Personnel Email Changes (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I change a personnel email address for a private corporation
    And the account is deleted
    Then I see the login page

@cannabis @publiccorporation @crsemail
Scenario: UAT CRS Personnel Email Changes (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Dashboard
    And I change a personnel email address for a public corporation
    And the account is deleted
    Then I see the login page

@cannabis @society @crsemail
Scenario: UAT CRS Personnel Email Changes (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Dashboard
    And I change a personnel email address for a society
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @crsemail
Scenario: UAT CRS Personnel Email Changes (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I change a personnel email address for a sole proprietorship
    And the account is deleted
    Then I see the login page

Feature: CRSApplicationPersonnelNameChanges
    As a logged in business user
    I want to submit a CRS Application for different business types
    And request personnel email and name changes for the approved application

@cannabis @partnership @crspersonnelnamechange
Scenario: UAT CRS Personnel Name Changes (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I request a personnel name change for a partnership
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @crspersonnelnamechange
Scenario: UAT CRS Personnel Name Changes (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Dashboard
    And I request a personnel name change for a private corporation
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And the account is deleted
    Then I see the login page

@cannabis @publiccorporation @crspersonnelnamechange
Scenario: UAT CRS Personnel Name Changes (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a public corporation
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a public corporation
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Dashboard
    And I request a personnel name change for a public corporation
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And the account is deleted
    Then I see the login page

@cannabis @society @crspersonnelnamechange
Scenario: UAT CRS Personnel Name Changes (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a society
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a society
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Dashboard
    And I request a personnel name change for a society
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @crspersonnelnamechange
Scenario: UAT CRS Personnel Name Changes (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the secondary Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I request a personnel name change for a sole proprietorship
    And I click on the link for Dashboard
    And I confirm that the director name has been updated
    And the account is deleted
    Then I see the login page

Feature: MultiLevelCRSApplicationPrivateCorp
    As a logged in business user
    I want to submit a CRS Application for a private corporation
    With multiple nested business shareholders

@privatecorporation @validation @multilevel
Scenario: Multiple Nested Shareholders (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I add in multiple nested business shareholders
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I review the security screening requirements for a multilevel business
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And the account is deleted
    Then I see the login page

Feature: PrivateCorporationOrgStructure
    As a logged in private corporation business user
    I want to confirm the organization structure functionality

@privatecorporation @validation @privatecorporgstructure
Scenario: Change director name and pay fee (Private Corporation)
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
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the link for Dashboard
    And I click on the Review Organization Information button
    And I modify the director name
    And I click on the button for Submit Organization Information
    And I pay the name change fee
    And the director name is now updated
    And the account is deleted
    Then I see the login page

@privatecorporation @validation @privatecorporgstructure
Scenario: Delete an individual who is both a director and shareholder (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete only the director record
    And I click on the Complete Organization Information button
    And only the shareholder record is displayed
    And the account is deleted
    Then I see the login page

@privatecorporation @validation @privatecorporgstructure
Scenario: Change director and shareholder same name (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I modify only the director record
    And I click on the Complete Organization Information button
    And the director and shareholder name are identical
    And the account is deleted
    Then I see the login page

@privatecorporation @validation @privatecorporgstructure
Scenario: Confirm business shareholder org structure update (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I add a business shareholder with the same individual as a director and a shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I add a second individual as a director and a shareholder to the business shareholder
    And I click on the button for Confirm Organization Information is Complete
    And I click on the Complete Organization Information button
    And the org structure is correct
    And I remove the latest director and shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the latest director and shareholder is removed
    And I remove the business shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the business shareholder is removed
    And the account is deleted
    Then I see the login page

@privatecorporation @validation @privatecorporgstructure
Scenario: Confirm business shareholder org structure update after payment (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I add a business shareholder with the same individual as a director and a shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I add a second individual as a director and a shareholder to the business shareholder
    And I click on the button for Confirm Organization Information is Complete
    And I click on the Complete Organization Information button
    And the org structure is correct
    And I click on the button for Confirm Organization Information is Complete
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I click on the button for Confirm Organization Information is Complete
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Dashboard tab
    And I click on the Review Organization Information button
    And the org structure is correct after payment
    And the account is deleted
    Then I see the login page

@privatecorporation @validation @privatecorporgstructure
Scenario: Save for Later feature for org structure (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I enter the same individual as a director and a shareholder
    And I click on the button for Save for Later
    And I click on the Complete Organization Information button
    And I add a business shareholder with the same individual as a director and a shareholder
    And I click on the button for Save for Later
    And I click on the Complete Organization Information button
    And I add a second individual as a director and a shareholder to the business shareholder
    And I click on the button for Save for Later
    And I click on the Complete Organization Information button
    And I remove the latest director after saving
    And I click on the button for Save for Later
    And I click on the Complete Organization Information button
    And I remove the latest shareholder after saving
    And I click on the button for Save for Later
    And I click on the Complete Organization Information button
    And the latest director and shareholder is removed
    And I remove the business shareholder
    And I click on the button for Save for Later
    And I click on the Complete Organization Information button
    And the saved org structure is present
    And the account is deleted
    Then I see the login page

@privatecorporation @validation @privatecorporgstructure1
Scenario: CRS application with mixed business shareholder types (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I add in business shareholders of different business types
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I review the security screening requirements for mixed business shareholders
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And I click on the link for Return to Dashboard
    And I click on the Complete Organization Information button
    And the mixed business shareholder org structure is correct
    And the account is deleted
    Then I see the login page

@privatecorporation @validation @privatecorporgstructure1
Scenario: Complex Save for Later mixed business shareholders (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I review the organization structure for a private corporation
    And I enter business shareholders of different business types to be saved for later
    And I click on the button for Save for Later
    And I click on the Complete Organization Information button
    And I review the security screening requirements for saved for later mixed business shareholder
    And the account is deleted
    Then I see the login page

@privatecorporation @validation @privatecorporgstructure1
Scenario: Confirm org structure records not duplicated (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I review the organization structure for a private corporation
    And I confirm that no duplicates are shown in the org structure
    And the account is deleted
    Then I see the login page

Feature: ReviewOrgStructureData
    As a logged in business user
    I want to confirm the data saved for the org structure

@cannabis @partnership @orgstructuredata
Scenario: Data for Org Structure (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Complete Organization Information button
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a partnership
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @orgstructuredata
Scenario: Data for Org Structure (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a private corporation
    And the account is deleted
    Then I see the login page

@cannabis @publiccorporation @orgstructuredata
Scenario: Data for Org Structure (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Complete Organization Information button
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a public corporation
    And the account is deleted
    Then I see the login page

@cannabis @society @orgstructuredata
Scenario: Data for Org Structure (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Complete Organization Information button
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a society
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @orgstructuredata
Scenario: Data for Org Structure (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Complete Organization Information button
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a sole proprietorship
    And the account is deleted
    Then I see the login page

 @cannabis @university @orgstructuredata
 Scenario: Data for Org Structure (University)
    Given I am logged in to the dashboard as a university
    And I click on the Complete Organization Information button
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a university
    And the account is deleted
    Then I see the login page

Feature: ReviewOrgStructureDeletion
    As a logged in business user
    I want to confirm the successful deletion of personnel from the org structure

@cannabis @partnership @orgstructuredeletion
Scenario: Deletion from Org Structure (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Complete Organization Information button
    And I add personnel to the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete the personnel for a partnership
    And I click on the button for Save For Later
    And the org structure data is successfully deleted for a partnership
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @orgstructuredeletion
Scenario: Deletion from Org Structure (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I add personnel to the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete the personnel for a private corporation
    And I click on the button for Save For Later
    And the org structure data is successfully deleted for a private corporation
    And the account is deleted
    Then I see the login page

@cannabis @publiccorporation @orgstructuredeletion
Scenario: Deletion from Org Structure (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Complete Organization Information button
    And I add personnel to the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete the personnel for a public corporation
    And I click on the button for Save For Later
    And the org structure data is successfully deleted for a public corporation
    And the account is deleted
    Then I see the login page

@cannabis @society @orgstructuredeletion
Scenario: Deletion from Org Structure (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Complete Organization Information button
    And I add personnel to the organization structure for a society
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete the personnel for a society
    And I click on the button for Save For Later
    And the org structure data is successfully deleted for a society
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @orgstructuredeletion
Scenario: Deletion from Org Structure (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Complete Organization Information button
    And I add personnel to the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete the personnel for a sole proprietorship
    And I click on the button for Save For Later
    And the org structure data is successfully deleted for a sole proprietorship
    And the account is deleted
    Then I see the login page

Feature: ReviewSecurityScreening
    As a logged in business user
    I want to confirm that the security screening page is working correctly

@cannabis @partnership @securityscreening
Scenario: Validation for Security Screening (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the link for Security Screening
    And I click on the Complete Organization Information button
    And I review the organization structure for a partnership 
    And I click on the button for Submit Organization Information
    And I click on the link for Security Screening
    And I review the security screening requirements for a partnership
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @securityscreening
Scenario: Validation for Security Screening (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Security Screening
    And I click on the Complete Organization Information button
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I click on the link for Security Screening
    And I review the security screening requirements for a private corporation
    And the account is deleted
    Then I see the login page

@cannabis @publiccorporation @securityscreening
Scenario: Validation for Security Screening (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the link for Security Screening
    And I click on the Complete Organization Information button
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I click on the link for Security Screening
    And I review the security screening requirements for a public corporation
    And the account is deleted
    Then I see the login page

@cannabis @society @securityscreening
Scenario: Validation for Security Screening (Society)
    Given I am logged in to the dashboard as a society
    And I click on the link for Security Screening
    And I click on the Complete Organization Information button
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I click on the link for Security Screening
    And I review the security screening requirements for a society
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @securityscreening
Scenario: Validation for Security Screening (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the link for Security Screening
    And I click on the Complete Organization Information button
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I click on the link for Security Screening
    And I review the security screening requirements for a sole proprietorship
    And the account is deleted
    Then I see the login page

# @cannabis @indigenousnation @securityscreening
# Scenario: Validation for Indigenous Nation Security Screening
#    Given I am logged in to the dashboard as an indigenous nation
#    And I click on the link for Security Screening
#    And I click on the Complete Organization Information button
#    And I review the organization structure for an indigenous nation
#    And I click on the button for Submit Organization Information
#    And I click on the link for Security Screening
#    And I review the security screening requirements for an indigenous nation
#    And the account is deleted
#    Then I see the login page