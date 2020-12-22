Feature: ReviewOrgStructureData
    As a logged in business user
    I want to confirm the data saved for the org structure

@cannabis @partnership @orgstructuredata
Scenario: Data for Partnership Org Structure
    Given I am logged in to the dashboard as a partnership
    And I click on the Complete Organization Information button
    And I review the organization structure for a partnership
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a partnership
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @orgstructuredata
Scenario: Data for Private Corporation Org Structure
    Given I am logged in to the dashboard as a private corporation
    And I click on the Complete Organization Information button
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a private corporation
    And the account is deleted
    Then I see the login page

@cannabis @publiccorporation @orgstructuredata
Scenario: Data for Public Corporation Org Structure
    Given I am logged in to the dashboard as a public corporation
    And I click on the Complete Organization Information button
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a public corporation
    And the account is deleted
    Then I see the login page

@cannabis @society @orgstructuredata
Scenario: Data for Society Org Structure
    Given I am logged in to the dashboard as a society
    And I click on the Complete Organization Information button
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a society
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @orgstructuredata
Scenario: Data for Sole Proprietorship Org Structure
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Complete Organization Information button
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a sole proprietorship
    And the account is deleted
    Then I see the login page

 @cannabis @university @orgstructuredata
 Scenario: Data for University Org Structure
    Given I am logged in to the dashboard as a university
    And I click on the Complete Organization Information button
    And I review the organization structure for a university
    And I click on the button for Submit Organization Information
    And I click on the Complete Organization Information button
    And the org structure data is present for a university
    And the account is deleted
    Then I see the login page