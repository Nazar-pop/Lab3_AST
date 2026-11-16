using System.Text.Json.Serialization;

namespace RestApiTests.Models;

/// <summary>Дати заселення та виселення (поле bookingdates).</summary>
public class BookingDates
{
    [JsonPropertyName("checkin")]
    public string Checkin { get; set; } = "2024-01-01";

    [JsonPropertyName("checkout")]
    public string Checkout { get; set; } = "2024-01-05";
}

/// <summary>Модель бронювання restful-booker.</summary>
public class Booking
{
    [JsonPropertyName("firstname")]
    public string Firstname { get; set; } = "";

    [JsonPropertyName("lastname")]
    public string Lastname { get; set; } = "";

    [JsonPropertyName("totalprice")]
    public int Totalprice { get; set; }

    [JsonPropertyName("depositpaid")]
    public bool Depositpaid { get; set; }

    [JsonPropertyName("bookingdates")]
    public BookingDates Bookingdates { get; set; } = new();

    [JsonPropertyName("additionalneeds")]
    public string Additionalneeds { get; set; } = "Breakfast";

    public static Booking Create(string firstname, string lastname, int totalprice) => new()
    {
        Firstname = firstname,
        Lastname = lastname,
        Totalprice = totalprice,
        Depositpaid = true
    };
}
