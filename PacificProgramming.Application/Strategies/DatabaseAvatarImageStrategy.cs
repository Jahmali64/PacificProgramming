using PacificProgramming.Application.Interfaces;
using PacificProgramming.Application.Services;
using PacificProgramming.Application.ViewModels;

namespace PacificProgramming.Application.Strategies;

public sealed class DatabaseAvatarImageStrategy : IAvatarImageStrategy {
    private readonly IImageService _imageService;

    public DatabaseAvatarImageStrategy(IImageService imageService) {
        _imageService = imageService;
    }

    public async Task<ImageVM> GetAvatarImage(string userIdentifier) {
        if (string.IsNullOrWhiteSpace(userIdentifier)) throw new ArgumentNullException(nameof(userIdentifier));

        char lastChar = userIdentifier[^1];
        if (!char.IsDigit(lastChar)) throw new ArgumentException(nameof(lastChar));

        var lastDigit = int.Parse(lastChar.ToString());
        var image = await _imageService.GetImageById(lastDigit);

        if (image is null) throw new KeyNotFoundException(nameof(image));
        return image;
    }
}