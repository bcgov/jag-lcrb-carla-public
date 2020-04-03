Feature: Catering_request_event_authorization
    As a logged in business user
    I would like to request a Catering event authorization

Scenario: Request Catering Event Authorization
    Given the Catering application has been approved
    And I am logged in to the dashboard as a private corporation
    And I click on the Licences tab
    And the licence fee has been paid
    And I click on the Request Event Authorization link
    And I submit a catered event authorization request
    And I click on the Event History form
    Then the event is displayed