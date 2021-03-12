Feature: CateringRequestOfChangeInTermsDiscretion
    As a logged in business user
    I want to apply for a Request of Change in Terms and Conditions/Request for Discretion

@privatecorporation @cateringlicencedownload
Scenario: Catering Licence Request of Change in Terms and Conditions/Request for Discretion (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    And I request a change in terms and conditions application
    And I click on the Submit button
    And I enter the payment information
    And the account is deleted
    Then I see the login page