Feature: UBrewUVinRequestOfChangeInTermsDiscretion
    As a logged in business user
    I want to submit a Request of Change in Terms and Conditions/Request for Discretion application for a UBrew / UVin licence

@ubrewuvinlicence @privatecorporation 
Scenario: UBrew / UVin Application Change In Terms (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a UBrew UVin application
    And I review the account profile for a private corporation
    And I complete the UBrew / UVin application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a UBrew / UVin application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    And I request a change in terms and conditions application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page