using RestApiTests.Support;
using TechTalk.SpecFlow;

namespace RestApiTests.Hooks;

/// <summary>Після кожного сценарію видаляє бронювання, створені під час тесту.</summary>
[Binding]
public class CleanupHooks
{
    private readonly ApiContext _ctx;

    public CleanupHooks(ApiContext ctx) => _ctx = ctx;

    [AfterScenario]
    public void DeleteCreatedBookings()
    {
        if (_ctx.CreatedIds.Count == 0)
            return;

        try
        {
            var token = _ctx.Token ?? TokenFromServer();
            if (token == null)
                return;
            foreach (var id in _ctx.CreatedIds)
                _ctx.Booker.DeleteBooking(id, token);   // 404/201 однаково прийнятні
        }
        catch
        {
            // очищення не повинно ламати результат тесту
        }
    }

    private string? TokenFromServer()
    {
        var response = _ctx.Booker.CreateToken("admin", "password123");
        using var doc = JsonHelper.Parse(response);
        return doc.RootElement.TryGetProperty("token", out var t) ? t.GetString() : null;
    }
}
