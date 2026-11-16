Feature: Restful-booker authentication
  As an API client
  I want to obtain an auth token with my credentials
  So that I can modify and delete bookings

Scenario: Token is issued for valid credentials
  When the user requests a token with username "admin" and password "password123"
  Then the response status code is 200
  And the response contains a token

Scenario Outline: Token is not issued for invalid credentials
  When the user requests a token with username "<username>" and password "<password>"
  Then the response status code is 200
  And the response contains reason "Bad credentials"

  Examples:
    | username | password |
    | admin    | wrong    |
    | wrong    | password123 |
    | wrong    | wrong    |
