Feature: FoodPrimaryRelease
    As a logged in business user
    I want to run a release test for a Food Primary licence

@8release
Scenario: Food Primary Release Test #1 (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a private corporation
    And I complete the Food Primary application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the link for Dashboard
    # And the dashboard status is updated as Application Under Review
    And the application is approved
    # And I click on the link for Licences & Authorizations
    # And I click on the link for Establishment Name Change Application
    # And I click on the Continue to Application button
    # And I request a valid store name or branding change for Food Primary
    # And I click on the link for Licences & Authorizations
    # And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    # And I click on the Continue to Application button
    # And I request a change in terms and conditions application
    # And I click on the Submit button
    # And I enter the payment information
    And I click on the link for Licences & Authorizations
    # And I click on the link for Download Licence
    # And I confirm the terms and conditions for a Food Primary licence
    # And I click on the link for Add Licensee Representative
    # And I request a licensee representative
    And I click on the link for New Outdoor Patio
    And I click on the Continue to Application button
    And I request a new outdoor patio application
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I click on the link for Patron Participation Entertainment Endorsement
    And I click on the Continue to Application button
    And I request a Patron Participation Entertainment Endorsement application
    And I click on the Submit button
    # And I click on the link for Licences & Authorizations
    # And I click on the link for Relocation Application
    # And I click on the Continue to Application button
    # And I request a Food Primary relocation application
    And the account is deleted
    Then I see the login page

Scenario: Food Primary Release Test #2 (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Food Primary
    And I review the account profile for a private corporation
    And I complete the Food Primary application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I click on the link for Structural Change Application
    And I click on the Continue to Application button
    And I submit a Food Primary structural change application
    # And I click on the link for Licences & Authorizations
    # And I click on the link for Temporary Extension of Licensed Area
    # And I click on the Continue to Application button
    # And I submit a temporary extension of licensed area application
    And I click on the link for Licences & Authorizations
    And I click on the link for Temporary Use Area Endorsement Application
    And I click on the Continue to Application button
    And I submit a temporary use area endorsement application
    And I click on the link for Licences & Authorizations
    And I click on the link for Catering Endorsement Application
    And I click on the Continue to Application button
    And I request a catering endorsement application
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    # And I click on the link for Temporary Expanded Service Area Application
    # And I click on the Continue to Application button
    # And I complete the TESA application for a Food Primary licence
    # And I click on the Submit button
    # And I click on the link for Licences & Authorizations
    And I click on the link for Temporary Change to Hours of Sale (After Midnight)
    And I click on the Continue to Application button
    And I request an after midnight temporary change to hours of sale
    And I click on the Submit button
    And I click on the link for Licences & Authorizations
    And I click on the link for Temporary Change to Hours of Sale (Before Midnight)
    And I click on the Continue to Application button
    And I request a before midnight temporary change to hours of sale
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I click on the link for Temporary Suspension Request
    And I click on the Continue to Application button
    And I complete the temporary suspension request
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I request a third party operator
    And I request a transfer of ownership for Food Primary
    And the account is deleted
    Then I see the login page

Scenario: Food Primary Release Test #1 (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Food Primary
    And I review the account profile for a sole proprietorship
    And I complete the Food Primary application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And I confirm the payment receipt for a Food Primary application
    And I click on the link for Dashboard
    And the dashboard status is updated as Application Under Review
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I click on the link for Establishment Name Change Application
    And I click on the Continue to Application button
    And I request a valid store name or branding change for Food Primary
    And I click on the link for Licences & Authorizations
    And I click on the link for Request of Change in Terms and Conditions/Request for Discretion
    And I click on the Continue to Application button
    And I request a change in terms and conditions application
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I click on the link for Download Licence
    And I confirm the terms and conditions for a Food Primary licence
    And I click on the link for Add Licensee Representative
    And I request a licensee representative
    And I click on the link for New Outdoor Patio
    And I click on the Continue to Application button
    And I request a new outdoor patio application
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I click on the link for Patron Participation Entertainment Endorsement
    And I click on the Continue to Application button
    And I request a Patron Participation Entertainment Endorsement application
    And I click on the Submit button
    And I click on the link for Licences & Authorizations
    And I click on the link for Relocation Application
    And I click on the Continue to Application button
    And I request a Food Primary relocation application
    And the account is deleted
    Then I see the login page

Scenario: Food Primary Release Test #2 (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Food Primary
    And I review the account profile for a sole proprietorship
    And I complete the Food Primary application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the link for Licences & Authorizations
    And I click on the link for Structural Change Application
    And I click on the Continue to Application button
    And I submit a Food Primary structural change application
    # And I click on the link for Licences & Authorizations
    # And I click on the link for Temporary Extension of Licensed Area
    # And I click on the Continue to Application button
    # And I submit a temporary extension of licensed area application
    And I click on the link for Licences & Authorizations
    And I click on the link for Temporary Use Area Endorsement Application
    And I click on the Continue to Application button
    And I submit a temporary use area endorsement application
    And I click on the link for Licences & Authorizations
    And I click on the link for Catering Endorsement Application
    And I click on the Continue to Application button
    And I request a catering endorsement application
    And I click on the Submit button
    And I enter the payment information
    # And I click on the link for Licences & Authorizations
    # And I click on the link for Temporary Expanded Service Area Application
    # And I click on the Continue to Application button
    # And I complete the TESA application for a Food Primary licence
    # And I click on the Submit button
    And I click on the link for Licences & Authorizations
    And I click on the link for Temporary Change to Hours of Sale (After Midnight)
    And I click on the Continue to Application button
    And I request an after midnight temporary change to hours of sale
    And I click on the Submit button
    And I click on the link for Licences & Authorizations
    And I click on the link for Temporary Change to Hours of Sale (Before Midnight)
    And I click on the Continue to Application button
    And I request a before midnight temporary change to hours of sale
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I click on the link for Temporary Suspension Request
    And I click on the Continue to Application button
    And I complete the temporary suspension request
    And I click on the Submit button
    And I enter the payment information
    And I click on the link for Licences & Authorizations
    And I request a third party operator
    And I request a transfer of ownership for Food Primary
    And the account is deleted
    Then I see the login page