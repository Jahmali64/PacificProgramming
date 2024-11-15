using PacificProgramming.Application.ViewModels;
using System.Net;
using System.Text.Json;

namespace PacificProgramming.Tests.EndToEndTests;

public class AvatarEndToEndTests : IClassFixture<CustomWebApplicationFactory> {
    private readonly HttpClient _httpClient;

    public AvatarEndToEndTests(CustomWebApplicationFactory factory) {
        _httpClient = factory.CreateClient();
    }

    [Theory]
    [InlineData("/avatar?userIdentifier=", HttpStatusCode.BadRequest)]
    public async Task GetAvatarImage_WithEmptyIdentifier_ReturnsHttpStatusCodeBadRequest(string requestUri, HttpStatusCode expectedStatusCode) {
        var response = await _httpClient.GetAsync(requestUri);
        
        Assert.Equal(response.StatusCode, expectedStatusCode);
    }

    [Theory]
    [InlineData("/avatar?userIdentifier=myth1", "https://api.dicebear.com/8.x/pixel-art/png?seed=db1&size=150")]
    public async Task GetAvatarImage_WithLastDigitBetweenOneAndFiveInIdentifier_ReturnsImageWithUrlFromDatabase(string requestUri, string expectedUrl) {
        var response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var image = JsonSerializer.Deserialize<ImageVM>(content, options);

        Assert.NotNull(image);
        Assert.Equal(image.Url, expectedUrl);
    }

    [Theory]
    [InlineData("/avatar?userIdentifier=myth7", "https://api.dicebear.com/8.x/pixel-art/png?seed=7&size=150")]
    public async Task GetAvatarImage_WithLastDigitBetweenSixAndNineInIdentifier_ReturnsImageWithUrlFromJson(string requestUri, string expectedUrl) {
        var response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var image = JsonSerializer.Deserialize<ImageVM>(content, options);

        Assert.NotNull(image);
        Assert.Equal(image.Url, expectedUrl);
    }

    [Theory]
    [InlineData("/avatar?userIdentifier=mythos", "https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150")]
    public async Task GetAvatarImage_WithVowelInIdentifier_ReturnsImageWithVowelUrl(string requestUri, string expectedUrl) {
        var response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var image = JsonSerializer.Deserialize<ImageVM>(content, options);

        Assert.NotNull(image);
        Assert.Equal(image.Url, expectedUrl);
    }

    [Theory]
    [InlineData("/avatar?userIdentifier=myth+", "https://api.dicebear.com/8.x/pixel-art")]
    public async Task GetAvatarImage_WithSpecialCharacterInIdentifier_ReturnsImageWithRandomUrl(string requestUri, string expectedUrl) {
        var response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var image = JsonSerializer.Deserialize<ImageVM>(content, options);

        Assert.NotNull(image);
        Assert.InRange<int>(image.Id, 1, 5);
        Assert.StartsWith(expectedUrl, image.Url);
    }

    [Theory]
    [InlineData("/avatar?userIdentifier=myth", "https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150")]
    public async Task GetAvatarImage_WithNoMetConditionsIdentifier_ReturnsImageWithDefaultUrl(string requestUri, string expectedUrl) {
        var response = await _httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var image = JsonSerializer.Deserialize<ImageVM>(content, options);

        Assert.NotNull(image);
        Assert.Equal(image.Url, expectedUrl);
    }
}