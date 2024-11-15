using PacificProgramming.Application.Interfaces;
using PacificProgramming.Application.ViewModels;
using System.Text.Json;

namespace PacificProgramming.Application.Strategies;

public sealed class JsonAvatarImageStrategy : IAvatarImageStrategy {
    private readonly string _baseImageUrl;
    private readonly CancellationToken _cancellationToken;

    public JsonAvatarImageStrategy(CancellationToken cancellationToken) {
        _baseImageUrl = "https://my-json-server.typicode.com/ck-pacificdev/tech-test/images";
        _cancellationToken = cancellationToken;
    }

    public async Task<ImageVM> GetAvatarImage(string userIdentifier) {
        if (string.IsNullOrWhiteSpace(userIdentifier)) throw new ArgumentNullException(nameof(userIdentifier));

        char lastChar = userIdentifier[^1];
        if (!char.IsDigit(lastChar)) throw new ArgumentException(nameof(lastChar));

        var lastDigit = int.Parse(lastChar.ToString());

        using var httpClient = new HttpClient();

        var httpResponse = await httpClient.GetAsync($"{_baseImageUrl}/{lastDigit}", _cancellationToken);

        if (!httpResponse.IsSuccessStatusCode) throw new HttpRequestException("Unsuccessful request.");
        var jsonResponse = await httpResponse.Content.ReadAsStringAsync(_cancellationToken);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var image = JsonSerializer.Deserialize<ImageVM>(jsonResponse, options);

        if (image is null) throw new JsonException("Could not deserialize json.");
        return image;
    }
}