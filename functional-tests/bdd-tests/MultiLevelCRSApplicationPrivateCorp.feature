Feature: MultiLevelCRSApplicationPrivateCorp
    As a logged in business user
    I want to submit a CRS Application for a private corporation
    With multiple nested business shareholders

@e2e @cannabis @privatecorporation @validation @multilevel
Scenario: Multiple Nested Shareholders
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I review the organization structure for a private corporation
    And I add in multiple nested business shareholders
    And I click on the Submit Organization Information button
    And I complete the Cannabis Retail Store application for a private corporation
    # And I review the security screening requirements for a multilevel business
    # And I click on the Pay for Application button
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And I return to the dashboard
    And the account is deleted
    Then I see the login page