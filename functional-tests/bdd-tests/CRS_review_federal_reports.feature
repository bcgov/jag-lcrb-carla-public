Feature: CRS_review_federal_reports
    As a logged in business user
    I want to review the Cannabis Retail Store federal reports

Scenario: Review CRS Federal Reports
    Given the CRS application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Review Federal Reports link
    Then the monthly reports will be available