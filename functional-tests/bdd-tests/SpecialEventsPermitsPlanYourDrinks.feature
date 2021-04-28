Feature: SpecialEventsPermitsPlanYourDrinks
    As a logged in business user
    I want to plan my drinks for my Special Events Permits applications

Scenario: SEP Plan Your Drinks (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Special Events Permits Dashboard
    And I click on the button for Plan Your Drinks
    And the Plan Your Drinks label is displayed
    And I complete the Drink Planner form
    And the Drink Planner calculations are correct
    And the account is deleted
    Then I see the login page