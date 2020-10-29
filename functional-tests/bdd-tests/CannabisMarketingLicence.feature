Feature: CannabisMarketingLicence
    As a logged in business user
    I want to submit a Cannabis Marketing Licence for different business types

@e2e @cannabismktg @indigenousnation 
Scenario: Indigenous Nation Cannabis Marketing Application
    Given I am logged in to the dashboard as an indigenous nation
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for an indigenous nation
    And I complete the Cannabis Marketing application
    And I click on the Submit button
    And I review the security screening requirements for an indigenous nation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @cannabismktg @partnership 
Scenario: Partnership Cannabis Marketing Application
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a partnership
    And I complete the Cannabis Marketing application
    And I click on the Submit button
    And I review the security screening requirements for a partnership
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @cannabismktg @privatecorporation 
Scenario: Private Corporation Cannabis Marketing Application
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a private corporation
    And I complete the Cannabis Marketing application
    And I click on the Submit button
    And I review the security screening requirements for a private corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @cannabismktg @publiccorporation 
Scenario: Public Corporation Cannabis Marketing Application
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a public corporation
    And I complete the Cannabis Marketing application
    And I click on the Submit button
    And I review the security screening requirements for a public corporation
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @cannabismktg @society 
Scenario: Society Cannabis Marketing Application
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a society
    And I complete the Cannabis Marketing application
    And I click on the Submit button
    And I review the security screening requirements for a society
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @cannabismktg @soleproprietorship 
Scenario: Sole Proprietorship Cannabis Marketing Application
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a sole proprietorship
    And I complete the Cannabis Marketing application
    And I click on the Submit button
    And I review the security screening requirements for a sole proprietorship
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @cannabismktg @localgovernment 
Scenario: Local Government Cannabis Marketing Application
    Given I am logged in to the dashboard as a local government
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a local government
    And I complete the Cannabis Marketing application
    And I click on the Submit button
    And I review the security screening requirements for a local government
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page

@e2e @cannabismktg @university 
Scenario: University Cannabis Marketing Application
    Given I am logged in to the dashboard as a university
    And I click on the Start Application button for a Cannabis Marketing Licence
    And I review the account profile for a university
    And I complete the Cannabis Marketing application
    And I click on the Submit button
    And I review the security screening requirements for a university
    And I click on the button for Pay for Application
    And I enter the payment information
    And I confirm the payment receipt for a Catering application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page