Feature: PublicSmokeTest
    As a business user who is not logged in
    I want to confirm that I can view the publicly available content

@smoketest
Scenario: View Public Content
    Given I am not logged in to the portal
    And I click on Home page
    And the Map of Cannabis Stores Header Text is displayed
    And I click on the Licence Types page
    And the Cannabis Retail Store Licence is displayed
    And I click on the Worker Information page
    Then the Cannabis Worker Information is displayed