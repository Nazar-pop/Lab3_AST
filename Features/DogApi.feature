Feature: Dog CEO public API
  As a client of the public Dog CEO API (https://dog.ceo/dog-api/)
  I want to get breeds and dog images
  So that I can show them in an application

Scenario: List of all breeds
  When the user requests the list of all breeds
  Then the response status code is 200
  And the response status field is "success"
  And the breeds list contains breed "hound"

Scenario: Random dog image
  When the user requests a random dog image
  Then the response status code is 200
  And the response status field is "success"
  And the image url starts with "https://images.dog.ceo/breeds/"

Scenario Outline: Random image of a given breed
  When the user requests a random image of breed "<breed>"
  Then the response status code is 200
  And the response status field is "success"
  And the image url contains "/breeds/<breed>"

  Examples:
    | breed   |
    | hound   |
    | bulldog |
    | poodle  |

Scenario: Sub-breeds of a breed
  When the user requests sub-breeds of "hound"
  Then the response status code is 200
  And the sub-breeds contain "afghan"

Scenario: Several random images
  When the user requests 3 random dog images
  Then the response status code is 200
  And the response contains 3 image urls

Scenario: Unknown breed returns an error
  When the user requests images of breed "unicorn"
  Then the response status code is 404
  And the response status field is "error"
  And the error message contains "Breed not found"
