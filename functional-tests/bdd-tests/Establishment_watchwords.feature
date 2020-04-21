Feature: Establishment_watchwords
    As a logged in business user
    I want to submit an establishment name
    And confirm that watch words are not used

Scenario Outline: Confirm establishment watch words
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure
    And I enter a "<watch_word>" on the application page
    Then the message "The store name contains at least one word that doesn’t comply with naming requirements. The application can’t be submitted until the prohibited word(s) are removed." is displayed
   
    Examples:
    | watch_word    |
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