using System.Text.Json;
using RestApiTests.Models;
using RestSharp;

namespace RestApiTests.Clients;

/// <summary>
/// Обгортка над RestSharp для https://restful-booker.herokuapp.com.
/// Кожен метод відповідає одній операції API і повертає RestResponse без перевірок.
/// </summary>
public class BookerClient
{
    private readonly RestClient _client;

    public BookerClient(string baseUrl)
    {
        var options = new RestClientOptions(baseUrl)
        {
            MaxTimeout = 60000   // безкоштовний Heroku може «прокидатися» кілька секунд
        };
        _client = new RestClient(options);
    }

    private RestResponse Send(RestRequest request) =>
        _client.ExecuteAsync(request).GetAwaiter().GetResult();

    private static RestRequest Json(string resource, Method method, string? token = null)
    {
        var request = new RestRequest(resource, method);
        request.AddHeader("Accept", "application/json");
        if (token != null)
            request.AddHeader("Cookie", $"token={token}");
        return request;
    }

    /// <summary>GET /ping — перевірка доступності (очікується 201).</summary>
    public RestResponse Ping() => Send(new RestRequest("ping", Method.Get));

    /// <summary>POST /auth — отримання токена.</summary>
    public RestResponse CreateToken(string username, string password)
    {
        var request = Json("auth", Method.Post);
        request.AddStringBody(JsonSerializer.Serialize(new { username, password }), DataFormat.Json);
        return Send(request);
    }

    /// <summary>GET /booking — список ідентифікаторів.</summary>
    public RestResponse GetBookingIds() => Send(Json("booking", Method.Get));

    /// <summary>GET /booking/{id}.</summary>
    public RestResponse GetBooking(int id) => Send(Json($"booking/{id}", Method.Get));

    /// <summary>POST /booking — створення бронювання (Create).</summary>
    public RestResponse CreateBooking(Booking booking)
    {
        var request = Json("booking", Method.Post);
        request.AddStringBody(JsonSerializer.Serialize(booking), DataFormat.Json);
        return Send(request);
    }

    /// <summary>PUT /booking/{id} — повне оновлення (Update). Токен — у cookie.</summary>
    public RestResponse UpdateBooking(int id, Booking booking, string? token)
    {
        var request = Json($"booking/{id}", Method.Put, token);
        request.AddStringBody(JsonSerializer.Serialize(booking), DataFormat.Json);
        return Send(request);
    }

    /// <summary>PATCH /booking/{id} — часткове оновлення.</summary>
    public RestResponse PartialUpdateBooking(int id, object fields, string? token)
    {
        var request = Json($"booking/{id}", Method.Patch, token);
        request.AddStringBody(JsonSerializer.Serialize(fields), DataFormat.Json);
        return Send(request);
    }

    /// <summary>DELETE /booking/{id} — видалення (Delete).</summary>
    public RestResponse DeleteBooking(int id, string? token) =>
        Send(Json($"booking/{id}", Method.Delete, token));
}
