using Microsoft.Extensions.DependencyInjection;
using PacificProgramming.Application.Interfaces;
using PacificProgramming.Application.Strategies;

namespace PacificProgramming.Application.Factories;

public interface IAvatarImageStrategyFactory {
    IAvatarImageStrategy GetStrategy(string userIdentifier);
}

public class AvatarImageStrategyFactory : IAvatarImageStrategyFactory {
    private readonly IServiceProvider _serviceProvider;
    private static readonly Dictionary<Func<string, bool>, Type> s_strategies = new() {
        { id => string.IsNullOrWhiteSpace(id), typeof(DefaultAvatarImageStrategy) },
        { id => id.Length > 0 && char.IsDigit(id[^1]) && int.Parse(id[^1].ToString()) is >= 6 and <= 9, typeof(JsonAvatarImageStrategy) },
        { id => id.Length > 0 && char.IsDigit(id[^1]) && int.Parse(id[^1].ToString()) is >= 1 and <= 5, typeof(DatabaseAvatarImageStrategy) },
        { id => id.IndexOfAny(new[] { 'a', 'e', 'i', 'o', 'u' }) >= 0, typeof(VowelAvatarImageStrategy) },
        { id => id.Any(ch => !char.IsLetterOrDigit(ch)), typeof(RandomAvatarImageStrategy) }
    };

    public AvatarImageStrategyFactory(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public IAvatarImageStrategy GetStrategy(string userIdentifier) {
        foreach (var (condition, strategyType) in s_strategies) {
            if (condition(userIdentifier)) return (IAvatarImageStrategy)_serviceProvider.GetRequiredService(strategyType);
        }

        return _serviceProvider.GetRequiredService<DefaultAvatarImageStrategy>();
    }
}