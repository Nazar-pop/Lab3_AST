using RestApiTests.Clients;
using RestApiTests.Models;
using RestSharp;

namespace RestApiTests.Support;

/// <summary>
/// Спільний стан сценарію. SpecFlow створює новий екземпляр для кожного сценарію
/// та впроваджує його в класи кроків через конструктор.
/// Адреси API можна перевизначити змінними середовища BOOKER_URL та DOG_URL.
/// </summary>
public class ApiContext
{
    public BookerClient Booker { get; } = new(
        Environment.GetEnvironmentVariable("BOOKER_URL") ?? "https://restful-booker.herokuapp.com");

    public DogClient Dog { get; } = new(
        Environment.GetEnvironmentVariable("DOG_URL") ?? "https://dog.ceo/api");

    /// <summary>Остання відповідь сервера — її перевіряють кроки Then.</summary>
    public RestResponse? Response { get; set; }

    public string? Token { get; set; }
    public int BookingId { get; set; }
    public Booking? SentBooking { get; set; }

    /// <summary>Усі бронювання, створені в сценарії, — для очищення після нього.</summary>
    public List<int> CreatedIds { get; } = new();

    public RestResponse LastResponse =>
        Response ?? throw new InvalidOperationException("Запит ще не виконувався.");
}
