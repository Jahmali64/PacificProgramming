using PacificProgramming.Application.Interfaces;
using PacificProgramming.Application.ViewModels;

namespace PacificProgramming.Application.Strategies;

public sealed class DefaultAvatarImageStrategy : IAvatarImageStrategy {
    private readonly ImageVM _defaultImage = new ImageVM { Url = "https://api.dicebear.com/8.x/pixel-art/png?seed=default&size=150" };

    public async Task<ImageVM> GetAvatarImage(string userIdentifier) => await Task.FromResult(_defaultImage);
}