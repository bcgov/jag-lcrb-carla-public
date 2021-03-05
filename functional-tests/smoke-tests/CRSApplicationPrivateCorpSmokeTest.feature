Feature: CRSApplicationPrivateCorpSmokeTest
    As a logged in business user
    I want to submit a CRS Application for a private corporation

@smoketest
Scenario: Smoke Test Private Corporation CRS Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    # And the account is deleted
    # Then I see the login page