using System.Text.Json;
using NUnit.Framework;
using RestApiTests.Support;
using TechTalk.SpecFlow;

namespace RestApiTests.Steps;

[Binding]
public class DogApiSteps
{
    private readonly ApiContext _ctx;

    public DogApiSteps(ApiContext ctx) => _ctx = ctx;

    // ---------- When ----------

    [When(@"the user requests the list of all breeds")]
    public void WhenTheUserRequestsAllBreeds() => _ctx.Response = _ctx.Dog.AllBreeds();

    [When(@"the user requests a random dog image")]
    public void WhenTheUserRequestsARandomImage() => _ctx.Response = _ctx.Dog.RandomImage();

    [When(@"the user requests (\d+) random dog images")]
    public void WhenTheUserRequestsSeveralRandomImages(int count) =>
        _ctx.Response = _ctx.Dog.RandomImages(count);

    [When(@"the user requests a random image of breed ""(.*)""")]
    public void WhenTheUserRequestsARandomImageOfBreed(string breed) =>
        _ctx.Response = _ctx.Dog.BreedRandomImage(breed);

    [When(@"the user requests images of breed ""(.*)""")]
    public void WhenTheUserRequestsImagesOfBreed(string breed) =>
        _ctx.Response = _ctx.Dog.BreedImages(breed);

    [When(@"the user requests sub-breeds of ""(.*)""")]
    public void WhenTheUserRequestsSubBreeds(string breed) =>
        _ctx.Response = _ctx.Dog.SubBreeds(breed);

    // ---------- Then ----------

    [Then(@"the response status field is ""(.*)""")]
    public void ThenTheStatusFieldIs(string expected)
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        Assert.That(JsonHelper.String(doc.RootElement, "status"), Is.EqualTo(expected));
    }

    [Then(@"the breeds list contains breed ""(.*)""")]
    public void ThenTheBreedsListContains(string breed)
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        var message = JsonHelper.Property(doc.RootElement, "message");
        Assert.That(message.ValueKind, Is.EqualTo(JsonValueKind.Object));
        Assert.That(message.TryGetProperty(breed, out _), Is.True, $"Порода \"{breed}\" відсутня у списку");
    }

    [Then(@"the image url starts with ""(.*)""")]
    public void ThenTheImageUrlStartsWith(string prefix)
    {
        Assert.That(ImageUrl(), Does.StartWith(prefix));
    }

    [Then(@"the image url contains ""(.*)""")]
    public void ThenTheImageUrlContains(string part)
    {
        Assert.That(ImageUrl(), Does.Contain(part));
    }

    [Then(@"the sub-breeds contain ""(.*)""")]
    public void ThenTheSubBreedsContain(string subBreed)
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        var names = JsonHelper.Property(doc.RootElement, "message").EnumerateArray()
            .Select(e => e.GetString()).ToList();
        Assert.That(names, Does.Contain(subBreed));
    }

    [Then(@"the response contains (\d+) image urls")]
    public void ThenTheResponseContainsImageUrls(int count)
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        var message = JsonHelper.Property(doc.RootElement, "message");
        Assert.That(message.ValueKind, Is.EqualTo(JsonValueKind.Array));
        Assert.That(message.GetArrayLength(), Is.EqualTo(count));
        foreach (var url in message.EnumerateArray())
            Assert.That(url.GetString(), Does.StartWith("https://"));
    }

    [Then(@"the error message contains ""(.*)""")]
    public void ThenTheErrorMessageContains(string text)
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        Assert.That(JsonHelper.String(doc.RootElement, "message"), Does.Contain(text));
    }

    private string ImageUrl()
    {
        using var doc = JsonHelper.Parse(_ctx.LastResponse);
        return JsonHelper.String(doc.RootElement, "message");
    }
}
