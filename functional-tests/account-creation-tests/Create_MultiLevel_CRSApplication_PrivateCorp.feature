Feature: Create_MultiLevel_CRSApplication_PrivateCorp
    As a logged in business user
    I want to submit a CRS Application for a private corporation
    With multiple nested business shareholders
    To be used as test data

Scenario: Create Multi Level CRS Application Private Corporation
    Given I am logged in to the dashboard as a private corporation
    # And the account is deleted
    # And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I add in multiple nested business shareholders
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Pay for Application button
    And I enter the payment information
    Then I confirm the payment receipt for a Cannabis Retail Store application