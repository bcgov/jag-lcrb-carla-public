Feature: Establishment_watchwords
    As a logged in business user
    I want to submit an establishment name
    And confirm that watch words are not used

Background:
    Given I am logged in to the dashboard as a private corporation
    And the account is deleted
    And I am logged in to the dashboard as a private corporation
    And I click on the Start Application button
    And I complete the eligibility disclosure
    And I review the account profile
    And I review the organization structure

Scenario Outline: Confirm establishment watch words
    When I enter a <watch_word>
    Then the correct error message is displayed  
    
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