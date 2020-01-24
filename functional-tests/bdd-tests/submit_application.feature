Feature: Submit Cannabis Retail Application
    In order to obtain a cannabis retail licence
    As a potential licensee
    I want to submit and pay for an application for a retail store licence

Background:
    Given that I log in to CaRLA with a valid BCeID business account
    And the licensing dashboard is displayed
    And I click on the START APPLICATION button for a Cannabis Retail Store Licence on the licensing dashboard
    And I click on the CONTINUE TO APPLICATION button on the account review page

Scenario Outline: Submit and pay for application
    When I enter an <establishment> on the application page
    And I enter an <address> on the application page
    And I enter a <city> on the application page
    And I enter a <postal> on the application page
    And I enter a <pid> on the application page
    And I enter a <store_email> on the application page
    And I enter a <store_phone> on the application page
    And I upload an Associates Form on the application page
    And I upload a Financial Integrity form on the application page
    And I upload a Supporting Document on the application page
    And I enter a <given> on the application page
    And I enter a <surname> on the application page
    And I enter a <title> on the application page
    And I enter a <phone> on the application page
    And I enter a <email> on the application page
    And I click on the authorization checkbox on the application page
    And I click on the truthfulness checkbox on the application page
    And I click on the SUBMIT & PAY button on the application page
    And I enter the credit card details on the payment page  
    And I click on the Pay Now button on the payment page 
    And the receipt page is displayed
    # TBD: Check details on receipt page
    And I click on the Return to Dashboard link on the receipt page
    Then the application for <establishment> has a status of "APPLICATION UNDER REVIEW" on the licensing dashboard

    Examples:
    | establishment        | address     | city     | postal | pid       | store_email   | store_phone | given     | surname | title | phone      | email          |
    | Automated Industries | 645 Tyee Rd | Victoria | V9A6X5 | 012345678 | test@test.com | 2505555555  | Automated | Test    | CEO   | 2501111111 | test1@test.com |
    | 1                    | 1           | 1        | V9A6X5 | 012345678 | 1@1.c         | 1           | 1         | 1       |       | 1          | 1              |

Scenario Outline: Confirm data validation for application page
    When I enter an <establishment> on the application page
    And I enter a <postal> on the application page
    And I enter a <pid> on the application page
    And I enter a <store_email> on the application page
    And I enter a <store_phone> on the application page
    And I enter a <phone> on the application page
    And I click on the SUBMIT & PAY button on the application page
    Then the message "Some required fields have not been completed" is displayed

    Examples:
    | establishment     | postal  | pid      | store_email | store_phone | phone    |
    | Club Med Cannabis | 123     | 123      | test@test   | 456         | 789      |
    | Cannabis Health   | V9X 6X5 | 00000000 | test@test.  | 00000000    | 00000000 |
    |                   |         |          |             |             |          |