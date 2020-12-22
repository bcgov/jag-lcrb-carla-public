Feature: FoodPrimaryRelease
    As a logged in business user
    I want to run a release test for a Food Primary licence

@foodprimary @privatecorporation
Scenario: Private Corporation Food Primary Release Test
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
    And I request a valid store name or branding change for Food Primary
    And I request a catering endorsement application
    And I request a change in terms and conditions application
    And I click on the link for Download Licence
    And I request a licensee representative
    And I request a new outdoor patio application
    And I request a Patron Participation Entertainment Endorsement application
    And I request a store relocation for Food Primary
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And I request a structural change
    And I click on the link for Dashboard
    And I confirm the structural change request is displayed on the dashboard
    And I submit a temporary extension of licensed area application
    And I submit a temporary use area endorsement application
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page

@foodprimary @soleproprietorship
Scenario: Sole Proprietorship Food Primary Release Test
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Food Primary
    And I review the account profile for a sole proprietorship
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I complete the Food Primary application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the Dashboard tab
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I pay the licensing fee 
    And I request a valid store name or branding change for Food Primary
    And I request a catering endorsement application
    And I request a change in terms and conditions application
    And I click on the link for Download Licence
    And I request a licensee representative
    And I request a new outdoor patio application
    And I request a Patron Participation Entertainment Endorsement application
    And I request a store relocation for Food Primary
    And I click on the link for Dashboard
    And I confirm the relocation request is displayed on the dashboard
    And I request a structural change
    And I click on the link for Dashboard
    And I confirm the structural change request is displayed on the dashboard
    And I submit a temporary extension of licensed area application
    And I submit a temporary use area endorsement application
    And I request a third party operator
    And I click on the link for Cancel Application
    And I cancel the third party operator application
    And I request a transfer of ownership
    And the account is deleted
    Then I see the login page