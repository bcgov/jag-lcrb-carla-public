Feature: FoodPrimaryChangeInTerms
    As a logged in business user
    I want to submit a Request of Change in Terms and Conditions/Request for Discretion application for a Food Primary licence

@foodprimarychangeterms @partnership 
Scenario: Partnership Food Primary Change in Terms
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Food Primary
    And I review the account profile for a partnership
    And I complete the Food Primary application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    And I request a change in terms and conditions application
    And the account is deleted
    Then I see the login page

@foodprimarychangeterms @privatecorporation
Scenario: Private Corporation Food Primary Change in Terms
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a private corporation
    And I complete the Food Primary application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    And I request a change in terms and conditions application
    And the account is deleted
    Then I see the login page

@foodprimarychangeterms @publiccorporation
Scenario: Public Corporation Food Primary Change in Terms
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a public corporation
    And I complete the Food Primary application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    And I request a change in terms and conditions application
    And the account is deleted
    Then I see the login page

@foodprimarychangeterms @society
Scenario: Society Food Primary Change in Terms
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Food Primary
    And I review the account profile for a society
    And I complete the Food Primary application for a society
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    And I request a change in terms and conditions application
    And the account is deleted
    Then I see the login page

@foodprimarychangeterms @soleproprietorship
Scenario: Sole Proprietorship Food Primary Change in Terms
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Food Primary
    And I review the account profile for a sole proprietorship
    And I complete the Food Primary application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee
    And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    And I request a change in terms and conditions application
    And the account is deleted
    Then I see the login page