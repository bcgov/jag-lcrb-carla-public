Feature: FoodPrimaryPatronParticipation
    As a logged in business user
    I want to request a Patron Participation Entertainment Endorsement Application for a Food Primary licence

@foodprimarypatronparticipation @partnership 
Scenario: Partnership Food Primary Patron Participation Entertainment Endorsement
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
    And I click on the link for Patron Participation Entertainment Endorsement
    And I request a Patron Participation Entertainment Endorsement application
    And I click on the Submit button
    And the dashboard status is updated as Pending External Review
    And the account is deleted
    Then I see the login page

@foodprimarypatronparticipation @privatecorporation
Scenario: Private Corporation Food Primary Patron Participation Entertainment Endorsement
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
    And I click on the link for Patron Participation Entertainment Endorsement
    And I request a Patron Participation Entertainment Endorsement application
    And I click on the Submit button
    And the dashboard status is updated as Pending External Review
    And the account is deleted
    Then I see the login page

@foodprimarypatronparticipation @publiccorporation
Scenario: Public Corporation Food Primary Patron Participation Entertainment Endorsement
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
    And I click on the link for Patron Participation Entertainment Endorsement
    And I request a Patron Participation Entertainment Endorsement application
    And I click on the Submit button
    And the dashboard status is updated as Pending External Review
    And the account is deleted
    Then I see the login page

@foodprimarypatronparticipation @society
Scenario: Society Food Primary Patron Participation Entertainment Endorsement
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
    And I click on the link for Patron Participation Entertainment Endorsement
    And I request a Patron Participation Entertainment Endorsement application
    And I click on the Submit button
    And the dashboard status is updated as Pending External Review
    And the account is deleted
    Then I see the login page

@foodprimarypatronparticipation @soleproprietorship
Scenario: Sole Proprietorship Food Primary Patron Participation Entertainment Endorsement
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
    And I click on the link for Patron Participation Entertainment Endorsement
    And I request a Patron Participation Entertainment Endorsement application
    And I click on the Submit button
    And the dashboard status is updated as Pending External Review
    And the account is deleted
    Then I see the login page