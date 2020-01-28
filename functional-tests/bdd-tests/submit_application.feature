Feature: Submit Cannabis Retail Application
    In order to obtain a cannabis retail licence
    As a potential licensee
    I want to submit and pay for an application for a retail store licence

Background:
    Given that I am logged in to CaRLA with a valid BCeID business account
    And the licensing dashboard is displayed
    And I click on the "START APPLICATION" button for a Cannabis Retail Store Licence on the licensing dashboard
    And I click on the "CONTINUE TO APPLICATION" button on the account review page

Scenario Outline: Submit and pay for application
    When I enter an <establishment> on the application page
    And I enter a <street_address> on the application page
    And I enter a <city> on the application page
    And I enter a <postal> on the application page
    And I enter a <pid> on the application page
    And I enter a <store_email> on the application page
    And I enter a <store_phone> on the application page
    And I upload an Associates Form on the application page
    And I upload a Financial Integrity form on the application page
    And I upload a Supporting Document on the application page
    And I enter a <contact_given> on the application page
    And I enter a <contact_surname> on the application page
    And I enter a <contact_title> on the application page
    And I enter a <contact_phone> on the application page
    And I enter a <contact_email> on the application page
    And I click on the Authorized to Submit checkbox on the application page
    And I click on the Signature Agreement checkbox on the application page
    And I click on the "SUBMIT & PAY" button on the application page
    And I enter the credit card details on the payment page  
    And I click on the "Pay Now" button on the payment page 
    And the receipt page is displayed
    And the payment amount of "7,500.00" displayed on the receipt page
    And I click on the "Return to Dashboard" link on the receipt page
    Then the application for <establishment> has a status of "APPLICATION UNDER REVIEW" on the licensing dashboard

    Examples:
    | establishment        | street_address | city     | postal | pid       | store_email   | store_phone | contact_given | contact_surname | contact_title | contact_phone | contact_ email |
    | Automated Industries | 645 Tyee Rd    | Victoria | V9A6X5 | 012345678 | test@test.com | 2505555555  | Automated     | Test            | CEO           | 2501111111    | test1@test.com |
    | 1                    | 1              | 1        | V9A6X5 | 012345678 | 1@1.c         | 1           | 1             | 1               |               | 1             | 1              |

#Scenario Outline: Confirm data validation for application page
#    When I enter an <establishment> on the application page
#    And I enter a <postal> on the application page
#    And I enter a <pid> on the application page
#    And I enter a <store_email> on the application page
#    And I enter a <store_phone> on the application page
#    And I enter a <contact_phone> on the application page
#    And I click on the "SUBMIT & PAY" button on the application page
#    Then the message "Some required fields have not been completed" is displayed

#    Examples:
#    | establishment     | postal  | pid      | store_email | store_phone | phone    |
#    | Club Med Cannabis | 123     | 123      | test@test   | 456         | 789      |
#    | Cannabis Health   | V9X 6X5 | 00000000 | test@test.  | 00000000    | 00000000 |
#    |                   |         |          |             |             |          |


Scenario: Check required fields on application page
    When I click on the "SUBMIT & PAY" button on the application page
    Then the message "Establishment Name is a required field" is displayed on the application page
    And the message "Please enter the street address" is displayed on the application page
    And the message "Please enter the city" is displayed on the application page
    And the message "Please enter the postal code" is displayed on the application page
    And the message "Please enter the Parcel Identifier (format: 9 digits)" is displayed on the application page
    And the message "Please enter the business contact's first name" is displayed on the application page
    And the message "Please enter the business contact's last name" is displayed on the application page
    And the message "Please enter the business contact's 10-digit phone number" is displayed on the application page
    And the message "Please affirm that you are authorized to submit the application." is displayed on the application page
    And the message "Please affirm that all of the information provided for this application is true and complete." is displayed on the application page
    And the message "Associate form is required." is displayed on the application page
    And the message "Financial Integrity form is required." is displayed on the application page
    And the message "At least one supporting document is required." is displayed on the application page
    And the message "Establishment name is required." is displayed on the application page
    And the message "Some required fields have not been completed" is displayed on the application page


Scenario Outline: Confirm establishment watch words verification
    When I enter an <establishment> on the application page
    Then the message 
    """The store name contains at least one word that doesn’t comply with naming requirements. 
    The application can’t be submitted until the prohibited word(s) are removed.""" is displayed

    Examples:
    | establishment |
    | Antidote      |
    | Apothecary    |
    | Compassion    |
    | Cure          |
    | Dispensary    |
    | Doctor        |
    | Dr.           |
    | Elixir        |
    | Heal          |
    | Healing       |
    | Health        |
    | Herbal        |
    | Hospital      |
    | Med           |
    | Medi          |
    | Medical       |
    | Medicinal     |
    | Medicine      |
    | Pharmacy      |
    | Potion        |
    | Prescription  |
    | Relief        |
    | Remedy        |
    | Restore       |
    | Solution      |
    | Therapeutics  |
    | Therapy       |
    | Tonics        |
    | Treatment     |