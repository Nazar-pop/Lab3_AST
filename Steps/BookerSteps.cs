using System.Text.Json;
using NUnit.Framework;
using RestApiTests.Models;
using RestApiTests.Support;
using TechTalk.SpecFlow;

namespace RestApiTests.Steps;

[Binding]
public class BookerSteps
{
    private readonly ApiContext _ctx;

    public BookerSteps(ApiContext ctx) => _ctx = ctx;

    // ---------- Given ----------

    [Given(@"the Restful-booker API is available")]
    public void GivenTheApiIsAvailable()
    {
        var response = _ctx.Booker.Ping();
        Assert.That((int)response.StatusCode, Is.EqualTo(201), "GET /ping має повертати 201 Created");
    }

    [Given(@"the user has a valid auth token")]
    public void GivenTheUserHasAValidAuthToken()
    {
        var response = _ctx.Booker.CreateToken("admin", "password123");
        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        using var doc = JsonHelper.Parse(response);
        _ctx.Token = JsonHelper.String(doc.RootElement, "token");
        Assert.That(_ctx.Token, Is.Not.Empty, "Токен не отримано");
    }

    [Given(@"a booking exists for ""(.*)"" ""(.*)""")]
    public void GivenABookingExists(string firstname, string lastname)
    {
        CreateBooking(Booking.Create(firstname, lastname, 100));
        Assert.That((int)_ctx.LastResponse.StatusCode, Is.EqualTo(200), "Не вдалося створити бронювання");
    }

    // ---------- When: authentication ----------

    [When(@"the user requests a token with username ""(.*)"" and password ""(.*)""")]
    public void WhenTheUserRequestsAToken(string username, string password)
    {
        _ctx.Response = _ctx.Booker.CreateToken(username, password);
    }

    // ---------- When: Create ----------

    [When(@"the user creates a booking for ""(.*)"" ""(.*)"" with total price (\d+) and deposit paid")]
    public void WhenTheUserCreatesABooking(string firstname, string lastname, int price)
    {
        CreateBooking(Booking.Create(firstname, lastname, price));
    }

    // ---------- When: Read ----------

    [When(@"the user requests the booking by its id")]
    public void WhenTheUserRequestsTheBookingByItsId()
    {
        _ctx.Response = _ctx.Booker.GetBooking(_ctx.BookingId);
    }

    [When(@"the user requests the booking with id (\d+)")]
    public void WhenTheUserRequestsTheBookingWithId(int id)
    {
        _ctx.Response = _ctx.Booker.GetBooking(id);
    }

    [When(@"the user requests the list of booking ids")]
    public void WhenTheUserRequestsTheListOfBookingIds()
    {
        _ctx.Response = _ctx.Booker.GetBookingIds();
    }

    // ---------- When: Update ----------

    [When(@"the user updates the booking to ""(.*)"" ""(.*)"" with total price (\d+)")]
    public void WhenTheUserUpdatesTheBooking(string firstname, string lastname, int price)
    {
        var booking = Booking.Create(firstname, lastname, price);
        _ctx.Response = _ctx.Booker.UpdateBooking(_ctx.BookingId, booking, _ctx.Token);
    }

    [When(@"the user updates the booking to ""(.*)"" ""(.*)"" with total price (\d+) without an auth token")]
    public void WhenTheUserUpdatesTheBookingWithoutToken(string firstname, string lastname, int price)
    {
        var booking = Booking.Create(firstname, lastname, price);
        _ctx.Response = _ctx.Booker.UpdateBooking(_ctx.BookingId, booking, null);
    }

    [When(@"the user changes only the first name to ""(.*)""")]
    public void WhenTheUserChangesOnlyTheFirstName(string firstname)
    {
        _ctx.Response = _ctx.Booker.PartialUpdateBooking(_ctx.BookingId, new { firstname }, _ctx.Token);
    }

    // ---------- When: Delete ----------

    [When(@"the user deletes the booking")]
    public void WhenTheUserDeletesTheBooking()
    {
        _ctx.Response = _ctx.Booker.DeleteBooking(_ctx.BookingId, _ctx.Token);
    }

    [When(@"the user deletes the booking without an auth token")]
    public void WhenTheUserDeletesTheBookingWithoutToken()
    {
        _ctx.Response = _ctx.Booker.DeleteBooking(_ctx.BookingId, null);
    }

    // ---------- Then ----------

