Feature: PartnershipOrgStructure.feature
    As a logged in partnership business user
    I want to confirm the organization structure functionality

@e2e @cannabis @partnership @validation @partnerorgstructure1
Scenario: Change individual partner name and pay fee
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
    And I click on the Licences tab
    And I pay the licensing fee for Cannabis
    And I click on the Dashboard link
    And I click on the Review Organization Information button
    And I modify only the individual partner name
    And I click on the button for Submit Organization Information
    And I pay the name change fee
    And the individual partner name is now updated
    And the account is deleted
    Then I see the login page

@cannabis @partnership @validation @partnerorgstructure2
Scenario: Delete an individual who is both an individual partner and individual partner of business partner
    Given I am logged in to the dashboard as a partnership
    And I click on the Complete Organization Information button
    And I enter the same person as an individual partner and an individual partner of a business partner (person 1)
    And I enter a second person as an individual partner (person 2)
    And I enter a third person as an individual partner of a business partner (person 3)
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I delete only the individual partner record (person 1)
    And I click on the Complete Organization Information button
    And only the individual partner of a business partner record is displayed (person 1, 2, 3)
    And the account is deleted
    Then I see the login page

@cannabis @partnership @validation @partnerorgstructure3
Scenario: Change individual partner and business partner same name 
    # under development
    Given I am logged in to the dashboard as a partnership
    And I click on the Complete Organization Information button
    And I enter the same person as an individual partner and a business shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I modify only the individual partner record
    And I click on the Complete Organization Information button
    And the individual partner and business shareholder name are identical
    And the account is deleted
    Then I see the login page

@cannabis @partnership @validation @partnerorgstructure4
Scenario: Confirm partnership business shareholder org structure update
    Given I am logged in to the dashboard as a partnership
    And I click on the Complete Organization Information button
    And I enter the same individual as an individual partner and a business shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I add a business shareholder with the same individual as an individual partner and a business shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I add a second individual as an individual partner and a business shareholder 
    And I click on the button for Confirm Organization Information is Complete
    And I click on the Complete Organization Information button
    And the partnership org structure is correct
    And I remove the latest individual partner and business shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the latest individual partner and business shareholder is removed
    And I remove the business shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the business shareholder is removed
    And the account is deleted
    Then I see the login page

@cannabis @partnership @validation @partnerorgstructure5
Scenario: Confirm partnership business shareholder org structure update after payment 
    Given I am logged in to the dashboard as a partnership
    And I click on the Complete Organization Information button
    And I enter the same individual as an individual partner and a business shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I add a business shareholder with the same individual as an individual partner and a business shareholder
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And I add a second individual as an individual partner and a business shareholder 
    And I click on the button for Confirm Organization Information is Complete
    And I click on the Complete Organization Information button
    And the partnership org structure is correct
    And I click on the button for Confirm Organization Information is Complete
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I click on the button for Confirm Organization Information is Complete
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And the application is approved
    And I click on the Review Organization Information button
    And the partnership org structure is correct after payment
    And the account is deleted
    Then I see the login page

@cannabis @partnership @validation @partnerorgstructure6
Scenario: Save for Later feature for partnership org structure
    Given I am logged in to the dashboard as a partnership
    And I click on the Complete Organization Information button
    And I enter the same individual as an individual partner and a business shareholder
    And I click on the Save for Later button
    And I click on the Complete Organization Information button
    And I add a business shareholder with the same individual as an individual partner and a business shareholder
    And I click on the Save for Later button
    And I click on the Complete Organization Information button
    And I add a second individual as an individual partner and a business shareholder
    And I click on the Save for Later button
    And I click on the Complete Organization Information button
    And I remove the latest individual shareholder after saving
    And I click on the Save for Later button
    And I click on the Complete Organization Information button
    And I remove the latest business shareholder after saving
    And I click on the Save for Later button
    And I click on the Complete Organization Information button
    And the latest director and shareholder is removed
    And I remove the business shareholder
    And I click on the Save for Later button
    And I click on the Complete Organization Information button
    And the saved org structure for partnership is present
    And the account is deleted
    Then I see the login page