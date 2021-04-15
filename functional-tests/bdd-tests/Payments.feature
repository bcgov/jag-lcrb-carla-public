Feature: Payments
    As a logged in business user
    I want to test the payments for a CRS and Catering application

Scenario: Payments for CRS/Catering Applications (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Return to Dashboard
    And I click on the Start Application button for Catering
    And I click on the button for Continue to Organization Review
    And I click on the button for Confirm Organization Information is Complete
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I click on the button for Pay for Application
    And I enter the payment information
    And the account is deleted
    Then I see the login page