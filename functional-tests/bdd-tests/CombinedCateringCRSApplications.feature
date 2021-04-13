Feature: CombinedCateringCrsApplications
    As a logged in business user
    I want to submit a CRS and Catering application for the same account

Scenario: Combined Catering / CRS Applications (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Cannabis Retail Store application
    And I click on the link for Dashboard
    And I click on the Start Application button for Catering
    And I click on the button for Continue to Organization Review
    And I complete the Catering application for a combined application
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And the account is deleted
    Then I see the login page