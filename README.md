# RestApiTests — Lab 3, variant 5

API tests with **RestSharp + SpecFlow (BDD) + NUnit** on .NET 8.

* `Features/RestfulBooker.feature` — CRUD for https://restful-booker.herokuapp.com
* `Features/Authentication.feature` — token endpoint
* `Features/DogApi.feature` — public Dog CEO API (https://dog.ceo/dog-api/)

Run: `dotnet test` (23 test runs). Override base URLs with `BOOKER_URL` / `DOG_URL`.
