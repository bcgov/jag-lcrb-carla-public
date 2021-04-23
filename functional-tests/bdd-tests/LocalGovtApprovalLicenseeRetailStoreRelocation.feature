Feature: LocalGovtApprovalLicenseeRetailStoreRelocation
    As a logged in business user
    I want to submit a LRS Relocation Application for review and approval

Scenario: Local Government Approval for LRS Relocation (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And an LRS licence has been created
    And I click on the link for Licences & Authorizations 
    And I click on the link for Request Relocation
    And I click on the Continue to Application button
    And I complete the LRS application
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority
    And I log in as a return user
    And I review the local government response for a LRS relocation
    And the dashboard status is updated as Application Under Review
    And the account is deleted
    Then I see the login page 