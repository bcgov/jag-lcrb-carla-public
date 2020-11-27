Feature: ReviewSecurityScreening
    As a logged in business user
    I want to confirm that the security screening page is working correctly

@cannabis @partnership @securityscreening
Scenario: Validation for Partnership Security Screening
    Given I am logged in to the dashboard as a partnership
    And I click on the link for Security Screening
    And I click on the Complete Organization Information button
    And I review the organization structure for a partnership 
    And I click on the button for Submit Organization Information
    And I click on the link for Security Screening
    And I review the security screening requirements for a partnership
    And the account is deleted
    Then I see the login page

@cannabis @privatecorporation @securityscreening
Scenario: Validation for Private Corporation Security Screening
    Given I am logged in to the dashboard as a private corporation
    And I click on the link for Security Screening
    And I click on the Complete Organization Information button
    And I review the organization structure for a private corporation
    And I click on the button for Submit Organization Information
    And I click on the link for Security Screening
    And I review the security screening requirements for a private corporation
    And the account is deleted
    Then I see the login page

@cannabis @publiccorporation @securityscreening
Scenario: Validation for Public Corporation Security Screening
    Given I am logged in to the dashboard as a public corporation
    And I click on the link for Security Screening
    And I click on the Complete Organization Information button
    And I review the organization structure for a public corporation
    And I click on the button for Submit Organization Information
    And I click on the link for Security Screening
    And I review the security screening requirements for a public corporation
    And the account is deleted
    Then I see the login page

@cannabis @society @securityscreening
Scenario: Validation for Society Security Screening
    Given I am logged in to the dashboard as a society
    And I click on the link for Security Screening
    And I click on the Complete Organization Information button
    And I review the organization structure for a society
    And I click on the button for Submit Organization Information
    And I click on the link for Security Screening
    And I review the security screening requirements for a society
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @securityscreening
Scenario: Validation for Sole Proprietorship Security Screening
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the link for Security Screening
    And I click on the Complete Organization Information button
    And I review the organization structure for a sole proprietorship
    And I click on the button for Submit Organization Information
    And I click on the link for Security Screening
    And I review the security screening requirements for a sole proprietorship
    And the account is deleted
    Then I see the login page

# @cannabis @indigenousnation @securityscreening
# Scenario: Validation for Indigenous Nation Security Screening
#    Given I am logged in to the dashboard as an indigenous nation
#    And I click on the link for Security Screening
#    And I click on the Complete Organization Information button
#    And I review the organization structure for an indigenous nation
#    And I click on the button for Submit Organization Information
#    And I click on the link for Security Screening
#    And I review the security screening requirements for an indigenous nation
#    And the account is deleted
#    Then I see the login page