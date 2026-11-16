Feature: Restful-booker bookings CRUD
  As an API client
  I want to create, read, update and delete bookings
  So that I can manage hotel reservations

Background:
  Given the Restful-booker API is available
  And the user has a valid auth token

# ---------- Create ----------

Scenario Outline: Create a booking
  When the user creates a booking for "<firstname>" "<lastname>" with total price <price> and deposit paid
  Then the response status code is 200
  And the response contains a booking id
  And the created booking has first name "<firstname>" and last name "<lastname>"
  And the created booking matches the sent data

  Examples:
    | firstname | lastname | price |
    | Jim       | Brown    | 111   |
    | Sally     | Smith    | 250   |
    | Ivan      | Petrenko | 1     |

# ---------- Read ----------

Scenario: Read a booking by id
  Given a booking exists for "Sally" "Brown"
  When the user requests the booking by its id
  Then the response status code is 200
  And the returned booking has first name "Sally" and last name "Brown"

Scenario: Read the list of booking ids
  When the user requests the list of booking ids
  Then the response status code is 200
  And the list of booking ids is not empty

Scenario: Read a booking that does not exist
  When the user requests the booking with id 999999999
  Then the response status code is 404

# ---------- Update ----------

Scenario: Update a booking
  Given a booking exists for "Sally" "Brown"
  When the user updates the booking to "Mary" "Smith" with total price 222
  Then the response status code is 200
  And the returned booking has first name "Mary" and last name "Smith"
  And the booking stored on the server has first name "Mary" and last name "Smith"

Scenario: Partially update a booking
  Given a booking exists for "Sally" "Brown"
  When the user changes only the first name to "Anna"
  Then the response status code is 200
  And the returned booking has first name "Anna" and last name "Brown"

Scenario: Update without an auth token is forbidden
  Given a booking exists for "Sally" "Brown"
  When the user updates the booking to "Mary" "Smith" with total price 222 without an auth token
  Then the response status code is 403
  And the booking stored on the server has first name "Sally" and last name "Brown"

# ---------- Delete ----------

Scenario: Delete a booking
  Given a booking exists for "Sally" "Brown"
  When the user deletes the booking
  Then the response status code is 201
  And requesting the booking by its id returns status code 404

Scenario: Delete without an auth token is forbidden
  Given a booking exists for "Sally" "Brown"
  When the user deletes the booking without an auth token
  Then the response status code is 403
  And requesting the booking by its id returns status code 200
