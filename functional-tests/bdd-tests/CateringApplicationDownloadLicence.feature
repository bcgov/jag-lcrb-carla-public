Feature: CateringApplicationDownloadLicence
    As a logged in business user
    I want to pay the first year catering licence fee
    And download the licence for different business types

@privatecorporation @cateringlicencedownload
Scenario: Catering Licence Download (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for Catering
    And I review the account profile for a private corporation
    And I complete the Catering application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the link for Download Licence
    And the licence is successfully downloaded
    And the account is deleted
    Then I see the login page

@society @cateringlicencedownload
Scenario: Catering Licence Download (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for Catering
    And I review the account profile for a society
    And I complete the Catering application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the link for Download Licence
    And the licence is successfully downloaded
    And the account is deleted
    Then I see the login page

@partnership @cateringlicencedownload
Scenario: Catering Licence Download (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for Catering
    And I review the account profile for a partnership
    And I complete the Catering application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the link for Download Licence
    And the licence is successfully downloaded
    And the account is deleted
    Then I see the login page

@soleproprietorship @cateringlicencedownload
Scenario: Catering Licence Download (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for Catering
    And I review the account profile for a sole proprietorship
    And I complete the Catering application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee 
    And I click on the link for Download Licence
    And the licence is successfully downloaded
    And the account is deleted
    Then I see the login page