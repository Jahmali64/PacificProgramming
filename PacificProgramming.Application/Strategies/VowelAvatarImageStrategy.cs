using PacificProgramming.Application.Interfaces;
using PacificProgramming.Application.ViewModels;

namespace PacificProgramming.Application.Strategies;

public sealed class VowelAvatarImageStrategy : IAvatarImageStrategy {
    private readonly string _imageUrl = "https://api.dicebear.com/8.x/pixel-art/png?seed=vowel&size=150";

    public async Task<ImageVM> GetAvatarImage(string userIdentifier) {
        var image = new ImageVM { Url = _imageUrl };
        return await Task.FromResult(image);
    }
}