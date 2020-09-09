Feature: PrivateCorporationOrgStructure.feature
    As a logged in private corporation business user
    I want to confirm the organization structure functionality

@e2e @cannabis @privatecorporation @validation @privatecorporgstructure
Scenario: Change private corporation director name and pay fee
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee for Cannabis
    And I click on the link for Dashboard
    And I click on the Review Organization Information button
    And I modify the director name
    And I click on the button for Submit Organization Information
    And I pay the name change fee
    And the director name is now updated
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @validation @privatecorporgstructure
Scenario: Delete an individual who is both a private corporation director and shareholder
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

@cannabis @privatecorporation @validation @privatecorporgstructure
Scenario: Change private corporation director and shareholder same name
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

@cannabis @privatecorporation @validation @privatecorporgstructure
Scenario: Confirm private corporation business shareholder org structure update
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

@cannabis @privatecorporation @validation @privatecorporgstructure
Scenario: Confirm private corporation business shareholder org structure update after payment
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
    And I review the account profile for a private corporation
    And I click on the button for Confirm Organization Information is Complete
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Review Organization Information button
    And the org structure is correct after payment
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @validation @privatecorporgstructure
Scenario: Save for Later feature for private corporation org structure
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

@e2e @cannabis @privatecorporation @validation @privatecorporgstructure1
Scenario: CRS application with mixed business shareholder types
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
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

@e2e @cannabis @privatecorporation @validation @privatecorporgstructure1
Scenario: Complex Save for Later mixed business shareholders
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I enter business shareholders of different business types to be saved for later
    And I click on the button for Save for Later
    And I click on the Complete Organization Information button
    And I review the security screening requirements for saved for later mixed business shareholder
    And the account is deleted
    Then I see the login page

@e2e @cannabis @privatecorporation @validation @privatecorporgstructure1
Scenario: Confirm org structure records not duplicated
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I confirm that no duplicates are shown in the org structure
    And the account is deleted
    Then I see the login page