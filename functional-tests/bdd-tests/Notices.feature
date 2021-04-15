Feature: Notices
    As a logged in business user
    I want to confirm the functionality on the Notices tab

Scenario: No Notices (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Notices
    And there are no notices attached to the account
    And the account is deleted
    Then I see the login page

# Scenario: Upload a Notice (Private Corporation)
#    Given I am logged in to the dashboard as a private corporation
#    And I click on the link for Notices
#    And there are no notices attached to the account
#    And I navigate to the Dynamics Account page for this account
#    And I click on the Open Document Location link
#    And I upload a file prefixed Notice__
#    And I return to the portal
#    And I click on the link for Notices
#    And the uploaded file is displayed
#    And I click on the uploaded file
#    And the file is successfully downloaded
#    And the account is deleted
#    Then I see the login page
