Feature: RuralLRSNameBrandingChange
    As a logged in business user
    I want to request a name or branding change for a rural LRS application

@privatecorporation @ruralLRS @release2
Scenario: Rural LRS Name Branding Change (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a private corporation
    And I complete the Rural LRS application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Establishment Name Change Application
    # TODO
    And the account is deleted
    Then I see the login page

@publiccorporation @ruralLRS 
Scenario: Rural LRS Name Branding Change (Public Corporation)
    Given I am logged in to the dashboard as a public corporation
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a public corporation
    And I complete the Rural LRS application for a public corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Establishment Name Change Application
    # TODO
    And the account is deleted
    Then I see the login page

@partnership @ruralLRS 
Scenario: Rural LRS Name Branding Change (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a partnership
    And I complete the Rural LRS application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Establishment Name Change Application
    # TODO
    And the account is deleted
    Then I see the login page

@society @ruralLRS 
Scenario: Rural LRS Name Branding Change (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a society
    And I complete the Rural LRS application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Establishment Name Change Application
    # TODO
    And the account is deleted
    Then I see the login page

@soleproprietorship @ruralLRS
Scenario: Rural LRS Name Branding Change (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Rural LRS
    And I review the account profile for a sole proprietorship
    And I complete the Rural LRS application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the Licences tab
    And I click on the link for Establishment Name Change Application
    # TODO
    And the account is deleted
    Then I see the login page