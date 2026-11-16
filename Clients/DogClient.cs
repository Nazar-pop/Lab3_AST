using RestSharp;

namespace RestApiTests.Clients;

/// <summary>Обгортка над RestSharp для публічного Dog CEO API (https://dog.ceo/dog-api/).</summary>
public class DogClient
{
    private readonly RestClient _client;

    public DogClient(string baseUrl)
    {
        _client = new RestClient(new RestClientOptions(baseUrl) { MaxTimeout = 30000 });
    }

    /// <summary>Виконує GET-запит до вказаного ресурсу.</summary>
    public RestResponse Get(string resource)
    {
        var request = new RestRequest(resource, Method.Get);
        request.AddHeader("Accept", "application/json");
        return _client.ExecuteAsync(request).GetAwaiter().GetResult();
    }

    public RestResponse AllBreeds() => Get("breeds/list/all");
    public RestResponse RandomImage() => Get("breeds/image/random");
    public RestResponse RandomImages(int count) => Get($"breeds/image/random/{count}");
    public RestResponse BreedRandomImage(string breed) => Get($"breed/{breed}/images/random");
    public RestResponse BreedImages(string breed) => Get($"breed/{breed}/images");
    public RestResponse SubBreeds(string breed) => Get($"breed/{breed}/list");
}
