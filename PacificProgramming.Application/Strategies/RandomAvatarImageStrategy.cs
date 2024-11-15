using PacificProgramming.Application.Interfaces;
using PacificProgramming.Application.ViewModels;

namespace PacificProgramming.Application.Strategies;

public sealed class RandomAvatarImageStrategy : IAvatarImageStrategy {
    private readonly string _baseImageUrl = "https://api.dicebear.com/8.x/pixel-art";
    private readonly int _minRandomValue = 1;
    private readonly int _maxRandomValue = 6;

    public async Task<ImageVM> GetAvatarImage(string userIdentifier) {
        var random = new Random().Next(_minRandomValue, _maxRandomValue);
        var image = new ImageVM { Id = random, Url = $"{_baseImageUrl}/png?seed={random}&size=150" };
        return await Task.FromResult(image);
    }
}