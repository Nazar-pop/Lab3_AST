using NUnit.Framework;
using RestApiTests.Support;
using TechTalk.SpecFlow;

namespace RestApiTests.Steps;

/// <summary>Кроки, спільні для всіх API.</summary>
[Binding]
public class CommonSteps
{
    private readonly ApiContext _ctx;

    public CommonSteps(ApiContext ctx) => _ctx = ctx;

    [Then(@"the response status code is (\d+)")]
    public void ThenTheResponseStatusCodeIs(int expected)
    {
        var response = _ctx.LastResponse;
        Assert.That((int)response.StatusCode, Is.EqualTo(expected),
            $"Тіло відповіді: {response.Content}");
    }
}