    [Then(@"the response contains a token")]
    public void ThenTheResponseContainsAToken()
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        Assert.That(JsonHelper.String(doc.RootElement, "token"), Is.Not.Empty);
    }

    [Then(@"the response contains reason ""(.*)""")]
    public void ThenTheResponseContainsReason(string reason)
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        Assert.That(JsonHelper.String(doc.RootElement, "reason"), Is.EqualTo(reason));
    }

    [Then(@"the response contains a booking id")]
    public void ThenTheResponseContainsABookingId()
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        var id = JsonHelper.Property(doc.RootElement, "bookingid").GetInt32();
        Assert.That(id, Is.GreaterThan(0));
    }

    [Then(@"the created booking has first name ""(.*)"" and last name ""(.*)""")]
    public void ThenTheCreatedBookingHasName(string firstname, string lastname)
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        var booking = JsonHelper.Property(doc.RootElement, "booking");
        Assert.That(JsonHelper.String(booking, "firstname"), Is.EqualTo(firstname));
        Assert.That(JsonHelper.String(booking, "lastname"), Is.EqualTo(lastname));
    }

    [Then(@"the created booking matches the sent data")]
    public void ThenTheCreatedBookingMatchesTheSentData()
    {
        var sent = _ctx.SentBooking ?? throw new InvalidOperationException("Бронювання не надсилалось.");
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        var booking = JsonHelper.Property(doc.RootElement, "booking");
        Assert.Multiple(() =>
        {
            Assert.That(JsonHelper.Property(booking, "totalprice").GetInt32(), Is.EqualTo(sent.Totalprice));
            Assert.That(JsonHelper.Property(booking, "depositpaid").GetBoolean(), Is.EqualTo(sent.Depositpaid));
            Assert.That(JsonHelper.String(booking, "additionalneeds"), Is.EqualTo(sent.Additionalneeds));
            var dates = JsonHelper.Property(booking, "bookingdates");
            Assert.That(JsonHelper.String(dates, "checkin"), Is.EqualTo(sent.Bookingdates.Checkin));
            Assert.That(JsonHelper.String(dates, "checkout"), Is.EqualTo(sent.Bookingdates.Checkout));
        });
    }

    [Then(@"the returned booking has first name ""(.*)"" and last name ""(.*)""")]
    public void ThenTheReturnedBookingHasName(string firstname, string lastname)
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        Assert.Multiple(() =>
        {
            Assert.That(JsonHelper.String(doc.RootElement, "firstname"), Is.EqualTo(firstname));
            Assert.That(JsonHelper.String(doc.RootElement, "lastname"), Is.EqualTo(lastname));
        });
    }

    [Then(@"the booking stored on the server has first name ""(.*)"" and last name ""(.*)""")]
    public void ThenTheStoredBookingHasName(string firstname, string lastname)
    {
        var response = _ctx.Booker.GetBooking(_ctx.BookingId);
        Assert.That((int)response.StatusCode, Is.EqualTo(200));
        using var doc = JsonHelper.Parse(response);
        Assert.Multiple(() =>
        {
            Assert.That(JsonHelper.String(doc.RootElement, "firstname"), Is.EqualTo(firstname));
            Assert.That(JsonHelper.String(doc.RootElement, "lastname"), Is.EqualTo(lastname));
        });
    }

    [Then(@"the list of booking ids is not empty")]
    public void ThenTheListOfBookingIdsIsNotEmpty()
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        Assert.That(doc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(doc.RootElement.GetArrayLength(), Is.GreaterThan(0));
        Assert.That(JsonHelper.Property(doc.RootElement[0], "bookingid").GetInt32(), Is.GreaterThan(0));
    }

    [Then(@"requesting the booking by its id returns status code (\d+)")]
    public void ThenRequestingTheBookingReturnsStatus(int expected)
    {
        var response = _ctx.Booker.GetBooking(_ctx.BookingId);
        Assert.That((int)response.StatusCode, Is.EqualTo(expected));
    }

    // ---------- helpers ----------

    private void CreateBooking(Booking booking)
    {
        _ctx.SentBooking = booking;
        _ctx.Response = _ctx.Booker.CreateBooking(booking);
        if ((int)_ctx.Response.StatusCode != 200)
            return;

        using var doc = JsonHelper.Parse(_ctx.Response);
        _ctx.BookingId = JsonHelper.Property(doc.RootElement, "bookingid").GetInt32();
        _ctx.CreatedIds.Add(_ctx.BookingId);
    }
}
