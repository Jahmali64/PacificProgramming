using PacificProgramming.Application.Factories;
using PacificProgramming.Application.ViewModels;

namespace PacificProgramming.Application.Services;

public interface IAvatarImageService {
    Task<ImageVM> GetAvatarImage(string userIdentifier);
}

public sealed class AvatarImageService : IAvatarImageService {
    private readonly IAvatarImageStrategyFactory _avatarImageStrategyFactory;

    public AvatarImageService(IAvatarImageStrategyFactory avatarImageStrategyFactory) {
        _avatarImageStrategyFactory = avatarImageStrategyFactory;
    }

    public async Task<ImageVM> GetAvatarImage(string userIdentifier) {
        var strategy = _avatarImageStrategyFactory.GetStrategy(userIdentifier);
        return await strategy.GetAvatarImage(userIdentifier);
    }
}